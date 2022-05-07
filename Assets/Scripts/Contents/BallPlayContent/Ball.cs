using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using CellBig.Common;
using CellBig.Scene;
using CellBig;
using UnityEngine.SceneManagement;
public enum State
{
    NonColliding = 0,
    Colliding,
    Stop,
    OutOfScreen,
}

public class Ball : MonoBehaviour
{
    public BallManager _bm;
    [SerializeField] float _activeTime = 0;
    private float _limitime = 15;
    public Rigidbody _rig;
    public State state = State.NonColliding;
    public Vector3 velocity;
    private Camera _camera;
    private ParticleSystem FX_Collider;
    private PhysicMaterial _ballPM;
    Coroutine mCheckTimeCoroutine;
    Coroutine mCheckPosCoroutine;
    public int Ballnumber;

    void Awake()
    {
        _rig = GetComponent<Rigidbody>();
        FX_Collider = this.transform.GetChild(1).GetComponent<ParticleSystem>();
        _camera = GameObject.Find("BallCam").GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        velocity = _rig.velocity;
    }
    void SetBallPhysics()
    {
        _ballPM = this.GetComponent<CapsuleCollider>().material;
        _ballPM.dynamicFriction = _bm.BallDynamicFriction;
        _ballPM.staticFriction = _bm.BallStaticFriction;
        _ballPM.bounciness = _bm.BallBounciness;
    }

    public void ResetZPosition()
    {
        this.transform.SetParent(_bm.gameObject.transform);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
    }
    private void Start()
    {
        _bm = this.transform.parent.GetComponent<BallManager>();
        _limitime = _bm.BallLiveLimitTime;
        SetBallPhysics();

        if (mCheckTimeCoroutine != null)
        {
            StopCoroutine(mCheckTimeCoroutine);
            mCheckTimeCoroutine = null;
        }
        if (mCheckPosCoroutine != null)
        {
            StopCoroutine(mCheckPosCoroutine);
            mCheckPosCoroutine = null;
        }
        mCheckTimeCoroutine = StartCoroutine(_cCheckActiveTime());
        mCheckPosCoroutine = StartCoroutine(_cCheckPos());
        Ballnumber = Random.Range(0, 9999);
    }

    bool isTargetVisible()
    {
        if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene) return true;
        var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(this.transform.position) < 0)
                return false;
        }
        return true;
    }

    IEnumerator _cCheckPos()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            switch (_bm.BGState)
            {
                case CellBig.Constants.BallBGState.Change:
                    break;
                case CellBig.Constants.BallBGState.Earth:
                    _rig.angularDrag = 0.6f;
                    break;
                case CellBig.Constants.BallBGState.Moon:
                    _rig.angularDrag = 0.1f;
                    break;
                case CellBig.Constants.BallBGState.Space:
                    _rig.angularDrag = 0.0f;
                    break;
                default:
                    break;
            }


            if (this.gameObject.activeSelf == true && isTargetVisible() == false)
                _DestroyBall();



            yield return null;
        }
    }

    IEnumerator _cCheckActiveTime()
    {
        while (_activeTime <= _limitime)
        {
            yield return null;

            _activeTime += Time.deltaTime;
            if (this.state != State.Colliding)
                this.transform.localScale = Vector3.one;

            if (transform.position.y < -50f)
            {
                _DestroyBall();
                if (mCheckTimeCoroutine != null)
                {
                    StopCoroutine(mCheckTimeCoroutine);
                    mCheckTimeCoroutine = null;
                }
                yield break;
            }
        }
        _DestroyBall();
    }

    public void _DestroyBall()
    {
        if (mCheckTimeCoroutine != null)
        {
            StopCoroutine(mCheckTimeCoroutine);
            mCheckTimeCoroutine = null;
        }
        _bm.CurLiveBall -= 1;
        _activeTime = 0;
        _rig.velocity = Vector3.zero;
        this.transform.localScale = Vector3.one;
        _bm.BallPool.PoolObject(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(_bm._cSet_FX_BallCol(collision.contacts[0].point));
        FX_Collider.Play();
        if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
        {
            if (collision.transform.tag != "Shard")
                SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_float_0);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name != this.name)
            _activeTime = 0;
    }

    private void OnEnable()
    {
        this.transform.localScale = Vector3.one;
    }

}
