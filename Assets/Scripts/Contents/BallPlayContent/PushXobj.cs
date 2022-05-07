using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushXobj : MonoBehaviour
{

    public Vector3 AddX = new Vector3(200,0,0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider col)
	{
        if (col.tag == "Ball")
        {
            var colrig = col.GetComponent<Rigidbody>();
            colrig.AddForce(AddX);
        }
	}
}
