using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class CustomSection : SectionBase
    {
        public CustomSection(DroneConfiguration configuration)
            : base(configuration, "custom")
        {
        }

        public String ApplicationId
        {
            get { return GetString("application_id"); }
            set { Set("application_id", value); }
        }

        public String ApplicationDescription
        {
            get { return GetString("application_desc"); }
            set { Set("application_desc", value); }
        }

        public String ProfileId
        {
            get { return GetString("profile_id"); }
            set { Set("profile_id", value); }
        }

        public String ProfileDescription
        {
            get { return GetString("profile_desc"); }
            set { Set("profile_desc", value); }
        }

        public String SessionId
        {
            get { return GetString("session_id"); }
            set { Set("session_id", value); }
        }

        public String SessionDescription
        {
            get { return GetString("session_desc"); }
            set { Set("session_desc", value); }
        }
    }
}