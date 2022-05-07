using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Output = CellBig.Module.Detection.CV.Output;
using UnityEngine.SceneManagement;
using CellBig.Models;


namespace CellBig.Contents
{
    public class WaterPlayTutorialContent : IContent
    {
        private Transform _ObjRoot;

        public Camera _camera;
        GameObject Lines;
        GameObject Balloons;
        SettingModel sm;
        int StartTick = 0;

        Coroutine tutorialTime;
        Coroutine checkguideSoundTime;

        protected override void OnLoadStart()
        {
            _ObjRoot = new GameObject("ObjRoot").transform;
            _ObjRoot.SetParent(transform);
            _ObjRoot.ResetLocal();

            StartCoroutine(_cLoadProcess());
        }

        private IEnumerator _cLoadProcess()
        {
            yield return StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>("WaterPlayBundle", "WaterPlayTutorialGame", (o) =>
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
            sm = Model.First<SettingModel>();
            _ObjRoot.gameObject.SetActive(true);
            _camera = _ObjRoot.GetChild(0).Find("Main Camera").GetComponent<Camera>();
            _camera.transform.localPosition = new Vector3(0, 10, -20);
            Lines = _ObjRoot.GetChild(0).Find("Lines").gameObject;
            Lines.transform.localPosition = Vector3.zero;
            Balloons = _ObjRoot.GetChild(0).Find("Balloons").gameObject;
            Balloons.transform.localPosition = new Vector3(-9, 10, 0.246f);
            tutorialTime = StartCoroutine(TimeCheck());
            CellBig.UI.IDialog.RequestDialogEnter<WaterPlayTutorialDialog>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg(true, true, 0.5f));

            StartTick = 0;
            this.GetComponent<VoiceObjSound>().enabled = true;
            SoundManager.Instance.PlaySound(SoundType.Voice_WaterPlay_Wellcome);

            SoundManager.Instance.PlaySound(SoundType.BGM_WaterPlay_Waiting);
            SoundManager.Instance.PlaySound(SoundType.AMB_WaterPlay_Water);

            Message.AddListener<Output.ViewportContours>(Process);
            if (checkguideSoundTime == null)
                checkguideSoundTime = StartCoroutine(_cPlayWaitingGuideSound());
        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound(SoundType.BGM_WaterPlay_Waiting);
            SoundManager.Instance.StopSound(SoundType.AMB_WaterPlay_Water);
            SoundManager.Instance.StopSound(SoundType.SFX_Voice_Guide_1);
            SoundManager.Instance.StopSound(SoundType.SFX_Voice_Guide_2);

            if (checkguideSoundTime !=null)
            {
                StopCoroutine(checkguideSoundTime);
                checkguideSoundTime = null;
            }

            if (tutorialTime != null)
                {
                    StopCoroutine(tutorialTime);
                    tutorialTime = null;
                }
            this.GetComponent<VoiceObjSound>().enabled = false;
            Message.RemoveListener<Output.ViewportContours>(Process);

            CellBig.UI.IDialog.RequestDialogExit<WaterPlayTutorialDialog>();
        }

        protected void Process(Output.ViewportContours output)
        {
            //Debug.Log("============= output count : " + output.Value.Count);
            if (output.Value.Count > sm.TutorialPassSensorCount)
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
            if (StartTick > sm.TutorialPassSensorTick)
            {
                StartCoroutine(SceneChange());
            }
        }

        IEnumerator TimeCheck()
        {
            float time = 0;
            while (time < sm.TutorialTime)
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
            IContent.RequestContentExit<WaterPlayTutorialContent>();
            IContent.RequestContentEnter<WaterPlayTutorialContent>();
        }

        IEnumerator SceneChange()
        {
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            SoundManager.Instance.StopAllSound();
            yield return new WaitForSeconds(0.5f);
            CellBig.Scene.SceneManager.Instance.Load(Constants.SceneName.WaterPlayScene);
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
    }
}