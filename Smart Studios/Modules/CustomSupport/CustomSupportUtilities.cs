using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
