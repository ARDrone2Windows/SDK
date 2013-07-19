using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    [StructLayout(LayoutKind.Sequential)]
    public class PicSection
    {
        public readonly ReadWriteItem<int> UltrasoundFreq = new ReadWriteItem<int>("pic:ultrasound_freq");
        public readonly ReadWriteItem<int> UltrasoundWatchdog = new ReadWriteItem<int>("pic:ultrasound_watchdog");
        public readonly ReadOnlyItem<int> Version = new ReadOnlyItem<int>("pic:pic_version");
    }
}