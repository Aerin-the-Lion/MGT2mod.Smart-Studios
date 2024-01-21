using BepInEx;
using HarmonyLib;
using Smart_Studios.Modules.Hooks;
using Smart_Studios.Config;


namespace Smart_Studios
{
    [BepInPlugin(SmartStudios.PluginGuid, SmartStudios.PluginName, SmartStudios.PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    internal class SmartStudios : BaseUnityPlugin
    {
        public const string PluginGuid = "me.Aerin.MGT2mod.SmartStudios";
        public const string PluginName = "Smart Studios";
        public const string PluginVersion = "1.1.6.0";

        void Awake()
        {
            ConfigManager configManager = new ConfigManager(Config);
            LoadHooks();
        }

        void LoadHooks()
        {
            Harmony.CreateAndPatchAll(typeof(Menu_Unterstuetzen_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(taskUnterstuetzen_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(buildRoomScript_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(mapScript_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(Menu_W_Aufgabe_Abbrechen_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(savegameScript_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(Menu_NewGameSettings_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(roomButtonScript_Patch), PluginGuid);
            Harmony.CreateAndPatchAll(typeof(Abbrechen_Patch), PluginGuid);


        }
    }


}
