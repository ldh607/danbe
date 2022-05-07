using System.Collections;
using UnityEngine;
using CellBig;

public enum ReflectionState
{
    WaitingBall,
    WaitDelaying,
}

public class Reflection : MonoBehaviour
{
    ReflectionManager _RefMgr;
    public float _forceY;
    public bool _isStay = false;
    Coroutine _cShootBallCor;
    public ReflectionState state;

    public void setInitValue()
    {
        _RefMgr = this.transform.parent.GetComponent<ReflectionManager>();
        _forceY = _RefMgr.ForceY;
        _isStay = _RefMgr._StayInfinity;
        state = ReflectionState.WaitingBall;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Ball" && state == ReflectionState.WaitingBall)
        {
            Ball ball = col.transform.GetComponent<Ball>();
            if (_cShootBallCor == null)
                _cShootBallCor = StartCoroutine(_cShootBall(ball));
        }
    }


    IEnumerator _cShootBall(Ball ball)
    {
        if (ball.state == State.NonColliding)
        {
            ball.state = State.Colliding;
            float RandomX = Random.Range(-0.3f, 0.3f);
            var ballrig = ball.GetComponent<Rigidbody>();
            ballrig.velocity = Vector3.zero;
            Vector3 vec = this.transform.rotation * new Vector3(RandomX, 2, 0) * _forceY * ((2f / (-9.81f / (Physics.gravity.y / 2))) + 0.1f);
            ballrig.AddForce(vec);
            ball.state = State.NonColliding;

            CB.Log("Ball: " + ball.Ballnumber.ToString() + "/ Reflection Shootball: " + this.gameObject.name);
        }
        _cShootBallCor = null;
        yield return null;
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.transform.tag == "Ball" && state == ReflectionState.WaitingBall)
        {
            Ball ball = col.transform.GetComponent<Ball>();
            ball.state = State.NonColliding;
            if (_isStay == false)
            {
                state = ReflectionState.WaitDelaying;
                _RefMgr.SetupObj(this.gameObject);
            }

            if (_cShootBallCor != null)
            {
                StopCoroutine(_cShootBallCor);
                _cShootBallCor = null;
            }
        }
    }


}
