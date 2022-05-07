using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Tool;

public class MiddleObjectsActive : MonoBehaviour
{

    public List<GameObject> TargetObj = new List<GameObject>();
    public List<GameObject> ObjPositionCenter = new List<GameObject>();
    public List<GameObject> ObjPositionLeft  = new List<GameObject>();
    public List<GameObject> ObjPositionRight = new List<GameObject>();
    public FloorObjectsActive floor;

    private void OnEnable()
    {
        SetPosition();
    }

    void SetPosition()
    {
        if (floor == null)
            return;
        Transform centers;
        if (floor.IsLeftToCenter())
            centers = ObjPositionCenter[Random.Range(0,2)].transform;
        else
            centers = ObjPositionCenter[Random.Range(2, 4)].transform;

        TargetObj[0].SetActive(false);
        var pos = centers.position;
        TargetObj[0].transform.position = pos;
        TargetObj[0].transform.rotation = Quaternion.identity;
        TargetObj[0].SetActive(true);
        var component = TargetObj[0].GetComponentsInChildren<ActiveOneChildObject>();
        foreach (var item in component)
        {
            if (item.gameObject != TargetObj[0])
                item.ActiveObj(centers.GetComponent<ValueBool>().Value ? 0 : 1);
        }

        Transform left;
        if (floor.IsLeftToLeft())
            left = ObjPositionLeft[Random.Range(0, 2)].transform;
        else
            left = ObjPositionLeft[Random.Range(2, 4)].transform;

        TargetObj[1].SetActive(false);
        pos = left.position;
        TargetObj[1].transform.position = pos;
        TargetObj[1].transform.rotation = Quaternion.identity;
        TargetObj[1].SetActive(true);
        component = TargetObj[1].GetComponentsInChildren<ActiveOneChildObject>();
        foreach (var item in component)
        {
            if (item.gameObject != TargetObj[1])
                item.ActiveObj(left.GetComponent<ValueBool>().Value ? 0 : 1);
        }

        Transform right;
        if (floor.IsLeftToRight())
            right = ObjPositionRight[Random.Range(0, 2)].transform;
        else
            right = ObjPositionRight[Random.Range(2, 4)].transform;

        TargetObj[2].SetActive(false);
        pos = right.position;
        TargetObj[2].transform.position = pos;
        TargetObj[2].transform.rotation = Quaternion.identity;
        TargetObj[2].SetActive(true);
        component = TargetObj[2].GetComponentsInChildren<ActiveOneChildObject>();
        foreach (var item in component)
        {
            if (item.gameObject != TargetObj[2])
                item.ActiveObj(right.GetComponent<ValueBool>().Value ? 0 : 1);
        }

    }
}
