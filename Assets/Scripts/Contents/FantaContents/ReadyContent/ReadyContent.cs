using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig;
using CellBig.Contents.Event;
using CellBig.UI.Event;

namespace CellBig.Contents
{
	public class ReadyContent : IContent
	{
        protected override void OnEnter()
        {
            // UI OnOFF
            UI.IDialog.RequestDialogEnter<UI.ReadyDialog>();
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.ReadyDialog>();
        }
    }
}
