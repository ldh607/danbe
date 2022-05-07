using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.UI;

namespace CellBig.Module
{
    public class ProjectionCameraModule : IModule
    {
        public int Projetion_UICameraCount = 1;
        Dictionary<string, UICamera> _UICamera = new Dictionary<string, UICamera>();

        public bool useKiosk;

        protected override void OnLoadStart()
        {
            LoadComplete();
        }

        void LoadComplete()
        {
            var projection_UICamera = gameObject.GetComponentInChildren<UICamera>();

            for (int i = 0; i < Projetion_UICameraCount; i++)
            {
                projection_UICamera.Setup(i);
                CanvasObjs.Add(projection_UICamera._Canvas[i].gameObject);
            }

            if(useKiosk)
            {
                CanvasObjs.Add(projection_UICamera._KioskCanvas.gameObject);
            }

            UIManager.Instance.SetCanvasObj(CanvasObjs, true);

            SetResourceLoadComplete();
        }

        public override void OnSceneLoadComplete()
        {
     
        }
    }
}
