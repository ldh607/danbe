using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI Message를 모아놈
namespace CellBig.UI.Event
{
    // Timer
    // 타이머 시작
    public class PlayTimerStartMsg : Message
    {
        public int Timer;

        public PlayTimerStartMsg(int timer)
        {
            Timer = timer;
        }
    }

    // 시간 오버
    public class PlayTimeOverMsg : Message
    {

    }

    public class PlaySkipMsg : Message { }


    // Score
    public class ADDScore : Message
    {
        public int AddScore;

        public ADDScore(int addScore)
        {
            AddScore = addScore;
        }
    }

    // ReadyGo
    public class ReadyGoEndMesg : Message
    {

    }

    // Result
    public class ResultTextMsg : Message
    {
        public int Score;

        public ResultTextMsg(int score)
        {
            Score = score;
        }
    }
    public class ResultScoreUpMsg : Message
    {
     

    }

    public class IsScoreContentMsg : Message
    {
        public bool IsScore;
        public IsScoreContentMsg(bool isScore)
        {
            IsScore = isScore;
        }
    }

    public class RestartGameMsg : Message { }

    // 다음씬으로 이동
    public class NextGameStartMsg : Message
    {

    }
}

