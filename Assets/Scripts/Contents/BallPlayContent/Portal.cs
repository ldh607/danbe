using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using CellBig;

public class Portal : MonoBehaviour
{
    public enum PortalState
    {
        Catching,
        Waiting,
        Resting,
    }

    SettingModel _sm;
    public GameObject _PortalOut;
    private GameObject _InPosGroup;
    private GameObject _OutPosGroup;
    private PortalState state = PortalState.Waiting;
    private bool isFirst = true;
    public List<Transform> _PortalInPosList;
    public List<Transform> _PortalOutPosList;
    public List<GameObject> _PortalCatchBallList = new List<GameObject>();

    public float LiveTime = 5;
    public float PortalReActiveTime = 10;

    IEnumerator _cResetPosition()
    {
        ResetPosition();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        float curTime = 0;
        state = PortalState.Waiting;
        while (curTime <= LiveTime)
        {
            if (state == PortalState.Catching)
                curTime = 2;

            curTime += Time.deltaTime;
            yield return null;
        }
        state = PortalState.Resting;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        float curWaitTime = 0f;
        while (curWaitTime <= PortalReActiveTime)
        {
            curWaitTime += Time.deltaTime;

            yield return null;
        }
        StartCoroutine(_cResetPosition());
    }

    private void Start()
    {
        _sm = Model.First<SettingModel>();
        if (_sm != null)
        {
            LiveTime = _sm.Item_PortalLiveTime;
            PortalReActiveTime = _sm.Item_PortalReActiveTime;
        }
        StartCoroutine(_cResetPosition());
    }


    void ResetPosition()
    {
        if (_sm != null)
        {
            if (_PortalCatchBallList.Count != 0 || _sm.BallPlay_isTutorial == true) return;
        }
        this.transform.GetChild(1).gameObject.SetActive(false);
        _PortalOut.transform.GetChild(1).gameObject.SetActive(false);

        int InRandomIndex = Random.Range(0, _PortalInPosList.Count);
        this.transform.SetParent(_PortalInPosList[InRandomIndex]);
        this.transform.localPosition = Vector3.zero;
        this.transform.GetChild(1).gameObject.SetActive(true);

        int OutRandomIndex = Random.Range(0, _PortalOutPosList.Count);
        _PortalOut.transform.SetParent(_PortalOutPosList[OutRandomIndex]);
        _PortalOut.transform.localPosition = Vector3.zero;
        _PortalOut.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Ball" && state != PortalState.Resting)
        {
            CB.Log("PortalCols ball");
            if (col.GetComponent<Ball>().state != State.Colliding)
            {
                col.GetComponent<Ball>().state = State.Colliding;
                _PortalCatchBallList.Add(col.gameObject);
                StartCoroutine(RotateBallInside(col.gameObject));
            }
        }
    }

    IEnumerator RotateBallInside(GameObject BallOBJ)
    {
        state = PortalState.Catching;
        float radius = 1.3f;
        var BallRig = BallOBJ.GetComponent<Rigidbody>();
        BallRig.useGravity = false;
        BallRig.velocity = Vector3.zero;
        var BallCollider = BallOBJ.GetComponent<CapsuleCollider>();
        BallCollider.isTrigger = true;
        Vector3 retVector = Vector3.zero;
        int degree = 0;
        if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
            SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_blackhall_0);
        while (radius > 0.3f)
        {
            retVector = this.transform.position;
            degree += 10;
            float radian = degree * Mathf.PI / 180;
            retVector.x += radius * Mathf.Cos(radian);
            retVector.y += radius * Mathf.Sin(radian);
            BallOBJ.transform.position = new Vector3(retVector.x, retVector.y, 0);
            radius -= 0.008f;
            BallOBJ.transform.localScale = BallOBJ.transform.localScale * 0.99f;
            if (radius < 0.3f) break;
            yield return null;
        }
        BallOBJ.transform.position =
              _PortalOut.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.transform.position;
        BallOBJ.transform.localScale = Vector3.one;
        var ball = BallOBJ.GetComponent<Ball>();
        ball.state = State.NonColliding;
        ball.ResetZPosition();
        BallRig.useGravity = true;
        BallCollider.isTrigger = false;
        _PortalCatchBallList.Remove(BallOBJ);
        BallOBJ.transform.localScale = Vector3.one;
        yield return null;
        ball._DestroyBall();
        state = PortalState.Waiting;
    }
}
