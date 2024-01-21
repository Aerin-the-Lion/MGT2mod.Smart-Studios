using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Studios.Modules.Hooks
{
    internal class Menu_NewGameSettings_Patch
    {
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_NewGameSettings), "BUTTON_OK")]
        public static void AfterLoadGame_Patch()
        {
            // Roomオブジェクトが追加された後に実行されるコード
            CustomSupportManager.CacheRoomScripts();
        }
    }
}
