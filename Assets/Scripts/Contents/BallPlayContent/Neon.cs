using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using UnityEngine.SceneManagement;
public class Neon : MonoBehaviour
{
    NeonManager _NeonMgr;
    public GameObject NeonParticleOBJ;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "BallPlayScene")
            _NeonMgr = GameObject.Find("Neon").GetComponent<NeonManager>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Ball" && NeonParticleOBJ.activeSelf == false)
        {
            if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
                SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_neon_0);
            NeonParticleOBJ.SetActive(true);
            if (SceneManager.GetActiveScene().name == "BallPlayScene")
                _NeonMgr.CheckisOnAllNeons(NeonParticleOBJ);
        }
    }

}
