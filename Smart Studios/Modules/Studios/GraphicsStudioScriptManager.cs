﻿using HarmonyLib;
using Smart_Studios.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.Studios
{
    public class GraphicsStudioScriptManager
    {
        public static Menu_GFX_GrafikVerbessern Menu_Graphics { get; private set; }
        private GameObject main_;
        private mainScript mS_;
        private engineFeatures eF_;
        private gameplayFeatures gF_;
        private GUI_Main guiMain_;
        private textScript tS_;
        private roomDataScript rdS_;
        private sfxScript sfx_;
        private gameScript selectedGame;
        public roomScript rS_;
        private bool[] buttonAdds;
        private bool[] deactiveStudioFeatures;
        public bool[] finishedOrWipStudioFeatures;

        void Start()
        {
            //ここで、StartStudiosのSupport待機処理のためのクラスを初期化する。
            FindScripts();
        }

        void FindScripts()
        {
            if (this.main_)
            {
                return;
            }
            if (!this.main_)
            {
                this.main_ = GameObject.FindGameObjectWithTag("Main");
            }
            if (!this.mS_)
            {
                this.mS_ = this.main_.GetComponent<mainScript>();
            }
            if (!this.guiMain_)
            {
                this.guiMain_ = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            }
            if (!this.eF_)
            {
                this.eF_ = this.main_.GetComponent<engineFeatures>();
            }
            if (!this.gF_)
            {
                this.gF_ = this.main_.GetComponent<gameplayFeatures>();
            }
            if (!this.tS_)
            {
                this.tS_ = this.main_.GetComponent<textScript>();
            }
            if (!this.rdS_)
            {
                this.rdS_ = this.main_.GetComponent<roomDataScript>();
            }
            if (!this.sfx_)
            {
                this.sfx_ = GameObject.Find("SFX").GetComponent<sfxScript>();
            }
        }

        public void Init(roomScript roomScript, gameScript destGameScript)
        {
            FindScripts();
            Menu_Graphics = guiMain_.uiObjects[174].GetComponent<Menu_GFX_GrafikVerbessern>();
            buttonAdds = Traverse.Create(Menu_Graphics).Field("buttonAdds").GetValue<bool[]>();
            Traverse.Create(Menu_Graphics).Method("FindScripts").GetValue();
            Traverse.Create(Menu_Graphics).Field("rS_").SetValue(roomScript);
            Traverse.Create(Menu_Graphics).Field("selectedGame").SetValue(destGameScript);

            rS_ = roomScript;
            selectedGame = destGameScript;
        }

        public void AutoStart(roomScript room, gameScript destGameScript)
        {
            // Initialization
            Init(room, destGameScript);
            finishedOrWipStudioFeatures = GetFinishedOrWipStudioFeatures();
            deactiveStudioFeatures = GetDeactiveStudioFeatures(finishedOrWipStudioFeatures);

            if (!this.selectedGame || !this.rS_) { return; }

            int devCosts = Mathf.RoundToInt((float)GetDevCosts());
            bool isNotEnoughMoney = this.mS_.NotEnoughMoney(devCosts);
            if (!isNotEnoughMoney)
            {
                this.mS_.Pay(devCosts, 10);
            }

            taskGrafikVerbessern task = this.guiMain_.AddTask_GrafikVerbessern();
            task.Init(false);
            LinkTaskToRoom(task);
            InitializeTask(task, isNotEnoughMoney);
        }

        private void InitializeTask(taskGrafikVerbessern task, bool isNotEnoughMoney)
        {
            task.targetID = this.selectedGame.myID;
            SetTaskFeatures(task, isNotEnoughMoney);
            task.FindNewAdd();
            //本体の起動メソッド
        }

        private void SetTaskFeatures(taskGrafikVerbessern task, bool isNotEnoughMoney)
        {
            for (int i = 0; i < deactiveStudioFeatures.Length; i++)
            {
                task.adds[i] = deactiveStudioFeatures[i] && !isNotEnoughMoney &&
                               (ConfigManager.IsGraphicsAllEnabled.Value || ConfigManager.GraphicsLevels[i]);

                //GameDevelopmentStudioのGameplayFeaturesのScene Cut？が有効じゃないと使えないため。
                if (i == 5 && this.selectedGame && !this.selectedGame.gameGameplayFeatures[40])
                {
                    task.adds[i] = false;
                }
            }
        }

        private void LinkTaskToRoom(taskGrafikVerbessern task)
        {
            GameObject roomObject = GameObject.Find("Room_" + this.rS_.myID.ToString());
            if (roomObject)
            {
                roomObject.GetComponent<roomScript>().taskID = task.myID;
            }
        }

        public bool[] GetDeactiveStudioFeatures(bool[] studioFeatures)
        {
            bool[] deactiveFeatures = new bool[studioFeatures.Length];
            for (int i = 0; i < studioFeatures.Length; i++)
            {
                deactiveFeatures[i] = !studioFeatures[i];
            }
            return deactiveFeatures;
        }

        public bool[] GetFinishedOrWipStudioFeatures()
        {
            bool[] finishedOrWipFeatures = new bool[buttonAdds.Length];
            for (int i = 0; i < buttonAdds.Length; i++)
            {
                if (this.selectedGame)
                {
                    if (this.selectedGame.grafikStudio[i])
                    {
                        finishedOrWipFeatures[i] = true;
                    }
                    else
                    {
                        finishedOrWipFeatures[i] = false;
                    }
                    bool BeingProcessedInAnotherRoom = Traverse.Create(Menu_Graphics).Method("WirdInAnderenRaumBearbeitet", new object[] { i }).GetValue<bool>();
                    if (BeingProcessedInAnotherRoom)
                    {
                        finishedOrWipFeatures[i] = true;
                    }

                    //変数名と違うが、GameDevelopmentStudioのGameplayFeaturesのScene Cut？が有効じゃないと使えないため。
                    if (i == 5 && this.selectedGame && !this.selectedGame.gameGameplayFeatures[40])
                    {
                        finishedOrWipFeatures[i] = true;
                    }
                }
            }
            return finishedOrWipFeatures;
        }

        public int CountActiveStudioFeatures(bool[] studioFeatures)
        {
            int activeCount = 0;
            foreach (bool feature in studioFeatures)
            {
                if (feature)
                {
                    activeCount++;
                }
            }
            return activeCount;
        }

        private long GetDevCosts()
        {
            long totalCosts = 0L;
            //条件式 ? 真の場合の値 : 偽の場合の値
            int featureCount = ConfigManager.IsGraphicsAllEnabled.Value ? deactiveStudioFeatures.Length : ConfigManager.GraphicsLevels.Count;

            for (int i = 0; i < featureCount; i++)
            {
                if (!deactiveStudioFeatures[i]) continue;

                bool featureActive = ConfigManager.IsGraphicsAllEnabled.Value || ConfigManager.GraphicsLevels[i];
                if (featureActive)
                {
                    totalCosts += (long)GetCosts(i, this.selectedGame);
                }
            }

            return totalCosts;
        }

        public int GetCosts(int i, gameScript script_)
        {
            if (!script_)
            {
                return 0;
            }
            int[] costs = Traverse.Create(Menu_Graphics).Field("costs").GetValue<int[]>();
            int num = costs[i] * script_.GetPointsForAdds();
            num = num / 1000 * 1000;
            if (num < 1000)
            {
                num = 1000;
            }
            return num;
        }
    }
}
