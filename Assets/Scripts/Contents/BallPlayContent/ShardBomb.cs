using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Models;
using CellBig;
using UnityEngine.SceneManagement;
public class ShardBomb : MonoBehaviour
{
    Shard _shard;
    SettingModel _sm;
    public bool isColison = false;
    public Vector3 initPos;
    public Rigidbody rig;

    void Start()
    {
      
        _shard = transform.parent.GetComponent<Shard>();
    }

    private void OnEnable()
    {
        _sm = Model.First<SettingModel>();
        rig = this.GetComponent<Rigidbody>();
        Reset();
    }

    public void Reset()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);
        if (_shard == null)
        {
            initPos = this.transform.localPosition;
        }
        rig.velocity = Vector3.zero;
        this.transform.localPosition = initPos;
        isColison = false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Ball" || col.transform.tag == "Shard")
        {
            if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
                SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_shard_0);

            StartCoroutine(_cShardBomb());
        }
    }



    IEnumerator _cShardBomb()
    {
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        isColison = true;
        _shard.CheckALLBonus();
        yield return new WaitForSeconds(0.3f);
        this.transform.gameObject.SetActive(false);
    }
}
