using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public GameObject Target;
    public Vector3 minRot;
    public Vector3 maxRot;
    public bool isLocal = true;

    [Header("Smoothing")]
    public bool Smooting;
    public float SmootingSpeed;
    Coroutine smootingCor;

    private void OnEnable()
    {
        if (!Smooting)
            SetRotation();
        else
        {
            if( smootingCor == null )
                smootingCor = StartCoroutine(SmoothingChange());
        }
    }

    private void OnDisable()
    {
        if( smootingCor != null )
        {
            StopCoroutine(smootingCor);
            smootingCor = null;
        }
    }

    void SetRotation()
    {
        float x = Random.Range(minRot.x, maxRot.x);
        float y = Random.Range(minRot.y, maxRot.y);
        float z = Random.Range(minRot.z, maxRot.z);

        Target.transform.rotation = Quaternion.Euler(x, y, z);
    }

    IEnumerator SmoothingChange()
    {
        float x = Random.Range(minRot.x, maxRot.x);
        float y = Random.Range(minRot.y, maxRot.y);
        float z = Random.Range(minRot.z, maxRot.z);

        Debug.Log("Rot start");

        Quaternion destination;
        if( isLocal)
            destination = Quaternion.Euler(Target.transform.rotation.eulerAngles + new Vector3( x, y, z));
        else
            destination = Quaternion.Euler(new Vector3(x, y, z));
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
        smootingCor = null;
    }
}
