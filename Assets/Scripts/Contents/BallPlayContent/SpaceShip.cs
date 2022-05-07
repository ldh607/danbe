using System.Collections;
using UnityEngine;
using CellBig.Models;
using DG.Tweening;

public enum SpaceShipState
{
    Catching,
    Finding,
    Delaying
}
public enum SpaceMoveArrow
{
    Down,
    Up,
    Left,
    Right,
    Tuto,
}

public class SpaceShip : MonoBehaviour
{
    public Transform _DestTransform;
    public Transform _StartTransform;
    public Coroutine mCheckCoroutine;
    public float _MoveDistanceY = 5f;
    public float _MoveDistanceX = 10f;
    public float _CatchTime = 3f;
    public float _curCatchDelayTime = 0;
    private float _CatchDelayTime = 5f;
    public SettingModel _sm;
    public SpaceShipState state;
    public SpaceMoveArrow MoveArrow;
    public GoalManager _gm;

    void Start()
    {
        state = SpaceShipState.Finding;
        _sm = Model.First<SettingModel>();
        _gm = this.transform.parent.GetComponent<GoalManager>();
        _DestTransform = this.transform.Find("DestPos");
        _StartTransform = this.transform.Find("StartPos");
        if (_sm != null)
        {
            _MoveDistanceY = _sm.Item_SpaceShip_MoveDistanceY;
            _MoveDistanceX = _sm.Item_SpaceShip_MoveDistanceX;
            _CatchTime = _sm.Item_SpaceShip_Catch_Time;
            _CatchDelayTime = _sm.Item_SpaceShip_Catch_Delay;
        }
        mCheckCoroutine = null;
        _MoveShip();
    }

    public IEnumerator _cCheckCatchTime(GameObject catchobj)
    {
        //catchobj.transform.localPosition = _StartTransform.localPosition;
        catchobj.transform.DOLocalMove(_DestTransform.localPosition, _CatchTime);
        if (_gm != null && _gm.isBonus == false)
            StartCoroutine(_gm._cCheckAllGoal());

        yield return new WaitForSeconds(_CatchTime);
        catchobj.transform.SetParent(catchobj.GetComponent<Ball>()._bm.gameObject.transform.GetChild(1));
        var catchobjRig = catchobj.GetComponent<Rigidbody>();
        catchobjRig.useGravity = true;
        catchobj.GetComponent<CapsuleCollider>().isTrigger = false;
        Ball ball = catchobj.GetComponent<Ball>();
        ball.state = State.NonColliding;
        catchobjRig.velocity = Vector3.zero;
        Vector3 vec = this.transform.up * 1.5f * -400f * (((Physics.gravity.y) / -9.81f) + 0.1f);
        catchobjRig.AddForce(vec);
        state = SpaceShipState.Delaying;
        while (_curCatchDelayTime <= _CatchDelayTime)
        {
            _curCatchDelayTime += Time.deltaTime;
            yield return null;
        }

        _curCatchDelayTime = 0;
        state = SpaceShipState.Finding;
        mCheckCoroutine = null;
        yield break;
    }

    public void _MoveShip()
    {
        switch (MoveArrow)
        {
            case SpaceMoveArrow.Down:
                this.transform.DOLocalMoveY(this.transform.localPosition.y - _MoveDistanceY, 3).SetLoops(-1, LoopType.Yoyo);
                break;
            case SpaceMoveArrow.Up:
                this.transform.DOLocalMoveY(this.transform.localPosition.y + _MoveDistanceY, 3).SetLoops(-1, LoopType.Yoyo);
                break;
            case SpaceMoveArrow.Left:
                this.transform.DOLocalMoveX(this.transform.localPosition.x - _MoveDistanceX, 3).SetLoops(-1, LoopType.Yoyo);
                break;
            case SpaceMoveArrow.Right:
                this.transform.DOLocalMoveX(this.transform.localPosition.x + _MoveDistanceX, 3).SetLoops(-1, LoopType.Yoyo);
                break;
            case SpaceMoveArrow.Tuto:
                this.transform.DOLocalMoveX(this.transform.localPosition.x - 2f, 3).SetLoops(-1, LoopType.Yoyo);
                _CatchTime = 3f;
                break;
            default:
                break;
        }
    }

}
