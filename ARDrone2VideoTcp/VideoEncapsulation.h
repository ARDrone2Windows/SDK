#include "pch.h"

struct video_encapsulation_t
{
    uint8 signature[4]; // <Ctype "c_unsigned int8 * 4">
    uint8 version;
    uint8 video_codec;
    unsigned short header_size;
    unsigned int payload_size;
    unsigned short encoded_stream_width;
    unsigned short encoded_stream_height;
    unsigned short display_width;
    unsigned short display_height;
    unsigned int frame_number;
    unsigned int timestamp;
    uint8 total_chuncks;
    uint8 chunck_index;
    uint8 frame_type;
    uint8 control;
    unsigned int stream_uint8_position_lw;
    unsigned int stream_uint8_position_uw;
    unsigned short stream_id;
    uint8 total_slices;
    uint8 slice_index;
    uint8 header1_size;
    uint8 header2_size;
    uint8 reserved2[2]; // <Ctype "c_unsigned int8 * 2">
    unsigned int advertised_size;
    uint8 reserved3[12]; // <Ctype "c_unsigned int8 * 12">
};