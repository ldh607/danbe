using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;

public class Shard : MonoBehaviour
{
    List<GameObject> ShardList = new List<GameObject>();
    public bool isBonus = false;
    GoalManager _gm;
    Coroutine _cCheckBonusTimeCor;
    public float curTime = 0f;
    public bool isStart = true;

    private void OnEnable()
    {
        _gm = transform.parent.Find("GoalManager").GetComponent<GoalManager>();
        for (int i = 0; i < transform.childCount; i++)
        {
            ShardList.Add(transform.GetChild(i).gameObject);
        }
    }

    public void OnBonus()
    {
        isStart = false;
        try
        {
            foreach (var item in ShardList)
            {
                item.gameObject.SetActive(true);
                item.GetComponent<ShardBomb>().isColison = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }
        _cCheckBonusTimeCor = StartCoroutine(_cCheckBonusTime());
    }

    IEnumerator _cCheckBonusTime()
    {
        curTime = 0f;
        while (curTime <= _gm.Goal_WaitTime)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        _gm.isBonus = false;
        isBonus = false;
        _gm.OffGoalFX();
        ResetBonus();
        transform.gameObject.SetActive(false);

        _cCheckBonusTimeCor = null;
    }

    public void CheckALLBonus()
    {
        foreach (var item in ShardList)
        {
            if (item.GetComponent<ShardBomb>().isColison == false)
                return;
        }

        _gm.isBonus = false;
        isBonus = false;
        _gm.OffGoalFX();
        ResetBonus();
        transform.gameObject.SetActive(false);
    }

    void ResetBonus()
    {
        foreach (var item in ShardList)
        {
            item.GetComponent<ShardBomb>().Reset();
            item.gameObject.SetActive(false);
        }
    }

}
