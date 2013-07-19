#pragma once
#include "..\ARDrone2VideoTcp\VideoPacket.h"
#include <memory>
namespace ARDrone2VideoTcpWrapper{
	public ref class VideoPacketWrapper sealed
	{
	private:
		std::shared_ptr<VideoPacket> _packet;
	internal:
		VideoPacketWrapper(const std::shared_ptr<VideoPacket>& packet);
	public:
		virtual ~VideoPacketWrapper(void);

		int32 getTimeStamp(){
			return _packet->Timestamp;
		}
		int32 getDuration(){
			return _packet->Duration;
		}
		unsigned int getFrameNumber(){
			return _packet->FrameNumber;
		}
		unsigned short getHeight(){
			return _packet->Height;
		}
		unsigned short getWidth(){
			return _packet->Width;
		}
		bool isFrameTypeI(){
			return _packet->FrameTypev == FrameType::I;
		}

		int64 getDataLength(){
			return _packet->Buffer.getSize();
		}

		void readBuffer(Platform::WriteOnlyArray<unsigned char>^ outputBytes, int destOffset, int srcOffset, int count);

	};
}

