using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;

public class ReflectionManager : MonoBehaviour
{
    public SettingModel _sm;
    public float ForceY = 300;
    float _waitTime = 5f;
    public bool _StayInfinity = false;
    // Start is called before the first frame update
    void Start()
    {
        _sm = Model.First<SettingModel>();
        if (_sm != null)
        {
            _waitTime = _sm.Item_Reflection_NewCreateDelayTime;
            ForceY = _sm.Item_Reflection_AddforceY;
            _StayInfinity = _sm.Item_Reflection_InfinityStay;
            SetReflectionInit();
        }
    }

    public void SetReflectionInit()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).transform.GetComponent<Reflection>().setInitValue();
        }
    }

    public void SetupObj(GameObject reflectionOBJ)
    {
        StartCoroutine(CheckDelayTime(reflectionOBJ));
    }

    public IEnumerator CheckDelayTime(GameObject reflectionOBJ)
    {
        var anim = reflectionOBJ.transform.GetChild(0).GetComponent<Animator>();
        anim.SetTrigger("Hide");
        yield return new WaitForSeconds(2.0f);
        reflectionOBJ.gameObject.SetActive(false);
        anim.SetTrigger("Show");
        float _curWaitTime = 0;
        while (_curWaitTime <= _waitTime)
        {
            _curWaitTime += Time.deltaTime;
            yield return null;
        }
        reflectionOBJ.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        reflectionOBJ.GetComponent<Reflection>().state = ReflectionState.WaitingBall;
    }
}
