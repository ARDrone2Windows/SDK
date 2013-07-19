using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common.Input
{
    class InputHelper
    {
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
    }
}
