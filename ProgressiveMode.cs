using System;
namespace ARDrone2Client.Common
{
    [Flags]
    public enum ProgressiveMode
    {
        Hovering = 0,
        Progressive = 1,
        CombinedYaw = 1 << 2 | Progressive,
        AbsoluteControl = 1 << 3 | Progressive
    }
}