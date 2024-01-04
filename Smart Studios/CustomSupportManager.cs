﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using BepInEx;
using System.Security.Cryptography;

namespace Smart_Studios
{
    /// <summary>
    /// StartStudios専用のSupport待機処理のためのクラス
    /// Room_*******のGameObjectにアタッチする。
    /// </summary>
    public class CustomSupportManager: MonoBehaviour
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

        void Start()
        {
            //ここで、StartStudiosのSupport待機処理のためのクラスを初期化する。
            FindScripts();
        }

        void Update()
        {
            if (ShouldRetrieveTaskUnterstuetzen())
            {
                GetMyTaskUnterstuetzen();
            }
        }

        bool ShouldRetrieveTaskUnterstuetzen()
        {
            return rS_.taskGameObject == null && rS_.taskID == -1;
        }

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
        /// __instance.nameと一致するtaskGameObjectを持つroomScriptを探す関数
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static roomScript FindRoomScriptForInstance(string name)
        {
            var roomScripts = GameObject.FindGameObjectsWithTag("Room");
            foreach (var room in roomScripts)
            {
                var roomComponent = room.GetComponent<roomScript>();
                if (roomComponent != null && roomComponent.taskGameObject != null && roomComponent.taskGameObject.name == name)
                {
                    return roomComponent;
                }
            }
            return null; // 見つからなかった場合はnullを返す
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(taskUnterstuetzen), "Update")]
        static void FindMyRoom_Postfix(taskUnterstuetzen __instance, ref roomScript ___rS_)
        {
            roomScript srcRoomScript = FindRoomScriptForInstance(__instance.name);
            if (srcRoomScript == null) { return; }
            bool hoge = CustomSupportStatus.IsSuitableCustomSupportForGameDev(srcRoomScript, ___rS_);
            if (CustomSupportStatus.IsSuitableCustomSupportForGameDev(srcRoomScript, ___rS_))
            {
                GameObject taskGameObject = ___rS_.taskGameObject;
                if (taskGameObject == null)
                {
                    return;
                }
                taskGame destTaskGame = taskGameObject.GetComponent<taskGame>();
                gameScript destGameScript = destTaskGame.gS_;

                //___rS_の元のGameObjectにSmartStudiosCustomSupportManagerをアタッチさせる。
                if (srcRoomScript.gameObject.GetComponent<CustomSupportManager>() == null)
                {
                    srcRoomScript.gameObject.AddComponent(typeof(CustomSupportManager));
                }
                QA_ScriptManager qA_ScriptManager = new QA_ScriptManager();
                qA_ScriptManager.AutoStart(srcRoomScript, destGameScript); //これsrcRoomScript入れないといけない。Support元で行わないといけないので。
                //status.SetCustomSupportWaiting(false);
            }
        }
    }
}
