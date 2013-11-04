using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class LedsSection : SectionBase
    {
        public LedsSection(DroneConfiguration configuration)
            : base(configuration, "leds")
        {
        }

        public String Animation
        {
            get { return GetString("leds_anim"); }
            set { Set("leds_anim", value); }
        }
    }
}