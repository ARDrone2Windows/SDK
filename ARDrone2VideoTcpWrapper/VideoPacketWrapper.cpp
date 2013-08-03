#include "pch.h"
#include "VideoPacketWrapper.h"

namespace ARDrone2VideoTcpWrapper{

VideoPacketWrapper::VideoPacketWrapper( const std::shared_ptr<VideoPacket>& packet ):_packet(packet)
{

}


VideoPacketWrapper::~VideoPacketWrapper(void)
{
}

void VideoPacketWrapper::readBuffer( Platform::WriteOnlyArray<unsigned char>^ outputBytes, int destOffset, int srcOffset, int count )
{
	if(srcOffset+count <= (int)_packet->Buffer.getSize()){
		memcpy_s(outputBytes->Data+ destOffset, outputBytes->Length-destOffset, _packet->Buffer.getData()+srcOffset, count);
	}

}

}