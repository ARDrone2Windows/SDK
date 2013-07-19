using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_vision_of_t
    {
        public ushort tag;
        public ushort size;

        public float tag1_dx;
        public float tag2_dx;
        public float tag3_dx;
        public float tag4_dx;
        public float tag5_dx;

        public float tag1_dy;
        public float tag2_dy;
        public float tag3_dy;
        public float tag4_dy;
        public float tag5_dy;
    }
}