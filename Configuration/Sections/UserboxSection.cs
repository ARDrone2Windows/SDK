using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class UserboxSection : SectionBase
    {
        public UserboxSection(DroneConfiguration configuration)
            : base(configuration, "userbox")
        {
        }

        public UserboxCommand Command
        {
            get { return GetUserboxCommand("userbox_cmd"); }
            set { Set("userbox_cmd", value); }
        }
    }
}