using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.CustomSupport
{
    /// <summary>
    /// CustomSupportの情報をセーブ・ロードにより管理するクラス
    /// </summary>
    [System.Serializable]
    public class CustomSupportRoomData
    {
        public bool hasCustomSupportMode = false;
        public int roomID = -1;
        public int taskID = -1;

        // --------------------------------------------------------------------------------------------------
        /// <summary>
        /// パラメータレスコンストラクタ
        /// </summary>
        public CustomSupportRoomData() { }

        /// <summary>
        /// コンストラクタ：taskUnterstuetzenを所有している場合
        /// </summary>
        /// <param name="room"></param>
        /// <param name="task"></param>
        public CustomSupportRoomData(roomScript room, taskUnterstuetzen task)
        {
            hasCustomSupportMode = true;
            roomID = room.myID;
            taskID = task.myID;
        }

        /// <summary>
        /// コンストラクタ：taskUnterstuetzenを所有していない場合
        /// </summary>
        /// <param name="room"></param>
        public CustomSupportRoomData(roomScript room)
        {
            hasCustomSupportMode = false;
            roomID = room.myID;
        }

        // --------------------------------------------------------------------------------------------------
        /// <summary>
        /// CustomSupportRoomDataをセーブデータに保存
        /// </summary>
        /// <param name="writer"></param>
        public static void SaveCustomSupportRoomData(ES3Writer writer)
        {
            GameObject[] arrayRooms = GameObject.FindGameObjectsWithTag("Room");
            int length = arrayRooms.Length;
            for (int i = 0; i < length; i++)
            {
                roomScript component = arrayRooms[i].GetComponent<roomScript>();
                GameObject taskGameObject = component.taskGameObject;
                taskUnterstuetzen task = Traverse.Create(component).Field("myTaskUnterstuetzen").GetValue<taskUnterstuetzen>();
                if(task == null) { continue; }
                if (task.gameObject == taskGameObject) { continue; }
                if (task != null)
                {
                    CustomSupportRoomData data = new CustomSupportRoomData(component, task);
                    writer.Write<CustomSupportRoomData>("customSupportRoomData_" + i, data);
                }
                else
                {
                    CustomSupportRoomData data = new CustomSupportRoomData(component);
                    writer.Write<CustomSupportRoomData>("customSupportRoomData_" + i, data);
                }
            }
        }

        /// <summary>
        /// セーブデータから CustomSupportRoomDataをロード
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="filename"></param>
        /// <param name="___es3file"></param>
        /// <returns></returns>
        public static List<CustomSupportRoomData> LoadCustomSupportRoomData(ES3Reader reader, ES3File ___es3file)
        {
            List<CustomSupportRoomData> dataList = new List<CustomSupportRoomData>();
            int anzRooms = reader.Read<int>("anzRooms", -1);

            for (int i = 0; i < anzRooms; i++)
            {
                string key = "customSupportRoomData_" + i;
                if (___es3file.KeyExists(key))
                {
                    CustomSupportRoomData data = reader.Read<CustomSupportRoomData>(key);
                    dataList.Add(data);
                }
            }

            return dataList;
        }

        /// <summary>
        /// taskUnterstuetzenのGameObjectを取得する
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectTaskUnterstuetzen(int taskID)
        {
            Debug.Log("Task_" + taskID.ToString());
            GameObject taskObject = GameObject.Find("Task_" + taskID.ToString());
            if (taskObject == null) { Debug.Log("見つかりませんでした"); return null; }
            return taskObject;
        }
    }
}
