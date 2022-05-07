using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CellBig.Constants;
using CellBig.Contents.Event;
using CellBig.UI.Event;


namespace CellBig.Contents
{
	public class LobbyContent : IContent
	{
        protected override void OnEnter()
        {
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());

            UI.IDialog.RequestDialogEnter<UI.ScreenLobbyDialog>();
            UI.IDialog.RequestDialogEnter<UI.KioskLobbyDialog>();
            Message.AddListener<UI.Event.GameModeClickMsg>(OnGameModeClick);
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.ScreenLobbyDialog>();
            UI.IDialog.RequestDialogExit<UI.KioskLobbyDialog>();
            Message.RemoveListener<UI.Event.GameModeClickMsg>(OnGameModeClick);
        }

        void OnGameModeClick(UI.Event.GameModeClickMsg msg)
        {
            StartCoroutine(ChangeScene());
        }

        IEnumerator ChangeScene()
        {
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg());
            yield return new WaitForSeconds(0.5f);
          //어따쓰는겨?  Scene.SceneManager.Instance.Load(SceneName.InGame);
        }
    }
}
