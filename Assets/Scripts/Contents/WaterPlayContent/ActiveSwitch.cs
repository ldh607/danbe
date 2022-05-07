using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSwitch : MonoBehaviour
{
    public GameObject Root;
    public GameObject Slave;
    public bool isUpdate = true;
    bool lateState;
    private void OnEnable()
    {
        lateState = Root.activeSelf;
    }
    void Update()
    {
        if (isUpdate)
        {
            Slave.SetActive(!Root.activeSelf);
        }
        else
        {
           if( lateState != Root.activeSelf)
            {
                Slave.SetActive(!Root.activeSelf);
                lateState = Root.activeSelf;
            }
        }
    }
}
