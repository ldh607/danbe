using System;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    [Serializable]
    public class VideoDeviceSettings
    {
        public VideoDeviceAPI DeviceAPI;
        public string DeviceName;
        public float ForcedReconnectionInterval;
        public float ReconnectionDelay;
        public Vector2Int RequestedResolution;
        public double RequestedFPS;

        [Space]
        
        public bool FlipX;
        public bool FlipY;
        public double Contrast;
        public double Exposure;
        public double AutoExposure;
        public double Focus;
        public double AutoFocus;
    }
}