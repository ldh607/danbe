using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReEnable : MonoBehaviour
{
    public List<GameObject> target = new List<GameObject>();
    public Vector2 RandomTimmer;
    public bool DoStart = false;
    public bool SetSpin = true;
    public bool IsEnable = true;

    private void OnEnable()
    {   
        foreach (var item in target)
        {
            if( SetSpin)
                item.GetComponentInChildren<SpinTarget>().RoundEnd = SetActive;
            if (DoStart)
                SetActive(item);
        }
    }

    public void SetActive(GameObject obj)
    {
        if (target.Contains(obj))
        {
            StartCoroutine(Reset(obj));
        }
    }

    IEnumerator Reset(GameObject obj)
    {
        obj.SetActive(!IsEnable);
        yield return new WaitForSeconds(Random.Range(RandomTimmer.x, RandomTimmer.y));
        obj.SetActive(IsEnable);
    }
}
