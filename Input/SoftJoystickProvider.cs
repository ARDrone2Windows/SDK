using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

using ARDrone2Client.Common;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.Input;

namespace ARDrone2Client.Common.Input
{
    public class SoftJoystickProvider : IInputProvider
    {
        private DroneClient _DroneClient = null;
        private ThreadPoolTimer _Timer;
        public SoftJoystickProvider(DroneClient droneClient, IJoystickControl rollPitchThumb, IJoystickControl yawGazThumb)
        {
            if (droneClient == null)
                throw new ArgumentNullException("DroneClient");
            _DroneClient = droneClient;
            _RollPitchThumb = rollPitchThumb;
            _YawGazThumb = yawGazThumb;
            _Timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(timerElapsedHandler), TimeSpan.FromMilliseconds(1000 / 12));
        }
        public DroneClient DroneClient
        {
            get
            {
                return _DroneClient;
            }
        }
        public string Name
        {
            get
            {
                return "Touch joystick";
            }
        }
        private IJoystickControl _RollPitchThumb;
        public IJoystickControl RollPitchThumb
        {
            get
            {
                return _RollPitchThumb;
            }
            set
            {
                if (value == _RollPitchThumb)
                    return;
                _RollPitchThumb = value;
            }
        }
        private IJoystickControl _YawGazThumb;
        public IJoystickControl YawGazThumb
        {
            get
            {
                return _YawGazThumb;
            }
            set
            {
                if (value == _YawGazThumb)
                    return;
                _YawGazThumb = value;
            }
        }
        private void timerElapsedHandler(ThreadPoolTimer timer)
        {
            float pitch = 0, roll = 0, yaw = 0, gaz = 0;
            if (_DroneClient == null)
                return;
            if (RollPitchThumb != null)
            {
                pitch = RollPitchThumb.Y;
                roll = RollPitchThumb.X;
            }
            if (YawGazThumb != null)
            {
                yaw = YawGazThumb.X;
                gaz = YawGazThumb.Y;
            }
            //Debug.WriteLine(string.Format("roll={0}, pitch={1}, yaw={2}, gaz={3}", roll, pitch, yaw, gaz));
            DroneClient.InputState.Update(roll, -pitch, yaw, gaz);
        }
        public void Dispose()
        {
            if (_Timer != null)
            {
                _Timer.Cancel();
                _Timer = null;
            }
        }
    }
}
