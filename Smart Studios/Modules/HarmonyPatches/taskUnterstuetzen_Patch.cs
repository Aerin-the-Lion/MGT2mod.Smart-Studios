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
        private static float updateInterval = 0.5f; // 更新間隔を秒で設定
        private static float timer = 0f;

        /// <summary>
        /// ----------------------------------------------------
        /// SupportモードのSmart Studios用拡張Harmony Patch
        /// ----------------------------------------------------
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
        static void TryToAutoStartStudiosAfterUpdate_Postfix(taskUnterstuetzen __instance, ref roomScript ___rS_)
        {
            timer += Time.deltaTime;

            if (timer >= updateInterval)
            {
                GetCacheRoomScriptsWhenStart();
                if (!IsValidForCustomSupport(__instance, ___rS_))
                {
                    return;
                }
                AttachCustomSupportManagerIfNeeded(__instance);
                PerformCustomSupportActions(__instance, ___rS_);
                timer = 0f;
            }
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

            //これで、特定のtaskUnterstuetzenをtaskGameObjectに所持しているroomScriptのみ取得する。
            var srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(instance.name);
            if (srcRoomScript == null) return;

            //念のため、srcRoomScriptのtaskGameObjectがtaskUnterstuetzenを所持しているかどうかを確認する。
            if (srcRoomScript.taskGameObject.GetComponent<taskUnterstuetzen>() == null) { Debug.Log("DEBUG: Detected unusual behavior.");  return; }

            if(CustomSupportUtilities.IsTypeOfQaStudio(srcRoomScript))
            {
                Debug.Log("QA Studio : Auto Start");
                QaStudioScriptManager qA_ScriptManager = new QaStudioScriptManager();
                qA_ScriptManager.AutoStart(srcRoomScript, destGameScript); //これsrcRoomScript入れないといけない。Support元で行わないといけないので。
            }
            else if(CustomSupportUtilities.IsTypeOfGraphicStudio(srcRoomScript))
            {
                Debug.Log("Graphic Studio : Auto Start");
                GraphicsStudioScriptManager graphicsStudioScriptManager = new GraphicsStudioScriptManager();
                graphicsStudioScriptManager.AutoStart(srcRoomScript, destGameScript);
            }
            else if(CustomSupportUtilities.IsTypeOfSoundStudio(srcRoomScript))
            {
                Debug.Log("Sound Studio : Auto Start");
                SoundStudioScriptManager soundStudioScriptManager = new SoundStudioScriptManager();
                soundStudioScriptManager.AutoStart(srcRoomScript, destGameScript);
            }
            else if(CustomSupportUtilities.IsTypeOfMotionCaptureStudio(srcRoomScript))
            {
                Debug.Log("Motion Capture Studio : Auto Start");
                MotionCaptureStudioScriptManager motionCaptureStudioScriptManager = new MotionCaptureStudioScriptManager();
                motionCaptureStudioScriptManager.AutoStart(srcRoomScript, destGameScript);
            }
        }
    }
}
