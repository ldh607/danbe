using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public class oCam : IVideoDevice
    {
        private static class Dll
        {
            [DllImport("oCam")]
            public static extern bool Connect(int width, int height, double fps);
            [DllImport("oCam")]
            public static extern bool Disconnect();
            [DllImport("oCam")]
            public static extern bool Play();
            [DllImport("oCam")]
            public static extern bool Stop();
            [DllImport("oCam")]
            public static extern bool CopyBuffer(byte[] dst);
            [DllImport("oCam")]
            public static extern bool IsConnected();
            [DllImport("oCam")]
            public static extern bool IsPlaying();
            [DllImport("oCam")]
            public static extern bool GetExposure(out long value);
            [DllImport("oCam")]
            public static extern bool SetExposure(long value);
        }

        public void Connect(Vector2Int resolution, double fps)
        {
            if (!Dll.Connect(resolution.x, resolution.y, fps))
                Debug.LogError($"[{nameof(oCam)}] 카메라 연결 실패");

            Resolution = resolution;
        }

        public void Disconnect()
        {
            if (!Dll.Disconnect())
                Debug.LogError($"[{nameof(oCam)}] 카메라 연결 해제 실패");
        }

        public void Play()
        {
            if (!Dll.Play())
                Debug.LogError($"[{nameof(oCam)}] 카메라 재생 실패");
        }

        public void Stop()
        {
            if (!Dll.Stop())
                Debug.LogError($"[{nameof(oCam)}] 카메라 정지 실패");
        }

        public void CopyBuffer(byte[] dst)
        {
            if (!Dll.CopyBuffer(dst))
            {
                if (IsConnected)
                    Disconnect();
            }
        }

        public bool IsConnected => Dll.IsConnected();
        public bool IsPlaying => Dll.IsPlaying();
        public Vector2Int Resolution { get; private set; }

        public VideoCapability Capability => VideoCapability.Exposure;

        public double Exposure
        {
            get
            {
                Dll.GetExposure(out long value);
                return value;
            }

            set
            {
                Dll.SetExposure((long)value);
            }
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