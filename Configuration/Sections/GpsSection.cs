using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class GpsSection : SectionBase
    {
        public GpsSection(DroneConfiguration configuration)
            : base(configuration, "gps")
        {
        }

        public Double Latitude
        {
            get { return GetDouble("latitude"); }
        }

        public Double Longitude
        {
            get { return GetDouble("longitude"); }
        }

        public Double Altitude
        {
            get { return GetDouble("altitude"); }
        }
    }
}