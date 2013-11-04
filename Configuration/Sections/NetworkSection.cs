using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class NetworkSection : SectionBase
    {
        public NetworkSection(DroneConfiguration configuration)
            : base(configuration, "network")
        {
        }

        public String SsidSinglePlayer
        {
            get { return GetString("ssid_single_player"); }
            set { Set("ssid_single_player", value); }
        }

        public String SsidMultiPlayer
        {
            get { return GetString("ssid_multi_player"); }
            set { Set("ssid_multi_player", value); }
        }

        public Int32 WifiMode
        {
            get { return GetInt32("wifi_mode"); }
            set { Set("wifi_mode", value); }
        }

        public Int32 WifiRate
        {
            get { return GetInt32("wifi_rate"); }
            set { Set("wifi_rate", value); }
        }

        public String OwnerMac
        {
            get { return GetString("owner_mac"); }
            set { Set("owner_mac", value); }
        }
    }
}