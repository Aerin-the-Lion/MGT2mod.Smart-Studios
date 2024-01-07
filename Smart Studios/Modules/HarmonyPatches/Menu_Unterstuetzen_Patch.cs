using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using Smart_Studios.Modules.CustomSupport;


namespace Smart_Studios.Modules.HarmonyPatch
{
    /// <summary>
    /// Menu_Support in English
    /// </summary>
    public class Menu_Unterstuetzen_Patch: MonoBehaviour
    {

        //４種のStudioを判定処理
        //Game Development Studioに、4種のStudioをSupportとして割当させる処理を追加

        /// <summary>
        /// Support選択時、マウスカーソルの移動処理かつ部屋の許容判定処理
        /// まずこちらを改変しない限り、選択自体ができないため、こちらを改変する必要がある。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___rS_"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_Unterstuetzen), "MouseMovement")]
        static bool MouseMovement_Prefix(Menu_Unterstuetzen __instance, mainScript ___mS_, pickObjectScript ___pOS_, Camera ___myCamera, mapScript ___mapS_, roomScript ___roomOutlineOld, sfxScript ___sfx_)
        {
            Menu_Unterstuetzen instance = __instance;
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
                }
            }
            else if (roomOutlineOld)
            {
                roomOutlineOld.DisableOutlineLayer();
                roomOutlineOld = null;
            }

            //フィールドの値を更新
            __instance = instance;
            Traverse.Create(__instance).Field("roomOutlineOld").SetValue(roomOutlineOld);

            return false;
        }

        private static void HandleRoomOutlineChange(ref roomScript roomOutlineOld, roomScript currentRoomScript, Menu_Unterstuetzen instance, int numX, int numZ, bool mouseButtonUp, sfxScript sfxScript)
        {
            if (roomOutlineOld != currentRoomScript)
            {
                roomOutlineOld?.DisableOutlineLayer();
                roomOutlineOld = currentRoomScript;
                if (ShouldSetOutline(instance.rS_, currentRoomScript, currentRoomScript.myID, instance.rS_.myID))
                {
                    currentRoomScript.SetOutlineLayer();
                }
            }

            if (mouseButtonUp)
            {
                if (ShouldSetOutline(instance.rS_, currentRoomScript, currentRoomScript.myID, instance.rS_.myID))
                {
                    AcceptRoomChange(instance, currentRoomScript);
                    return;
                }
                sfxScript.PlaySound(2, true);
                return;
            }
        }


        private static bool ShouldSetOutline(roomScript room, roomScript currentRoom, int currentRoomId, int roomId)
        {
            /*
            typ = 1 ; Game Development
            typ = 3 ; QA
            typ = 4 ; Graphics
            typ = 5 ; Music
            typ = 10 ; Motion Capture
            */
            bool originalLogic = (room.typ == currentRoom.typ && currentRoomId != roomId);
            bool customSupportLogic = CustomSupportUtilities.IsSuitableCustomSupportForGameDev(room, currentRoom);
            return originalLogic || customSupportLogic;
        }

        private static void AcceptRoomChange(Menu_Unterstuetzen instance, roomScript currentRoomScript)
        {
            Traverse.Create(instance).Method("Accept", new object[] { currentRoomScript }).GetValue();
        }


        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_Unterstuetzen), "Accept")]
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

            if (CustomSupportUtilities.IsSuitableCustomSupportForGameDev(sourceRoomScript, targetRoomScript))
            {
                AttachCustomSupportManagerToGameObject(sourceRoomScript, targetRoomScript, mainScript, guiMain);
            }

            instance.BUTTON_Close();

            //フィールドの値を更新
            Traverse.Create(__instance).Field("rS_").SetValue(sourceRoomScript);
            Traverse.Create(__instance).Field("guiMain_").SetValue(guiMain);

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
