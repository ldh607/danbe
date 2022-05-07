using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using CellBig.Models;
public class NeonManager : MonoBehaviour
{
    public SettingModel _sm;
    public List<GameObject> Neons;
    public float DelayTime = 20f;

    private void Start()
    {
        _sm = Model.First<SettingModel>();
        if (_sm != null)
        {
            DelayTime = _sm.Item_Neon_DelayTime;
        }
    }

    public void CheckisOnAllNeons(GameObject neon)
    {
        foreach (var neongroup in Neons)
        {
            for (int i = 0; i < neongroup.transform.childCount; i++)
            {
                if (neongroup.transform.GetChild(i).gameObject == neon)
                {
                    for (int j = 0; j < neongroup.transform.childCount; j++)
                    {
                        if (neongroup.transform.GetChild(j).gameObject.activeSelf == false)
                            return;

                        if (j == neongroup.transform.childCount - 1)
                        {
                            StartCoroutine(_cDelayingNeonGroup(neongroup));
                            return;
                        }
                    }
                    break;
                }
            }
        }
    }

    IEnumerator _cDelayingNeonGroup(GameObject neons)
    {
        float curTime = 0f;
        while (curTime <= DelayTime)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < neons.transform.childCount; i++)
        {
            neons.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

}
