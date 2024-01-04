using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;


namespace Smart_Studios
{
    public class SmartStudiosManager: MonoBehaviour
    {

        //４種のStudioを判定処理
        //Game Development Studioに、4種のStudioをSupportとして割当させる処理を追加

        /*
            typ = 1 ; Game Development
            typ = 3 ; QA
            typ = 4 ; Graphics
            typ = 5 ; Music
            typ = 10 ; Motion Capture
        */

        /// <summary>
        /// Support選択時、マウスカーソルの移動処理かつ部屋の許容判定処理
        /// まずこちらを改変しない限り、選択自体ができないため、こちらを改変する必要がある。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___rS_"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_Unterstuetzen), "MouseMovement")]
        static bool MouseMovement_Prefix(Menu_Unterstuetzen __instance, roomScript ___rS_, mainScript ___mS_, pickObjectScript ___pOS_, Camera ___myCamera, mapScript ___mapS_, roomScript ___roomOutlineOld, sfxScript ___sfx_)
        {

            Menu_Unterstuetzen instance = __instance;
            roomScript roomScriptOld = ___roomOutlineOld;
            mainScript mainScript = ___mS_;
            pickObjectScript pickObjectScript = ___pOS_;
            Camera camera = ___myCamera;
            mapScript mapScript = ___mapS_;
            roomScript roomOutlineOld = ___roomOutlineOld;
            sfxScript sfxScript = ___sfx_;

            if (!mainScript)
            {
                return false;
            }

            bool mouseButtonUp = Input.GetMouseButtonUp(0);
            pickObjectScript.disableMouseButton = mouseButtonUp;
            RaycastHit hit;

            if (Physics.Raycast(camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)), out hit, 200f, instance.layerMaskFloor))
            {
                int numX = Mathf.RoundToInt(hit.point.x);
                int numZ = Mathf.RoundToInt(hit.point.z);

                if (mapScript.mapRoomID[numX, numZ] != 1)
                {
                    var currentRoomScript = mapScript.mapRoomScript[numX, numZ];
                    if (currentRoomScript)
                    {
                        HandleRoomOutlineChange(ref roomOutlineOld, currentRoomScript, instance, numX, numZ, mouseButtonUp, sfxScript);
                    }
                }
                else if (roomOutlineOld)
                {
                    roomOutlineOld.DisableOutlineLayer();
                    roomOutlineOld = null;
                    return false;
                }
            }
            else if (roomOutlineOld)
            {
                roomOutlineOld.DisableOutlineLayer();
                roomOutlineOld = null;
            }

            return false;
        }

        private static void HandleRoomOutlineChange(ref roomScript roomOutlineOld, roomScript currentRoomScript, Menu_Unterstuetzen instance, int numX, int numZ, bool mouseButtonUp, sfxScript sfxScript)
        {
            if (roomOutlineOld != currentRoomScript)
            {
                roomOutlineOld?.DisableOutlineLayer();
                roomOutlineOld = currentRoomScript;
                if (ShouldSetOutline(instance.rS_.typ, currentRoomScript.typ, currentRoomScript.myID, instance.rS_.myID))
                {
                    currentRoomScript.SetOutlineLayer();
                }
            }

            if (mouseButtonUp)
            {
                if (ShouldSetOutline(instance.rS_.typ, currentRoomScript.typ, currentRoomScript.myID, instance.rS_.myID))
                {
                    AcceptRoomChange(instance, currentRoomScript);
                    return;
                }
                sfxScript.PlaySound(2, true);
            }
        }

        private static bool ShouldSetOutline(int roomType, int currentRoomType, int currentRoomId, int roomId)
        {
            int[] validTypes = { 3, 4, 5, 10 };
            return (roomType == currentRoomType && currentRoomId != roomId) ||
                   (Array.IndexOf(validTypes, roomType) >= 0 && currentRoomType == 1 && currentRoomId != roomId);
        }

        private static void AcceptRoomChange(Menu_Unterstuetzen instance, roomScript currentRoomScript)
        {
            Traverse.Create(instance).Method("Accept", new object[] { currentRoomScript }).GetValue();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_Unterstuetzen), "Accept")]
        static bool Prefix(Menu_Unterstuetzen __instance, ref roomScript script_, ref roomScript ___rS_, ref mainScript ___mS_, ref GUI_Main ___guiMain_)
        {
            Menu_Unterstuetzen instance = __instance;
            roomScript targetRoomScript = script_;
            roomScript sourceRoomScript = ___rS_;
            mainScript mainScript = ___mS_;
            GUI_Main guiMain = ___guiMain_;

            if (sourceRoomScript == targetRoomScript)
            {
                return false;
            }

            if (sourceRoomScript && targetRoomScript && sourceRoomScript.typ == targetRoomScript.typ)
            {
                CancelExistingTasks(mainScript, sourceRoomScript, targetRoomScript);
                taskUnterstuetzen newTask = guiMain.AddTask_Unterstuetzen();
                InitializeTask(newTask, targetRoomScript, sourceRoomScript);
            }

            if (CustomSupportStatus.IsSuitableCustomSupportForGameDev(sourceRoomScript, targetRoomScript))
            {
                AttachCustomSupportManagerToGameObject(sourceRoomScript, targetRoomScript, mainScript, guiMain);
            }

            instance.BUTTON_Close();
            return false;
        }

        private static void CancelExistingTasks(mainScript mainScript, roomScript sourceRoomScript, roomScript targetRoomScript)
        {
            foreach (var room in mainScript.arrayRoomScripts)
            {
                if (room && room.taskGameObject)
                {
                    var supportTask = room.GetTaskUnterstuetzen();
                    if (supportTask && supportTask.roomID == sourceRoomScript.myID)
                    {
                        supportTask.Abbrechen();
                    }
                }
            }

            if (targetRoomScript.taskID != -1)
            {
                GameObject taskObject = GameObject.Find($"Task_{targetRoomScript.taskID}");
                taskObject?.GetComponent<taskUnterstuetzen>()?.Abbrechen();
            }
        }

        private static void InitializeTask(taskUnterstuetzen task, roomScript targetRoomScript, roomScript sourceRoomScript)
        {
            task.Init(false);
            task.roomID = targetRoomScript.myID;
            sourceRoomScript.taskID = task.myID;
            sourceRoomScript.DisableOutlineLayer();
            targetRoomScript.DisableOutlineLayer();
        }

        private static void AttachCustomSupportManagerToGameObject(roomScript sourceRoomScript, roomScript targetRoomScript, mainScript mainScript, GUI_Main guiMain)
        {
            var customSupportManager = sourceRoomScript.gameObject.GetComponent<CustomSupportManager>() ?? sourceRoomScript.gameObject.AddComponent<CustomSupportManager>();
            customSupportManager.EnterSupportMode(targetRoomScript, sourceRoomScript, mainScript, guiMain);
            sourceRoomScript.DisableOutlineLayer();
            targetRoomScript.DisableOutlineLayer();
        }
    }
}
