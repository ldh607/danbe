using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendeerQueSet : MonoBehaviour
{
    Material mt;
    // Start is called before the first frame update
    void Start()
    {
        mt = this.transform.GetComponent<SpriteRenderer>().material;
        mt.renderQueue = 3000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
