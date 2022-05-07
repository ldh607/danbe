using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig;
using CellBig.Contents.Event;
using CellBig.UI.Event;

namespace CellBig.Contents
{
	public class ResultContent : IContent
	{

        PlayContentModel pcm;
        protected override void OnEnter()
        {
            pcm = Model.First<PlayContentModel>();
            UI.IDialog.RequestDialogEnter<UI.ResultDialog>();

            Message.Send<IsScoreContentMsg>(new IsScoreContentMsg(pcm.GetCurrentContent().isScore));
        }

        protected override void OnExit()
        {

            UI.IDialog.RequestDialogExit<UI.ResultDialog>();
        }

    }
}
