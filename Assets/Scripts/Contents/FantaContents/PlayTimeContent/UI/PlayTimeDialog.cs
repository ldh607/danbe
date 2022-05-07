using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CellBig.UI
{
    public class PlayTimeDialog : IDialog
    {

        public Image mTimerImage;
        public Image mClockImage;
        public Image mHandleImage;

        Coroutine gameStartCor;

        public float a, b;


        protected override void OnEnter()
        {
            if (mTimerImage == null || mHandleImage == null)
                new System.Exception("NULL Timer UI/Handle");

            Message.AddListener<Event.PlayTimerStartMsg>(TimerStart);
            Message.AddListener<Event.PlaySkipMsg>(OnPlaySkipMsg);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Event.PlayTimerStartMsg>(TimerStart);
            Message.RemoveListener<Event.PlaySkipMsg>(OnPlaySkipMsg);
            if (gameStartCor != null)
            {
                StopCoroutine(gameStartCor);
                gameStartCor = null;
            }

        }

        // 스코어 합산
        void TimerStart(Event.PlayTimerStartMsg msg)
        {
            gameStartCor = StartCoroutine(Cor_TimerStart(msg.Timer));
        }

        IEnumerator Cor_TimerStart(int Timer)
        {
            float gameTime = Timer;
            float currentTimer = Timer;
            float x, y;
            int shakeIndex = 0;

            mClockImage.transform.localPosition = new Vector3(-550f, 260f, 0);

            // 시간이 간다.
            while (true)
            {
                //Debug.LogWarning("남은 시간: " + currentTimer);
                //mTimerImage.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(-600, 0, currentTimer / gameTime),0);
                mClockImage.transform.localPosition = new Vector3(-550, 260 - (520 - (520 * (currentTimer / gameTime))), mClockImage.transform.localPosition.z);
                mHandleImage.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 360.0f * (currentTimer / gameTime)));
                mTimerImage.fillAmount = currentTimer / gameTime;

                currentTimer -= Time.deltaTime;
                shakeIndex++;

                if (currentTimer <= 0)
                {
                    break;
                }
                else if (currentTimer < gameTime * 0.2f && shakeIndex % 5 == 0)
                {
                    x = Random.Range(-10.0f, 10.0f);
                    y = Random.Range(-10.0f, 10.0f);

                    mClockImage.transform.localPosition = new Vector3(mClockImage.transform.localPosition.x + x, 260 - (520 - (520 * (currentTimer / gameTime))) + y, mClockImage.transform.localPosition.z);
                }

                yield return null;

            }
            // 시간 종료시 끝나는 Msg 출력
            Message.Send<Event.PlayTimeOverMsg>(new Event.PlayTimeOverMsg());
        }


        void OnPlaySkipMsg(Event.PlaySkipMsg msg)
        {
            if (gameStartCor != null)
            {
                StopCoroutine(gameStartCor);
                gameStartCor = null;
            }

            Message.Send<Event.PlayTimeOverMsg>(new Event.PlayTimeOverMsg());
        }
    }
}

