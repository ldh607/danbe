using System;
using System.Runtime.InteropServices;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.VideoDevice
{
    public class OpenCVCam : IVideoDevice
    {
        private VideoCapture _videoCapture;
        private Mat _frameMat;
        private bool _isPlaying;

        private bool CheckDeviceConnected()
        {
            if (!IsConnected)
                Debug.LogError($"[{nameof(OpenCVCam)}] 카메라에 연결된 상태가 아닙니다.");
            return IsConnected;
        }

        private bool CheckDevicePlaying()
        {
            if (!IsPlaying)
                Debug.LogError($"[{nameof(OpenCVCam)}] 카메라가 재생 상태가 아닙니다.");
            return IsPlaying;
        }
        
        public void Connect(Vector2Int resolution, double fps)
        {
            _videoCapture = new VideoCapture(0);
            if (!IsConnected)
            {
                Debug.LogError($"[{nameof(OpenCVCam)}] 카메라 연결에 실패했습니다.");
                return;
            }

            Resolution = resolution;
            _videoCapture.set(Videoio.CAP_PROP_FPS, fps);
        }

        public void Disconnect()
        {
            Stop();

            if (_videoCapture != null)
            {
                _videoCapture.release();
                _videoCapture.Dispose();
                _videoCapture = null;
            }

            if (_frameMat != null)
            {
                _frameMat.release();
                _frameMat.Dispose();
            }
        }

        public void Play()
        {
            if (!CheckDeviceConnected())
                return;
            
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void CopyBuffer(byte[] dst)
        {
            if (!CheckDevicePlaying())
                return;

            if (_videoCapture.read(_frameMat))
            {
                Imgproc.cvtColor(_frameMat, _frameMat, Imgproc.COLOR_BGR2RGB);
                Core.flip(_frameMat, _frameMat, 0);
                Marshal.Copy((IntPtr)_frameMat.dataAddr(), dst, 0, dst.Length);

                IsPlaying = true;
            }
            else if (IsConnected)
            {
                Disconnect();
            }
        }

        public bool IsConnected => _videoCapture != null && _videoCapture.isOpened();
        public bool IsPlaying { get => IsConnected && _isPlaying; private set => _isPlaying = value; }

        public Vector2Int Resolution
        {
            get
            {
                if (!CheckDeviceConnected())
                    return new Vector2Int();

                return new Vector2Int((int)_videoCapture.get(Videoio.CAP_PROP_FRAME_WIDTH), (int)_videoCapture.get(Videoio.CAP_PROP_FRAME_HEIGHT));
            }

            private set
            {
                if (!CheckDeviceConnected())
                    return;

                _videoCapture.set(Videoio.CAP_PROP_FRAME_WIDTH, value.x);
                _videoCapture.set(Videoio.CAP_PROP_FRAME_HEIGHT, value.y);
                _frameMat = new Mat(Resolution.y, Resolution.x, CvType.CV_8UC3);
            }
        }

        public VideoCapability Capability => 
            VideoCapability.Exposure        | 
            VideoCapability.AutoExposure    | 
            VideoCapability.Focus           | 
            VideoCapability.AutoFocus;

        public double Exposure
        {
            get => _videoCapture.get(Videoio.CAP_PROP_EXPOSURE);
            set => _videoCapture.set(Videoio.CAP_PROP_EXPOSURE, value);
        }

        public double AutoExposure
        {
            get => _videoCapture.get(Videoio.CAP_PROP_AUTO_EXPOSURE);
            set => _videoCapture.set(Videoio.CAP_PROP_AUTO_EXPOSURE, value);
        }

        public double Focus
        {
            get => _videoCapture.get(Videoio.CAP_PROP_FOCUS);
            set => _videoCapture.set(Videoio.CAP_PROP_FOCUS, value);
        }

        public double AutoFocus
        {
            get => _videoCapture.get(Videoio.CAP_PROP_AUTOFOCUS);
            set => _videoCapture.set(Videoio.CAP_PROP_AUTOFOCUS, value);
        }
    }
}
