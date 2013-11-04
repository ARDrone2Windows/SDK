using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class DetectSection : SectionBase
    {
        public DetectSection(DroneConfiguration configuration)
            : base(configuration, "detect")
        {
        }

        public Int32 EnemyColors
        {
            get { return GetInt32("enemy_colors"); }
            set { Set("enemy_colors", value); }
        }

        public Int32 GroundstripeColors
        {
            get { return GetInt32("groundstripe_colors"); }
            set { Set("groundstripe_colors", value); }
        }

        public Int32 EnemyWithoutShell
        {
            get { return GetInt32("enemy_without_shell"); }
            set { Set("enemy_without_shell", value); }
        }

        public Int32 Type
        {
            get { return GetInt32("detect_type"); }
            set { Set("detect_type", value); }
        }

        public Int32 DetectionsSelectH
        {
            get { return GetInt32("detections_select_h"); }
            set { Set("detections_select_h", value); }
        }

        public Int32 DetectionsSelectVHsync
        {
            get { return GetInt32("detections_select_v_hsync"); }
            set { Set("detections_select_v_hsync", value); }
        }

        public Int32 DetectionsSelectV
        {
            get { return GetInt32("detections_select_v"); }
            set { Set("detections_select_v", value); }
        }
    }
}