using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBowlRenderSet : MonoBehaviour
{

    Material mt;
    // Start is called before the first frame update
    void Start()
    {
        mt = this.GetComponent<MeshRenderer>().materials[0];
        mt.renderQueue = 3000;
    }
}
