using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Ball")
        {
            if(this.gameObject.name == "In")
            {
                col.gameObject.layer = 12;
            }
            else if (this.gameObject.name == "Out")
            {
                col.gameObject.layer = 11;
            }
        }
    }


}
