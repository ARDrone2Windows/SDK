using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;
using Windows.System.Threading;
using System.Diagnostics;
using ARDrone2Client.Common.Navigation;

namespace ARDrone2Client.Common.Input
{
    public class XBox360JoystickProvider : IInputProvider, IDisposable
    {
        private DroneClient _DroneClient = null;
        private Controller _Controller = null;
        private State _ControllerState;
        private State _ControllerPreviousState;
        private float _PitchThrottle = 1;
        private float _RollThrottle = 1;
        private float _YawThrottle = 1;
        private float _GazThrottle = 1;
        private short _JoystickRange = 32767;
        private int _FailCounter = 0;
        private int _FailCounterMax = 5;
        private ThreadPoolTimer _Timer;

        public XBox360JoystickProvider()
        {
        }
        public XBox360JoystickProvider(DroneClient droneClient)
        {
            if (droneClient == null)
                throw new ArgumentNullException("DroneClient");
            _DroneClient = droneClient;
            _Controller = new Controller(0);
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
                return "Joystick";
            }
        }
        private async void timerElapsedHandler(ThreadPoolTimer timer)
        {
            float pitch = 0, roll = 0, yaw = 0, gaz = 0;
            if (_DroneClient == null)
                return;
            if (!_Controller.IsConnected || !_Controller.GetState(out _ControllerState))
            {
                _FailCounter++;
                if (_FailCounter > _FailCounterMax)
                {
                    DroneClient.InputState.Update(0, 0, 0, 0);
                    //Avoid overflow
                    _FailCounter = _FailCounterMax;
                }
                return;
            }
            if (_ControllerState.PacketNumber <= _ControllerPreviousState.PacketNumber)
                return;
            //Thumbs
            var leftThumb = InputHelper.NormalizeInput(_ControllerState.Gamepad.LeftThumbX, _ControllerState.Gamepad.LeftThumbY, Convert.ToInt16(SharpDX.XInput.Gamepad.LeftThumbDeadZone * 1.1), _JoystickRange);
            if (leftThumb.NormalizedMagnitude > 0)
            {
                roll = (float)_ControllerState.Gamepad.LeftThumbX * _RollThrottle / this._JoystickRange;
                pitch = (float)_ControllerState.Gamepad.LeftThumbY * _PitchThrottle / this._JoystickRange;
            }
            var rightThumb = InputHelper.NormalizeInput(_ControllerState.Gamepad.RightThumbX, _ControllerState.Gamepad.RightThumbY, Convert.ToInt16(SharpDX.XInput.Gamepad.RightThumbDeadZone * 1.1), _JoystickRange);
            if (rightThumb.NormalizedMagnitude > 0)
            {
                yaw = (float)_ControllerState.Gamepad.RightThumbX * _YawThrottle / this._JoystickRange;
                gaz = (float)_ControllerState.Gamepad.RightThumbY * _GazThrottle / this._JoystickRange;
            }

            _FailCounter = 0;
            DroneClient.InputState.Update(roll, -pitch, yaw, gaz);

            //Buttons
            var buttons = _ControllerState.Gamepad.Buttons;
            if (buttons.HasFlag(GamepadButtonFlags.Start))
            {
                if (await DroneClient.ConnectAsync())
                {
                    if (DroneClient.NavigationData.State.HasFlag(NavigationState.Landed))
                    {
                        DroneClient.TakeOff();
                    }
                    else
                    {
                        DroneClient.Land();
                    }
                }
            }
            if (buttons.HasFlag(GamepadButtonFlags.Back))
            {
                DroneClient.Emergency();
            }
            if (buttons.HasFlag(GamepadButtonFlags.Y))
            {
                DroneClient.Hover();
            }
            if (buttons.HasFlag(GamepadButtonFlags.X))
            {
                DroneClient.CycleProgressiveMode();
            }
            _ControllerPreviousState = _ControllerState;
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
