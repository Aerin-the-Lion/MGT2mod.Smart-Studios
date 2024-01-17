using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using BepInEx;
using System.Security.Cryptography;

namespace Smart_Studios.Modules.CustomSupport
{
    /// <summary>
    /// StartStudios専用のSupport待機処理など、全体的な管理を行うクラス。
    /// (!!!)Room_*******のGameObjectにアタッチするようにしてください。
    /// </summary>
    public class CustomSupportManager : MonoBehaviour
    {
        // //////////////////////////////////////////////////////////////////////////////////////////////////
        // Fields
        // //////////////////////////////////////////////////////////////////////////////////////////////////
        public int myID = -1;
        public int roomID = -1;
        private GameObject main_;
        private mainScript mS_;
        private engineFeatures eF_;
        private gameplayFeatures gF_;
        private GUI_Main guiMain_;
        private textScript tS_;
        private roomDataScript rdS_;
        private sfxScript sfx_;
        public gameScript selectedGame;
        public taskGame destMyTaskGame;
        public gameScript destGameScript;
        public roomScript myRoomScript;
        public GameObject myTaskUnterstuetzenObject;

        // //////////////////////////////////////////////////////////////////////////////////////////////////
        // Static Fields
        // //////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RoomScriptのキャッシュ。ここには、ゲーム全体のRoomScriptのインスタンスが格納される。
        /// 適切なタイミングでCacheRoomScriptsの更新が必要。
        ///　RoomScript cache. This contains instances of RoomScript for the entire game.
        ///　CacheRoomScripts needs to be updated at the appropriate time.
        /// </summary>
        public static List<roomScript> cachedRoomScripts = new List<roomScript>();

        public bool isInSupportMode = true;
        public bool isImprovingGame = false;

        // //////////////////////////////////////////////////////////////////////////////////////////////////
        // Timer
        // //////////////////////////////////////////////////////////////////////////////////////////////////
        public float updateInterval = 0.5f; // 更新間隔を秒で設定
        private float timer = 0f;
        // --------------------------------------------------------------------------------------------------


        // //////////////////////////////////////////////////////////////////////////////////////////////////
        // Unity Methods
        // //////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            //ここで、StartStudiosのSupport待機処理のためのクラスを初期化する。
            FindScripts();
            //RoomScriptのキャッシュを更新する関数
            CacheRoomScripts();
        }

        /// <summary>
        /// n秒ごとに、各種StudioのSupport先の開発が終了した後、taskGame、IDをCustomSupportManagerの付いたroomScriptオブジェクトに再設定させる処理。
        /// Every n seconds, after the development of the support destination of each studio is completed, the taskGame and ID are reset to the roomScript object with CustomSupportManager.
        /// </summary>
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                if (ShouldRetrieveTaskUnterstuetzen()) //各種StudioのSupport先の開発が終了した後、taskGame、IDをCustomSupportに再設定させる処理。
                {
                    GetMyTaskUnterstuetzen();
                }
                timer = 0f;
            }
        }
        // --------------------------------------------------------------------------------------------------

        // //////////////////////////////////////////////////////////////////////////////////////////////////
        // Methods
        // //////////////////////////////////////////////////////////////////////////////////////////////////
        bool ShouldRetrieveTaskUnterstuetzen()
        {
            return myRoomScript.taskGameObject == null && myRoomScript.taskID == -1 && isImprovingGame == false;
        }

        /// <summary>
        /// 各種StudioのSupport先の開発が終了した後、taskGame、IDをCustomSupportに再設定させる処理。
        /// </summary>
        void GetMyTaskUnterstuetzen()
        {
            taskUnterstuetzen myTaskUnterstuetzen = Traverse.Create(myRoomScript).Field("myTaskUnterstuetzen").GetValue<taskUnterstuetzen>();
            if (myTaskUnterstuetzen == null) { return; }
            myRoomScript.taskGameObject = myTaskUnterstuetzen.gameObject;
            myRoomScript.taskID = myTaskUnterstuetzen.gameObject.GetComponent<taskUnterstuetzen>().myID;
            this.SetIsInSupportMode(true);
        }

        public void SetIsInSupportMode(bool status)
        {
            isInSupportMode = status;
        }

        public void SetIsImprovingGame(bool status)
        {
            isImprovingGame = status;
        }

        public bool GetIsInSupportMode()
        {
            return isInSupportMode;
        }

        public bool GetIsImprovingGame()
        {
            return isImprovingGame;
        }

        /// <summary>
        /// __instance.nameと一致するtaskGameObjectを持つroomScriptを探す関数
        /// CachedRoomScriptsに対してのみ有効、適切なタイミングでCacheRoomScriptsの更新が必要。
        /// </summary>
        public static roomScript FindRoomScriptForInstance(string name)
        {
            return cachedRoomScripts.FirstOrDefault(r => r.taskGameObject?.name == name);
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
            if (!myRoomScript)
            {
                myRoomScript = this.gameObject.GetComponent<roomScript>();
            }
        }

        /// <summary>
        /// RoomScriptのキャッシュを更新する関数
        /// 適切なタイミングでの使用を推奨
        /// </summary>
        public static void CacheRoomScripts()
        {
            cachedRoomScripts.Clear();
            var roomObjects = GameObject.FindGameObjectsWithTag("Room");
            foreach (var room in roomObjects)
            {
                var roomComponent = room.GetComponent<roomScript>();
                if (roomComponent != null)
                {
                    cachedRoomScripts.Add(roomComponent);
                }
            }
        }

        // --------------------------------------------------------------------------------------------------
    }
}
