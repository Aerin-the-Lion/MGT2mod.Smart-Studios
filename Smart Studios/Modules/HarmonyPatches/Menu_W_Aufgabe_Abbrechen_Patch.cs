
using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using UnityEngine;

namespace Smart_Studios.Modules.HarmonyPatches
{
    /// <summary>
    /// --------------------------------------------------------
    /// !!キャンセルボタンの処理を変更するパッチ!!
    /// --------------------------------------------------------
    /// 英語の場合、を「Menu_Window_Cancel_Task」という記載。
    /// CustomSupport -> QA, Bugfixing, Polishingなどの動作中にキャンセル時に、CustomSupportManagerを削除するようにする。
    /// CustomSupport -> QA, Bugfixing, Polishingなどの動作中にキャンセル時に、CustomSupport関連のオブジェクトを削除するようにする。
    /// </summary>
    public class Menu_W_Aufgabe_Abbrechen_Patch
    {
        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_W_Aufgabe_Abbrechen), "BUTTON_Yes")]
        public static bool BUTTON_Yes_Postfix(Menu_W_Aufgabe_Abbrechen __instance, roomScript ___rS_)
        {
            //original previous code
            //...
            //if (gameObject.GetComponent<taskGameplayVerbessern>())
            //{
            //gameObject.GetComponent<taskGameplayVerbessern>().Abbrechen();
            //...
            //}

            //AFTER
            taskGameplayVerbessern myTaskGameplayVerbessern = Traverse.Create(___rS_).Field("myTaskGameplayVerbessern").GetValue<taskGameplayVerbessern>();
            taskGrafikVerbessern myTaskGrafikVerbessern = Traverse.Create(___rS_).Field("myTaskGrafikVerbessern").GetValue<taskGrafikVerbessern>();
            taskSoundVerbessern myTaskSoundVerbessern = Traverse.Create(___rS_).Field("myTaskSoundVerbessern").GetValue<taskSoundVerbessern>();
            taskAnimationVerbessern taskAnimationVerbessern = Traverse.Create(___rS_).Field("myTaskAnimationVerbessern").GetValue<taskAnimationVerbessern>();
            taskUnterstuetzen myTaskUnterstuetzen = Traverse.Create(___rS_).Field("myTaskUnterstuetzen").GetValue<taskUnterstuetzen>();
            taskBugfixing myTaskBugfixing = Traverse.Create(___rS_).Field("myTaskBugfixing").GetValue<taskBugfixing>();
            taskPolishing myTaskPolishing = Traverse.Create(___rS_).Field("myTaskPolishing").GetValue<taskPolishing>();

            //QA Studioのキャンセルの場合
            if (myTaskGameplayVerbessern && myTaskUnterstuetzen)
            {
                //myTaskGameplayVerbessern.Abbrechen();
                myTaskUnterstuetzen.Abbrechen();
                if(___rS_.gameObject.GetComponent<CustomSupportManager>())
                {
                    GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
                }
            }
            //バグフィックスのキャンセルの場合
            if (myTaskBugfixing && myTaskUnterstuetzen)
            {
                //myTaskBugfixing.Abbrechen();
                myTaskUnterstuetzen.Abbrechen();
                if (___rS_.gameObject.GetComponent<CustomSupportManager>())
                {
                    GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
                }
            }
            //ポリッシュのキャンセルの場合
            if (myTaskPolishing && myTaskUnterstuetzen)
            {
                //myTaskPolishing.Abbrechen();
                myTaskUnterstuetzen.Abbrechen();
                if (___rS_.gameObject.GetComponent<CustomSupportManager>())
                {
                    GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
                }
            }

            //Graphics Studioのキャンセルの場合
            if(myTaskGrafikVerbessern && myTaskUnterstuetzen)
            {
                //myTaskGrafikVerbessern.Abbrechen();
                myTaskUnterstuetzen.Abbrechen();
                if (___rS_.gameObject.GetComponent<CustomSupportManager>())
                {
                    GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
                }
            }

            //Sound Studioのキャンセルの場合
            if (myTaskSoundVerbessern && myTaskUnterstuetzen)
            {
                //myTaskSoundVerbessern.Abbrechen();
                myTaskUnterstuetzen.Abbrechen();
                if (___rS_.gameObject.GetComponent<CustomSupportManager>())
                {
                    GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
                }
            }

            //Motion Capture Studioのキャンセルの場合
            if(taskAnimationVerbessern && myTaskUnterstuetzen)
            {
                //taskAnimationVerbessern.Abbrechen();
                myTaskUnterstuetzen.Abbrechen();
                if (___rS_.gameObject.GetComponent<CustomSupportManager>())
                {
                    GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
                }
            }

            return true;
        }
    }
}
