using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_adc_data_frame_t
    {
        public UInt16 tag;
        public UInt16 size;
        public UInt32 version;
        public byte data_frame1;
        public byte data_frame2;
        public byte data_frame3;
        public byte data_frame4;
        public byte data_frame5;
        public byte data_frame6;
        public byte data_frame7;
        public byte data_frame8;
        public byte data_frame9;
        public byte data_frame10;
        public byte data_frame11;
        public byte data_frame12;
        public byte data_frame13;
        public byte data_frame14;
        public byte data_frame15;
        public byte data_frame16;
        public byte data_frame17;
        public byte data_frame18;
        public byte data_frame19;
        public byte data_frame20;
        public byte data_frame21;
        public byte data_frame22;
        public byte data_frame23;
        public byte data_frame24;
        public byte data_frame25;
        public byte data_frame26;
        public byte data_frame27;
        public byte data_frame28;
        public byte data_frame29;
        public byte data_frame30;
        public byte data_frame31;
        public byte data_frame32;
    }
}