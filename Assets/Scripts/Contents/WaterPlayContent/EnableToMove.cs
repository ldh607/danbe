using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;

public class EnableToMove : MonoBehaviour
{
    public GameObject target;

    private void OnEnable()
    {
        this.transform.position = target.transform.position;
    }
}
