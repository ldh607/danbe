using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Common;
using CellBig.Models;
using CellBig.Constants;

public class MonoPool<T> where T : MonoBehaviour
{
    readonly Queue<T> _pool = new Queue<T>();
    private Transform _Parent;
    private GameObject _Origin;

    /// <summary>
    /// If Origin is null, Create Empty GameObject
    /// </summary>
    public MonoPool(int count = 10, GameObject Origin = null, Transform Parent = null)
    {
        _Parent = Parent ?? new GameObject("PoolOf_" + typeof(T).Name).transform;

        if (Origin != null)
            Origin.SetActive(false);

        _Origin = Origin;

        for (int i = 0; i < count; ++i)
        {
            _pool.Enqueue(InstantiateObject());
        }
    }

    public T Get(bool Activate = true)
    {
        T go;

        if (_pool.Count == 0)
        {
            go = InstantiateObject();
        }
        else
        {
            go = _pool.Dequeue();
        }

        go.gameObject.SetActive(Activate);

        return go;
    }

    public void Pool(T go)
    {
        go.transform.SetParent(_Parent);
        go.gameObject.SetActive(false);

        _pool.Enqueue(go);
    }

    T InstantiateObject()
    {
        T newObj = _Origin == null ?
            new GameObject(typeof(T).Name).AddComponent<T>() :
            _Origin.GetComponent<T>() == null ?
            UnityEngine.Object.Instantiate(_Origin).AddComponent<T>() :
            UnityEngine.Object.Instantiate(_Origin).GetComponent<T>();

        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(_Parent);

        return newObj;
    }
}

public class BallManager : MonoBehaviour
{
    public SettingModel _sm;
    public ObjectPool BallPool;
    public ObjectPool FX_BallColPool;
    public float BallLiveLimitTime = 15f;
    public float BallRespawnTime = 3f;
    public float BallDynamicFriction = 0.5f;
    public float BallStaticFriction = 0.5f;
    public float BallBounciness = 0.5f;
    public float BallLiveCheckTime = 3.0f;
    public int BallCount = 5;
    public int CurLiveBall = 0;
    public BallBGState BGState;
    private GameObject _ball_Entrance;
    private Animator _entrance_anim;
    public Transform BallRespawn;
    public bool Tuto_BallRespawn = false;

    [SerializeField] GameObject Ball;
    [SerializeField] GameObject FX_Ball_Col;

    void Start()
    {
        _sm = Model.First<SettingModel>();


        if (_sm != null)
        {
            BallLiveLimitTime = _sm.BallLiveLimitTime;
            BallRespawnTime = _sm.BallRespawnTime;
            BallCount = _sm.BallCount;
            BallLiveCheckTime = _sm.BallLiveCheckTime;
            BGState = _sm.BGState;

            if (_sm.BallPlay_isTutorial)
            {
                BallRespawnTime = 5f;
                BallCount = 2;
                BallLiveLimitTime = 10f;
            }

        }
        if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
            BallRespawn = transform.Find("BallRespawnPoint").transform;

        SetBallPhysics();
        _ball_Entrance = this.transform.GetChild(0).gameObject;
        _entrance_anim = _ball_Entrance.GetComponent<Animator>();

        if (BallPool == null)
            BallPool = Util.Instance.CreateObjectPool(this.gameObject, Ball, 100);
        if (FX_BallColPool == null)
            FX_BallColPool = Util.Instance.CreateObjectPool(this.gameObject, FX_Ball_Col, 100);

        StartCoroutine(_cSpawnBall());
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Ball")
        {
            col.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.transform.GetComponent<Rigidbody>().AddForce(this.transform.up * (-200f));
        }
    }


    void SetBallPhysics()
    {
        if (_sm != null)
        {
            BallDynamicFriction = _sm.Ball_DynamicFriction;
            BallStaticFriction = _sm.Ball_StaticFriction;
            BallBounciness = _sm.Ball_Bounciness;
        }
    }

    public IEnumerator _cSet_FX_BallCol(Vector3 ColPos)
    {
        var FX_Ballcol = FX_BallColPool.GetObject(transform);
        FX_Ballcol.transform.position = ColPos;
        FX_Ballcol.GetComponent<ParticleSystem>().Play();
        float curFX_Time = 0;
        while (curFX_Time <= 1.0f)
        {
            curFX_Time += Time.deltaTime;
            yield return null;
        }
        FX_BallColPool.PoolObject(FX_Ballcol.gameObject);
    }

    public List<Vector2> BallPosList = new List<Vector2>();

    IEnumerator _cSpawnBall()
    {
        while (true)
        {
            if (!_sm.BallPlay_isTutorial)
            {
                if (CurLiveBall < BallCount)
                {
                    CurLiveBall += 1;
                    _entrance_anim.SetTrigger("Open");
                    yield return new WaitForSeconds(0.9f);
                    var ballObj = BallPool.GetObject(transform);

                    if (BallPosList.Count <= 0)
                        ballObj.transform.position = BallRespawn.position;
                    else
                    {
                        for (int i = 0; i < BallPosList.Count; i++)
                        {
                            if (i == 0)
                                ballObj.transform.position = BallPosList[0];
                            else
                            {
                                var ballObjs = BallPool.GetObject(transform);
                                ballObjs.transform.position = BallPosList[i];
                                ballObjs.GetComponent<Rigidbody>().AddForce(this.transform.up * (-200f));
                            }
                        }
                    }

                    yield return null;
                    ballObj.GetComponent<Rigidbody>().AddForce(this.transform.up * (-200f));
                    yield return new WaitForSeconds(BallRespawnTime);
                }
                BGState = _sm.BGState;
                yield return new WaitForSeconds(BallLiveCheckTime);
            }

            else if (_sm.BallPlay_isTutorial)
            {
                if (Tuto_BallRespawn)
                {
                    Tuto_BallRespawn = false;

                    _entrance_anim.SetTrigger("Open");
                    yield return new WaitForSeconds(0.9f);
                    var ballObj = BallPool.GetObject(transform);

                    if (BallPosList.Count <= 0)
                        ballObj.transform.position = BallRespawn.position;
                    else
                    {
                        for (int i = 0; i < BallPosList.Count; i++)
                        {
                            if (i == 0)
                                ballObj.transform.position = BallPosList[0];
                            else
                            {
                                var ballObjs = BallPool.GetObject(transform);
                                ballObjs.transform.position = BallPosList[i];
                                ballObjs.GetComponent<Rigidbody>().AddForce(this.transform.up * (-200f));
                            }
                        }
                    }
                    yield return null;
                    ballObj.GetComponent<Rigidbody>().AddForce(this.transform.up * (-200f));
                }
                BGState = _sm.BGState;
            }
            yield return null;
        }
    }

}