using HarmonyLib;
using Smart_Studios.Modules.CustomSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.Hooks
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
            CustomSupportManager.CacheRoomScripts();
            List<roomScript> rooms = CustomSupportManager.cachedRoomScripts;
            List<CustomSupportRoomData> loadedData = CustomSupportRoomData.LoadCustomSupportRoomData(reader, ___es3file);
            if (loadedData == null) { return; }
            foreach (CustomSupportRoomData data in loadedData)
            {
                if (!data.hasCustomSupportMode) { continue; }

                roomScript matchedRoom = rooms.FirstOrDefault(room => room.myID == data.roomID);
                if (matchedRoom != null)
                {
                    GameObject taskObject = CustomSupportRoomData.GetGameObjectTaskUnterstuetzen(data.taskID);
                    if (taskObject != null)
                    {
                        Debug.Log("CustomSupport: Set taskUnterstuetzen to room " + matchedRoom.myID);
                        if (matchedRoom.gameObject.GetComponent<CustomSupportManager>() == null)
                        {
                            matchedRoom.gameObject.AddComponent<CustomSupportManager>();
                        }
                        Debug.Log(taskObject.GetComponent<taskUnterstuetzen>().name);
                        Traverse.Create(matchedRoom).Field("myTaskUnterstuetzen").SetValue(taskObject.GetComponent<taskUnterstuetzen>());
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------
    }
}
