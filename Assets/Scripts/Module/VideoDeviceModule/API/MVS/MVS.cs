using System.Runtime.InteropServices;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public class MVS : IVideoDevice
    {
        private static class Dll
        {
            [DllImport("MVS")]
            public static extern bool Connect(uint width, uint height, float fps);
            [DllImport("MVS")]
            public static extern bool Disconnect();
            [DllImport("MVS")]
            public static extern bool Play();
            [DllImport("MVS")]
            public static extern bool Stop();
            [DllImport("MVS")]
            public static extern bool CopyBuffer(byte[] dst);
            [DllImport("MVS")]
            public static extern bool IsConnected();
            [DllImport("MVS")]
            public static extern bool IsPlaying();
            [DllImport("MVS")]
            public static extern bool GetWidth(out uint value);
            [DllImport("MVS")]
            public static extern bool GetHeight(out uint value);
            [DllImport("MVS")]
            public static extern bool GetExposureTime(out float value);
            [DllImport("MVS")]
            public static extern bool GetExposureAutoMode(out uint value);
            [DllImport("MVS")]
            public static extern bool SetExposureTime(float value);
            [DllImport("MVS")]
            public static extern bool SetExposureAutoMode(uint value);
        }

        public void Connect(Vector2Int resolution, double fps)
        {
            if (!Dll.Connect((uint)resolution.x, (uint)resolution.y, (float)fps))
                Debug.LogError($"[{nameof(MVS)}] 카메라 연결 실패");

            Dll.GetWidth(out uint width);
            Dll.GetHeight(out uint height);
            Resolution = new Vector2Int((int)width, (int)height);
        }

        public void Disconnect()
        {
            if (!Dll.Disconnect())
                Debug.LogError($"[{nameof(MVS)}] 카메라 연결 해제 실패");
        }

        public void Play()
        {
            if (!Dll.Play())
                Debug.LogError($"[{nameof(MVS)}] 카메라 재생 실패");
        }

        public void Stop()
        {
            if (!Dll.Stop())
                Debug.LogError($"[{nameof(MVS)}] 카메라 정지 실패");
        }

        public void CopyBuffer(byte[] dst)
        {
            if (!Dll.CopyBuffer(dst))
            {
                if (IsConnected)
                    Disconnect();
            }
            else
            {
                // 3 channels
                int len = Resolution.x * Resolution.y;
                for (int i = len - 1; i >= 0; i--)
                {
                    int baseIdx = i * 3;
                    dst[baseIdx + 2] = dst[i];
                    dst[baseIdx + 1] = dst[i];
                    dst[baseIdx] = dst[i];
                }
            }
        }

        public Vector2Int Resolution { get; private set; }
        public bool IsConnected => Dll.IsConnected();
        public bool IsPlaying => Dll.IsPlaying();

        public VideoCapability Capability => VideoCapability.Exposure | VideoCapability.AutoExposure;

        public double Exposure
        {
            get
            {
                Dll.GetExposureTime(out float value);
                return value;
            }

            set
            {
                Dll.SetExposureTime((float)value);
            }
        }
        
        public double AutoExposure
        {
            get
            {
                Dll.GetExposureAutoMode(out uint value);
                return value;
            }

            set
            {
                Dll.SetExposureAutoMode((uint)value);
            }
        }

        public double Focus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double AutoFocus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}