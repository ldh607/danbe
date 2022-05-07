//#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Common;
using System;


namespace CellBig.Module
{
	public class ModuleManager : MonoSingleton<ModuleManager>
	{
        public List<Constants.ModuleName> _ModuleLoads = new List<Constants.ModuleName>();

        Dictionary<Constants.ModuleName, GameObject> _modules;
        Transform _root;

        int _loadingCount = 0;
        Action _onLoadComplete;

        #region Initialize
        protected override void Init()
		{
            _root = GetComponent<Transform>();
            _modules = new Dictionary<Constants.ModuleName, GameObject>();
        }

		protected override void Release()
		{
			UnloadAll();
		}

        public GameObject GetRoot(Constants.ModuleName moduleName)
        {
            GameObject module;
            _modules.TryGetValue(moduleName, out module);
            return module;
        }
#endregion

#region Load/Unload
        public IEnumerator LoadAll(/*Action loadComplete*/)
        {
            //_onLoadComplete = loadComplete;
            _loadingCount = _ModuleLoads.Count;

            for (int i = 0; i < _loadingCount; i++)
                StartCoroutine(Load(_ModuleLoads[i]));

            yield return new WaitWhile(() => _loadingCount > 0);

            //if (_onLoadComplete != null)
            //    _onLoadComplete();
        }

        IEnumerator Load(Constants.ModuleName moduleName)
        {
            var module = GetRoot(moduleName);
            if (module == null)
            {
                var fullpath = string.Format("Modules/{0}", moduleName);
                yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));
            }
        }

        void OnPostLoadProcess(UnityEngine.Object o)
        {
            var module = Instantiate(o) as GameObject;

            var moduleScript = module.GetComponent<IModule>();
            module.name = moduleScript.moduleName.ToString();
            module.transform.SetParent(_root);

            _modules.Add(moduleScript.moduleName, module);

            module.SetActive(true);
            moduleScript.LoadModule( () => { _loadingCount--; } );
        }

        public void SceneLoadComplete()
        {
            foreach (var item in _modules)
            {
                var module = item.Value.GetComponent<IModule>();
                module.OnSceneLoadComplete();
            }
        }

        public void Unload(Constants.ModuleName moduleName)
        {
            var module = GetRoot(moduleName);
            if (module != null)
            {
                module.GetComponent<IModule>().Unload();
                _modules.Remove(moduleName);

                var fullpath = string.Format("Modules/{0}", module.name);
                if (ResourceLoader.isAlive)
                    ResourceLoader.Instance.Unload(fullpath);
            }
        }

        public void UnloadAll()
        {
            _modules.Clear();
        }
        #endregion
          public T GetModule<T>(Constants.ModuleName moduleName)
        {
            return _modules[moduleName].GetComponent<T>();
        }
    }
}
