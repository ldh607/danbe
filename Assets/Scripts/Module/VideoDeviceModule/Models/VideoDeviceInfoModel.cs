using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public class VideoDeviceInfoModel : Model
    {
        private readonly IVideoDevice _device;
        private readonly ISet<object> _connectorSet;

        public VideoDeviceInfoModel(VideoDeviceSettings settings, IVideoDevice device)
        {
            Settings = settings;
            _device = device;
        }
        
        public VideoDeviceSettings Settings { get; }
        public Vector2Int Resolution => _device.Resolution;
        public bool IsConnected => _device.IsConnected;
        public bool IsPlaying => _device.IsPlaying;
    }
}