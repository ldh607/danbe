using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Output = CellBig.Module.Detection.CV.Output;
using UnityEngine.SceneManagement;
using CellBig.Models;
using CellBig.UI.Event;
using System.IO;

namespace CellBig.Contents
{
    public class SelectGameContent : IContent
    {
        public Camera _camera;
        SettingModel _sm;
        int StartTick = 0;

        private void Start()
        {
            CellBig.Scene.SceneManager.Instance.nowScene = CellBig.Constants.SceneName.SelectGameScene;
        }

        protected override void OnEnter()
        {
            StartTick = 0;
            _sm = Model.First<SettingModel>();
            CellBig.Scene.SceneManager.Instance.nowScene = CellBig.Constants.SceneName.SelectGameScene;

            UI.IDialog.RequestDialogEnter<UI.SelectGameDialog>();
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.SelectGameDialog>();
        }

    }
}