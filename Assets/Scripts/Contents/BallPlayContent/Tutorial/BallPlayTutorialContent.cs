using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Output = CellBig.Module.Detection.CV.Output;
using UnityEngine.SceneManagement;
using CellBig.Models;
using CellBig.Constants;
using UnityEngine.Video;

namespace CellBig.Contents
{
    public class BallPlayTutorialContent : IContent
    {
        private Transform _ObjRoot;
        private VideoPlayer _Video;
        private float _VideoTransTime = 3.0f;
        private List<BGData> BGDataList = new List<BGData>();
        public class BGData
        {
            public BallBGState BGName;
            public float BGTime;
            public VideoClip VideoClip;
        }


        public int curindex = 0;
        public Camera _camera;
        SettingModel _sm;
        int StartTick = 0;
        Coroutine tutorialTime;
        Coroutine checkguideSoundTime;
        private GameObject BallManager;
        GameObject _BallManager;



        protected override void OnLoadStart()
        {
            _ObjRoot = new GameObject("ObjRoot").transform;
            _ObjRoot.SetParent(transform);
            _ObjRoot.ResetLocal();

            StartCoroutine(_cLoadProcess());
        }

        private IEnumerator _cLoadProcess()
        {
            yield return StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>("BallPlayBundle", "BallPlayTutorialGame", (o) =>
            {
                var bgs = _OnLoadBGObject(o);
            }));
            _ObjRoot.gameObject.SetActive(false);
            SetLoadComplete();
        }

        private GameObject _OnLoadBGObject(GameObject obj)
        {
            return Instantiate(obj, _ObjRoot);
        }


        protected override void OnEnter()
        {
            CellBig.UI.IDialog.RequestDialogEnter<BallPlayTutorialDialog>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg(true, true, 0.5f));
            _sm = Model.First<SettingModel>();
            _sm.BallPlay_isTutorial = true;

            _ObjRoot.gameObject.SetActive(true);

            _camera = _ObjRoot.GetChild(0).Find("BallCam").GetComponent<Camera>();



            BallManager = _ObjRoot.GetChild(0).GetComponentInChildren<BallManager>(true).gameObject;
            tutorialTime = StartCoroutine(TimeCheck());

            StartTick = 0;
            SoundManager.Instance.PlaySound(SoundType.SFBall_bgm_main_0);

            Message.AddListener<Output.ViewportContours>(Process);

            _Video = _ObjRoot.GetChild(0).GetChild(0).FindChildRecursive("Video").GetComponent<VideoPlayer>();
            _VideoTransTime = 3f;
            SetVideoClip();

            StartCoroutine(_cChangeVideo());
            checkguideSoundTime = StartCoroutine(_cPlayWaitingGuideSound());
        }

        IEnumerator _cPlayWaitingGuideSound()
        {
            yield return new WaitForSeconds(5.0f);
            while (true)
            {
                SoundManager.Instance.PlaySound(SoundType.SFX_Voice_Guide_1);
                yield return new WaitForSeconds(5.0f);
                SoundManager.Instance.PlaySound(SoundType.SFX_Voice_Guide_2);
                yield return new WaitForSeconds(20f);
            }
        }

        void SetVideoClip()
        {
            BGData bgdata = new BGData();
            bgdata.BGName = BallBGState.Change;
            bgdata.BGTime = _VideoTransTime;
            bgdata.VideoClip = Resources.Load<VideoClip>("Video/" + bgdata.BGName.ToString() + "Video");
            BGDataList.Add(bgdata);

            BGData bgdataA = new BGData();
            bgdataA.BGName = BallBGState.Earth;
            bgdataA.BGTime = 38;
            bgdataA.VideoClip = Resources.Load<VideoClip>("Video/" + bgdataA.BGName.ToString() + "Video");
            BGDataList.Add(bgdataA);

            BGData bgdataB = new BGData();
            bgdataB.BGName = BallBGState.Moon;
            bgdataB.BGTime = 10;
            bgdataB.VideoClip = Resources.Load<VideoClip>("Video/" + bgdataB.BGName.ToString() + "Video");
            BGDataList.Add(bgdataB);

            BGData bgdataC = new BGData();
            bgdataC.BGName = BallBGState.Space;
            bgdataC.BGTime = 10;
            bgdataC.VideoClip = Resources.Load<VideoClip>("Video/" + bgdataC.BGName.ToString() + "Video");
            BGDataList.Add(bgdataC);

        }

        void OnVideoPrepared(VideoPlayer source_)
        {
            _Video.Play();
        }

        IEnumerator _cChangeVideo()
        {
            int videoIndex = 1;
            while (true)
            {
                _Video.prepareCompleted += OnVideoPrepared;
                _Video.clip = BGDataList[videoIndex].VideoClip;
                _Video.Prepare();

                _sm.BGState = BGDataList[videoIndex].BGName;
                switch (_sm.BGState)
                {
                    case BallBGState.Earth:
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                        break;
                    case BallBGState.Moon:
                        Physics.gravity = new Vector3(0, -1.6f, 0);
                        break;
                    case BallBGState.Space:
                        Physics.gravity = new Vector3(0, 0f, 0);
                        break;
                    default:
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                        break;
                }


                if (BGDataList.Count > 2)
                {
                    yield return new WaitForSeconds(BGDataList[videoIndex].BGTime);
                    videoIndex += 1;
                    if (videoIndex >= BGDataList.Count) videoIndex = 1;

                    _Video.prepareCompleted += OnVideoPrepared;
                    _Video.clip = BGDataList[0].VideoClip;
                    _Video.Prepare();
                    yield return new WaitForSeconds(BGDataList[0].BGTime);
                }
                else
                {
                    yield return new WaitForSeconds(300f);
                }
            }
        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound(SoundType.BGM_WaterPlay_Waiting);
            SoundManager.Instance.StopSound(SoundType.AMB_WaterPlay_Water);
            //StopCoroutine(SetRotation());

            if (tutorialTime != null)
            {
                StopCoroutine(tutorialTime);
                tutorialTime = null;
            }
            if (_BallManager != null)
                Destroy(_BallManager);

            _sm.BallPlay_isTutorial = false;
            Message.RemoveListener<Output.ViewportContours>(Process);
            CellBig.UI.IDialog.RequestDialogExit<BallPlayTutorialDialog>();
        }

        protected void Process(Output.ViewportContours output)
        {
            //Debug.Log("============= output count : " + output.Value.Count);
            if (output.Value.Count > _sm.TutorialPassSensorCount)
            {
                StartTick++;
            }
            else
            {
                StartTick--;
                if (StartTick < 0)
                    StartTick = 0;
            }
            //Debug.Log("]]]]]]]]]]]]]]]]]]]]]]]] "+StartTick);
            if (StartTick > _sm.TutorialPassSensorTick)
            {
                StartCoroutine(SceneChange());
            }
        }

        IEnumerator TimeCheck()
        {
            float time = 0;
            while (time < _sm.TutorialTime)
            {
                time += Time.deltaTime;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        StartCoroutine(SceneChange());
                        yield break;
                    }
                }
                yield return null;
            }
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            yield return new WaitForSeconds(0.5f);
            CellBig.Scene.SceneManager.Instance.Load(Constants.SceneName.BallPlayTutorialScene);
        }

        IEnumerator SceneChange()
        {
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            SoundManager.Instance.StopAllSound();
            CellBig.Scene.SceneManager.Instance.nowScene = Constants.SceneName.BallPlayScene;
            CellBig.Scene.SceneManager.Instance.Load(Constants.SceneName.BallPlayScene);
            yield return null;
        }
    }
}