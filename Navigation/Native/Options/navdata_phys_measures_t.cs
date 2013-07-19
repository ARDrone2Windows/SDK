using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_phys_measures_t
    {
        public UInt16 tag;
        public UInt16 size;
        public float accs_temp;
        public UInt16 gyro_temp;
        public float phys_accs1;
        public float phys_accs2;
        public float phys_accs3;
        public float phys_gyros1;
        public float phys_gyros2;
        public float phys_gyros3;
        public UInt32 alim3V3;
        public UInt32 vrefEpson;
        public UInt32 vrefIDG;
    }
}