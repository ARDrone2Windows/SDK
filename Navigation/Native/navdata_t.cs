using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_t
    {
        public UInt32 header;
        public UInt32 ardrone_state;
        public UInt32 sequence;
        public UInt32 vision_defined;
    }
}