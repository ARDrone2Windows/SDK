using System;
namespace ARDrone2Client.Common
{
    [Flags]
    public enum ProgressiveMode
    {
        Hovering = 0,
        Progressive = 1 << 0,
        CombinedYaw = 1 << 1 | Progressive,
        AbsoluteControl = 1 << 2 | Progressive
    }
}
