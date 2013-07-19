using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct navdata_raw_measures_t
    {
        public UInt16 tag;
        public UInt16 size;
        public UInt16 raw_accs_1;
        public UInt16 raw_accs_2;
        public UInt16 raw_accs_3;
        public UInt16 raw_gyros_1;
        public UInt16 raw_gyros_2;
        public UInt16 raw_gyros_3;
        public UInt16 raw_gyros_110_1;
        public UInt16 raw_gyros_110_2;
        public UInt32 vbat_raw;
        public UInt16 us_debut_echo;
        public UInt16 us_fin_echo;
        public UInt16 us_association_echo;
        public UInt16 us_distance_echo;
        public UInt16 us_courbe_temps;
        public UInt16 us_courbe_valeur;
        public UInt16 us_courbe_ref;
        public UInt16 flag_echo_ini;
        public UInt16 nb_echo;
        public UInt32 sum_echo;
        public UInt32 alt_temp_raw;
        public UInt16 gradient;
    }
}