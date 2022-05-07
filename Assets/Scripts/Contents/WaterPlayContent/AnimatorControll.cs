using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControll : MonoBehaviour
{
    public Animator Ani;
    public string KeyName = "Active";
    private void OnEnable()
    {
        Ani.SetBool(KeyName, true);
    }

    private void OnDisable()
    {
        Ani.SetBool(KeyName, false);
    }
}
