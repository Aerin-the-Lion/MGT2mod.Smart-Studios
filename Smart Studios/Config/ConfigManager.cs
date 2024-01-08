using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace Smart_Studios.Config
{
    [BepInPlugin(SmartStudios.PluginGuid, SmartStudios.PluginName, SmartStudios.PluginVersion)]
    public class ConfigManager : BaseUnityPlugin
    {
        private ConfigFile ConfigFile { get; set; }

        public ConfigManager(ConfigFile configFile)
        {
            ConfigFile = configFile;
            LoadConfig();
        }

        // Config sections
        private const string ModSettingsSection = "0. MOD Settings";
        private const string QaStudioSettingSection = "1. Smart Studios : QA Studio";
        private const string CharacterEditorPerksSection = "2. Smart Studios : xxxx";

        // Config entries
        public static ConfigEntry<bool> IsModEnabled { get; private set; }
        public static ConfigEntry<bool> IsQaAutoBugfixing { get; private set; }
        public static ConfigEntry<bool> IsQaAllStudioFeaturesEnabled { get; private set; }
        public static ConfigEntry<bool> IsQaAllEnabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel1Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel2Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel3Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel4Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel5Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel6Enabled { get; private set; }
        public static List<bool> QaLevels { get; private set; }
        //public ConfigEntry<float> SkillPointIncrement { get; private set; }
        //public ConfigEntry<bool> IsPerkCountIndividual { get; private set; }
        //public ConfigEntry<int> MaxPerksCount { get; private set; }

        private void LoadConfig()
        {
            // Config setting definitions here
            // ----------------------------------------------------------------------------------------------------------------
            // Main Settings
            IsModEnabled = ConfigFile.Bind(
                ModSettingsSection,
                "Activate the MOD",
                true,
                "Toggle 'Enabled' to activate the mod");

            // ----------------------------------------------------------------------------------------------------------------
            // QA Studio Config
            IsQaAutoBugfixing = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : Auto Bugfixing",
                true,
                "Setting this option to 'Enabled' will activate automatic bug fixing after improving gameplay in QA Studio.");

            IsQaAllEnabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : All Studio Features",
                true,
                "Setting this option to 'Enabled' create all the implove gameplay functions of QA Studio during Support.");

            // ----------------------------------------------------------------------------------------------------------------
            // QA Studio Level Config
            
            IsQaLevel1Enabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : Level1 : QA(Balance)",
                true,
                "Setting this option to 'Enabled' create the implove gameplay function of QA Studio during Support.");

            IsQaLevel2Enabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : Level2 : QA(Controls)",
                true,
                "Setting this option to 'Enabled' create the implove gameplay function of QA Studio during Support.");

            IsQaLevel3Enabled = ConfigFile.Bind(
                 QaStudioSettingSection,
                 "QA Studio : Level3 : QA(Performance)",
                 true,
                 "Setting this option to 'Enabled' create the implove gameplay function of QA Studio during Support.");
            IsQaLevel4Enabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : Level4 : QA(User Interface)",
                true,
                "Setting this option to 'Enabled' create the implove gameplay function of QA Studio during Support.");
            IsQaLevel5Enabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : Level5 : QA(Game mechanics)",
                true,
                "Setting this option to 'Enabled' create the implove gameplay function of QA Studio during Support.");
            IsQaLevel6Enabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : Level6 : QA(Level design)",
                true,
                "Setting this option to 'Enabled' create the implove gameplay function of QA Studio during Support.");
            // ----------------------------------------------------------------------------------------------------------------
            InitQaLevels();

            /*
            // ----------------------------------------------------------------------------------------------------------------
            SkillPointDecreaseMultiplier = Config.Bind(
                CharacterEditorSkillPointsSection,
                "Skill Point Decrease Multiplier On Change",
                0.9f,
                new ConfigDescription(
                    "Multiplier applied to total skill points when changing skills or profession, " +
                    "for difficulty adjustment. Default is '0.9'. Set '1.0' for no change.",
                    new AcceptableValueRange<float>(0.2f, 1.0f)));

            // ---------------------------------------------------------------------
            SkillPointIncrement = ConfigFile.Bind(
                "1. Character Editor : Skill Points",
                "Skill Point Increment Value",
                5.0f,
                "Value to adjust skill points. Default is '5.0'.");

            // ---------------------------------------------------------------------
            IsPerkCountIndividual = ConfigFile.Bind(
                "2. Character Editor : Perks",
                "Is Individual Perk Count Applied",
                true,
                "Toggle 'Enabled' for individual perk count limit per character.");

            // ---------------------------------------------------------------------
            MaxPerksCount = ConfigFile.Bind(
                "2. Character Editor : Perks",
                "Total Perks Count",
                4,
                "Maximum allowable perks per character. Default is '4'.");

            */

            // ---------------------------------------------------------------------
            // ---------------------------------------------------------------------
            ConfigFile.SettingChanged += OnConfigSettingChanged;
        }

        private void OnConfigSettingChanged(object sender, SettingChangedEventArgs e)
        {
            // Config setting change handling logic here
            InitQaLevels();
        }

        private void InitQaLevels()
        {
            QaLevels = new List<bool>
            {
                IsQaLevel1Enabled.Value,
                IsQaLevel2Enabled.Value,
                IsQaLevel3Enabled.Value,
                IsQaLevel4Enabled.Value,
                IsQaLevel5Enabled.Value,
                IsQaLevel6Enabled.Value
            };
        }
    }
}