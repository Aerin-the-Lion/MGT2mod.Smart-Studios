using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios
{
    internal class taskUnterstuetzen_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(taskUnterstuetzen), "Update")]
        static void FindMyRoom_Postfix(taskUnterstuetzen __instance, ref roomScript ___rS_)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            if (srcRoomScript == null) { return; }
            bool hoge = CustomSupportStatus.IsSuitableCustomSupportForGameDev(srcRoomScript, ___rS_);
            if (CustomSupportStatus.IsSuitableCustomSupportForGameDev(srcRoomScript, ___rS_))
            {
                GameObject taskGameObject = ___rS_.taskGameObject;
                if (taskGameObject == null)
                {
                    return;
                }
                taskGame destTaskGame = taskGameObject.GetComponent<taskGame>();
                gameScript destGameScript = destTaskGame.gS_;

                //___rS_の元のGameObjectにSmartStudiosCustomSupportManagerをアタッチさせる。
                if (srcRoomScript.gameObject.GetComponent<CustomSupportManager>() == null)
                {
                    srcRoomScript.gameObject.AddComponent(typeof(CustomSupportManager));
                }
                QA_ScriptManager qA_ScriptManager = new QA_ScriptManager();
                qA_ScriptManager.AutoStart(srcRoomScript, destGameScript); //これsrcRoomScript入れないといけない。Support元で行わないといけないので。
                //status.SetCustomSupportWaiting(false);
            }
        }
    }
}
