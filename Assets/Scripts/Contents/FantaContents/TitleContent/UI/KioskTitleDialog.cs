using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;

namespace CellBig.UI
{
    public class KioskTitleDialog : IDialog
    {
        public Button Start_Btn = null;

        private void Start()
        {
            Start_Btn.onClick.AddListener(OnStartClick);
        }

        void OnStartClick()
        {
            Message.Send<Event.StartClickMsg>(new Event.StartClickMsg());
        }
    }
}

