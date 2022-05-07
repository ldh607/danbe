using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WindRigid : MonoBehaviour
{
    private float _WindStrength = 4;
    private Transform _WindTransformPosition;
    private GameObject _WindRangeObj;
    private int _BallLayerMask;
    public float WindStrengthMin = 0;
    public float WindStrengthMax = 10;
    public float WindRadius = 10;


    private void Start()
    {
        _WindTransformPosition = this.transform;
        _WindRangeObj = transform.GetChild(0).transform.gameObject;
        _BallLayerMask = 1 << 10;
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        _WindRangeObj = transform.GetChild(0).transform.gameObject;
#endif
        _WindRangeObj.transform.localScale = new Vector3(WindRadius, WindRadius, WindRadius) * 2;

        if (_WindTransformPosition != null)
        {
            _WindStrength = Random.Range(WindStrengthMin, WindStrengthMax);

            var hitColliders = Physics.OverlapSphere(_WindTransformPosition.transform.position, WindRadius, _BallLayerMask);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].GetComponent<Rigidbody>() != null)//&& hitColliders[i].name == "BallPrefab(Clone)")
                {
                    var rayDirection = hitColliders[i].transform.position - _WindTransformPosition.transform.position;
                    RaycastHit hit;

                    if (Physics.BoxCast(_WindTransformPosition.transform.position, Vector3.one * .1f, rayDirection, out hit, Quaternion.Euler(Vector3.zero), Mathf.Infinity))
                    {
                        //Debug.DrawRay(transform.position, rayDirection, Color.blue);
                        //Debug.LogError(hit.transform.name);
                        if (hit.transform != hitColliders[i].transform)
                        {
                            //Debug.Log("Line Collider");
                            return;
                        }
                    }
                    Ball ball = hitColliders[i].GetComponent<Ball>();
                    if(ball != null&& ball.state != State.Stop)
                    hitColliders[i].GetComponent<Rigidbody>().AddForce(_WindTransformPosition.transform.forward * _WindStrength, ForceMode.Acceleration);
                }
            }
        }
    }
}


//public class LateFollow : MonoBehaviour
//{
//    public Transform FollowTarget;
//    Vector3 _localPosShift;
//    Quaternion _localRotShift;

//    void Awake()
//    {
//        if (FollowTarget == null)
//        {
//            Debug.LogError("No follow target assigned for object " + gameObject.name);
//            enabled = false; return;
//        }
//        _localPosShift = FollowTarget.InverseTransformPoint(transform.position);
//        _localRotShift = Quaternion.Inverse(FollowTarget.rotation) * transform.rotation;
//    }

//    void LateUpdate()
//    {
//        transform.rotation = FollowTarget.rotation * _localRotShift;
//        transform.position = FollowTarget.TransformPoint(_localPosShift);
//    }
//}
