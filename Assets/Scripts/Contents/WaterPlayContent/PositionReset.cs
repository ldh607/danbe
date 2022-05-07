using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionReset : MonoBehaviour
{
    Coroutine reset;
    Vector3 StartPos;
    private void OnEnable()
    {
        StartPos = transform.position;
        reset = StartCoroutine(ResetPosition());
    }

    private void OnDisable()
    {
        if (reset != null)
        {
            transform.position = StartPos;
            StopCoroutine(reset);
        }
    }

    IEnumerator ResetPosition()
    {
        transform.position = transform.position + new Vector3(0, 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        transform.position = transform.position + new Vector3(0, 0, -0.1f);
    }
}
