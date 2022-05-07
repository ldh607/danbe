using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationReset : MonoBehaviour
{
    public GameObject Target;
    public Vector3 StandardRot;
    public bool isSmooting;
    public float SmootingSpeed;

    private void OnEnable()
    {
        if (isSmooting)
            StartCoroutine(ResetRot());
        else
        {
            Target.transform.rotation = Quaternion.Euler(StandardRot);
        }
    }

    IEnumerator ResetRot()
    {
        var destination = Quaternion.Euler(StandardRot);
        while (true)
        {
            Target.transform.rotation = Quaternion.Lerp(Target.transform.rotation, destination, Time.deltaTime * SmootingSpeed);
            if (Quaternion.Angle(Target.transform.rotation, destination) < 0.1f)
            {
                Debug.Log("Rot End");
                break;
            }
            yield return null;
        }
    }
}
