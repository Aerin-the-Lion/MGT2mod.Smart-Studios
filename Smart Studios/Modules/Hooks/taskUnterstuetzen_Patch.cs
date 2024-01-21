using HarmonyLib;
using UnityEngine;
using Smart_Studios.Modules.CustomSupport;
using Smart_Studios.Modules.Studios;
using System.Collections.Generic;
using System.Linq;

namespace Smart_Studios.Modules.Hooks
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
                if (!IsCustomSupportReady(__instance))
                {
                    return;
                }
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

        private static bool IsCustomSupportReady(taskUnterstuetzen instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            return  manager != null && manager.isInSupportMode && !manager.isImprovingGame;
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
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(instance.name);
            if (srcRoomScript == null) return;
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null) return;

            //念のため、srcRoomScriptのtaskGameObjectがtaskUnterstuetzenを所持しているかどうかを確認する。
            if (srcRoomScript.taskGameObject.GetComponent<taskUnterstuetzen>() == null) { Debug.Log("DEBUG: Detected unusual behavior.");  return; }

            Debug.Log("////////////////////////////////////////////////////////////////////////////");
            Debug.Log("// Smart Studios - DEBUG MODE START//");
            Debug.Log("//////////////////////////////////////////////");
            Debug.Log("-// main checking //-----------------------------------------");
            Debug.Log("DEBUG: srcRoomScript.name = " + srcRoomScript.name);
            Debug.Log("DEBUG: srcRoomScript.taskGameObject.name = " + srcRoomScript.taskGameObject.name);
            Debug.Log("DEBUG: destTaskGame.name = " + destTaskGame.name);
            Debug.Log("DEBUG: destGameScript.name = " + destGameScript.name);
            Debug.Log("-// roomScript task checking... //-----------------------------------------");
            Debug.Log("DEBUG: srcRoomScript.taskGameObject.GetComponent<taskUnterstuetzen>() = " + srcRoomScript.taskGameObject.GetComponent<taskUnterstuetzen>());
            Debug.Log("DEBUG: srcRoomScript.taskGameObject.GetComponent<taskGameplayVerbessern>() = " + srcRoomScript.taskGameObject.GetComponent<taskGameplayVerbessern>());
            Debug.Log("DEBUG: srcRoomScript.taskGameObject.GetComponent<taskGrafikVerbessern>() = " + srcRoomScript.taskGameObject.GetComponent<taskGrafikVerbessern>());
            Debug.Log("DEBUG: srcRoomScript.taskGameObject.GetComponent<taskSoundVerbessern>() = " + srcRoomScript.taskGameObject.GetComponent<taskSoundVerbessern>());
            Debug.Log("DEBUG: srcRoomScript.taskGameObject.GetComponent<taskAnimationVerbessern>() = " + srcRoomScript.taskGameObject.GetComponent<taskAnimationVerbessern>());
            Debug.Log("-// CustomSupportManager checking //-----------------------------------------");
            Debug.Log("DEBUG: manager.isInSupportMode = " + manager.isInSupportMode);
            Debug.Log("DEBUG: manager.isImprovingGame = " + manager.isImprovingGame);
            Debug.Log("//////////////////////////////////////////////");
            Debug.Log("// Smart Studios - DEBUG MODE END //");
            Debug.Log("//////////////////////////////////////////////");




            if (CustomSupportUtilities.IsTypeOfQaStudio(srcRoomScript))
            {
                Debug.Log("+++++++++++++++++++++++++++++++++");
                Debug.Log("QA Studio : Auto Start");
                Debug.Log("+++++++++++++++++++++++++++++++++");
                QaStudioScriptManager qA_ScriptManager = new QaStudioScriptManager();
                qA_ScriptManager.AutoStart(srcRoomScript, destGameScript); //これsrcRoomScript入れないといけない。Support元で行わないといけないので。
                manager.SetIsInSupportMode(false);
                manager.SetIsImprovingGame(true);
            }
            else if(CustomSupportUtilities.IsTypeOfGraphicStudio(srcRoomScript))
            {
                Debug.Log("+++++++++++++++++++++++++++++++++");
                Debug.Log("Graphic Studio : Auto Start");
                Debug.Log("+++++++++++++++++++++++++++++++++");
                GraphicsStudioScriptManager graphicsStudioScriptManager = new GraphicsStudioScriptManager();
                graphicsStudioScriptManager.AutoStart(srcRoomScript, destGameScript);
                manager.SetIsInSupportMode(false);
                manager.SetIsImprovingGame(true);
            }
            else if(CustomSupportUtilities.IsTypeOfSoundStudio(srcRoomScript))
            {
                Debug.Log("+++++++++++++++++++++++++++++++++");
                Debug.Log("Sound Studio : Auto Start");
                Debug.Log("+++++++++++++++++++++++++++++++++");
                SoundStudioScriptManager soundStudioScriptManager = new SoundStudioScriptManager();
                soundStudioScriptManager.AutoStart(srcRoomScript, destGameScript);
                manager.SetIsInSupportMode(false);
                manager.SetIsImprovingGame(true);
            }
            else if(CustomSupportUtilities.IsTypeOfMotionCaptureStudio(srcRoomScript))
            {
                Debug.Log("+++++++++++++++++++++++++++++++++");
                Debug.Log("Motion Capture Studio : Auto Start");
                Debug.Log("+++++++++++++++++++++++++++++++++");
                MotionCaptureStudioScriptManager motionCaptureStudioScriptManager = new MotionCaptureStudioScriptManager();
                motionCaptureStudioScriptManager.AutoStart(srcRoomScript, destGameScript);
                manager.SetIsInSupportMode(false);
                manager.SetIsImprovingGame(true);
            }
            else
            {
                Debug.Log("+++++++++++++++++++++++++++++++++");
                Debug.Log("DEBUG: Detected unusual behavior. the type of Studio is odd.");
                Debug.Log("+++++++++++++++++++++++++++++++++");
            }
        }
    }
}
