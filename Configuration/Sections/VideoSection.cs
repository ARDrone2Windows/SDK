using System;
using System.Runtime.InteropServices;

namespace ARDrone2Client.Common.Configuration.Sections
{
    public class VideoSection : SectionBase
    {
        public VideoSection(DroneConfiguration configuration)
            : base(configuration, "video")
        {
        }

        public Int32 CamifFps
        {
            get { return GetInt32("camif_fps"); }
        }

        public Int32 CodecFps
        {
            get { return GetInt32("codec_fps"); }
            set { Set("codec_fps", value); }
        }

        public Int32 CamifBuffers
        {
            get { return GetInt32("camif_buffers"); }
        }

        public Int32 Trackers
        {
            get { return GetInt32("num_trackers"); }
        }

        public VideoCodecType Codec
        {
            get { return GetEnum<VideoCodecType>("video_codec"); }
            set { SetEnum<VideoCodecType>("video_codec", value); }
        }

        public Int32 Slices
        {
            get { return GetInt32("video_slices"); }
            set { Set("video_slices", value); }
        }

        public Int32 LiveSocket
        {
            get { return GetInt32("video_live_socket"); }
            set { Set("video_live_socket", value); }
        }

        public Int32 StorageSpace
        {
            get { return GetInt32("video_storage_space"); }
        }

        public Int32 Bitrate
        {
            get { return GetInt32("bitrate"); }
            set { Set("bitrate", value); }
        }

        public Int32 MaxBitrate
        {
            get { return GetInt32("max_bitrate"); }
            set { Set("max_bitrate", value); }
        }

        public VideoBitrateControlMode BitrateCtrlMode
        {
            get { return GetEnum<VideoBitrateControlMode>("bitrate_ctrl_mode"); }
            set { SetEnum<VideoBitrateControlMode>("bitrate_ctrl_mode", value); }
        }

        public Int32 BitrateStorage
        {
            get { return GetInt32("bitrate_storage"); }
            set { Set("bitrate_storage", value); }
        }

        public VideoChannelType Channel
        {
            get { return GetEnum<VideoChannelType>("video_channel"); }
            set { SetEnum<VideoChannelType>("video_channel", value); }
        }

        public Boolean OnUsb
        {
            get { return GetBoolean("video_on_usb"); }
            set { Set("video_on_usb", value); }
        }

        public Int32 FileIndex
        {
            get { return GetInt32("video_file_index"); }
            set { Set("video_file_index", value); }
        }
    }
}