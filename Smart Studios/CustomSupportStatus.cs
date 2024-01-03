using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios
{
    public class CustomSupportStatus : MonoBehaviour
    {
        public bool isCustomSupport = false;
        public bool isCustomSupportWaiting = false;

        void Start()
        {
            isCustomSupport = true;
            isCustomSupportWaiting = true;
        }

        // この関数を呼び出すことで、isCustomSupportWaitingを切り替えます
        public void ToggleCustomSupportWaiting()
        {
            isCustomSupportWaiting = !isCustomSupportWaiting;
        }

        public void SetCustomSupportWaiting(bool waiting)
        {
            isCustomSupportWaiting = waiting;
        }

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
    }
}
