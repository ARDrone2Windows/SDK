using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class PicSection : SectionBase
    {
        public PicSection(DroneConfiguration configuration)
            : base(configuration, "pic")
        {
        }

        public Int32 UltrasoundFreq
        {
            get { return GetInt32("ultrasound_freq"); }
            set { Set("ultrasound_freq", value); }
        }

        public Int32 UltrasoundWatchdog
        {
            get { return GetInt32("ultrasound_watchdog"); }
            set { Set("ultrasound_watchdog", value); }
        }

        public Int32 Version
        {
            get { return GetInt32("pic_version"); }
        }
    }
}