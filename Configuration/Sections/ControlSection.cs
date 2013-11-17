using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class ControlSection : SectionBase
    {
        public ControlSection(DroneConfiguration configuration)
            : base(configuration, "control")
        {
        }

        public String AccsOffset
        {
            get { return GetString("accs_offset"); }
        }

        public String AccsGains
        {
            get { return GetString("accs_gains"); }
        }

        public String GyrosOffset
        {
            get { return GetString("gyros_offset"); }
        }

        public String GyrosGains
        {
            get { return GetString("gyros_gains"); }
        }

        public String Gyros110Offset
        {
            get { return GetString("gyros110_offset"); }
        }

        public String Gyros110Gains
        {
            get { return GetString("gyros110_gains"); }
        }

        public String MagnetoOffset
        {
            get { return GetString("magneto_offset"); }
        }

        public Single MagnetoRadius
        {
            get { return GetSingle("magneto_radius"); }
        }

        public Single GyroOffsetThrX
        {
            get { return GetSingle("gyro_offset_thr_x"); }
        }

        public Single GyroOffsetThrY
        {
            get { return GetSingle("gyro_offset_thr_y"); }
        }

        public Single GyroOffsetThrZ
        {
            get { return GetSingle("gyro_offset_thr_z"); }
        }

        public Int32 PwmRefGyros
        {
            get { return GetInt32("pwm_ref_gyros"); }
        }

        public Int32 OsctunValue
        {
            get { return GetInt32("osctun_value"); }
        }

        public Boolean OsctunTest
        {
            get { return GetBoolean("osctun_test"); }
        }

        public Int32 ControlLevel
        {
            get { return GetInt32("control_level"); }
            set { Set("control_level", value); }
        }

        public Double EulerAngleMax
        {
            get { return GetDouble("euler_angle_max"); }
            set { Set("euler_angle_max", value); }
        }

        public Int32 AltitudeMax
        {
            get { return GetInt32("altitude_max"); }
            set { Set("altitude_max", value); }
        }

        public Int32 AltitudeMin
        {
            get { return GetInt32("altitude_min"); }
            set { Set("altitude_min", value); }
        }

        public Single ControliPhoneTilt
        {
            get { return GetSingle("control_iphone_tilt"); }
            set { Set("control_iphone_tilt", value); }
        }

        public Double ControlVzMax
        {
            get { return GetDouble("control_vz_max"); }
            set { Set("control_vz_max", value); }
        }

        public Double ControlYaw
        {
            get { return GetDouble("control_yaw"); }
            set { Set("control_yaw", value); }
        }

        public Boolean Outdoor
        {
            get { return GetBoolean("outdoor"); }
            set { Set("outdoor", value); }
        }

        public Boolean FlightWithoutShell
        {
            get { return GetBoolean("flight_without_shell"); }
            set { Set("flight_without_shell", value); }
        }

        public Boolean AutonomousFlight
        {
            get { return GetBoolean("autonomous_flight"); }
        }

        public Boolean ManualTrim
        {
            get { return GetBoolean("manual_trim"); }
            set { Set("manual_trim", value); }
        }

        public Single IndoorEulerAngleMax
        {
            get { return GetSingle("indoor_euler_angle_max"); }
            set { Set("indoor_euler_angle_max", value); }
        }

        public Single IndoorControlVzMax
        {
            get { return GetSingle("indoor_control_vz_max"); }
            set { Set("indoor_control_vz_max", value); }
        }

        public Single IndoorControlYaw
        {
            get { return GetSingle("indoor_control_yaw"); }
            set { Set("indoor_control_yaw", value); }
        }

        public Single OutdoorEulerAngleMax
        {
            get { return GetSingle("outdoor_euler_angle_max"); }
            set { Set("outdoor_euler_angle_max", value); }
        }

        public Single OutdoorControlVzMax
        {
            get { return GetSingle("outdoor_control_vz_max"); }
            set { Set("outdoor_control_vz_max", value); }
        }

        public Single OutdoorControlYaw
        {
            get { return GetSingle("outdoor_control_yaw"); }
            set { Set("outdoor_control_yaw", value); }
        }

        public Int32 FlyingMode
        {
            get { return GetInt32("flying_mode"); }
            set { Set("flying_mode", value); }
        }

        public Int32 HoveringRange
        {
            get { return GetInt32("hovering_range"); }
            set { Set("hovering_range", value); }
        }

        public FlightAnimation FlightAnimation
        {
            get { return GetFlightAnimation("flight_anim"); }
            set { Set("flight_anim", value); }
        }
    }
}