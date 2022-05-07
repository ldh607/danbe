using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeToActive : MonoBehaviour
{
    public bool Activate = true;
    public float ActiveTimmer;
    public bool Deactivate = false;
    public float DeactiveTimmer;

    [Header("Smooting")]
    public bool Smooting;
    public float SmootingTime;

    public bool AxisX;
    public bool AxisY;
    public bool AxisZ;

    [Header("Target GameObject")]
    public GameObject Target;

    Vector3 targetScale;


    Coroutine ActiveChecker;
    Coroutine DeactiveChecker;
    Coroutine SmootingCor;

    private void Awake()
    {
        targetScale = Target.transform.localScale;
        Target.SetActive(false);
    }

    private void OnEnable()
    {
        if ( Activate)
            ActiveChecker = StartCoroutine(ActiveTimmerCheck());
        if( Deactivate)
            DeactiveChecker = StartCoroutine(DeactiveTimmerCheck());
    }

    private void OnDisable()
    {
        if (ActiveChecker != null)
        {
            StopCoroutine(ActiveChecker);
        }

        if (DeactiveChecker != null)
        {
            StopCoroutine(DeactiveChecker);
        }

        if( SmootingCor != null )
        {
            Target.transform.localScale = targetScale;
            StopCoroutine(SmootingCor);
        }
    }

    IEnumerator ActiveTimmerCheck()
    {
        yield return new WaitForSeconds(ActiveTimmer);
        if(Smooting)
        {
            SmootingCor = StartCoroutine(SmooTingActive(true));
        }
        else
            Target.SetActive(true);
    }

    IEnumerator DeactiveTimmerCheck()
    {
        yield return new WaitForSeconds(DeactiveTimmer);
        if (Smooting)
        {
            SmootingCor = StartCoroutine(SmooTingActive(false));
        }
        else
            Target.SetActive(false);
    }

    IEnumerator SmooTingActive(bool isActive)
    {
        Vector3 tempScale = targetScale;

        if (isActive)
        {
            if(AxisX )
            {
                tempScale.x = 0;
            }
            if( AxisY)
            {
                tempScale.y = 0;
            }
            if( AxisZ)
            {
                tempScale.z = 0;
            }
            Target.transform.localScale = tempScale;
            Target.SetActive(true);
        }
        yield return null;
        if( isActive )
        {
            Target.transform.DOScale(targetScale, SmootingTime);
        }
        else
        {
            Target.transform.DOScale(tempScale, SmootingTime);
        }

        yield return new WaitForSeconds(SmootingTime);
        if (!isActive)
            Target.SetActive(false);
    }
}
