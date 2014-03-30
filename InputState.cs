using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common
{
    public class InputState
    {
        public float Roll = 0F;
        public float Pitch = 0F;
        public float Yaw = 0F;
        public float Gaz = 0F;
        public float Mag = 0F;
        public float MagAcu = 0F;
        private readonly float _Precision = 0.01F;
        public void Update(float roll, float pitch, float yaw, float gaz, float mag = 0F, float mag_acu = 0F)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
            Gaz = gaz;
            Mag = mag;
            MagAcu = mag_acu;
        }
        public override string ToString()
        {
            return string.Format("Roll={0}, Pitch={1}, Yaw={2}, Gaz={3}, Mag={4}, MagAcu={5}", Roll, Pitch, Yaw, Gaz, Mag, MagAcu);
        }

        public bool IsEmpty()
        {
            return (Math.Abs(Roll) < _Precision) && (Math.Abs(Pitch) < _Precision) && (Math.Abs(Yaw) < _Precision) && (Math.Abs(Gaz) < _Precision);
        }
    }
}
