using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class GeneralSection : SectionBase
    {
        public GeneralSection(DroneConfiguration configuration)
            : base(configuration, "general")
        {
        }

        public Int32 ConfigurationVersion
        {
            get { return GetInt32("num_version_config"); }
        }

        public Int32 HardwareVersion
        {
            get { return GetInt32("num_version_mb"); }
        }

        public String FirmwareVersion
        {
            get { return GetString("num_version_soft"); }
        }

        public String DroneSerial
        {
            get { return GetString("drone_serial"); }
        }

        public String FirmwareBuildDate
        {
            get { return GetString("soft_build_date"); }
        }

        public String Motor1Soft
        {
            get { return GetString("motor1_soft"); }
        }

        public String Motor1Hard
        {
            get { return GetString("motor1_hard"); }
        }

        public String Motor1Supplier
        {
            get { return GetString("motor1_supplier"); }
        }

        public String Motor2Soft
        {
            get { return GetString("motor2_soft"); }
        }

        public String Motor2Hard
        {
            get { return GetString("motor2_hard"); }
        }

        public String Motor2Supplier
        {
            get { return GetString("motor2_supplier"); }
        }

        public String Motor3Soft
        {
            get { return GetString("motor3_soft"); }
        }

        public String Motor3Hard
        {
            get { return GetString("motor3_hard"); }
        }

        public String Motor3Supplier
        {
            get { return GetString("motor3_supplier"); }
        }

        public String Motor4Soft
        {
            get { return GetString("motor4_soft"); }
        }

        public String Motor4Hard
        {
            get { return GetString("motor4_hard"); }
        }

        public String Motor4Supplier
        {
            get { return GetString("motor4_supplier"); }
        }

        public String ARDroneName
        {
            get { return GetString("ardrone_name"); }
            set { Set("ardrone_name", value); }
        }

        public Int32 FlyingTime
        {
            get { return GetInt32("flying_time"); }
        }

        public Boolean NavdataDemo
        {
            get { return GetBoolean("navdata_demo"); }
            set { Set("navdata_demo", value); }
        }

        public NavdataOptions NavdataOptions
        {
            get { return GetEnum<NavdataOptions>("navdata_options"); }
            set { SetEnum<NavdataOptions>("navdata_options", value); }
        }

        public Int32 ComWatchdog
        {
            get { return GetInt32("com_watchdog"); }
            set { Set("com_watchdog", value); }
        }

        public Boolean Video
        {
            get { return GetBoolean("video_enable"); }
            set { Set("video_enable", value); }
        }

        public Boolean Vision
        {
            get { return GetBoolean("vision_enable"); }
            set { Set("vision_enable", value); }
        }

        public Int32 BatteryVoltageMin
        {
            get { return GetInt32("vbat_min"); }
            set { Set("vbat_min", value); }
        }

        public Int32 LocalTime
        {
            get { return GetInt32("localtime"); }
            set { Set("localtime", value); }
        }
    }
}