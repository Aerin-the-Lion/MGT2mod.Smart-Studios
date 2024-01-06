using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart_Studios.Modules.CustomSupport
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
    }
}
