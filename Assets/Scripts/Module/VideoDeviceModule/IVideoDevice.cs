using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public interface IVideoDevice
    {
        void Connect(Vector2Int resolution, double fps);
        void Disconnect();
        bool IsConnected { get; }
        void Play();
        void Stop();
        bool IsPlaying { get; }
        void CopyBuffer(byte[] dst);
        Vector2Int Resolution { get; }

        VideoCapability Capability { get; }
        double Exposure { get; set; }
        double AutoExposure { get; set; }
        double Focus { get; set; }
        double AutoFocus { get; set; }
    }
}