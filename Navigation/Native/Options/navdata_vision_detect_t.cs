using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_vision_detect_t
    {
        internal UInt16 Tag;
        internal UInt16 Size;

        internal UInt32 NbDetected;

        internal UInt32 Tag1Type;
        internal UInt32 Tag2Type;
        internal UInt32 Tag3Type;
        internal UInt32 Tag4Type;

        internal UInt32 Tag1X;
        internal UInt32 Tag2X;
        internal UInt32 Tag3X;
        internal UInt32 Tag4X;

        internal UInt32 Tag1Y;
        internal UInt32 Tag2Y;
        internal UInt32 Tag3Y;
        internal UInt32 Tag4Y;

        internal UInt32 Tag1BoxWidth;
        internal UInt32 Tag2BoxWidth;
        internal UInt32 Tag3BoxWidth;
        internal UInt32 Tag4BoxWidth;

        internal UInt32 Tag1BoxHeight;
        internal UInt32 Tag2BoxHeight;
        internal UInt32 Tag3BoxHeight;
        internal UInt32 Tag4BoxHeight;

        internal UInt32 Tag1Distance;
        internal UInt32 Tag2Distance;
        internal UInt32 Tag3Distance;
        internal UInt32 Tag4Distance;
    }
}