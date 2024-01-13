using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.HarmonyPatches
{
    internal class savegameScript_Patch: MonoBehaviour
    {
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(savegameScript), "Load")]
        public static void CacheRoomScriptsAfterLoadGame_Patch()
        {
            // Roomオブジェクトが追加された後に実行されるコード
            CustomSupportManager.CacheRoomScripts();
        }

        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(savegameScript), "SaveRooms")]
        public static void AfterSaveRooms_Patch(ES3Writer writer)
        {
            CustomSupportRoomData.SaveCustomSupportRoomData(writer);
        }

        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(savegameScript), "LoadTasks")]
        public static void SetCustomSupportModeAfterLoad_Patch(ES3Reader reader, ES3File ___es3file)
        {
            Debug.Log("--------------------------");
            Debug.Log("AfterLoadTasks_Patch");
            Debug.Log("--------------------------");
            CustomSupportManager.CacheRoomScripts();
            Debug.Log("1");
            List<roomScript> rooms = CustomSupportManager.cachedRoomScripts;
            Debug.Log("2");
            List<CustomSupportRoomData> loadedData = CustomSupportRoomData.LoadCustomSupportRoomData(reader, ___es3file);
            Debug.Log("3");
            if (loadedData == null) { return; }
            Debug.Log("4");
            foreach (CustomSupportRoomData data in loadedData)
            {
                Debug.Log("5");
                if (!data.hasCustomSupportMode) { continue; }

                Debug.Log("6");
                roomScript matchedRoom = rooms.FirstOrDefault(room => room.myID == data.roomID);
                Debug.Log("7");
                if (matchedRoom != null)
                {
                    Debug.Log("8");
                    GameObject taskObject = CustomSupportRoomData.GetGameObjectTaskUnterstuetzen(data.taskID);
                    Debug.Log("9");
                    if (taskObject != null)
                    {
                        Debug.Log("10");
                        Traverse.Create(matchedRoom).Field("myTaskUnterstuetzen").SetValue(taskObject.GetComponent<taskUnterstuetzen>());
                        if (matchedRoom.gameObject.GetComponent<CustomSupportManager>() == null)
                        {
                            matchedRoom.gameObject.AddComponent<CustomSupportManager>();
                        }
                        Debug.Log("END");
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------
    }
}
