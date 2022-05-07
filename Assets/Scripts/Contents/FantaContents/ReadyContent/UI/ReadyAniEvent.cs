using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;
using CellBig;

public class ReadyAniEvent : MonoBehaviour
{
    public void AniEvent_UISound_Ready()
    {
    }

    public void AniEvent_UISound_Go()
    {
    }

    public void AniEvent_ReadyGoEnd()
    {
        Message.Send<ReadyGoEndMesg>(new ReadyGoEndMesg());
    }
}
