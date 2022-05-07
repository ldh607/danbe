using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Common;
public class TrailOBJ : MonoBehaviour
{
    public List<Vector2> MovePosList = new List<Vector2>();
    Coroutine _cMovingCor;
    TrailRenderer trail;
    public ObjectPool trailpool;

    private void OnEnable()
    {
        trail = this.transform.GetComponent<TrailRenderer>();
        trail.enabled = false;

        this.transform.localPosition = Vector3.zero;
        MovePosList.Clear();
        if (_cMovingCor == null)
            _cMovingCor = StartCoroutine(_cMoving());
    }

    IEnumerator _cMoving()
    {
        yield return null;
        if (MovePosList.Count == 0 || MovePosList == null)
        {
            CB.Log("MovePos is null");
            yield break;
        }


        int index = MovePosList.Count - 1;
        while (index >= 0)
        {
            this.transform.position = MovePosList[index];
            trail.enabled = true;
            index--;
            yield return null;
        }
        trailpool.PoolObject(this.gameObject);
    }

    private void OnDisable()
    {
        if (_cMovingCor != null)
            StopCoroutine(_cMoving());
        _cMovingCor = null;

        if (trailpool != null)
            trailpool.PoolObject(this.gameObject);
    }
}
