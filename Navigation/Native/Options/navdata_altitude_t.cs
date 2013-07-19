using System.Runtime.InteropServices;
using ARDrone2Client.Common.Helpers;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_altitude_t
    {
        public ushort tag;
        public ushort size;
        public int altitude_vision;
        public float altitude_vz;
        public int altitude_ref;
        public int altitude_raw;
        public float obs_accZ;
        public float obs_alt;
        public vector31_t obs_x;
        public uint obs_state;
        public vector21_t est_vb;
        public uint est_state;
    }
}