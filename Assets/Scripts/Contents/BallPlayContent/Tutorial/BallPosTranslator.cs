using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPosTranslator : MonoBehaviour
{
    BallManager _bm;
    public List<Vector2> BallPosList = new List<Vector2>();

    private void OnEnable()
    {
        _bm = GameObject.Find("BallManager").GetComponent<BallManager>();
        if (_bm == null)
            CB.Log("BM Null");
        else
        {
            _bm.BallPosList.Clear();
            for (int i = 0; i < BallPosList.Count; i++)
            {
                _bm.BallPosList.Add(BallPosList[i]);
            }
            _bm.Tuto_BallRespawn = true;
        }
    }
}
