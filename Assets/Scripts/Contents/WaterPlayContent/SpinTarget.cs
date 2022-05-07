using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTarget : MonoBehaviour
{
    public delegate void RotateEnd(GameObject var);

    public GameObject Target;

    public float SpinSpeed = 1;
    float SpeedValue;
    public Vector3 Axis;
    public bool reverse;
    public Quaternion StartQuat;

    bool initTarget;
    public RotateEnd RoundEnd;

    private void OnEnable()
    {
        var scale = Target.transform.localScale;
        scale.z = Mathf.Abs(scale.z);
        if (Random.Range(-10,10 ) > 0)
        {
            reverse = true;
            scale.z *= -1;
        }
        else
        {
            reverse = false;
        }
        Target.transform.localScale = scale;
        SpeedValue = Random.Range(SpinSpeed - SpinSpeed/5, SpinSpeed + SpinSpeed / 5);
        initTarget = true;

        StartCoroutine(InitRot());

    }

    void Update()
    {
        //var destiny = Quaternion.Euler(Target.transform.localRotation.eulerAngles + Axis);
        if (reverse)
        {
            Target.transform.Rotate(-Axis * Time.deltaTime * SpeedValue, Space.Self);
        }
        else
        {
            Target.transform.Rotate(Axis * Time.deltaTime * SpeedValue, Space.Self); //  = Quaternion.Lerp(Target.transform.localRotation, destiny, Time.deltaTime * SpinSpeed);
        }
        if (!initTarget && Quaternion.Angle(StartQuat, Target.transform.rotation) < 5f)
        {
            RoundEnd(this.gameObject);
        }
    }

    IEnumerator InitRot()
    {
        yield return null;
        StartQuat = Target.transform.rotation;
        while (true)
        {
            if (Quaternion.Angle(StartQuat, Target.transform.rotation) > 5f)
            {
                initTarget = false;
                yield break;
            }
            yield return null;
        }
    }
}
