using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

using ARDrone2Client.Common.Navigation.Native;

namespace ARDrone2Client.Common.Navigation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NavigationData
    {
        // Used to acknowledge Config modification & Alerts
        internal def_ardrone_state_mask_t Masks;

        public NavigationState State;
        public float Yaw; // radians - Yaw - Z
        public float Pitch; // radians - Pitch - Y
        public float Roll; // radians - Roll - X
        public float Altitude; // meters
        public Vector3 Velocity; // meter/second
        public Battery Battery;
        private Battery _Battery;
        public float Time; // seconds
        public Wifi Wifi;
        public Video Video;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Battery
    {
        public bool Low;
        public float Percentage;
        public float Voltage; // in volts

        public override string ToString()
        {
            return string.Format("{{Low:{0} Percentage:{1} Voltage:{2}}}", Low, Percentage, Voltage);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float X; // meter/second
        public float Y; // meter/second
        public float Z; // meter/second

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1} Z:{2}}}", X, Y, Z);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Wifi
    {
        public float LinkQuality; // 1 is perfect, less than 1 is worse

        public override string ToString()
        {
            return string.Format("{{LinkQuality:{0}}}", LinkQuality);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Video
    {
        public uint FrameNumber;

        public override string ToString()
        {
            return string.Format("{{FrameNumber:{0}}}", FrameNumber);
        }
    }
}