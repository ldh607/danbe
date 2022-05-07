using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;

public class ColliderActive : MonoBehaviour
{
    public GameObject target;
    public string ObjectsName;
    public bool isActive = true;
    public SoundType ColliderSound;

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.name == ObjectsName )
        {
            SoundManager.Instance.PlaySound(ColliderSound);
            target.SetActive(isActive);
        }
    }
}
