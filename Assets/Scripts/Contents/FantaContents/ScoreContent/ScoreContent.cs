using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig;
using CellBig.Contents.Event;
using CellBig.UI.Event;

namespace CellBig.Contents
{
	public class ScoreContent : IContent
	{
        protected override void OnEnter()
        {
            // UI OnOFF
            UI.IDialog.RequestDialogEnter<UI.ScoreDialog>();
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.ScoreDialog>();
        }
    }
}
