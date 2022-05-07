using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRoot : MonoBehaviour
{
    public float m_fLifeTime = 1.0f;

    public void Active()
    {
    }

    IEnumerator Cor_Time()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(m_fLifeTime);
        gameObject.SetActive(false);
    }
}
