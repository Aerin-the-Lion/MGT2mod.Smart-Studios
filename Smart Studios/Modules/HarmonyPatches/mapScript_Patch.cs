using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Studios.Modules.HarmonyPatches
{
    public class mapScript_Patch
    {
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(mapScript), "RemoveRoom")]
        public static void AfterObjectRemoved()
        {
            // Roomオブジェクトが追加された後に実行されるコード
            CustomSupportManager.CacheRoomScripts();
        }
    }
}
