#ifndef _VIDEO_PACKET_
#define _VIDEO_PACKET_
#include <memory>
#include "FrameType.h"
#include <vector>
#include "ARBuffer.h"

using namespace Platform;

struct VideoPacket
{
	long Timestamp;
	long Duration;
	unsigned int FrameNumber;
	unsigned short Height;
	unsigned short Width;
	FrameType FrameTypev;
	ARBuffer Buffer;

	VideoPacket(
		long timestamp,
		long duration,
		unsigned int frameNumber,
		unsigned short height,
		unsigned short width,
		FrameType frameType,
		uint32_t bufferLength) :
	Timestamp(timestamp),
		Duration(Duration),
		FrameNumber(frameNumber),
		Height(height),
		Width(width),
		FrameTypev(frameType),
		Buffer(bufferLength)
	{
	}



};

#endif