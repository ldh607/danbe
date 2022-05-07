using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Tool;

public class FloorObjectsActive : MonoBehaviour
{
    public List<GameObject> TargetObj = new List<GameObject>();

    public GameObject ObjPosition;

    public List<GameObject> RightObj = new List<GameObject>();
    public List<GameObject> CenterObj = new List<GameObject>();
    public List<GameObject> LeftObj = new List<GameObject>();

    private void OnEnable()
    {
        ObjPosition.SetActive(false);
        SetPosition();
    }


    void SetPosition()
    {
        ObjPosition.SetActive(true);
        List<GetChildPos> list = new List<GetChildPos>();
        list.AddRange(ObjPosition.GetComponentsInChildren<GetChildPos>());

        var index = GetRandomIndex(3);

        for (int i = 0; i < TargetObj.Count; i++)
        {
            TargetObj[i].SetActive(false);
            var pos = list[index[i]].position;
            TargetObj[i].transform.position = pos;
            TargetObj[i].transform.rotation = Quaternion.identity;
            TargetObj[i].SetActive(true);
        }
        ObjPosition.SetActive(false);
    }

    List<int> GetRandomIndex(int count)
    {
        List<int> index = new List<int>();
        for (int i = 0; i < count; i++)
        {
            while (true)
            {
                bool isChack = false;
                int value = Random.Range(0, count);
                for (int j = 0; j < index.Count; j++)
                {
                    if (index[j] == value)
                        isChack = true;
                }
                if (!isChack)
                {
                    index.Add(value);
                    break;
                }
            }
        }
        return index;
    }

    public bool IsLeftToCenter( )
    {
        foreach (var item in TargetObj)
        {
            foreach (var Center in CenterObj)
            {
                if (item.transform.position == Center.transform.position)
                    return true;
            }
        }
        return false;
    }
    public bool IsLeftToLeft()
    {
        foreach (var item in TargetObj)
        {
            foreach (var Center in LeftObj)
            {
                if (item.transform.position == Center.transform.position)
                    return true;
            }
        }
        return false;

    }
    public bool IsLeftToRight()
    {
        foreach (var item in TargetObj)
        {
            foreach (var Center in RightObj)
            {
                if (item.transform.position == Center.transform.position)
                    return true;
            }
        }
        return false;

    }
}
