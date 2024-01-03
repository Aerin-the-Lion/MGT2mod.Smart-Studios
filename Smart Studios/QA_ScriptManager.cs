using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Smart_Studios
{
    public class QA_ScriptManager : MonoBehaviour
    {
		public static bool IsAutoStartQAEnabled { get; private set; } = false;
		public static Menu_QA_GameplayVerbessern Menu_QA { get; private set; }
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
        private bool[] deactiveGameplay;

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

        public void AutoStart(roomScript roomScript, gameScript destGameScript)
		{
            //初期化
            Init(roomScript, destGameScript);

            deactiveGameplay = GetDeactiveGameFeatures(destGameScript.gameplayStudio);
            Traverse.Create(Menu_QA).Field("buttonAdds").SetValue(deactiveGameplay);

            int num = Mathf.RoundToInt((float)GetDevCosts());
            if (!this.selectedGame)
			{
				return;
			}
			if (!this.rS_)
			{
				return;
			}
			if (this.mS_.NotEnoughMoney((long)num))

			{
				this.guiMain_.ShowNoMoney();
				return;
			}
			this.sfx_.PlaySound(3, true);
			this.mS_.Pay((long)num, 10);
			taskGameplayVerbessern taskGameplayVerbessern = this.guiMain_.AddTask_GameplayVerbessern();
			taskGameplayVerbessern.Init(false);
			taskGameplayVerbessern.targetID = this.selectedGame.myID;
			for (int i = 0; i < deactiveGameplay.Length; i++)
			{
				if (deactiveGameplay[i])

				{
					taskGameplayVerbessern.adds[i] = true;
				}
			}
            taskGameplayVerbessern.autoBugfix = true;
            GameObject gameObject = GameObject.Find("Room_" + this.rS_.myID.ToString());
			if (gameObject)
			{
				gameObject.GetComponent<roomScript>().taskID = taskGameplayVerbessern.myID;
			}
            //本体の起動メソッド
            taskGameplayVerbessern.FindNewAdd();
            
        }

        public bool[] GetDeactiveGameFeatures(bool[] gameFeatures)
        {
            bool[] deactiveFeatures = new bool[gameFeatures.Length];
            for (int i = 0; i < gameFeatures.Length; i++)
            {
                deactiveFeatures[i] = !gameFeatures[i];
            }
            return deactiveFeatures;
        }

        public int CountActiveGameFeatures(bool[] gameFeatures)
        {
            int activeCount = 0;
            foreach (bool feature in gameFeatures)
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
            long num = 0L;
            for (int i = 0; i < deactiveGameplay.Length; i++)
            {
                if (deactiveGameplay[i])
                {
                    num += (long)this.GetCosts(i, this.selectedGame);
                }
            }
            return num;
        }

        public int GetCosts(int i, gameScript script_)
        {
            if (!script_)
            {
                return 0;
            }
            int[] costs = Traverse.Create(Menu_QA).Field("costs").GetValue<int[]>();
            int num = costs[i] * script_.GetPointsForAdds();
            num = num / 1000 * 1000;
            if (num < 1000)
            {
                num = 1000;
            }
            return num;
        }

        public void Init(roomScript roomScript, gameScript destGameScript)
        {
            FindScripts();
            Menu_QA = guiMain_.uiObjects[172].GetComponent<Menu_QA_GameplayVerbessern>();
            Traverse.Create(Menu_QA).Method("FindScripts").GetValue();
            Traverse.Create(Menu_QA).Field("rS_").SetValue(roomScript);
            Traverse.Create(Menu_QA).Field("selectedGame").SetValue(destGameScript);

            rS_ = roomScript;
            selectedGame = destGameScript;
        }
    }
}
