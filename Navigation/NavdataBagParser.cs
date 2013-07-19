using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using ARDrone2Client.Common.Navigation.Native.Options;
using ARDrone2Client.Common.Navigation.Native;

namespace ARDrone2Client.Common.Navigation
{
    public class NavdataBagParser
    {
        private const int NavdataHeader = 0x55667788;
        private const int navigationDataHeaderSize = 16;
        //private UInt32 navigationDataCheckSum;

        public static bool TryParse(ref NavigationPacket packet, out NavdataBag navigationData)
        {
            navigationData = new NavdataBag();
            navdata_tag_t navigationDataTag = 0;
            UInt16 size = 0;

            using (MemoryStream memoryStream = new MemoryStream(packet.Data))
            {
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                navigationData.navigationDataHeader.header = binaryReader.ReadUInt32();
                navigationData.navigationDataHeader.ardrone_state = binaryReader.ReadUInt32();
                navigationData.navigationDataHeader.sequence = binaryReader.ReadUInt32();
                navigationData.navigationDataHeader.vision_defined = binaryReader.ReadUInt32();
                memoryStream.Position = navigationDataHeaderSize;

                if (navigationData.navigationDataHeader.header == NavdataHeader)
                {
                    while (memoryStream.Position < memoryStream.Length)
                    {
                        navigationDataTag = (navdata_tag_t)binaryReader.ReadUInt16();
                        size = binaryReader.ReadUInt16();

                        switch (navigationDataTag)
                        {
                            case navdata_tag_t.NAVDATA_DEMO:
                                MapNavigationData(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                memoryStream.Position += size - 4;
                                break;
                            case navdata_tag_t.NAVDATA_CKS:
                                navigationData.cks.cks = binaryReader.ReadUInt32();
                                break;
                            case navdata_tag_t.NAVDATA_VISION_DETECT:
                                MapVisionDetect(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                memoryStream.Position += size - 4;
                                break;
                            case navdata_tag_t.NAVDATA_MAGNETO:
                                MapMagneto(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                memoryStream.Position += size - 4;
                                break;
                            case navdata_tag_t.NAVDATA_TIME:
                                navigationData.time.time = binaryReader.ReadUInt32();
                                break;
                            case navdata_tag_t.NAVDATA_RAW_MEASURES:
                                MapRawMeasures(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                break;
                            case navdata_tag_t.NAVDATA_ADC_DATA_FRAME:
                                MapAdcDataFrame(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                break;
                            case navdata_tag_t.NAVDATA_GYROS_OFFSETS:
                                MapGyrosOffsets(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                break;
                            case navdata_tag_t.NAVDATA_PHYS_MEASURES:
                                MapPhysMeasures(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                break;
                            case navdata_tag_t.NAVDATA_VISION_OF:
                                MapVisionOfTag(packet.Data, (int)(memoryStream.Position - 4), ref navigationData);
                                break;
                            default:
                                //Currently not used
                                memoryStream.Position += size - 4; // substract 4 bytes for Tag and Size field
                                break;
                        }
                    }
                }
                    
                uint dataCheckSum = CalculateChecksum(packet.Data);
                if (navigationData.cks.cks == dataCheckSum)
                {
                    return true;
                }
            }

            return false;
        }


        private static void ProcessOption(navdata_tag_t navigationDataTag, byte[] data, int position, ref NavdataBag navigationData)
        {
            //switch (navigationDataTag)
            //{
            //    #region DONE

            //    //case NavigationDataTag.NAVDATA_DEMO:
            //    //    MapNavigationData(data, position, ref navigationData);
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_VISION_DETECT:
            //    //    MapVisionDetect(data, position, ref navigationData);
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_MAGNETO:
            //    //    MapMagneto(data, position, ref navigationData);
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_CKS:
            //    //    navigationData.cks.cks = binaryReader.ReadUInt32();
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_TIME:
            //    //    navigationData.time = *(navdata_time_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_RAW_MEASURES:
            //    //    navigationData.raw_measures = *(navdata_raw_measures_t*) option;
            //    //    break;
            //                    //case NavigationDataTag.NAVDATA_ADC_DATA_FRAME:
            //    //    navigationData.adc_data_frame = *(navdata_adc_data_frame_t*) option;
            //    //    break;
            //                    //case NavigationDataTag.NAVDATA_GYROS_OFFSETS:
            //    //    navigationData.gyros_offsets = *(navdata_gyros_offsets_t*) option;
            //    //    break;
            //                    //case NavigationDataTag.NAVDATA_PHYS_MEASURES:
            //    //    navigationData.phys_measures = *(navdata_phys_measures_t*) option;
            //    //    break;
            //                    //case NavigationDataTag.NAVDATA_VISION_OF:
            //    //    navigationData.vision_of_tag = *(navdata_vision_of_t*) option;
            //    //    break;
            //    #endregion

            //    #region NOT DONE


            //    //case NavigationDataTag.NAVDATA_EULER_ANGLES:
            //    //    navigationData.euler_angles = *(navdata_euler_angles_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_REFERENCES:
            //    //    navigationData.references = *(navdata_references_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_TRIMS:
            //    //    navigationData.trims = *(navdata_trims_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_RC_REFERENCES:
            //    //    navigationData.rc_references = *(navdata_rc_references_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_PWM:
            //    //    navigationData.pwm = *(navdata_pwm_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_ALTITUDE:
            //    //    navigationData.altitude = *(navdata_altitude_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_VISION_RAW:
            //    //    navigationData.vision_raw = *(navdata_vision_raw_t*) option;
            //    //    break;

            //    //case NavigationDataTag.NAVDATA_VISION:
            //    //    navigationData.vision = *(navdata_vision_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_VISION_PERF:
            //    //    navigationData.vision_perf = *(navdata_vision_perf_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_TRACKERS_SEND:
            //    //    navigationData.trackers_send = *(navdata_trackers_send_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_WATCHDOG:
            //    //    navigationData.watchdog = *(navdata_watchdog_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_VIDEO_STREAM:
            //    //    navigationData.video_stream = *(navdata_video_stream_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_GAMES:
            //    //    navigationData.games = *(navdata_games_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_PRESSURE_RAW:
            //    //    navigationData.pressure_raw = *(navdata_pressure_raw_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_WIND:
            //    //    navigationData.wind_speed = *(navdata_wind_speed_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_KALMAN_PRESSURE:
            //    //    navigationData.kalman_pressure = *(navdata_kalman_pressure_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_HDVIDEO_STREAM:
            //    //    navigationData.hdvideo_stream = *(navdata_hdvideo_stream_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_WIFI:
            //    //    navigationData.wifi = *(navdata_wifi_t*) option;
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_ZIMMU_3000:
            //    //    // do nothing
            //    //    break;
            //    //case NavigationDataTag.NAVDATA_NUMS:
            //    //    // do nothing
            //    //    break;
            //    //default:
            //    //    throw new ArgumentOutOfRangeException();

            //    #endregion
            //}
        }

        /// <summary>
        /// Populates the navigation data structure.
        /// </summary>
        /// <param name="position">The current position in the byte sequence.</param>
        private static void MapNavigationData(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.demo.tag = reader.ReadUInt16();
                navigationData.demo.size = reader.ReadUInt16();
                navigationData.demo.ctrl_state = reader.ReadUInt32();
                navigationData.demo.vbat_flying_percentage = reader.ReadUInt32();
                navigationData.demo.theta = reader.ReadSingle();
                navigationData.demo.phi = reader.ReadSingle();
                navigationData.demo.psi = reader.ReadSingle();
                navigationData.demo.altitude = reader.ReadInt32();
                navigationData.demo.vx = reader.ReadSingle();
                navigationData.demo.vy = reader.ReadSingle();
                navigationData.demo.vz = reader.ReadSingle();
                navigationData.demo.num_frames = reader.ReadUInt32();

                navigationData.demo.detection_camera_rot.m11 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m12 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m13 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m21 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m22 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m23 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m31 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m32 = reader.ReadSingle();
                navigationData.demo.detection_camera_rot.m33 = reader.ReadSingle();

                navigationData.demo.detection_camera_trans.x = reader.ReadSingle();
                navigationData.demo.detection_camera_trans.y = reader.ReadSingle();
                navigationData.demo.detection_camera_trans.z = reader.ReadSingle();
                navigationData.demo.detection_tag_index = reader.ReadUInt32();
                navigationData.demo.detection_camera_type = reader.ReadUInt32();

                navigationData.demo.drone_camera_rot.m11 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m12 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m13 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m21 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m22 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m23 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m31 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m32 = reader.ReadSingle();
                navigationData.demo.drone_camera_rot.m33 = reader.ReadSingle();

                navigationData.demo.drone_camera_trans.x = reader.ReadSingle();
                navigationData.demo.drone_camera_trans.y = reader.ReadSingle();
                navigationData.demo.drone_camera_trans.z = reader.ReadSingle();
            }
        }

        /// <summary>
        /// Populates the vision detect data structure.
        /// </summary>
        /// <param name="data">The byte sequence.</param>
        /// <param name="position">The current position in the byte sequence.</param>
        /// <param name="navigationData">The navigation data bag.</param>
        private static void MapVisionDetect(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.vision_detect.Tag = reader.ReadUInt16();
                navigationData.vision_detect.Size = reader.ReadUInt16();

                navigationData.vision_detect.NbDetected = reader.ReadUInt32();

                navigationData.vision_detect.Tag1Type = reader.ReadUInt32();
                navigationData.vision_detect.Tag2Type = reader.ReadUInt32();
                navigationData.vision_detect.Tag3Type = reader.ReadUInt32();
                navigationData.vision_detect.Tag4Type = reader.ReadUInt32();

                navigationData.vision_detect.Tag1X = reader.ReadUInt32();
                navigationData.vision_detect.Tag2X = reader.ReadUInt32();
                navigationData.vision_detect.Tag3X = reader.ReadUInt32();
                navigationData.vision_detect.Tag4X = reader.ReadUInt32();

                navigationData.vision_detect.Tag1Y = reader.ReadUInt32();
                navigationData.vision_detect.Tag2Y = reader.ReadUInt32();
                navigationData.vision_detect.Tag3Y = reader.ReadUInt32();
                navigationData.vision_detect.Tag4Y = reader.ReadUInt32();

                navigationData.vision_detect.Tag1BoxWidth = reader.ReadUInt32();
                navigationData.vision_detect.Tag2BoxWidth = reader.ReadUInt32();
                navigationData.vision_detect.Tag3BoxWidth = reader.ReadUInt32();
                navigationData.vision_detect.Tag4BoxWidth = reader.ReadUInt32();

                navigationData.vision_detect.Tag1BoxHeight = reader.ReadUInt32();
                navigationData.vision_detect.Tag2BoxHeight = reader.ReadUInt32();
                navigationData.vision_detect.Tag3BoxHeight = reader.ReadUInt32();
                navigationData.vision_detect.Tag4BoxHeight = reader.ReadUInt32();

                navigationData.vision_detect.Tag1Distance = reader.ReadUInt32();
                navigationData.vision_detect.Tag2Distance = reader.ReadUInt32();
                navigationData.vision_detect.Tag3Distance = reader.ReadUInt32();
                navigationData.vision_detect.Tag4Distance = reader.ReadUInt32();
            }
        }

        /// <summary>
        /// Populates data elated to the Magneto
        /// </summary>
        /// <param name="data">The byte sequence.</param>
        /// <param name="position">The current position in the byte sequence.</param>
        /// <param name="navigationData">The navigation data bag.</param>
        private static void MapMagneto(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.magneto.tag = reader.ReadUInt16();
                navigationData.magneto.size = reader.ReadUInt16();

                navigationData.magneto.mx = reader.ReadUInt16();
                navigationData.magneto.my = reader.ReadUInt16();
                navigationData.magneto.mz = reader.ReadUInt16();

                navigationData.magneto.magneto_raw.x = reader.ReadSingle();
                navigationData.magneto.magneto_raw.y = reader.ReadSingle();
                navigationData.magneto.magneto_raw.z = reader.ReadSingle();

                navigationData.magneto.magneto_rectified.x = reader.ReadSingle();
                navigationData.magneto.magneto_rectified.y = reader.ReadSingle();
                navigationData.magneto.magneto_rectified.z = reader.ReadSingle();

                navigationData.magneto.magneto_offset.x = reader.ReadSingle();
                navigationData.magneto.magneto_offset.y = reader.ReadSingle();
                navigationData.magneto.magneto_offset.z = reader.ReadSingle();

                navigationData.magneto.heading_unwrapped = reader.ReadSingle();
                navigationData.magneto.heading_gyro_unwrapped = reader.ReadSingle();
                navigationData.magneto.heading_fusion_unwrapped = reader.ReadSingle();

                navigationData.magneto.magneto_calibration_ok = reader.ReadByte();

                navigationData.magneto.magneto_state = reader.ReadUInt32();

                navigationData.magneto.magneto_radius = reader.ReadSingle();
                navigationData.magneto.error_mean = reader.ReadSingle();
                navigationData.magneto.error_var = reader.ReadSingle();
            }
        }

        private static void MapRawMeasures(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.raw_measures.tag = reader.ReadUInt16();
                navigationData.raw_measures.size = reader.ReadUInt16();

                navigationData.raw_measures.raw_accs_1 = reader.ReadUInt16();
                navigationData.raw_measures.raw_accs_2 = reader.ReadUInt16();
                navigationData.raw_measures.raw_accs_3 = reader.ReadUInt16();
                navigationData.raw_measures.raw_gyros_1 = reader.ReadUInt16();
                navigationData.raw_measures.raw_gyros_2 = reader.ReadUInt16();
                navigationData.raw_measures.raw_gyros_3 = reader.ReadUInt16();
                navigationData.raw_measures.raw_gyros_110_1 = reader.ReadUInt16();
                navigationData.raw_measures.raw_gyros_110_2 = reader.ReadUInt16();

                navigationData.raw_measures.vbat_raw = reader.ReadUInt32();
                navigationData.raw_measures.us_debut_echo = reader.ReadUInt16();
                navigationData.raw_measures.us_fin_echo = reader.ReadUInt16();
                navigationData.raw_measures.us_association_echo = reader.ReadUInt16();
                navigationData.raw_measures.us_distance_echo = reader.ReadUInt16();
                navigationData.raw_measures.us_courbe_temps = reader.ReadUInt16();
                navigationData.raw_measures.us_courbe_valeur = reader.ReadUInt16();
                navigationData.raw_measures.us_courbe_ref = reader.ReadUInt16();
                navigationData.raw_measures.flag_echo_ini = reader.ReadUInt16();
                navigationData.raw_measures.nb_echo = reader.ReadUInt16();
                navigationData.raw_measures.sum_echo = reader.ReadUInt32();
                navigationData.raw_measures.alt_temp_raw = reader.ReadUInt32();
                navigationData.raw_measures.gradient = reader.ReadUInt16();
            }
        }

        private static void MapAdcDataFrame(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.adc_data_frame.tag = reader.ReadUInt16();
                navigationData.adc_data_frame.size = reader.ReadUInt16();

                navigationData.adc_data_frame.version = reader.ReadUInt32();

                navigationData.adc_data_frame.data_frame1 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame2 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame3 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame4 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame5 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame6 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame7 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame8 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame9 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame10 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame11 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame12 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame13 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame14 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame15 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame16 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame17 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame18 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame19 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame20 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame21 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame22 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame23 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame24 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame25 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame26 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame27 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame28 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame29 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame30 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame31 = reader.ReadByte();
                navigationData.adc_data_frame.data_frame32 = reader.ReadByte();
            }
        }

        private static void MapGyrosOffsets(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.gyros_offsets.tag = reader.ReadUInt16();
                navigationData.gyros_offsets.size = reader.ReadUInt16();

                navigationData.gyros_offsets.offset_g1 = reader.ReadSingle();
                navigationData.gyros_offsets.offset_g2 = reader.ReadSingle();
                navigationData.gyros_offsets.offset_g3 = reader.ReadSingle();
            }
        }

        private static void MapPhysMeasures(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.phys_measures.tag = reader.ReadUInt16();
                navigationData.phys_measures.size = reader.ReadUInt16();

                navigationData.phys_measures.accs_temp = reader.ReadSingle();
                navigationData.phys_measures.gyro_temp = reader.ReadUInt16();
                navigationData.phys_measures.phys_accs1 = reader.ReadSingle();
                navigationData.phys_measures.phys_accs2 = reader.ReadSingle();
                navigationData.phys_measures.phys_accs3 = reader.ReadSingle();
                navigationData.phys_measures.phys_gyros1 = reader.ReadSingle();
                navigationData.phys_measures.phys_gyros2 = reader.ReadSingle();
                navigationData.phys_measures.phys_gyros3 = reader.ReadSingle();
                navigationData.phys_measures.alim3V3 = reader.ReadUInt32();
                navigationData.phys_measures.vrefEpson = reader.ReadUInt32();
                navigationData.phys_measures.vrefIDG = reader.ReadUInt32();
            }
        }

        private static void MapVisionOfTag(byte[] data, int position, ref NavdataBag navigationData)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);

                navigationData.vision_of_tag.tag = reader.ReadUInt16();
                navigationData.vision_of_tag.size = reader.ReadUInt16();

                navigationData.vision_of_tag.tag1_dx = reader.ReadSingle();
                navigationData.vision_of_tag.tag2_dx = reader.ReadSingle();
                navigationData.vision_of_tag.tag3_dx = reader.ReadSingle();
                navigationData.vision_of_tag.tag4_dx = reader.ReadSingle();
                navigationData.vision_of_tag.tag5_dx = reader.ReadSingle();
                navigationData.vision_of_tag.tag1_dy = reader.ReadSingle();
                navigationData.vision_of_tag.tag2_dy = reader.ReadSingle();
                navigationData.vision_of_tag.tag3_dy = reader.ReadSingle();
                navigationData.vision_of_tag.tag4_dy = reader.ReadSingle();
                navigationData.vision_of_tag.tag5_dy = reader.ReadSingle();
            }
        }
        
        
        private static uint CalculateChecksum(byte[] buffer)
        {
            uint checksum = 0;
            for (int i = 0; i < buffer.Length - 8; ++i)
                checksum += buffer[i];
            return checksum;
        }
    }
}