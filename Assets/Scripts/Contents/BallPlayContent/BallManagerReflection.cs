using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManagerReflection : MonoBehaviour
{
    public float reflectionForce = 300f;


    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Ball")
        {
            Ball ball = col.GetComponent<Ball>();
            if (ball.state == State.NonColliding)
            {
                ball.state = State.Colliding;
                Vector3 vec = this.transform.up * -reflectionForce;

                col.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().AddForce(vec);
                CB.Log("Door col");
            }
            CB.Log("Door col But..");
        
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Ball")
        {
            Ball ball = col.GetComponent<Ball>();
            ball.state = State.NonColliding;
        }
    }


}
