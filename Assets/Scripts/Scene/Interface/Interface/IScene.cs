using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CellBig.Contents;


namespace CellBig.Scene
{
    // 해당 씬을 구성하는 역할을 한다.
    // 씬의 구성은 씬에서 사용하는 UI를 로드하고 씬에 사용될 Prefab 등을 로드하는 것을 기본으로 한다.
    public abstract class IScene : MonoBehaviour
    {
        public Constants.SceneName sceneName;

        public List<string> contentsList = new List<string>();
        public List<string> defaultContentList = new List<string>();
        List<string> _enterContentList;

        Action _onLoadComplete = null;
        int _loadingContentsCount = 0;

        protected bool _isPostLoadScene = false;

        public void LoadAssets(List<string> enterContentList, Action onComplete, bool postLoad = false)
        {
            _enterContentList = enterContentList;
            _onLoadComplete = onComplete;
            _isPostLoadScene = postLoad;

            StartCoroutine(LoadContents());
        }

        IEnumerator LoadContents()
        {
            OnLoadStart();
            Application.targetFrameRate = -1;

            _loadingContentsCount = contentsList.Count;

            for (int i = 0; i < contentsList.Count; ++i)
            {
                yield return StartCoroutine(ContentsLoader.Instance.Load(contentsList[i],
                    c =>
                    {
                        _loadingContentsCount--;
                        OnContentLoadComplete(c);
                    }));
            }

            if (!_isPostLoadScene) EnterContents();
            OnLoadComplete();

            Application.targetFrameRate = 60;

            if (_onLoadComplete != null)
                _onLoadComplete();

            if (_isPostLoadScene) gameObject.SetActive(false);
        }

        void EnterContents()
        {
            for (int i = 0; i < defaultContentList.Count; i++)
            {
                Message.Send<Contents.Event.EnterContentMsg>(defaultContentList[i], new Contents.Event.EnterContentMsg());
            }
        }

        public void SetResourceLoadComplete()
        {
            //_resourceLoadComplete = true;
        }

        /// <summary>
        /// 이 콜백을 재정의 하게 되면 적절한 타이밍에 SetAssetLoadComplete() 를 호출해주어야 한다.
        /// </summary>
        protected virtual void OnLoadStart()
        {
            SetResourceLoadComplete();
        }

        protected virtual void OnLoadComplete()
        {
            /* BLANK */
        }

        protected virtual void OnContentLoadComplete(GameObject content)
        {
            /* BLANK */
        }

        public void Unload()
        {
            OnUnload();

            if (ContentsLoader.isAlive)
            {
                for (int i = 0; i < contentsList.Count; ++i)
                    ContentsLoader.Instance.Unload(contentsList[i]);
            }

            Destroy(gameObject);
        }

        protected virtual void OnUnload()
        {
            /* BLANK */
        }
    }
}
