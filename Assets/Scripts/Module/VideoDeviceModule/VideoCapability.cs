using System;

namespace CellBig.Module.VideoDevice
{
    [Flags]
    public enum VideoCapability
    {
        None            = 0x00,
        Exposure        = 0x01,
        AutoExposure    = 0x02,
        Focus           = 0x04,
        AutoFocus       = 0x08,
    }
}