using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Studios.Modules.HarmonyPatches
{
    public class buildRoomScript_Patch
    {
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(buildRoomScript), "CreateRoom")]
        public static void AfterObjectAdded()
        {
            // Roomオブジェクトが追加された後に実行されるコード
            CustomSupportManager.CacheRoomScripts();
        }
    }
}
