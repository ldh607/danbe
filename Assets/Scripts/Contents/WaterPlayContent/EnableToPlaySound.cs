using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using UnityEngine.SceneManagement;
public class EnableToPlaySound : MonoBehaviour
{
    public SoundType sound;
    private void OnEnable()
    {
        if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
            SoundManager.Instance.PlaySound(sound);
        
    }
}
