using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRenderQueSet : MonoBehaviour
{
    Material mt;
    // Start is called before the first frame update
    void Start()
    {
        mt = this.GetComponent<ParticleSystemRenderer>().material;
        mt.renderQueue = 3000;
    }
}
