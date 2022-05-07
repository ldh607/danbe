using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CellBig.UI;

namespace CellBig.Contents
{
    public class IContentUILoader : MonoBehaviour
    {
        public List<string> _uiList = new List<string>();

        int _loadingCount = 0;
        Action _onLoadComplete;


        public virtual void Load(Action loadComplete)
        {
            _onLoadComplete = loadComplete;
            _loadingCount = _uiList.Count;

            for (int i = 0; i < _uiList.Count; i++)
                UIManager.Instance.Load(_uiList[i], OnUILoadComplete);

            StartCoroutine(Loading());
        }

        public virtual void Unload()
        {
            if (!UIManager.isAlive)
                return;

            for (int i = 0; i < _uiList.Count; i++)
            {
                GameObject ui = UIManager.Instance.Get(_uiList[i]);
                if (ui != null)
                    ui.GetComponent<IDialog>().Unload();

                UIManager.Instance.Unload(_uiList[i]);
            }
        }

        void OnUILoadComplete(GameObject ui)
        {
            ui.SetActive(true);

            var dialog = ui.GetComponent<IDialog>();
            dialog.Load();

            _loadingCount--;

            //Debug.LogFormat("UI load complete) Name: {0}", ui.name);
        }

        IEnumerator Loading()
        {
            yield return new WaitWhile(() => _loadingCount > 0);

            if (_onLoadComplete != null)
                _onLoadComplete();
        }

        public float CalculateLoadingProgress()
        {
            if (!UIManager.isAlive)
                return 0.0f;

            var progress = 0.0f;

            if (_uiList.Count > 0)
            {
                for (int i = 0; i < _uiList.Count; ++i)
                {
                    progress += UIManager.Instance.GetProgress(_uiList[i]);
                }

                progress /= (float)_uiList.Count;
            }
            else
            {
                progress = 1.0f;
            }

            return progress;
        }
    }
}
