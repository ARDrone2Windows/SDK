using ARDrone2Client.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common
{
    public static class Command
    {
        public static readonly string SequenceArg = "[SEQUENCE]";
        public static string Calibration()
        {
            return "AT*CALIB=[SEQUENCE],0\r";
        }
        public static string Config(string key, bool value)
        {
            return Config(key, value.ToString().ToUpper());
        }

        public static string Config(string key, int value)
        {
            return Config(key, value.ToString("D"));
        }
        public static string Config(string key, float value)
        {
            return Config(key, ConversionHelper.ToInt(value));
        }
        public static string Config(string key, Enum value)
        {
            return Config(key, value.ToString("D"));
        }
        public static string Config(string key, string value) 
        {
            return string.Format("AT*CONFIG=[SEQUENCE],\"{0}\",\"{1}\"\r", key, value);
        }
        public static string ConfigIds(string applicationId, string userId, string sessionId)
        {
            return string.Format("AT*CONFIG_IDS=[SEQUENCE],\"{0}\",\"{1}\",\"{2}\"\r", applicationId, userId, sessionId);
        }
        public static string Control(ControlMode controlMode)
        {
            return string.Format("AT*CTRL=[SEQUENCE],{0},0\r", (int)controlMode);
        }
        public static string ControlInit()
        {
            return "AT*CTRL=[SEQUENCE],0\r";
        }
        public static string FlatTrim()
        {
            return "AT*FTRIM=[SEQUENCE]\r";
        }
        public static string Progressive(ProgressiveMode mode, float roll, float pitch, float yaw, float gaz)
        {
            return string.Format("AT*PCMD=[SEQUENCE],{0},{1},{2},{3},{4}\r",
                (int)mode,
                ConversionHelper.ToInt(roll),
                ConversionHelper.ToInt(pitch),
                ConversionHelper.ToInt(gaz),
                ConversionHelper.ToInt(yaw));
        }
        public static string Absolute(ProgressiveMode mode, float roll, float pitch, float yaw, float gaz, float mag, float mag_acu)
        {
            return string.Format("AT*PCMD_MAG=[SEQUENCE],{0},{1},{2},{3},{4},{5},{6}\r",
                (int)mode,
                ConversionHelper.ToInt(roll),
                ConversionHelper.ToInt(pitch),
                ConversionHelper.ToInt(gaz),
                ConversionHelper.ToInt(yaw),
                ConversionHelper.ToInt(mag),
                ConversionHelper.ToInt(mag_acu));
        }
        public static string Ref(RefMode refMode)
        {
            return string.Format("AT*REF=[SEQUENCE],{0}\r", (int)refMode);
        }
        public static string Watchdog()
        {
            return "AT*COMWDG=[SEQUENCE]\r";
        }
    }
}
