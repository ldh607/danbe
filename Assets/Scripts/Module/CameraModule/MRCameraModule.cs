using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.UI;

namespace CellBig.Module
{
    public class MRCameraModule : IModule
    {
        public int _CameraCount = 2;
        Dictionary<string, UICamera> _UICamera = new Dictionary<string, UICamera>();

        protected override void OnLoadStart()
        {
            LoadComplete();
        }

        void LoadComplete()
        {
            var mr_UICamera = gameObject.GetComponentInChildren<UICamera>();

            for (int i = 0; i < _CameraCount; i++)
            {
                mr_UICamera.Setup(i);
                CanvasObjs.Add(mr_UICamera._Canvas[i].gameObject);
            }

            UIManager.Instance.SetCanvasObj(CanvasObjs, true);

            SetResourceLoadComplete();
        }

        public override void OnSceneLoadComplete()
        {
            IDialog[] dialog = CanvasObjs[0].GetComponentsInChildren<IDialog>();

            Debug.Log("------" + CanvasObjs[0].GetComponentsInChildren<IDialog>());

            for(int index = 0; index < dialog.Length; index++)
            {
                GameObject tempDialog = Instantiate(dialog[index].gameObject) as GameObject;

                tempDialog.name = "Right_" + dialog[index].name;
                tempDialog.transform.SetParent(CanvasObjs[1].transform);

                tempDialog.transform.localPosition = Vector3.zero;
                tempDialog.transform.localEulerAngles = Vector3.zero;
                tempDialog.transform.localScale = Vector3.one;

                // 씬이 모두 로드 된 후에 UI가 생성되기 때문에
                tempDialog.GetComponent<IDialog>().Load();
            }
        }
    }
}
