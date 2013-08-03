using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common.Input
{
    public struct JoystickState
    {
        public float Magnitude;
        public float NormalizedMagnitude;
        public float NormalizedX;
        public float NormalizedY;
    }
}
