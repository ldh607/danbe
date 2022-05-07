using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopObjectsActive : MonoBehaviour
{
    [System.Serializable]
    public class TargetObj
    {
        public int index;
        public GameObject target;
        public Vector3 StartRot;
    }
    public List<TargetObj> TargetObjs = new List<TargetObj>();
    public GameObject ObjPosition;
    public float LimitY;
    public bool EnableReset = true;

    private void OnEnable()
    {
        if (EnableReset)
        {
            ObjPosition.SetActive(false);
            foreach (var item in TargetObjs)
            {
                SetPosition(item);
            }
        }
    }

    void Update()
    {
        foreach (var item in TargetObjs)
        {
            if( item.target.transform.position.y < LimitY )
            {
                SetPosition(item);
                item.target.GetComponent<Rigidbody>().velocity = Vector3.zero;
                item.target.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }

    void SetPosition(TargetObj item)
    {
        item.target.SetActive(false);
        ObjPosition.SetActive(true);
        var list = ObjPosition.GetComponentsInChildren<GetChildPos>();
        bool isSet = false;
        int tempIndex = -1;
        while (!isSet)
        {
            tempIndex = Random.Range(0, list.Length);
            foreach (var target in TargetObjs)
            {
                if (target != item)
                {
                    if (target.index != tempIndex)
                    {
                        isSet = true;
                    }
                }
            }
        }
        item.target.transform.position = list[tempIndex].position;
        item.index = tempIndex;

        ObjPosition.SetActive(false);
        item.target.transform.rotation = Quaternion.Euler(item.StartRot);
        item.target.SetActive(true);
    }
}
