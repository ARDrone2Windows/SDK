using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_gyros_offsets_t
    {
        public UInt16 tag;
        public UInt16 size;
        public float offset_g1;
        public float offset_g2;
        public float offset_g3;
    }
}