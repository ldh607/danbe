using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetChildPos : MonoBehaviour
{
    [SerializeField]
    public GameObject child; 
    public Vector3 position;

    private void OnEnable()
    {
        foreach (var item in GetComponentsInChildren<Transform>(false))
        {
            if(item != this.transform )
            {
                child = item.gameObject;
                position = item.position;
                break;
            }
        }
    }
}
