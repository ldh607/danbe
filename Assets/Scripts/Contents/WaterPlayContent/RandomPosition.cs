using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public GameObject Target;
    public Vector3 minVal;
    public Vector3 maxVal;

    private void OnEnable()
    {
        SetPositioin();
    }

    public void SetPositioin()
    {
        float x = Random.Range(minVal.x, maxVal.x);
        float y = Random.Range(minVal.y, maxVal.y);
        float z = Random.Range(minVal.z, maxVal.z);

        Target.transform.localPosition = new Vector3(x, y, z);
    }
}
