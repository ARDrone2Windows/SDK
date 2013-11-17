using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.Drone2.Sample.W8.Model
{
    public class FlightData
    {
        public string Name { get; set; }

        public string Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Duration { get; set; }

        public string AltitudeMax { get; set; }

        public string AltitudeMin { get; set; }

        public List<FlightPictureData> Pictures { get; set; }

        public List<FlightVideoData> Videos { get; set; }

        public FlightData()
        {
            Videos = new List<FlightVideoData>();
            Pictures = new List<FlightPictureData>();
        }
    }
}
