using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace Smart_Studios.Config
{
    [BepInPlugin(SmartStudios.PluginGuid, SmartStudios.PluginName, SmartStudios.PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    public class ConfigManager
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
        private const string GraphicsStudioSettingSection = "2. Smart Studios : Graphics Studio";
        private const string SoundStudioSettingSection = "3. Smart Studios : Sound Studio";
        private const string MotionCaptureStudioSettingSection = "4. Smart Studios : Motion Capture Studio";

        // Config entries
        public static ConfigEntry<bool> IsModEnabled { get; private set; }

        // QA Studio Config ---------------------------------------------------------------
        public static ConfigEntry<bool> IsQaAutoBugfixing { get; private set; }
        public static ConfigEntry<bool> IsQaAllEnabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel1Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel2Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel3Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel4Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel5Enabled { get; private set; }
        public static ConfigEntry<bool> IsQaLevel6Enabled { get; private set; }

        // Graphics Studio Config ---------------------------------------------------------------
        public static ConfigEntry<bool> IsGraphicsAllEnabled { get; private set; }
        public static ConfigEntry<bool> IsGraphicsLevel1Enabled { get; private set; }
        public static ConfigEntry<bool> IsGraphicsLevel2Enabled { get; private set; }
        public static ConfigEntry<bool> IsGraphicsLevel3Enabled { get; private set; }
        public static ConfigEntry<bool> IsGraphicsLevel4Enabled { get; private set; }
        public static ConfigEntry<bool> IsGraphicsLevel5Enabled { get; private set; }
        public static ConfigEntry<bool> IsGraphicsLevel6Enabled { get; private set; }

        // Sound Studio Config ---------------------------------------------------------------
        public static ConfigEntry<bool> IsSoundAllEnabled { get; private set; }
        public static ConfigEntry<bool> IsSoundLevel1Enabled { get; private set; }
        public static ConfigEntry<bool> IsSoundLevel2Enabled { get; private set; }
        public static ConfigEntry<bool> IsSoundLevel3Enabled { get; private set; }
        public static ConfigEntry<bool> IsSoundLevel4Enabled { get; private set; }
        public static ConfigEntry<bool> IsSoundLevel5Enabled { get; private set; }
        public static ConfigEntry<bool> IsSoundLevel6Enabled { get; private set; }

        // Motion Capture Studio Config ---------------------------------------------------------------
        public static ConfigEntry<bool> IsMotionCaptureAllEnabled { get; private set; }
        public static ConfigEntry<bool> IsMotionCaptureLevel1Enabled { get; private set; }
        public static ConfigEntry<bool> IsMotionCaptureLevel2Enabled { get; private set; }
        public static ConfigEntry<bool> IsMotionCaptureLevel3Enabled { get; private set; }
        public static ConfigEntry<bool> IsMotionCaptureLevel4Enabled { get; private set; }
        public static ConfigEntry<bool> IsMotionCaptureLevel5Enabled { get; private set; }
        public static ConfigEntry<bool> IsMotionCaptureLevel6Enabled { get; private set; }

        // List Config ---------------------------------------------------------------
        public static List<bool> QaLevels { get; private set; }
        public static List<bool> GraphicsLevels { get; private set; }
        public static List<bool> SoundLevels { get; private set; }
        public static List<bool> MotionCaptureLevels { get; private set; }

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

            // ----------------------------------------------------------------------------------------------------------------
            // QA Studio Level Config

            IsQaAllEnabled = ConfigFile.Bind(
                QaStudioSettingSection,
                "QA Studio : All Studio Features",
                true,
                "Setting this option to 'Enabled' create all the implove gameplay functions of QA Studio during Support.");

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
            // ----------------------------------------------------------------------------------------------------------------
            // Graphics Studio Config
            IsGraphicsAllEnabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : All Studio Features",
                true,
                "Setting this option to 'Enabled' create all the implove gameplay functions of Graphics Studio during Support.");

            IsGraphicsLevel1Enabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : Level1 : GFX(Environment Graphics)",
                true,
                "Setting this option to 'Enabled' create the High quality Graphics function of Graphics Studio during Support.");

            IsGraphicsLevel2Enabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : Level2 : GFX(GUIs)",
                true,
                "Setting this option to 'Enabled' create the High quality Graphics function of Graphics Studio during Support.");

            IsGraphicsLevel3Enabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : Level3 : GFX(Backgrounds)",
                true,
                "Setting this option to 'Enabled' create the High quality Graphics function of Graphics Studio during Support.");

            IsGraphicsLevel4Enabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : Level4 : GFX(Special effects)",
                true,
                "Setting this option to 'Enabled' create the High quality Graphics function of Graphics Studio during Support.");

            IsGraphicsLevel5Enabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : Level5 : GFX(Characters)",
                true,
                "Setting this option to 'Enabled' create the High quality Graphics function of Graphics Studio during Support.");

            IsGraphicsLevel6Enabled = ConfigFile.Bind(
                GraphicsStudioSettingSection,
                "Graphics Studio : Level6 : GFX(Cut Sequences)",
                true,
                "Setting this option to 'Enabled' create the High quality Graphics function of Graphics Studio during Support.");
            // ----------------------------------------------------------------------------------------------------------------
            InitGraphicsLevels();
            // ----------------------------------------------------------------------------------------------------------------
            // Sound Studio Config
            IsSoundAllEnabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : All Studio Features",
                true,
                "Setting this option to 'Enabled' create all the implove gameplay functions of Sound Studio during Support.");

            IsSoundLevel1Enabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : Level1 : SFX(Sound effects)",
                true,
                "Setting this option to 'Enabled' create the High quality sound function of Sound Studio during Support.");

            IsSoundLevel2Enabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : Level2 : SFX(UI Sounds)",
                true,
                "Setting this option to 'Enabled' create the High quality sound function of Sound Studio during Support.");

            IsSoundLevel3Enabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : Level3 : SFX(Music)",
                true,
                "Setting this option to 'Enabled' create the High quality sound function of Sound Studio during Support.");

            IsSoundLevel4Enabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : Level4 : SFX(Amvience Sounds)",
                true,
                "Setting this option to 'Enabled' create the High quality sound function of Sound Studio during Support.");

            IsSoundLevel5Enabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : Level5 : SFX(Dynamic Sounds)",
                true,
                "Setting this option to 'Enabled' create the High quality sound function of Sound Studio during Support.");

            IsSoundLevel6Enabled = ConfigFile.Bind(
                SoundStudioSettingSection,
                "Sound Studio : Level6 : SFX(Voice Recording)",
                true,
                "Setting this option to 'Enabled' create the High quality sound function of Sound Studio during Support.");
            // ----------------------------------------------------------------------------------------------------------------
            InitSoundLevels();
            // ----------------------------------------------------------------------------------------------------------------
            // Motion Capture Studio Config
            IsMotionCaptureAllEnabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : All Studio Features",
                true,
                "Setting this option to 'Enabled' create all the High quality animations functions of Motion Capture Studio during Support.");
            IsMotionCaptureLevel1Enabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : Level1 : MC(Player animations I)",
                true,
                "Setting this option to 'Enabled' create the High quality animations function of Motion Capture Studio during Support.");
            IsMotionCaptureLevel2Enabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : Level2 : MC(Player animations II)",
                true,
                "Setting this option to 'Enabled' create the High quality animations function of Motion Capture Studio during Support.");
            IsMotionCaptureLevel3Enabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : Level3 : MC(Player animations III)",
                true,
                "Setting this option to 'Enabled' create the High quality animations function of Motion Capture Studio during Support.");
            IsMotionCaptureLevel4Enabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : Level4 : MC(NPC animations I)",
                true,
                "Setting this option to 'Enabled' create the High quality animations function of Motion Capture Studio during Support.");
            IsMotionCaptureLevel5Enabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : Level5 : MC(NPC animations II)",
                true,
                "Setting this option to 'Enabled' create the High quality animations function of Motion Capture Studio during Support.");
            IsMotionCaptureLevel6Enabled = ConfigFile.Bind(
                MotionCaptureStudioSettingSection,
                "Motion Capture Studio : Level6 : MC(NPC animations III)",
                true,
                "Setting this option to 'Enabled' create the High quality animations function of Motion Capture Studio during Support.");
            // ----------------------------------------------------------------------------------------------------------------
            InitMotionCaptureLevels();
            // ----------------------------------------------------------------------------------------------------------------




            // ---------------------------------------------------------------------
            // ---------------------------------------------------------------------
            ConfigFile.SettingChanged += OnConfigSettingChanged;
        }

        private void OnConfigSettingChanged(object sender, SettingChangedEventArgs e)
        {
            Debug.Log(SmartStudios.PluginName + " : Config setting changed");
            InitQaLevels();
            InitGraphicsLevels();
            InitSoundLevels();
            InitMotionCaptureLevels();
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

        private void InitGraphicsLevels()
        {
            GraphicsLevels = new List<bool>
            {
                IsGraphicsLevel1Enabled.Value,
                IsGraphicsLevel2Enabled.Value,
                IsGraphicsLevel3Enabled.Value,
                IsGraphicsLevel4Enabled.Value,
                IsGraphicsLevel5Enabled.Value,
                IsGraphicsLevel6Enabled.Value
            };
        }
        private void InitSoundLevels()
        {
            SoundLevels = new List<bool>
            {
                IsSoundLevel1Enabled.Value,
                IsSoundLevel2Enabled.Value,
                IsSoundLevel3Enabled.Value,
                IsSoundLevel4Enabled.Value,
                IsSoundLevel5Enabled.Value,
                IsSoundLevel6Enabled.Value
            };
        }
        private void InitMotionCaptureLevels()
        {
            MotionCaptureLevels = new List<bool>
            {
                IsMotionCaptureLevel1Enabled.Value,
                IsMotionCaptureLevel2Enabled.Value,
                IsMotionCaptureLevel3Enabled.Value,
                IsMotionCaptureLevel4Enabled.Value,
                IsMotionCaptureLevel5Enabled.Value,
                IsMotionCaptureLevel6Enabled.Value
            };
        }
    }
}