using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CellBig.Contents.Event;

namespace CellBig.UI
{
    public class KioskGlobalDialog : IDialog
    {
        public Image Fade_Img;
        public Image eventColorImage;

        public Text frameText;

        float frame;
        float ms;
        float deltaTime;

        bool isFrame;

        Coroutine eventColorCor;

        protected override void OnEnter()
        {
            Message.AddListener<Event.FadeInMsg>(OnFadeIn);
            Message.AddListener<Event.FadeOutMsg>(OnFadeOut);

            Message.AddListener<ColorCameraMsg>(OnColorCameraMsg);
            this.dialogView.transform.parent.gameObject.transform.SetAsLastSibling();
            
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Event.FadeInMsg>(OnFadeIn);
            Message.RemoveListener<Event.FadeOutMsg>(OnFadeOut);
            Message.RemoveListener<ColorCameraMsg>(OnColorCameraMsg);
        }

        void OnFadeIn(Event.FadeInMsg msg)
        {
            this.gameObject.transform.SetAsLastSibling();
            if (msg.all || msg.kiosk)
                Fade(true, msg.time);
        }

        void OnFadeOut(Event.FadeOutMsg msg)
        {
            this.gameObject.transform.SetAsLastSibling();
            if (msg.all || msg.kiosk)
                Fade(false, msg.time);
        }

        void Fade(bool fadeIn, float time)
        {
            this.gameObject.transform.SetAsLastSibling();
            var color = Fade_Img.color;
            color.a = fadeIn ? 0.0f : 1.0f;
            Fade_Img.DOFade(fadeIn ? 1.0f : 0.0f, time);
        }

        void OnColorCameraMsg(ColorCameraMsg msg)
        {
            eventColorImage.color = Color.clear;
            eventColorImage.DOColor(msg.Color, msg.Duration).SetLoops(2, LoopType.Yoyo);

            if (eventColorCor != null)
                StopCoroutine(eventColorCor);

            eventColorCor = StartCoroutine(ColorCameraInit(msg.Duration));
        }

        IEnumerator ColorCameraInit(float time)
        {
            yield return new WaitForSeconds(time*2f);
            eventColorImage.color = Color.clear;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                frameText.gameObject.SetActive(!frameText.gameObject.activeSelf);
                isFrame = frameText.gameObject.activeSelf;
            }

            if (isFrame)
            {
                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
                ms = deltaTime * 1000f;
                frame = 1.0f / deltaTime; 
                frameText.text = "Frame/ms : " + frame.ToString() + "FPS / " + ms + "ms";
            }
        }
    }
}

