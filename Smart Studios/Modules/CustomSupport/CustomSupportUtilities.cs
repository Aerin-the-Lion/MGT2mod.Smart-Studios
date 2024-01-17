using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.CustomSupport
{
    public class CustomSupportUtilities
    {
        /// <summary>
        /// サポート先がGame Development Studioの場合かつ、
        /// サポート元が特定のタイプ（3, 4, 5, 10）であるかどうかをチェックする関数
        /// </summary>
        /// <param name="srcRoomScript"></param>
        /// <param name="destRoomScript"></param>
        /// <returns></returns>
        public static bool IsSuitableCustomSupportForGameDev(roomScript srcRoomScript, roomScript destRoomScript)
        {
            // 有効なサポート元のタイプ
            int[] validTypes = { 3, 4, 5, 10 };

            // サポート先がGame Development Studioで、サポート元が有効なタイプかどうかをチェック
            return srcRoomScript != null && destRoomScript.typ == 1 && (Array.IndexOf(validTypes, srcRoomScript.typ) >= 0);
        }

        public static bool IsTypeOfQaStudio(roomScript srcRoomScript)
        {
            return srcRoomScript.typ == 3;
        }

        public static bool IsTypeOfGraphicStudio(roomScript srcRoomScript)
        {
            return srcRoomScript.typ == 4;
        }

        public static bool IsTypeOfSoundStudio(roomScript srcRoomScript)
        {
            return srcRoomScript.typ == 5;
        }

        public static bool IsTypeOfMotionCaptureStudio(roomScript srcRoomScript)
        {
            return srcRoomScript.typ == 10;
        }

        /// <summary>
        /// taskUnterstuetzenを所有している場合にこの処理が使われるはず。
        /// 
        /// </summary>
        /// <param name="destRoomScript"></param>
        /// <param name="srcRoomScript"></param>
        /// <param name="___mS_"></param>
        /// <param name="___guiMain_"></param>
        //script_はSupport先のroomScript, ___rS_はSupport元のroomScript, ___mS_はmainScript, ___guiMain_はGUI_Main
        public static void EnterSupportMode(roomScript destRoomScript, roomScript srcRoomScript, mainScript ___mS_, GUI_Main ___guiMain_)
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
            //taskUnterstuetzen2.gameObject.AddComponent(typeof(CustomSupportStatus)); //フラグ管理のために、ダミーでCustomSupportStatusを追加する。
            srcRoomScript.taskID = taskUnterstuetzen2.myID;
            srcRoomScript.DisableOutlineLayer();
            destRoomScript.DisableOutlineLayer();
        }
    }
}
