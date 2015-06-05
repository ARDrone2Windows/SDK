using System;
namespace ARDrone2Client.Common.Helpers
{
    /// <summary>
    /// This class is for normalizing values from compass to send it them directly to drone for absolute control mode
    /// </summary>
    public static class MagnetometerHelper
    {
        /// <summary>
        /// use it for calculate new compass angle when the device is in horizontal mode and north should be behind the device
        /// </summary>
        /// <param name="angle">use it for SensorReading.MagneticHeading</param>
        /// <returns><Proper angle/returns>
        public static float CalculateUserAngle(double angle)
        {
            float newAngle = (float)angle + 90;
            if (newAngle > 360)
                newAngle -= 360;

            return newAngle;
        }

        /// <summary>
        /// When you have values from CalculateUserAngle use that method to change them to values used by drone from range -1 to 1
        /// </summary>
        /// <param name="angle">Angle returned by CalculateUserAngle or SensorReading.HeadingAccuracy</param>
        /// <returns>Angle for absolute control</returns>
        public static float CalculateDroneAngle(float angle)
        {
            int max = 1;
            int maxEast = 180;
            float newAngle = 0;
            
            if (angle <= maxEast)
                newAngle = (angle * max) / maxEast;

            else newAngle = -(max - ((angle - maxEast * max) / maxEast));

            return newAngle;
        }
    }
}
