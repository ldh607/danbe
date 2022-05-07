using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Models;
using CellBig.Contents.Event;
using CellBig.UI.Event;
using CellBig.Constants;


namespace CellBig.Contents
{
    public class GlobalContent : IContent
    {
        GameModel _gm;
        SettingModel _sm;

        Coroutine shakeCamera_Cor;
        Coroutine colorCamera_Cor;
        Camera mainCamera;


        protected override void OnEnter()
        {
            _gm = Model.First<GameModel>();
            _sm = Model.First<SettingModel>();

            UI.IDialog.RequestDialogEnter<UI.KioskGlobalDialog>();
            UI.IDialog.RequestDialogEnter<UI.ScreenGlobalDialog>();
            UI.IDialog.RequestDialogEnter<UI.WaterPlaySensorSettingDialog>();
            Message.AddListener<MainCameraMsg>(OnMainCameraMsg);
            Message.AddListener<ShakeCameraMsg>(OnShakeCameraMsg);
            Message.AddListener<SelectContentMsg>(OnSelectContent);
        }

        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.KioskGlobalDialog>();
            UI.IDialog.RequestDialogExit<UI.ScreenGlobalDialog>();
            UI.IDialog.RequestDialogExit<UI.WaterPlaySensorSettingDialog>();
            Message.RemoveListener<MainCameraMsg>(OnMainCameraMsg);
            Message.RemoveListener<ShakeCameraMsg>(OnShakeCameraMsg);
            Message.RemoveListener<SelectContentMsg>(OnSelectContent);
        }

        void AddMessage()
        {

        }

        void RemoveMessage()
        {

        }

        void OnMainCameraMsg(MainCameraMsg msg)
        {
#if UNITY_EDITOR
            Debug.LogError("MainCamera : " + msg.MainCamera);
#endif

            mainCamera = msg.MainCamera;
            _gm.playContent.Model.mainCamera = mainCamera;
        }

        void OnShakeCameraMsg(ShakeCameraMsg msg)
        {
            if (shakeCamera_Cor != null)
                StopCoroutine(shakeCamera_Cor);

            shakeCamera_Cor = StartCoroutine(ShakeCamera(msg.IsX, msg.IsY, msg.Duration, msg.Amount));
        }

        IEnumerator ShakeCamera(bool isX, bool isY, float duration = 1.0f, float amount = 5.0f)
        {
            Vector3 orignalPosition = mainCamera.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = 0.0f;
                float y = 0.0f;

                if (isX)
                    x = Random.Range(-1f, 1f) * amount;
                if (isY)
                    y = Random.Range(-1f, 1f) * amount;

                mainCamera.transform.position = orignalPosition;
                mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x + x, mainCamera.transform.localPosition.y + y, mainCamera.transform.localPosition.z);

                elapsed += Time.deltaTime;
                yield return 0;
            }

            mainCamera.transform.position = orignalPosition;
            mainCamera.transform.localPosition = Vector3.zero;
        }

        public List<GameData> gameData = new List<GameData>();
        public int curGameIndex;
        public float curTime;
        public float EndTime;
        Coroutine _cStartRotationCor;

        IEnumerator StartRotation()
        {
            curTime = 0;
            EndTime = gameData[curGameIndex].GameTime * 60f;

            while (curTime <= EndTime)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            if (curGameIndex + 1 < gameData.Count)
            {
                curGameIndex += 1;
            }
            else if (curGameIndex + 1 >= gameData.Count)
            {
                curGameIndex = 0;
            }

            curTime = 0;
            EndTime = gameData[curGameIndex].GameTime * 60f;

            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            SoundManager.Instance.StopAllSound();
            yield return new WaitForSeconds(0.5f);
            CellBig.Scene.SceneManager.Instance.Load(gameData[curGameIndex].GameName, true);

            _cStartRotationCor = StartCoroutine(StartRotation());
        }

        public void SetRotation(List<GameData> contentsDatas)
        {
            gameData.Clear();
            foreach (var item in contentsDatas)
            {
                if (item.GameSelectToggle)
                {
                    GameData gamedata = new GameData();
                    gamedata.GameName = item.GameName;

                    if (item.GameTime != 0)
                    {
                        gamedata.GameTime = item.GameTime;
                    }
                    else
                    {
                        gamedata.GameTime = 30;
                    }
                    gameData.Add(gamedata);
                }
            }
            curGameIndex = 0;

            _cStartRotationCor = StartCoroutine(StartRotation());
        }

        public void OnSelectContent(SelectContentMsg msg)
        {
            StartCoroutine(SceneChange(msg.contentType));
            SetRotation(msg.contentsDatas);
        }

        IEnumerator SceneChange(Constants.ContentType gamename)
        {
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            SoundManager.Instance.StopAllSound();
            yield return new WaitForSeconds(0.5f);
            CellBig.Scene.SceneManager.Instance.nowScene = (Constants.SceneName)(int)(gamename);
            CellBig.Scene.SceneManager.Instance.Load((Constants.SceneName)(int)(gamename), true);
        }

        IEnumerator ReturnLoby()
        {
            if (_cStartRotationCor != null)
            {
                StopCoroutine(_cStartRotationCor);
                _cStartRotationCor = null;
            }
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            SoundManager.Instance.StopAllSound();
            yield return new WaitForSeconds(0.5f);
            CellBig.Scene.SceneManager.Instance.Load(Constants.SceneName.SelectGameScene, true);
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg());

        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKey(KeyCode.L))
                {
                    StartCoroutine(ReturnLoby());
                }

                if (Input.GetKey(KeyCode.Z))
                {
                    _sm.LineShow = !_sm.LineShow;
                }

                if(Input.GetKey(KeyCode.X))
                {
                    _sm.BallPlayLineShow = !_sm.BallPlayLineShow;
                }
            }


        }
    }
}
