using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    [StructLayout(LayoutKind.Sequential)]
    public class UserboxSection
    {
        public readonly ReadWriteItem<string> UserboxCmd = new ReadWriteItem<string>("userbox:userbox_cmd");
    }
}