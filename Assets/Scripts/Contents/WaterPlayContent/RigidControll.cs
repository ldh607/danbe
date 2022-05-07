using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;

public class RigidControll : MonoBehaviour
{
    public Rigidbody rigi;
    public MonoBehaviour script;
    public float value = 1;
    public SoundType sound;
    void Update()
    {
        if( rigi.angularVelocity.magnitude > value)
        {
            if (!SoundManager.Instance.IsPlaySound(sound))
                SoundManager.Instance.PlaySound(sound);
            script.enabled = true;
        }
        else
        {
            if (SoundManager.Instance.IsPlaySound(sound))
                SoundManager.Instance.StopSound(sound);
            script.enabled = false;
        }
    }
}
