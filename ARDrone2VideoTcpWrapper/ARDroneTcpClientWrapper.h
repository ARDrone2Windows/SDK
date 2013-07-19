#pragma once
#include "..\ARDrone2VideoTcp\ARDroneTcpClient.h"
#include "VideoPacketWrapper.h"

namespace ARDrone2VideoTcpWrapper{
	public delegate void TcpClientStartedDelegate(bool);
	public ref class ARDroneTcpClientWrapper sealed
	{
		ARDroneTcpClient _wrapped;
	public:
		ARDroneTcpClientWrapper(Platform::String^ hostName);
		virtual ~ARDroneTcpClientWrapper(void);
		void Start(TcpClientStartedDelegate^ onStarted);
		void Stop();
		Windows::Foundation::IAsyncOperation<VideoPacketWrapper^>^ PopVideoPacketAsync();
		VideoPacketWrapper^ PopVideoPacketSync();
	};

}