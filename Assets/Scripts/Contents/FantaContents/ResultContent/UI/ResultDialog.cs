using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CellBig.Models;
using CellBig.UI.Event;

namespace CellBig.UI
{

    public class ResultDialog : IDialog
    {
       public Animator mAnimator;
        int iFinalScore = 0;
        public Text mResultText;

        public GameObject scorePanel;

        GameModel gm;

        bool isScore;

        protected override void OnEnter()
        {
            gm = Model.First<GameModel>();

            AddMessage();

            // 애니메이션 시작
            mAnimator.gameObject.SetActive(true);
            mAnimator.Rebind();
            mAnimator.enabled = true;
            
            mResultText.text = "0";
        }

        protected override void OnExit()
        {
            mAnimator.enabled = false;

            RemoveMessage();
        }

        void AddMessage()
        {
            Message.AddListener<ResultTextMsg>(OnSetFinalScore);
            Message.AddListener<ResultScoreUpMsg>(OnReultScore);
            Message.AddListener<IsScoreContentMsg>(OnIsScoreContentMsg);
        }

        void RemoveMessage()
        {
            Message.RemoveListener<ResultTextMsg>(OnSetFinalScore);
            Message.RemoveListener<ResultScoreUpMsg>(OnReultScore);
            Message.RemoveListener<IsScoreContentMsg>(OnIsScoreContentMsg);
        }

        void OnIsScoreContentMsg(IsScoreContentMsg msg)
        {
            isScore = msg.IsScore;
        }

        void OnSetFinalScore(ResultTextMsg t)
        {
            iFinalScore = t.Score;
        }
        void OnReultScore(ResultScoreUpMsg m)
        {
            mAnimator.gameObject.SetActive(!isScore);
            scorePanel.SetActive(isScore);

            mResultText.text = 0.ToString();
            StartCoroutine(Cor_ScoreUp());
        }

        IEnumerator Cor_ScoreUp()
        {
            int Score = 0;
            while (Score < iFinalScore)
            {
                if (iFinalScore - 100000 > Score) Score += 10000;
                else if (iFinalScore - 10000 > Score) Score += 1000;
                else if (iFinalScore - 1000 > Score) Score += 100;
                else if (iFinalScore - 100 > Score) Score += 10;
                else Score++;
                mResultText.text = Score.ToString();
                yield return null;
            }

            yield return new WaitForSeconds(1.0f);

            // Clear 연출이 끝났을때
            Message.Send<Event.NextGameStartMsg>(new Event.NextGameStartMsg());
            scorePanel.gameObject.SetActive(false);

        }
    }
}

