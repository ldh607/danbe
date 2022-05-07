using System;
using UnityEngine;

namespace CellBig.Module.VideoDevice
{
    public class SetActiveKinectRGB : Message
    {
        public SetActiveKinectRGB(bool active)
        {
            Active = active;
        }

        public bool Active { get; }
    }

    public class SaveSettings : Message
    {

    }

    public class VideoDeviceConnectRequest : Message
    {
        public VideoDeviceConnectRequest(object connector, Action<Vector2Int, bool> onConnected)
        {
            Connector = connector;
            OnConnected = onConnected;
        }
        
        public object Connector { get; }
        public Action<Vector2Int, bool> OnConnected { get; }
    }

    public class VideoDeviceDisconnectRequest : Message
    {
        public VideoDeviceDisconnectRequest(object connector)
        {
            Connector = connector;
        }

        public object Connector { get; }
    }
    
    public class VideoDevicePlayRequest : Message
    {
        public VideoDevicePlayRequest(object connector, byte[] buffer)
        {
            Connector = connector;
            Buffer = buffer;
        }

        public object Connector { get; }
        public byte[] Buffer { get; }
    }

    public class VideoDeviceStopRequest : Message
    {
        public VideoDeviceStopRequest(object connector)
        {
            Connector = connector;
        }

        public object Connector { get; }
    }

    public class UpdateVideoProps : Message { }
    
    public class VideoDeviceEvent : Message
    {
        public enum EventType
        {
            Unplugged,
            Reconnected,
            FailedToConnect
        }
        
        public VideoDeviceEvent(EventType type, object data, string message)
        {
            Type = type;
            Data = data;
            Message = message;
        }

        public EventType Type { get; }
        public object Data { get; }
        public string Message { get; }
    }
}