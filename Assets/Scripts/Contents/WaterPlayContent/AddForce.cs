using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    public Rigidbody rig;
    public Vector3 distance;
    public bool DisableKinematic;

    private void OnEnable()
    {
        rig.velocity = Vector3.zero;
        if( DisableKinematic)
            rig.isKinematic = false;
    }

    private void OnDisable()
    {        
        rig.velocity = Vector3.zero;
        if (DisableKinematic)
            rig.isKinematic = true;
    }

    private void Update()
    {        
        rig.AddForce( distance );
    }
}
