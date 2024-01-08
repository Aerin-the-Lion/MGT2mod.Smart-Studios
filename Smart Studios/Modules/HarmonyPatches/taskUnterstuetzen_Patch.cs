using HarmonyLib;
using UnityEngine;
using Smart_Studios.Modules.CustomSupport;
using Smart_Studios.Modules.Studios;
using System.Collections.Generic;
using System.Linq;

namespace Smart_Studios.Modules.HarmonyPatches
{
    /// <summary>
    /// taskSupport in English
    /// </summary>
    internal class taskUnterstuetzen_Patch
    {
        //セーフ処理のため、一度だけキャッシュするための変数
        private static bool hasCachedRoomScripts = false;


        /*
        /// <summary>
        /// taskUnterstuetzenのUpdate関数のPostfix
        /// 英語だと、taskSupport、要はSupport時の１フレーム毎の処理を行う関数のパッチ。
        /// Postfixなので、元の関数の処理が終わった後に実行されるため、他modの影響を受けにくい。
        /// 特定のタイプ（3, 4, 5, 10）の部屋から、Game Development StudioにSupportを割り当てている場合にのみ、AutoStart処理が適用される。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___rS_"></param>
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(taskUnterstuetzen), "Update")]
        static void FindMyRoom_Postfix(taskUnterstuetzen __instance, ref roomScript ___rS_)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            if (srcRoomScript == null) { return; }
            if (CustomSupportUtilities.IsSuitableCustomSupportForGameDev(srcRoomScript, ___rS_))
            {
                //taskGameObjectがnullの場合は、引き続きSupport待機処理を行う。　taskGameObject …　Game Developmentでゲーム開発時のゲームのGameObject
                GameObject taskGameObject = ___rS_.taskGameObject;
                if (taskGameObject == null) { return; }

                taskGame destTaskGame = taskGameObject.GetComponent<taskGame>();
                gameScript destGameScript = destTaskGame.gS_;
                if (destGameScript == null) { return; }

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
        */

        /// <summary>
        /// taskUnterstuetzenのUpdate関数のPostfix
        /// 英語だと、taskSupport、要はSupport時の１フレーム毎の処理を行う関数のパッチ。
        /// Postfixなので、元の関数の処理が終わった後に実行されるため、他modの影響を受けにくい。
        /// 特定のタイプ（3, 4, 5, 10）の部屋から、Game Development StudioにSupportを割り当てている場合にのみ、AutoStart処理が適用される。
        /// 一応パフォーマンスに配慮しているが、気にかける。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___rS_">Support先のroomScript、distRoomScript</param>
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(taskUnterstuetzen), "Update")]
        static void Update_Postfix(taskUnterstuetzen __instance, ref roomScript ___rS_)
        {
            Debug.Log("taskUnterstuetzen.Update_Postfix 1");
            GetCacheRoomScriptsWhenStart();
            Debug.Log("taskUnterstuetzen.Update_Postfix 2");
            if (!IsValidForCustomSupport(__instance, ___rS_))
            {
                return;
            }
            Debug.Log("taskUnterstuetzen.Update_Postfix 3");
            AttachCustomSupportManagerIfNeeded(__instance);
            Debug.Log("taskUnterstuetzen.Update_Postfix 4");
            PerformCustomSupportActions(__instance, ___rS_);
            Debug.Log("taskUnterstuetzen.Update_Postfix 5");
        }

        /// <summary>
        /// Start時、一度だけキャッシュする。
        /// </summary>
        private static void GetCacheRoomScriptsWhenStart()
        {
            if (hasCachedRoomScripts) { return; }
            CustomSupportManager.CacheRoomScripts();
            hasCachedRoomScripts = true;
        }
        private static bool IsValidForCustomSupport(taskUnterstuetzen instance, roomScript roomScript)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(instance.name);
            return srcRoomScript != null && CustomSupportUtilities.IsSuitableCustomSupportForGameDev(srcRoomScript, roomScript);
        }

        private static void AttachCustomSupportManagerIfNeeded(taskUnterstuetzen instance)
        {
            var srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(instance.name);
            if (srcRoomScript.gameObject.GetComponent<CustomSupportManager>() == null)
            {
                srcRoomScript.gameObject.AddComponent<CustomSupportManager>();
            }
        }

        private static void PerformCustomSupportActions(taskUnterstuetzen instance, roomScript roomScript)
        {
            //taskGameObjectがnullの場合は、引き続きSupport待機処理を行う。　taskGameObject …　Game Developmentでゲーム開発時のゲームのGameObject
            GameObject taskGameObject = roomScript.taskGameObject;
            if (taskGameObject == null) return;

            taskGame destTaskGame = taskGameObject.GetComponent<taskGame>();
            if (destTaskGame == null) return;
            gameScript destGameScript = destTaskGame?.gS_;
            if (destGameScript == null) return;

            var srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(instance.name);
            QA_ScriptManager qA_ScriptManager = new QA_ScriptManager();
            qA_ScriptManager.AutoStart(srcRoomScript, destGameScript); //これsrcRoomScript入れないといけない。Support元で行わないといけないので。
        }
    }
}
