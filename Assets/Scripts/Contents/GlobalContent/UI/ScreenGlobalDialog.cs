using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CellBig.UI
{
    public class ScreenGlobalDialog : IDialog
    {
        public Image Fade_Img;

        protected override void OnEnter()
        {
            Message.AddListener<Event.FadeInMsg>(OnFadeIn);
            Message.AddListener<Event.FadeOutMsg>(OnFadeOut);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Event.FadeInMsg>(OnFadeIn);
            Message.RemoveListener<Event.FadeOutMsg>(OnFadeOut);
        }

        void OnFadeIn(Event.FadeInMsg msg)
        {
            if (msg.all || msg.kiosk)
                Fade(true, msg.time);
        }

        void OnFadeOut(Event.FadeOutMsg msg)
        {
            if (msg.all || msg.kiosk)
                Fade(false, msg.time);
        }

        void Fade(bool fadeIn, float time)
        {
            var color = Fade_Img.color;
            color.a = fadeIn ? 0.0f : 1.0f;
            Fade_Img.DOFade(fadeIn ? 1.0f : 0.0f, time);
        }
    }
}

