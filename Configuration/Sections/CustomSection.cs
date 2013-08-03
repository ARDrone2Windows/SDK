using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    [StructLayout(LayoutKind.Sequential)]
    public class CustomSection
    {
        public readonly ReadWriteItem<string> ApplicationId = new ReadWriteItem<string>("custom:application_id");
        public readonly ReadWriteItem<string> ApplicationDescription = new ReadWriteItem<string>("custom:application_desc");
        public readonly ReadWriteItem<string> ProfileId = new ReadWriteItem<string>("custom:profile_id");
        public readonly ReadWriteItem<string> ProfileDescription = new ReadWriteItem<string>("custom:profile_desc");
        public readonly ReadWriteItem<string> SessionId = new ReadWriteItem<string>("custom:session_id");
        public readonly ReadWriteItem<string> SessionDescription = new ReadWriteItem<string>("custom:session_desc");
    }
}