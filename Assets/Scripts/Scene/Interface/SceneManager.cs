//#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Common;
using CellBig.Models;
using CellBig.Module;
//using Microsoft.Win32;

namespace CellBig.Scene
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        public Constants.SceneName startScene;
        public Constants.SceneName nowScene;
        //public Constants.SceneName gameName;

        // TODO: 한번에 여러 씬을 로딩하는 경우를 위해 구현한 부분인데 이제는 그럴일이 없다. 수정하자.
        List<GameObject> _inSettingsScenes;
        int _inSettingsScenesCount;

        Dictionary<Constants.SceneName, GameObject> _scenes;
        LinkedList<Constants.SceneName> _showList;

        Transform _root;

        //싱글톤 객체 플래그
        bool _InitSingletonObj = false;

        #region Initialize
        protected override void Init()
        {
            for (int i = 0; i < Display.displays.Length; i++)
                if (Display.displays.Length > 1)
                    Display.displays[i].Activate();

            _inSettingsScenes = new List<GameObject>();
            _scenes = new Dictionary<Constants.SceneName, GameObject>();
            _showList = new LinkedList<Constants.SceneName>();

            _root = GetComponent<Transform>();

            if (!_InitSingletonObj)
                StartCoroutine(Load_Singleton());
            else
                LoadStartScene();
        }

        void LoadStartScene()
        {
            if (startScene == Constants.SceneName.None)
                return;

            var sceneName = startScene;
            var scene = GetRoot(sceneName);
            if (scene == null)
            {
                var fullpath = string.Format("Scenes/{0}", sceneName);
                StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o)));
            }
        }

        protected override void Release()
        {
            UnloadAll();
        }

        #endregion
        //테이블 사운드를 여기서 로딩함.
        IEnumerator Load_Singleton()
        {
            var gameModel = new GameModel();
            gameModel.Setup();
            yield return StartCoroutine(SoundManager.Instance.cSetup());

            while (!SoundManager.Instance.IsComplete)
                yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(ModuleManager.Instance.LoadAll());

            LoadStartScene();

            _InitSingletonObj = true;
        }

        GameObject GetRoot(Constants.SceneName sceneName)
        {
            GameObject scene;
            _scenes.TryGetValue(sceneName, out scene);
            return scene;
        }

        public T GetScript<T>(Constants.SceneName sceneName)
        {
            var scene = GetRoot(sceneName);
            if (scene != null)
                return scene.GetComponent<T>();

            return default(T);
        }

        public T GetScript<T>()
        {
            var scene = GetRoot(EnumExtensions.Parse<Constants.SceneName>(typeof(T).Name.Replace("Scene", "")));
            if (scene != null)
                return scene.GetComponent<T>();

            return default(T);
        }

        #region Load/Unload

        //unload == true : 활성화중인 모든 씬을 즉시 제거
        //postLoad == true : 씬을 로드한 후 진입하지않고 비활성화
        public void Load(Constants.SceneName sceneName, bool unload = true, bool postLoad = false)
        {
            LoadRoot(sceneName, unload, postLoad);
        }

        void LoadRoot(Constants.SceneName sceneName, bool unload, bool postLoad)
        {
            if (unload)
                UnloadAll();

            if (!postLoad) nowScene = sceneName;

            var scene = GetRoot(sceneName);
            if (scene == null)
            {
                var fullpath = string.Format("Scenes/{0}", sceneName);
                StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, o => OnPostLoadProcess(o, postLoad)));
            }
            else
            {
                var sceneScript = scene.GetComponent<IScene>();
                SetupScene(scene, sceneScript.contentsList, postLoad);
            }
        }

        void OnPostLoadProcess(Object o, bool postLoad = false)
        {
            var scene = Instantiate(o) as GameObject;

            var sceneScript = scene.GetComponent<IScene>();
            scene.name = sceneScript.sceneName.ToString();
            scene.transform.SetParent(_root);

            _scenes.Add(sceneScript.sceneName, scene);
            SetupScene(scene, sceneScript.contentsList, postLoad);
        }

        void SetupScene(GameObject scene, List<string> enterContentList, bool postLoad = false)
        {
            _inSettingsScenes.Add(scene);
            _inSettingsScenesCount++;

            scene.SetActive(true);

            var sceneScript = scene.GetComponent<IScene>();
            sceneScript.LoadAssets(enterContentList,
                () =>
                {
                    BringToTop(sceneScript.sceneName);
                    _inSettingsScenes.Remove(scene);
                    ModuleManager.Instance.SceneLoadComplete();
                },
                postLoad);
        }

        public void Unload(Constants.SceneName sceneName)
        {
            var scene = GetRoot(sceneName);
            if (scene != null)
            {
                scene.GetComponent<IScene>().Unload();

                _scenes.Remove(sceneName);
                _showList.Remove(sceneName);

                var fullpath = string.Format("Scenes/{0}", scene.name);
                if (ResourceLoader.isAlive)
                    ResourceLoader.Instance.Unload(fullpath);
            }
        }

        public void UnloadAll()
        {
            LinkedListNode<Constants.SceneName> node;

            while (true)
            {
                node = _showList.First;
                if (node == null)
                    break;

                Unload(node.Value);
            }
        }

        #endregion

        public void Show(Constants.SceneName sceneName)
        {
            BringToTop(sceneName);
        }

        void BringToTop(Constants.SceneName sceneName)
        {
            _showList.Remove(sceneName);
            _showList.AddFirst(sceneName);

            var scene = GetRoot(sceneName);
            scene.transform.SetAsFirstSibling();
        }
    }
}
