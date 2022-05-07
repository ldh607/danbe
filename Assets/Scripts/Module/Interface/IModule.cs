using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CellBig.Contents;


namespace CellBig.Module
{
	public abstract class IModule : MonoBehaviour
	{
        public Constants.ModuleName moduleName;
        Action _onLoadComplete = null;

        bool _resourceLoadComplete = false;
        protected List<GameObject> CanvasObjs = new List<GameObject>();

        public void LoadModule(Action onComplete)
        {
            _onLoadComplete = onComplete;
            StartCoroutine(Load());
        }

        IEnumerator Load()
        {
            OnLoadStart();

            while(!_resourceLoadComplete)
                yield return null;

            OnLoadComplete();

            if (_onLoadComplete != null)
                _onLoadComplete();
        }

        public void SetResourceLoadComplete()
        {
            _resourceLoadComplete = true;
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

        public virtual void OnSceneLoadComplete()
        {
            // 씬이 로드가 완료 될때 호출이 된다.
        }

        public void Unload()
        {
            OnUnload();
            Destroy(gameObject);
        }

        protected virtual void OnUnload()
        {
            /* BLANK */
        }
    }
}
