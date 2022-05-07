using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;

public class SpringManager : MonoBehaviour
{
    public SettingModel _sm;
    public float Force = 300;
    public float _waitTime = 5f;

    void Start()
    {
        _sm = Model.First<SettingModel>();
        if (_sm != null)
        {
            _waitTime = _sm.Item_Spring_NewCreateDelayTime;
            Force = _sm.Item_Spring_Addforce;
        }
    }

    public void SetupObj(GameObject springOBJ)
    {
        StartCoroutine(CheckDelayTime(springOBJ));
    }

    public IEnumerator CheckDelayTime(GameObject springOBJ)
    {
        yield return new WaitForSeconds(2.0f);
        var spring = springOBJ.transform.GetChild(1).GetComponent<Spring>();
        spring.state = SpringState.WaitingBall;
    }

}
