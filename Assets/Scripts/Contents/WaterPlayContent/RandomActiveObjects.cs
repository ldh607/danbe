using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActiveObjects : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public Vector2 RandomTime;

    Coroutine Checker;
    List<Coroutine> targetCor = new List<Coroutine>();

    private void OnEnable()
    {
        Checker = StartCoroutine(TargetCheck());
    }

    private void OnDisable()
    {
        foreach (var item in targetCor)
        {
            StopCoroutine(item);
        }
        targetCor.Clear();
        if (Checker != null)
            StopCoroutine(Checker);
    }

    IEnumerator TargetCheck()
    {
        while(true)
        {
            yield return null;
            foreach (var item in targets)
            {
                if(!item.activeSelf)
                {
                    targetCor.Add( StartCoroutine(TimeToActive( Random.Range(RandomTime.x, RandomTime.y) , item)) );
                }
            }
        }
    }

    IEnumerator TimeToActive(float time, GameObject target)
    {
        yield return new WaitForSeconds(time);
        target.SetActive(true);
    }
}
