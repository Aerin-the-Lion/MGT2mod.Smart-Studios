using BepInEx;
using HarmonyLib;
using Smart_Studios.Modules.HarmonyPatches;
using Smart_Studios.Config;

namespace Smart_Studios
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    public class SmartStudios : BaseUnityPlugin
    {
        public const string PluginGuid = "me.Aerin_the_Lion.Mad_Games_Tycoon_2.plugins.SmartStudios";
        public const string PluginName = "Smart Studios";
        public const string PluginVersion = "1.1.0.0";

        void Awake()
        {
            ConfigManager configManager = new ConfigManager(Config);
            LoadHarmonyPatches();
        }

        void LoadHarmonyPatches()
        {
            Harmony.CreateAndPatchAll(typeof(Menu_Unterstuetzen_Patch));
            Harmony.CreateAndPatchAll(typeof(taskUnterstuetzen_Patch));
            Harmony.CreateAndPatchAll(typeof(buildRoomScript_Patch));
            Harmony.CreateAndPatchAll(typeof(mapScript_Patch));
            Harmony.CreateAndPatchAll(typeof(Menu_W_Aufgabe_Abbrechen_Patch));
            Harmony.CreateAndPatchAll(typeof(savegameScript_Patch));
            Harmony.CreateAndPatchAll(typeof(Menu_NewGameSettings_Patch));
            Harmony.CreateAndPatchAll(typeof(roomButtonScript_Patch));


        }
    }


}
