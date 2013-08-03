using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARDrone2Client.Common.Navigation;
using Windows.UI.Core;

namespace ARDrone2Client.Common.ViewModel
{
    public class NavigationDataViewModel : BaseViewModel
    {
        #region Altitude
        private string altitude;
        public string Altitude
        {
            get
            {
                return this.altitude;
            }

            set
            {
                if (value != this.altitude)
                {
                    this.altitude = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region BatteryIsLow
        private string batteryIsLow;
        public string BatteryIsLow
        {
            get
            {
                return this.batteryIsLow;
            }

            set
            {
                if (value != this.batteryIsLow)
                {
                    this.batteryIsLow = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region BatteryPercentage
        private string batteryPercentage;
        public string BatteryPercentage
        {
            get
            {
                return this.batteryPercentage;
            }

            set
            {
                if (value != this.batteryPercentage)
                {
                    this.batteryPercentage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region BatteryVoltage
        private string batteryVoltage;
        public string BatteryVoltage
        {
            get
            {
                return this.batteryVoltage;
            }

            set
            {
                if (value != this.batteryVoltage)
                {
                    this.batteryVoltage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Pitch
        private string pitch;
        public string Pitch
        {
            get
            {
                return this.pitch;
            }

            set
            {
                if (value != this.pitch)
                {
                    this.pitch = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Roll
        private string roll;
        public string Roll
        {
            get
            {
                return this.roll;
            }

            set
            {
                if (value != this.roll)
                {
                    this.roll = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region State
        private string state;
        public string State
        {
            get
            {
                return this.state;
            }

            set
            {
                if (value != this.state)
                {
                    this.state = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Time
        private string time;
        public string Time
        {
            get
            {
                return this.time;
            }

            set
            {
                if (value != this.time)
                {
                    this.time = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region VelocityX
        private string velocityX;
        public string VelocityX
        {
            get
            {
                return this.velocityX;
            }

            set
            {
                if (value != this.velocityX)
                {
                    this.velocityX = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region VelocityY
        private string velocityY;
        public string VelocityY
        {
            get
            {
                return this.velocityY;
            }

            set
            {
                if (value != this.velocityY)
                {
                    this.velocityY = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region VelocityZ
        private string velocityZ;
        public string VelocityZ
        {
            get
            {
                return this.velocityZ;
            }

            set
            {
                if (value != this.velocityZ)
                {
                    this.velocityZ = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region VideoFrameNumber
        private string videoFrameNumber;
        public string VideoFrameNumber
        {
            get
            {
                return this.videoFrameNumber;
            }

            set
            {
                if (value != this.videoFrameNumber)
                {
                    this.videoFrameNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region WifiLinkQuality
        private string wifiLinkQuality;
        public string WifiLinkQuality
        {
            get
            {
                return this.wifiLinkQuality;
            }

            set
            {
                if (value != this.wifiLinkQuality)
                {
                    this.wifiLinkQuality = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Yaw
        private string yaw;
        public string Yaw
        {
            get
            {
                return this.yaw;
            }

            set
            {
                if (value != this.yaw)
                {
                    this.yaw = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public NavigationDataViewModel()
        {
        }

        public void Update(NavigationData navigationData)
        {
            this.Altitude = navigationData.Altitude.ToString();
            this.BatteryIsLow = navigationData.Battery.Low.ToString();
            this.BatteryPercentage = navigationData.Battery.Percentage.ToString();
            this.BatteryVoltage = navigationData.Battery.Voltage.ToString();
            this.Pitch = Math.Round(navigationData.Pitch, 3).ToString();
            this.Roll = Math.Round(navigationData.Roll, 3).ToString();
            this.State = navigationData.State.ToString();
            this.Time = navigationData.Time.ToString();
            this.VelocityX =  navigationData.Velocity.X.ToString();
            this.VelocityY =  navigationData.Velocity.Y.ToString();
            this.VelocityZ = navigationData.Velocity.Z.ToString();
            this.videoFrameNumber = navigationData.Video.FrameNumber.ToString();
            this.WifiLinkQuality = navigationData.Wifi.LinkQuality.ToString();
            this.Yaw = Math.Round(navigationData.Yaw, 3).ToString();
        }
    }
}