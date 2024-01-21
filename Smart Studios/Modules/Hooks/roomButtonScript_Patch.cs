using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.Hooks
{
    public class roomButtonScript_Patch
    {
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(roomButtonScript), "BUTTON_Unterstuetzung_Abbrechen")]
        public static void BUTTON_Unterstuetzung_Abbrechen_PostPatch(roomScript ___rS_)
        {
            // Roomオブジェクトが追加された後に実行されるコード
            if (___rS_.gameObject.GetComponent<CustomSupportManager>())
            {
                GameObject.Destroy(___rS_.gameObject.GetComponent<CustomSupportManager>());
            }
        }
    }
}
