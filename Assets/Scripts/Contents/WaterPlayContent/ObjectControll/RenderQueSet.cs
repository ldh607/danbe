using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderQueSet : MonoBehaviour
{
    Material[] mts;
    void Start()
    {
        mts = this.GetComponent<SkinnedMeshRenderer>().materials;
        foreach (var item in mts)
        {
            item.renderQueue = 3000;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
