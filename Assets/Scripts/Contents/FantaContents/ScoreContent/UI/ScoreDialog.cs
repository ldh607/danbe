using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CellBig.UI.Event;
using CellBig.Common;

namespace CellBig.UI
{
    public class ScoreDialog : IDialog
    {
        int Score;
       public Text mTextui;
        //public GameObject mTextEffect;
        //ObjectPool mTextEffectPool;

        protected override void OnEnter()
        {
            Score = 0;
            mTextui.text = Score.ToString();

           // mTextEffectPool.PreloadObject(100, mTextEffect);
            Message.AddListener<Event.ADDScore>(AddScore);
            Message.AddListener<Event.PlayTimeOverMsg>(FinalScore);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Event.ADDScore>(AddScore);
            Message.RemoveListener<Event.PlayTimeOverMsg>(FinalScore);
        }

        // 스코어 합산
        void AddScore(ADDScore msg)
        {
            Score += msg.AddScore;
            Score = Mathf.Clamp(Score, 0, int.MaxValue);
            mTextui.text = Score.ToString(); 

        }

        // 시간이 다되는 메시지를 받으면 최종점주를 알려주는 메시지를 생성해 ClearDialog에 전달해 준다.
        void FinalScore(PlayTimeOverMsg m)
        {
            Message.Send<ResultTextMsg>(new ResultTextMsg(Score));
        }

    }
}

