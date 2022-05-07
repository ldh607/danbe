using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;

public class GoalManager : MonoBehaviour
{
    public SettingModel _sm;
    List<GameObject> GoalList;
    GameObject BonusOBJ;
    public bool isBonus = false;
    Coroutine _cCheckGoalTimeCor;
    public float GoalTime = 20f;
    public float Goal_WaitTime = 15f;
    void OnEnable()
    {
        _sm = Model.First<SettingModel>();
        GoalTime = _sm.Item_Goal_DelayTime;
        Goal_WaitTime = _sm.Item_Goal_WaitTime;
    }

    void Start()
    {
        GoalList = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GoalList.Add(transform.GetChild(i).gameObject);
        }

        BonusOBJ = transform.parent.Find("Bonus").gameObject;
        BonusOBJ.SetActive(false);
    }

    IEnumerator _cCheckGoalTime()
    {
        var curTime = 0.0f;
        while (curTime < GoalTime)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        foreach (var item in GoalList)
        {
            if (item.GetComponent<Goal>() != null)
            {
                item.GetComponent<Goal>().state = Goal.State.WaitBall;
                item.GetComponent<Goal>().isGoal = false;
                item.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (item.GetComponent<SpaceShip>() != null)
            {
                /*SomeThing else*/
            }
        }
        _cCheckGoalTimeCor = null;
    }

    public IEnumerator _cCheckAllGoal()
    {
        if (isBonus == true) yield break;

        if (_cCheckGoalTimeCor == null)
            _cCheckGoalTimeCor = StartCoroutine(_cCheckGoalTime());

        foreach (var item in GoalList)
        {
            if (item.GetComponent<Goal>() != null)
            {
                if (item.GetComponent<Goal>().isGoal == false)
                    yield break;
            }
            else if (item.GetComponent<SpaceShip>() != null)
            {
                if (item.GetComponent<SpaceShip>().state != SpaceShipState.Catching)
                    yield break;
            }
        }
        if (_cCheckGoalTimeCor != null)
        {
            StopCoroutine(_cCheckGoalTimeCor);
            _cCheckGoalTimeCor = null;
        }
        SetBonus();
    }
    public void OffGoalFX()
    {
        foreach (var item in GoalList)
        {
            if (item.GetComponent<Goal>() != null)
            {
                item.GetComponent<Goal>().state = Goal.State.WaitBall;
                item.GetComponent<Goal>().isGoal = false;
                item.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void SetBonus()
    {
        BonusOBJ.SetActive(true);
        BonusOBJ.GetComponent<Shard>().isBonus = true;
        BonusOBJ.GetComponent<Shard>().OnBonus();
        isBonus = true;
    }
}
