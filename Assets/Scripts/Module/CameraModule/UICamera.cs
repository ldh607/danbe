using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CellBig.Contents;

namespace CellBig.Module
{
    public class UICamera : MonoBehaviour
    {
        public Camera[] _Camera = null;
        public Canvas[] _Canvas = null;
        public CanvasScaler[] _CanvasScaler = null;

        public Camera _KioskCamera = null;
        public Canvas _KioskCanvas = null;
        public CanvasScaler _KioskCanvasScaler = null;

        public void Setup(int targetDisplay)
        {
            for (int i = 0; i < _Camera.Length; i++)
                _Camera[i].targetDisplay = targetDisplay;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Home))
            {
                int target = _KioskCanvas.targetDisplay;
                Debug.Log(target);

                target = target >= 7 ? 0 : target + 1;
                _KioskCanvas.targetDisplay = target;
            }
        }
    }

 
}