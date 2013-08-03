using System;
using System.Runtime.InteropServices;
using ARDrone2Client.Common.Helpers;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_magneto_t
    {
        public UInt16 tag;
        public UInt16 size;
        public UInt16 mx;
        public UInt16 my;
        public UInt16 mz;
        public vector31_t magneto_raw;
        public vector31_t magneto_rectified;
        public vector31_t magneto_offset;
        public float heading_unwrapped;
        public float heading_gyro_unwrapped;
        public float heading_fusion_unwrapped;
        public byte magneto_calibration_ok;
        public uint magneto_state;
        public float magneto_radius;
        public float error_mean;
        public float error_var;
    }
}