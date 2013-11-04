using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class SyslogSection : SectionBase
    {
        public SyslogSection(DroneConfiguration configuration)
            : base(configuration, "syslog")
        {
        }

        public Int32 Output
        {
            get { return GetInt32("output"); }
            set { Set("output", value); }
        }

        public Int32 MaxSize
        {
            get { return GetInt32("max_size"); }
            set { Set("max_size", value); }
        }

        public Int32 NbFiles
        {
            get { return GetInt32("nb_files"); }
            set { Set("nb_files", value); }
        }
    }
}