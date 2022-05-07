using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBounce : MonoBehaviour
{
    public UnityEngine.PhysicMaterial mt;
    float Bounciness;
    Rigidbody rig;
    public float YForce;

    void Start()
    {
        this.GetComponent<CapsuleCollider>().material = Instantiate(mt, this.transform);
        this.GetComponent<CapsuleCollider>().material.bounciness = 0.6f;
        rig = this.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.name == "Line")
        {
            CB.Log("name: " + this.transform.name + "/ col: " + col.transform.name);
            this.GetComponent<CapsuleCollider>().material.bounciness = 0.7f;
            rig.velocity = Vector3.zero;
            Vector3 vec;
            vec = new Vector3(0, 1, 0) * YForce;
            rig.AddForce(vec);
        }
        else this.GetComponent<CapsuleCollider>().material.bounciness = 0.6f;

    }

}
