using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Navigation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NavigationPacket
    {
        public long Timestamp;
        public byte[] Data;
    }
}