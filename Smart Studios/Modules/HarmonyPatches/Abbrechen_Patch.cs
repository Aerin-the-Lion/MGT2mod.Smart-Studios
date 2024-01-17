using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Studios.Modules.HarmonyPatches
{
    internal class Abbrechen_Patch
    {
        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(taskAnimationVerbessern), "Abbrechen")]
        public static bool taskAnimationVerbessern_Abbrechen(taskAnimationVerbessern __instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null){return true;}

            manager.SetIsInSupportMode(true);
            manager.SetIsImprovingGame(false);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(taskBugfixing), "Abbrechen")]
        public static bool taskBugfixing_Abbrechen(taskBugfixing __instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null) { return true; }

            manager.SetIsInSupportMode(true);
            manager.SetIsImprovingGame(false);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(taskPolishing), "Abbrechen")]
        public static bool taskPolishing_Abbrechen(taskPolishing __instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null) { return true; }

            manager.SetIsInSupportMode(true);
            manager.SetIsImprovingGame(false);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(taskSoundVerbessern), "Abbrechen")]
        public static bool taskSoundVerbessern_Abbrechen(taskSoundVerbessern __instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null) { return true; }

            manager.SetIsInSupportMode(true);
            manager.SetIsImprovingGame(false);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(taskGrafikVerbessern), "Abbrechen")]
        public static bool taskGrafikVerbessern_Abbrechen(taskGrafikVerbessern __instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null) { return true; }

            manager.SetIsInSupportMode(true);
            manager.SetIsImprovingGame(false);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(taskGameplayVerbessern), "Abbrechen")]
        public static bool taskGameplayVerbessern_Abbrechen(taskGameplayVerbessern __instance)
        {
            roomScript srcRoomScript = CustomSupportManager.FindRoomScriptForInstance(__instance.name);
            CustomSupportManager manager = srcRoomScript.gameObject.GetComponent<CustomSupportManager>();
            if (manager == null) { return true; }

            manager.SetIsInSupportMode(true);
            manager.SetIsImprovingGame(false);
            return true;
        }
    }
}
