using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;

public class ScoreTextControl : MonoBehaviour
{
    Transform mainCam;
    TextMesh targetMesh;

    public float timer = 0.25f;
    private float curtimer = 0.0f;

    private Vector3 originalScale;

    GameModel gm;
    PlayContentModel playContentModel;

    Coroutine scoreTextControlCor;

    public bool isHold;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
    }

    void Start ()
    {
        gm = Model.First<GameModel>();
        playContentModel = Model.First<PlayContentModel>();
        targetMesh = GetComponent<TextMesh>();

        mainCam = gm.playContent.Model.mainCamera.transform;
        
        targetMesh.text = "";
    }

    public void SetScore(int val)
    {
        if (!playContentModel.GetCurrentContent().isScore)
            return;

        curtimer = 0;
        transform.localScale = originalScale;

        if (val >= 0) targetMesh.text = "+" + val.ToString();
        else if (val < 0) targetMesh.text = val.ToString();

        if (scoreTextControlCor != null)
            StopCoroutine(scoreTextControlCor);

        scoreTextControlCor = StartCoroutine(ScoreTextControl_Cor());
    }

    IEnumerator ScoreTextControl_Cor()
    {
        while (curtimer < timer)
        {
            curtimer += Time.deltaTime;
            if(!isHold)
                transform.parent.LookAt(mainCam);
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.x + 180f, transform.localEulerAngles.z);
            transform.localScale = originalScale * Mathf.Cos(curtimer / timer);
            yield return null;
        }

        targetMesh.text = "";
    }
}
