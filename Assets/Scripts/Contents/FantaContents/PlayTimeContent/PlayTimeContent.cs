using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

namespace CellBig.Contents
{
	public class PlayTimeContent : IContent
	{
        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.PlayTimeDialog>();
            var sm = Model.First<SettingModel>();
            Message.Send<PlayTimerStartMsg>(new PlayTimerStartMsg(sm.PlayTime));
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.PlayTimeDialog>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Message.Send<PlaySkipMsg>(new PlaySkipMsg());
        }
    }
}
