using System;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public class UnityCam : IVideoDevice
    {
        private string _deviceName;
        private bool _deviceFound;
        private bool _deviceConnected;
        private bool _wasPlaying;
        private WebCamTexture _webCamTexture;
        private Color32[] _pixels;

        public UnityCam(string deviceName = "")
        {
            _deviceName = deviceName;
        }

        private bool CheckDeviceFound()
        {
            if (!_deviceFound)
                Debug.LogError($"[{nameof(UnityCam)}] 카메라를 찾을 수 없습니다.");
            return _deviceFound;
        }

        private bool CheckDeviceConnected()
        {
            if (!_deviceConnected)
                Debug.LogError($"[{nameof(UnityCam)}] 카메라에 연결된 상태가 아닙니다.");
            return _deviceConnected;
        }

        private bool CheckPlaying(bool mustPlay)
        {
            if (mustPlay && (_webCamTexture == null || (_webCamTexture != null && !_webCamTexture.isPlaying)))
            {
                Debug.LogError($"[{nameof(UnityCam)}] 카메라가 재생 상태가 아닙니다.");
                return false;
            }
            else if (!mustPlay && _webCamTexture != null && _webCamTexture.isPlaying)
            {
                Debug.LogError($"[{nameof(UnityCam)}] 카메라 재생 상태에서는 불가능 합니다.");
                return false;
            }

            return true;
        }

        private void FindDevice(bool any)
        {
            _deviceFound = false;

            WebCamDevice[] devices = WebCamTexture.devices;
            foreach (var device in devices)
            {
                if (any)
                {
                    _deviceName = device.name;
                    _deviceFound = true;
                    break;
                }
                else if (_deviceName == device.name)
                {
                    _deviceFound = true;
                    break;
                }
            }
        }

        public void Connect(Vector2Int resolution, double fps)
        {
            if (IsConnected)
                return;

            FindDevice(string.IsNullOrEmpty(_deviceName));
            if (!CheckDeviceFound())
                return;

            if (_deviceFound)
            {
                _webCamTexture = new WebCamTexture(_deviceName, resolution.x, resolution.y, (int)fps);

                // 한번 재생하고 약간의 시간이 지난 후 해상도 값이 정상적으로 출력
                _webCamTexture.Play();
                if (_webCamTexture.isPlaying) // 웹캠이 연결되지않았는데도 장치를 찾는 경우가 있어서 재생으로 연결되있는지 확인
                {
                    _deviceConnected = true;
                    _wasPlaying = false;
                    _webCamTexture.Stop();
                }
                else
                {
                    _deviceConnected = false;
                }
            }
            else
            {
                _deviceConnected = false;
                _deviceName = null;
            }
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;

            Stop();

            _deviceFound = false;
            _deviceConnected = false;
            _webCamTexture = null;
        }

        public void Play()
        {
            if (CheckDeviceConnected())
            {
                _webCamTexture.Play();
                _wasPlaying = true;
            }
        }

        public void Stop()
        {
            //if (CheckDeviceConnected())
            _webCamTexture.Stop();
            _wasPlaying = false;
        }

        public void CopyBuffer(byte[] dst)
        {
            if (!CheckPlaying(true))
                return;
            
            if (_pixels == null) // Awake에서는 width, height 값이 정상적이지 않음
                _pixels = new Color32[_webCamTexture.width * _webCamTexture.height];
            _webCamTexture.GetPixels32(_pixels);
            
            int idx = 0;
            for (int i = 0; i < _pixels.Length; i++)
            {
                dst[idx++] = _pixels[i].r;
                dst[idx++] = _pixels[i].g;
                dst[idx++] = _pixels[i].b;
            }
        }

        public Vector2Int Resolution
        {
            get
            {
                if (!CheckDeviceConnected())
                    return new Vector2Int();

                Vector2Int res = new Vector2Int(_webCamTexture.width, _webCamTexture.height);
                return res;
            }

            set
            {
                if (!CheckDeviceConnected() || !CheckPlaying(false))
                    return;
                
                _webCamTexture.requestedWidth = value.x;
                _webCamTexture.requestedHeight = value.y;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _deviceConnected && (_wasPlaying == _webCamTexture.isPlaying);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _deviceConnected && _webCamTexture.isPlaying;
            }
        }
        
        public string DeviceName => _deviceName;

        public VideoCapability Capability => VideoCapability.None;

        public double Exposure
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double AutoExposure
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double Focus
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double AutoFocus
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}