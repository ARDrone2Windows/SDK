using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Helpers
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct matrix33_t
    {
        public float m11;
        public float m12;
        public float m13;
        public float m21;
        public float m22;
        public float m23;
        public float m31;
        public float m32;
        public float m33;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct vector21_t
    {
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct vector31_t
    {
        public float x;
        public float y;
        public float z;
    }
}
