using UnityEngine;
using UnityEngine.UI;
using CellBig.UI;

namespace CellBig.Module
{
	public class BaseCameraModule : IModule
    {
        public int _CameraCount = 1;
        //Dictionary<string, UICamera> _UICamera = new Dictionary<string, UICamera>();
        UICamera _UICamera = null;

        protected override void OnLoadStart()
        {
            var fullpath = "BaseCamera/UICamera";
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, LoadComplete));
        }

        void LoadComplete(Object o)
        {
            for (int i = 0; i < _CameraCount; i++)
            {
                //UICamera uiCamera = new 
                var obj = Instantiate(o) as GameObject;
                obj.SetActive(true);
                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = new Vector3(20.0f * i, -20.0f, 0.0f);
                obj.name = string.Format("{0}_{1}", o.name, i + 1);

                _UICamera = obj.GetComponent<UICamera>();
                _UICamera.Setup(i);

                CanvasObjs.Add(_UICamera._Canvas[i].gameObject);
            }

            UIManager.Instance.SetCanvasObj(CanvasObjs);

            SetResourceLoadComplete();
        }

        public Camera GetCamera()
        {
            return _UICamera._Camera[0];
        }

        public GraphicRaycaster GetRaycaster()
        {
            return _UICamera.GetComponentInChildren<GraphicRaycaster>();
        }
    }
}
