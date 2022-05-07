using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderObjectActive : MonoBehaviour
{
    [System.Serializable]
    public class ObjData
    {
        public GameObject Target;
        public GameObject PositionObj;
    }

    public List<ObjData> TargetObjs = new List<ObjData>();
    public GameObject ObjPosition;
    public float LimitY;
    public bool EnableReset = true;

    private void OnEnable()
    {
        if (EnableReset)
        {
            ObjPosition.SetActive(false);
            for (int i = 0; i < TargetObjs.Count; i++)
            {
                    SetPosition(TargetObjs[i].PositionObj, i);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < TargetObjs.Count; i++)
        {
            if (TargetObjs[i].PositionObj.transform.position.y > LimitY 
                || !TargetObjs[i].Target.activeSelf)
            {
                if (!TargetObjs[i].Target.activeSelf)
                    TargetObjs[i].Target.SetActive(true);
                SetPosition(TargetObjs[i].PositionObj, i);
            }
        }
    }

    void SetPosition(GameObject item, int index)
    {
        item.SetActive(false);
        ObjPosition.SetActive(true);
        var list = ObjPosition.GetComponentsInChildren<GetChildPos>();
        item.transform.position = list[index].position;

        ObjPosition.SetActive(false);
        item.transform.rotation = Quaternion.identity;
        item.SetActive(true);
    }
}
