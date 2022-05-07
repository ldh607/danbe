using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRotation : MonoBehaviour
{
    public Vector3 root;

    private void Update()
    {
        transform.Rotate((transform.rotation.eulerAngles - root.normalized) * Time.deltaTime );
    }
}
