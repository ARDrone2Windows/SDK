using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

using ARDrone2Client.Common.Configuration.Native;
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

        public XBox360JoystickProvider()
        {
        }
        public XBox360JoystickProvider(DroneClient droneClient)
        {
            if (droneClient == null)
                throw new ArgumentNullException("DroneClient");
            _DroneClient = droneClient;
            _Controller = new Controller(0);
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
        public async void Update()
        {
            float pitch = 0, roll = 0, yaw = 0, gaz = 0;
            if (_DroneClient == null || _Controller == null)
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
            var leftThumb = NormalizeInput(_ControllerState.Gamepad.LeftThumbX, _ControllerState.Gamepad.LeftThumbY, Convert.ToInt16(SharpDX.XInput.Gamepad.LeftThumbDeadZone * 1.1), _JoystickRange);
            if (leftThumb.NormalizedMagnitude > 0)
            {
                roll = (float)_ControllerState.Gamepad.LeftThumbX * _RollThrottle / _JoystickRange;
                pitch = (float)_ControllerState.Gamepad.LeftThumbY * _PitchThrottle / _JoystickRange;
            }
            var rightThumb = NormalizeInput(_ControllerState.Gamepad.RightThumbX, _ControllerState.Gamepad.RightThumbY, Convert.ToInt16(SharpDX.XInput.Gamepad.RightThumbDeadZone * 1.1), _JoystickRange);
            if (rightThumb.NormalizedMagnitude > 0)
            {
                yaw = (float)_ControllerState.Gamepad.RightThumbX * _YawThrottle / _JoystickRange;
                gaz = (float)_ControllerState.Gamepad.RightThumbY * _GazThrottle / _JoystickRange;
            }

            _FailCounter = 0;
            DroneClient.InputState.Update(roll, -pitch, yaw, gaz);
            //Debug.WriteLine("InputState=" + DroneClient.InputState.ToString());

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
                DroneClient.CycleProgressiveMode();
            }
            if (buttons.HasFlag(GamepadButtonFlags.X))
            {
                DroneClient.TakePicture();
            }
            if (buttons.HasFlag(GamepadButtonFlags.A))
            {
                DroneClient.StartRecordingVideo();
            }
            if (buttons.HasFlag(GamepadButtonFlags.B))
            {
                DroneClient.StopRecordingVideo();
            }
            if (buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            {
                DroneClient.PlayAnimation(ARDRONE_ANIMATION.ARDRONE_ANIMATION_FLIP_LEFT);
            }
            if (buttons.HasFlag(GamepadButtonFlags.DPadUp))
            {
                DroneClient.PlayAnimation(ARDRONE_ANIMATION.ARDRONE_ANIMATION_FLIP_AHEAD);
            }
            if (buttons.HasFlag(GamepadButtonFlags.DPadRight))
            {
                DroneClient.PlayAnimation(ARDRONE_ANIMATION.ARDRONE_ANIMATION_FLIP_RIGHT);
            }
            if (buttons.HasFlag(GamepadButtonFlags.DPadDown))
            {
                DroneClient.PlayAnimation(ARDRONE_ANIMATION.ARDRONE_ANIMATION_FLIP_BEHIND);
            }

            _ControllerPreviousState = _ControllerState;
        }

        internal static JoystickState NormalizeInput(short X, short Y, short inputDeadZone, short joystickRange)
        {
            //From http://msdn.microsoft.com/en-us/library/windows/desktop/ee417001(v=vs.85).aspx
            JoystickState result = new JoystickState();
            //determine how far the controller is pushed
            result.Magnitude = (float)Math.Sqrt(X * X + Y * Y);
            //determine the direction the controller is pushed
            result.NormalizedX = X / result.Magnitude;
            result.NormalizedY = Y / result.Magnitude;
            result.NormalizedMagnitude = 0;
            //check if the controller is outside a circular dead zone
            if (result.Magnitude > inputDeadZone)
            {
                //clip the magnitude at its expected maximum value
                if (result.Magnitude > joystickRange)
                    result.Magnitude = joystickRange;
                //adjust magnitude relative to the end of the dead zone
                result.Magnitude -= inputDeadZone;
                //optionally normalize the magnitude with respect to its expected range
                //giving a magnitude value of 0.0 to 1.0
                result.NormalizedMagnitude = result.Magnitude / (joystickRange - inputDeadZone);
            }
            else //if the controller is in the deadzone zero out the magnitude
            {
                result.Magnitude = 0;
                result.NormalizedMagnitude = 0;
            }
            return result;
        }

        public void Dispose()
        {
        }
    }
}
