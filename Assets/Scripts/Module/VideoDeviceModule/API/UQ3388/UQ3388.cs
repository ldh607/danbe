using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public class UQ3388 : IVideoDevice
    {
        private static class Dll
        {
            [DllImport("uq3388Wrapper")]
            public static extern UQResult Connect();
            [DllImport("uq3388Wrapper")]
            public static extern UQResult Disconnect();
            [DllImport("uq3388Wrapper")]
            public static extern bool IsConnected();
            [DllImport("uq3388Wrapper")]
            public static extern UQResult Play();
            [DllImport("uq3388Wrapper")]
            public static extern UQResult Stop();
            [DllImport("uq3388Wrapper")]
            public static extern bool IsPlaying();
            [DllImport("uq3388Wrapper")]
            public static extern UQResult CopyBuffer(byte[] dst);
            [DllImport("uq3388Wrapper")]
            public static extern UQResult GetVideoInputInfo(out VideoInputInfo info);
            [DllImport("uq3388Wrapper")]
            public static extern UQResult SetResolution(UInt16 width, UInt16 height);
        }

        private bool CheckUQResult(UQResult result)
        {
            if (result == UQResult.Failure_Unknown)
                Debug.LogError("[UQ3388] 알수없는 오류 발생");
            else if (result == UQResult.Failure_BoardNotFound)
                Debug.LogError("[UQ3388] 보드를 찾을 수 없습니다.");
            else if (result == UQResult.Failure_NotConnected)
                Debug.LogError("[UQ3388] 연결되지 않았습니다.");

            return result == UQResult.Success ? true : false;
        }

        public void Connect(Vector2Int resolution, double fps)
        {
            CheckUQResult(Dll.Connect());
            CheckUQResult(Dll.SetResolution((UInt16)resolution.x, (UInt16)resolution.y));
        }

        public void Disconnect()
        {
            CheckUQResult(Dll.Disconnect());
        }

        public void Play()
        {
            CheckUQResult(Dll.Play());
        }

        public void Stop()
        {
            CheckUQResult(Dll.Stop());
        }

        public void CopyBuffer(byte[] dst)
        {
            CheckUQResult(Dll.CopyBuffer(dst));
        }

        public Vector2Int Resolution
        {
            get
            {
                CheckUQResult(Dll.GetVideoInputInfo(out VideoInputInfo info));
                return new Vector2Int(info.BufferWidth, info.BufferHeight);
            }
        }

        public bool IsConnected => Dll.IsConnected();
        public bool IsPlaying => Dll.IsPlaying();

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