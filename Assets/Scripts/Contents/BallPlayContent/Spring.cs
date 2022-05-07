using System.Collections;
using UnityEngine;
using DG.Tweening;
using CellBig;
public enum SpringState
{
    WaitingBall,
    CatchingBall,
    WaitDelaying,
}
public class Spring : MonoBehaviour
{
    SpringManager _SprMgr;
    float _force;
    public SpringState state;
    public Sequence seq;
    public GameObject FX_Activate;
    public Transform Head_Entrance;
    private GameObject curBall;
    public Transform angle;

    void Start()
    {
        _SprMgr = this.transform.parent.parent.GetComponent<SpringManager>();
        FX_Activate = this.transform.GetChild(0).gameObject;
        Head_Entrance = this.transform.GetChild(1);
        FX_Activate.SetActive(false);
        _force = _SprMgr.Force;
        state = SpringState.WaitingBall;
        seq = DOTween.Sequence();
        seq.SetAutoKill(false);
        SetAngle();

    }

    void SetAngle()
    {
        angle = this.transform.parent.Find("angle");
    }

    private void OnEnable()
    {
        FX_Activate = this.transform.GetChild(0).gameObject;
        FX_Activate.SetActive(false);

    }

    public void KillSequence()
    {
    }

    private void OnTriggerEnter(Collider col)
    {
        CB.Log("OnTriggerEnter");
        if (col.transform.tag == "Ball" && state == SpringState.WaitingBall)
        {
            Ball ball = col.GetComponent<Ball>();
            if (ball.state == State.NonColliding)
            {
                curBall = col.gameObject;
                col.gameObject.transform.SetParent(Head_Entrance);
                StartCoroutine(_cShootBall(col.gameObject));
            }
        }
    }
    IEnumerator _cShootBall(GameObject ballOBJ)
    {
        Ball ball = ballOBJ.GetComponent<Ball>();
        var ballRig = ball.GetComponent<Rigidbody>();
        ball.state = State.Colliding;
        ball.transform.localPosition = Vector3.zero;
        ballRig.velocity = Vector3.zero;
        ballRig.useGravity = false;
        yield return new WaitForSeconds(1.0f);
        Vector3 vec;
        if (angle != null)
            vec = angle.transform.up * _force * (((Physics.gravity.y) / -9.81f) + 0.1f);
        else
            vec = this.transform.up * _force * (((Physics.gravity.y) / -9.81f) + 0.1f);

        ballRig.velocity = Vector3.zero;
        ballOBJ.transform.SetParent(ball._bm.gameObject.transform);
        ball.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y, 0);
        ballRig.AddForce(vec);
        FX_Activate.SetActive(true);
        if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
            SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_cannon_0);
        ballRig.useGravity = true;

    }


    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Ball" && state == SpringState.WaitingBall)
        {
            if (col.gameObject == curBall)
            {
                Ball ball = col.GetComponent<Ball>();
                ball.state = State.NonColliding;
                state = SpringState.WaitDelaying;
                _SprMgr.SetupObj(this.transform.parent.gameObject);
            }
        }
    }
}
