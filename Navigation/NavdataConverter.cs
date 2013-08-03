using System;
using ARDrone2Client.Common.Helpers;
using ARDrone2Client.Common.Navigation.Native;

namespace ARDrone2Client.Common.Navigation
{
    public static class NavdataConverter
    {
        private const float DegreeToRadian = (float)(Math.PI / 180.0f);

        public static NavigationData ToNavigationData(NavdataBag navdataBag)
        {
            var navigationData = new NavigationData();

            var ardroneState = (def_ardrone_state_mask_t)navdataBag.navigationDataHeader.ardrone_state;
            UpdateStateUsing(ardroneState, ref navigationData.State);
            navigationData.Masks = ardroneState;

            var ctrlState = (CTRL_STATES)(navdataBag.demo.ctrl_state >> 0x10);
            UpdateStateUsing(ctrlState, ref navigationData.State);

            navigationData.Yaw = DegreeToRadian * (navdataBag.demo.psi / 1000.0f);
            navigationData.Pitch = DegreeToRadian * (navdataBag.demo.theta / 1000.0f);
            navigationData.Roll = DegreeToRadian * (navdataBag.demo.phi / 1000.0f);

            navigationData.Altitude = navdataBag.demo.altitude / 1000.0f;

            navigationData.Time = navdataBag.time.time;
            DateTime date = new DateTime(navdataBag.time.time);

            navigationData.Velocity.X = navdataBag.demo.vx / 1000.0f;
            navigationData.Velocity.Y = navdataBag.demo.vy / 1000.0f;
            navigationData.Velocity.Z = navdataBag.demo.vz / 1000.0f;

            Battery bat = new Battery();
            bat.Low = ardroneState.HasFlag(def_ardrone_state_mask_t.ARDRONE_VBAT_LOW);
            bat.Percentage = navdataBag.demo.vbat_flying_percentage;
            bat.Voltage = navdataBag.raw_measures.vbat_raw / 1000.0f;
            navigationData.Battery = bat;

            navigationData.Wifi.LinkQuality = 1.0f - ConversionHelper.ToSingle(navdataBag.wifi.link_quality);

            navigationData.Video.FrameNumber = navdataBag.video_stream.frame_number;

            return navigationData;
        }

        private static void UpdateStateUsing(def_ardrone_state_mask_t ardroneState, ref NavigationState state)
        {
            if (ardroneState.HasFlag(def_ardrone_state_mask_t.ARDRONE_FLY_MASK))
                state |= NavigationState.Flying;
            else
                state |= NavigationState.Landed;

            if (ardroneState.HasFlag(def_ardrone_state_mask_t.ARDRONE_EMERGENCY_MASK))
                state |= NavigationState.Emergency;

            if (ardroneState.HasFlag(def_ardrone_state_mask_t.ARDRONE_COMMAND_MASK))
                state |= NavigationState.Command;

            if (ardroneState.HasFlag(def_ardrone_state_mask_t.ARDRONE_CONTROL_MASK))
                state |= NavigationState.Control;
        }

        private static void UpdateStateUsing(CTRL_STATES ctrlStates, ref NavigationState state)
        {
            switch (ctrlStates)
            {
                case CTRL_STATES.CTRL_TRANS_TAKEOFF:
                    state |= NavigationState.Takeoff;
                    break;
                case CTRL_STATES.CTRL_TRANS_LANDING:
                    state |= NavigationState.Landing;
                    break;
                case CTRL_STATES.CTRL_HOVERING:
                    state |= NavigationState.Hovering;
                    break;
            }
        }
    }
}