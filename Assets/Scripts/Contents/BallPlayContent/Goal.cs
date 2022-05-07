using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using DG.Tweening;
using CellBig;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private GoalManager _gm;
    public SettingModel _sm;
    private float _MoveTime = 5.0f;
    private float _DelayTime = 20.0f;
    public float curTime = 0f;
    public State state;
    public bool isGoal = false;
    private float _GoalDistanceValue = 1.0f;
    Coroutine _cCheckStayBallCor;
    Coroutine _cCheckFXColOnCor;
    bool isChangeFxColor = false;
    public enum State
    {
        WaitBall,
        Delaying,
    }

    void OnEnable()
    {
        _sm = Model.First<SettingModel>();
        if (_sm != null)
        {
            _gm = transform.parent.GetComponent<GoalManager>();
            _MoveTime = _sm.Item_Goal_MoveTime;
            _DelayTime = _sm.Item_Goal_DelayTime;
            _GoalDistanceValue = _sm.Item_GoalDistanceValue;
            if (CellBig.Scene.SceneManager.Instance.nowScene == CellBig.Constants.SceneName.BallPlayScene)
            {
                var YSignValue = Mathf.Abs(this.transform.rotation.x) > 0.5f ? 1 : -1;
                this.transform.DOLocalMove(new Vector3(
                     this.transform.localPosition.x + _GoalDistanceValue * Mathf.Sin(Mathf.Deg2Rad * this.transform.localEulerAngles.x),
                     this.transform.localPosition.y + _GoalDistanceValue * YSignValue * Mathf.Cos(Mathf.Deg2Rad * this.transform.localEulerAngles.x),
                     this.transform.localPosition.z),
                     _MoveTime).SetLoops(-1, LoopType.Yoyo).SetDelay(0.5f);
            }
        }
    }

    private void Start()
    {
        state = State.WaitBall;
    }
    void ChangeFXColor()
    {
        if (isChangeFxColor == true) return;

        var fxParantOBJ = this.transform.GetChild(0).transform;
        for (int i = 0; i < fxParantOBJ.childCount; i++)
        {
            var colorCom = fxParantOBJ.GetChild(i).GetComponent<ParticleSystem>().colorOverLifetime;

            float alpha = colorCom.color.gradient.alphaKeys[0].alpha;

            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] {
            new GradientColorKey(Color.red, 0.0f),
            new GradientColorKey(Color.red, 1.0f) },
        new GradientAlphaKey[] {
            new GradientAlphaKey(0, 0.0f),
            new GradientAlphaKey(255, 1.0f) });

            colorCom.color = grad;
        }
        isChangeFxColor = true;
    }

    IEnumerator _cCheckFXColOn()
    {
        this.transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.transform.GetChild(2).gameObject.SetActive(false);
        _cCheckFXColOnCor = null;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ball")
        {
            if (this.transform.GetChild(0).gameObject.activeSelf == false)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                ChangeFXColor();
            }

            if ((_cCheckFXColOnCor == null && CellBig.Scene.SceneManager.Instance.nowScene == CellBig.Constants.SceneName.BallPlayScene) ||
                (_cCheckFXColOnCor == null && CellBig.Scene.SceneManager.Instance.nowScene == CellBig.Constants.SceneName.BallPlayTutorialScene))
                _cCheckFXColOnCor = StartCoroutine(_cCheckFXColOn());

            if (state == State.WaitBall)
            {
                state = State.Delaying;
                if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
                    SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_post_0);

                isGoal = true;
                if (CellBig.Scene.SceneManager.Instance.nowScene == CellBig.Constants.SceneName.BallPlayScene
                    || CellBig.Scene.SceneManager.Instance.nowScene == CellBig.Constants.SceneName.BallPlayTutorialScene)
                {
                    if (_gm.isBonus == false)
                        StartCoroutine(_gm._cCheckAllGoal());
                }
            }
        }
    }

    IEnumerator _cCheckStayBall(Ball ball, float stayTime)
    {
        float curTime = 0;
        while (curTime < stayTime)
        {
            curTime += Time.deltaTime;
            yield return null;
        }

        float RandomX = Random.Range(-0.3f, 0.3f);
        Vector3 vec = new Vector3(RandomX * 10, 1, 0) * 1;
        ball.GetComponent<Rigidbody>().AddForce(vec);

        _cCheckStayBallCor = null;
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.transform.tag == "Ball")
        {
            Ball ball = col.GetComponent<Ball>();
            if (_cCheckStayBallCor == null)
                _cCheckStayBallCor = StartCoroutine(_cCheckStayBall(ball, 0.05f));
        }
    }
}
