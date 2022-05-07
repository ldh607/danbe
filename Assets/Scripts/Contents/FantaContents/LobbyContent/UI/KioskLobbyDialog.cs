using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class KioskLobbyDialog : IDialog
    {
        public Button Start_Btn = null;

        private void Start()
        {
            Start_Btn.onClick.AddListener(OnStartClick);
        }

        void OnStartClick()
        {
            Message.Send<Event.GameModeClickMsg>(new Event.GameModeClickMsg());
        }
    }
}

