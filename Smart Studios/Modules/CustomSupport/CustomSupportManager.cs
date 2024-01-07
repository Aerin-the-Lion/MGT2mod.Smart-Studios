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
    /// StartStudios専用のSupport待機処理のためのクラス
    /// Room_*******のGameObjectにアタッチする。
    /// </summary>
    public class CustomSupportManager : MonoBehaviour
    {
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
        public roomScript rS_;
        public GameObject myTaskUnterstuetzenObject;

        private static List<roomScript> cachedRoomScripts = new List<roomScript>();


        void Start()
        {
            //ここで、StartStudiosのSupport待機処理のためのクラスを初期化する。
            FindScripts();
            //RoomScriptのキャッシュを更新する関数
            CacheRoomScripts();
        }

        void Update()
        {
            //各種StudioのSupport先の開発が終了した後、taskGame、IDをCustomSupportに再設定させる処理。
            if (ShouldRetrieveTaskUnterstuetzen())
            {
                GetMyTaskUnterstuetzen();
            }
        }

        bool ShouldRetrieveTaskUnterstuetzen()
        {
            return rS_.taskGameObject == null && rS_.taskID == -1;
        }


        /// <summary>
        /// 各種StudioのSupport先の開発が終了した後、taskGame、IDをCustomSupportに再設定させる処理。
        /// </summary>
        void GetMyTaskUnterstuetzen()
        {
            taskUnterstuetzen myTaskUnterstuetzen = Traverse.Create(rS_).Field("myTaskUnterstuetzen").GetValue<taskUnterstuetzen>();
            if (myTaskUnterstuetzen == null) { return; }
            rS_.taskGameObject = myTaskUnterstuetzen.gameObject;
            rS_.taskID = myTaskUnterstuetzen.gameObject.GetComponent<taskUnterstuetzen>().myID;
            rS_.taskGameObject.GetComponent<CustomSupportStatus>().SetCustomSupportWaiting(true);
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
            if (!rS_)
            {
                rS_ = this.gameObject.GetComponent<roomScript>();
            }
        }

        //script_はSupport先のroomScript, ___rS_はSupport元のroomScript, ___mS_はmainScript, ___guiMain_はGUI_Main
        public void EnterSupportMode(roomScript destRoomScript, roomScript srcRoomScript, mainScript ___mS_, GUI_Main ___guiMain_)
        {
            //もしすでにSupport先のroomScriptがある場合は、Supportをキャンセルする。
            for (int i = 0; i < ___mS_.arrayRoomScripts.Length; i++)
            {
                if (___mS_.arrayRoomScripts[i] && ___mS_.arrayRoomScripts[i].taskGameObject)
                {
                    taskUnterstuetzen taskUnterstuetzen = ___mS_.arrayRoomScripts[i].GetTaskUnterstuetzen();
                    if (taskUnterstuetzen && taskUnterstuetzen.roomID == srcRoomScript.myID)
                    {
                        taskUnterstuetzen.Abbrechen();
                    }
                }
            }
            //もしSupport先がSupport状態である場合は、Supportをキャンセルする。
            if (destRoomScript.taskID != -1)
            {
                GameObject gameObject = GameObject.Find("Task_" + destRoomScript.taskID.ToString());
                if (gameObject && gameObject.GetComponent<taskUnterstuetzen>())
                {
                    gameObject.GetComponent<taskUnterstuetzen>().Abbrechen();
                }
            }

            //メイン処理
            taskUnterstuetzen taskUnterstuetzen2 = ___guiMain_.AddTask_Unterstuetzen();
            taskUnterstuetzen2.Init(false);
            taskUnterstuetzen2.roomID = destRoomScript.myID;
            taskUnterstuetzen2.gameObject.AddComponent(typeof(CustomSupportStatus)); //フラグ管理のために、ダミーでCustomSupportStatusを追加する。
            srcRoomScript.taskID = taskUnterstuetzen2.myID;
            srcRoomScript.DisableOutlineLayer();
            destRoomScript.DisableOutlineLayer();
        }

        //CustomSupportの設定・命名
        public void SetCustomSupport(GameObject gameObject)
        {
            if (gameObject.GetComponent(typeof(CustomSupportStatus)))
            {
                gameObject.GetComponent<CustomSupportStatus>().isCustomSupport = true;
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

        /// <summary>
        /// __instance.nameと一致するtaskGameObjectを持つroomScriptを探す関数
        /// CachedRoomScriptsに対してのみ有効、適切なタイミングでCacheRoomScriptsの更新が必要。
        /// </summary>
        public static roomScript FindRoomScriptForInstance(string name)
        {
            return cachedRoomScripts.FirstOrDefault(r => r.taskGameObject?.name == name);
        }
    }
}
