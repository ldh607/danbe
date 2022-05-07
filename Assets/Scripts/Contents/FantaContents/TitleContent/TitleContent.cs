using UnityEngine;
using System.Collections;
using CellBig.Constants;
using CellBig.Models;
using CellBig.UI.Event;
using CellBig.Contents.Event;

namespace CellBig.Contents
{
	public class TitleContent : IContent
	{
        protected override void OnEnter()
		{
            Message.Send<FadeOutMsg>(new FadeOutMsg());

            UI.IDialog.RequestDialogEnter<UI.ScreenTitleDialog>();
            UI.IDialog.RequestDialogEnter<UI.KioskTitleDialog>();
            Message.AddListener<UI.Event.StartClickMsg>(OnStartClick);
        }

		protected override void OnExit()
		{
            UI.IDialog.RequestDialogExit<UI.ScreenTitleDialog>();
            UI.IDialog.RequestDialogExit<UI.KioskTitleDialog>();
            Message.RemoveListener<UI.Event.StartClickMsg>(OnStartClick);

        }

        void OnStartClick(UI.Event.StartClickMsg msg)
        {
            StartCoroutine(ChangeScene());
        }

        IEnumerator ChangeScene()
        {
            Message.Send<FadeInMsg>(new FadeInMsg());
            yield return new WaitForSeconds(0.5f);
           //쓰긴쓰는거? Scene.SceneManager.Instance.Load(SceneName.Lobby);
        }
    }
}
