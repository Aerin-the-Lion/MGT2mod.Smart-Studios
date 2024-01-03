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
//Menu_Unterstuetzen.Acceptによって、SupportをRoomに置く。なので、Acceptを改造する必要がある。
//Acceptの処理が入ったら、そのままそのStudioの処理を自動で稼働するようにする。

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
        if (!___mS_)
        {
            return false;
        }
        bool mouseButtonUp = Input.GetMouseButtonUp(0);
        ___pOS_.disableMouseButton = mouseButtonUp;
        RaycastHit raycastHit;

        // ここで、マウスカーソルの位置を取得している。
        if (Physics.Raycast(___myCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)), out raycastHit, 200f, __instance.layerMaskFloor))
        {
            float x = raycastHit.point.x;
            float z = raycastHit.point.z;
            int numX = Mathf.RoundToInt(x);
            int numZ = Mathf.RoundToInt(z);
            //
            if (___mapS_.mapRoomID[numX, numZ] != 1)
            {
                if (___mapS_.mapRoomScript[numX, numZ])
                {
                    //マウスカーソルを動かした際の処理
                    if (___roomOutlineOld != ___mapS_.mapRoomScript[numX, numZ])
                    {
                        if (___roomOutlineOld)
                        {
                            ___roomOutlineOld.DisableOutlineLayer();
                        }
                        ___roomOutlineOld = ___mapS_.mapRoomScript[numX, numZ];
                        // ここで、部屋の許容判定を行っている。※オリジナル
                        if (___mapS_.mapRoomScript[numX, numZ].typ == __instance.rS_.typ && ___mapS_.mapRoomScript[numX, numZ].myID != __instance.rS_.myID)
                        {
                            ___mapS_.mapRoomScript[numX, numZ].SetOutlineLayer();
                        }

                            // ここで、追加の部屋の許容判定を行っている。
                            // サポート先がGame Development Studioの場合かつ、サポート元が__instance.rS_.typ=3,4,5,10の場合
                            int[] validTypes = { 3, 4, 5, 10 }; // 有効なサポート元のタイプ
                            if (Array.IndexOf(validTypes, __instance.rS_.typ) >= 0 && ___mapS_.mapRoomScript[numX, numZ].typ == 1 && ___mapS_.mapRoomScript[numX, numZ].myID != __instance.rS_.myID)
                        {
                            ___mapS_.mapRoomScript[numX, numZ].SetOutlineLayer();
                        }
                    }

                    //マウスをクリックした時の処理
                    if (mouseButtonUp)
                    {
                            // ここで、部屋の許容判定を行っている。※オリジナル
                            if (___mapS_.mapRoomScript[numX, numZ].typ == __instance.rS_.typ && ___mapS_.mapRoomScript[numX, numZ].myID != __instance.rS_.myID)
                        {
                            Traverse Accept = Traverse.Create(__instance).Method("Accept", new object[]{ ___mapS_.mapRoomScript[numX, numZ]});
                            Accept.GetValue();
                            return false;
                        }

                        // ここで、追加の部屋の許容判定を行っている。
                        // サポート先がGame Development Studioの場合かつ、サポート元が__instance.rS_.typ=3,4,5,10の場合
                        int[] validTypes = { 3, 4, 5, 10 }; // 有効なサポート元のタイプ
                        if (Array.IndexOf(validTypes, __instance.rS_.typ) >= 0 && ___mapS_.mapRoomScript[numX, numZ].typ == 1 && ___mapS_.mapRoomScript[numX, numZ].myID != __instance.rS_.myID)
                        {
                            Traverse Accept = Traverse.Create(__instance).Method("Accept", new object[] { ___mapS_.mapRoomScript[numX, numZ] });
                            Accept.GetValue();
                            return false;
                        }

                        ___sfx_.PlaySound(2, true);
                        return false;
                    }
                }
            }
            else if (___roomOutlineOld)
            {
                ___roomOutlineOld.DisableOutlineLayer();
                ___roomOutlineOld = null;
                return false;
            }
        }
        else if (___roomOutlineOld)
        {
            ___roomOutlineOld.DisableOutlineLayer();
            ___roomOutlineOld = null;
        }

        return false;
    }

            // RoomScriptクラスのAcceptメソッドに対するPrefix
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_Unterstuetzen), "Accept")]
        // Prefixメソッド
        static bool Prefix(Menu_Unterstuetzen __instance, ref roomScript script_, ref roomScript ___rS_, ref mainScript ___mS_, ref GUI_Main ___guiMain_)
        {
            //分かりづらいので、変数名を変更
            roomScript srcRoomScript = ___rS_;
            roomScript destRoomScript = script_;

            if (srcRoomScript == script_)
            {
                return false;
            }

            //オリジナルの処理
            if (srcRoomScript && destRoomScript && srcRoomScript.typ == destRoomScript.typ)
            {
                for (int i = 0; i < ___mS_.arrayRoomScripts.Length; i++)
                {
                    if (___mS_.arrayRoomScripts[i] && ___mS_.arrayRoomScripts[i].taskGameObject)
                    {
                        taskUnterstuetzen taskUnterstuetzen = ___mS_.arrayRoomScripts[i].GetTaskUnterstuetzen();
                        if (taskUnterstuetzen && taskUnterstuetzen.roomID == srcRoomScript.myID)
                        {
                            taskUnterstuetzen.Abbrechen();
                        }
                    }
                }
                if (script_.taskID != -1)
                {
                    GameObject gameObject = GameObject.Find("Task_" + destRoomScript.taskID.ToString());
                    if (gameObject && gameObject.GetComponent<taskUnterstuetzen>())
                    {
                        gameObject.GetComponent<taskUnterstuetzen>().Abbrechen();
                    }
                }
                taskUnterstuetzen taskUnterstuetzen2 = ___guiMain_.AddTask_Unterstuetzen();
                taskUnterstuetzen2.Init(false);
                taskUnterstuetzen2.roomID = destRoomScript.myID;
                srcRoomScript.taskID = taskUnterstuetzen2.myID;
                srcRoomScript.DisableOutlineLayer();
                destRoomScript.DisableOutlineLayer();
            }

            if(CustomSupportStatus.IsSuitableCustomSupportForGameDev(srcRoomScript, destRoomScript))
            {
                //___rS_の元のGameObjectにSmartStudiosCustomSupportManagerをアタッチさせる。
                if (srcRoomScript.gameObject.GetComponent<CustomSupportManager>() == null)
                {
                    srcRoomScript.gameObject.AddComponent(typeof(CustomSupportManager));
                }

                CustomSupportManager CustomSupportManager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
                CustomSupportManager.EnterSupportMode(destRoomScript, srcRoomScript, ___mS_, ___guiMain_);
                srcRoomScript.DisableOutlineLayer();
                destRoomScript.DisableOutlineLayer();
            }

                __instance.BUTTON_Close();
                return false;
            }
    }
}
