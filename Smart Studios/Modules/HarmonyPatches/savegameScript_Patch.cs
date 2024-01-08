using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Studios.Modules.HarmonyPatches
{
    internal class savegameScript_Patch
    {
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(savegameScript), "Load")]
        public static void AfterLoadGame_Patch()
        {
            // Roomオブジェクトが追加された後に実行されるコード
            CustomSupportManager.CacheRoomScripts();
        }
    }
}
