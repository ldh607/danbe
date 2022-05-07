using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;

namespace CellBig.UI.Event
{
    public class GimmicActive : Message
    {
    }
}

public class VoiceObjSound : MonoBehaviour
{
    int currentCount =0 ; 
    private void OnEnable()
    {
        Message.AddListener<CellBig.UI.Event.GimmicActive>(Sound);
    }

    private void OnDisable()
    {
        Message.RemoveListener<CellBig.UI.Event.GimmicActive>(Sound);
    }
    void Sound(CellBig.UI.Event.GimmicActive msg)
    {
        currentCount++;
        if( currentCount > Model.First<CellBig.Models.SettingModel>().GimmicSoundCount)
        {
            currentCount = 0;
            SoundManager.Instance.PlaySound(SoundType.Voice_WaterPlay_Correct_1 + (int)Random.Range(0, 5));
        }
    }
}
