#include "pch.h"
#include "ARDroneTcpClientWrapper.h"


namespace ARDrone2VideoTcpWrapper{
	ARDroneTcpClientWrapper::ARDroneTcpClientWrapper(Platform::String^ hostName) : 
		_wrapped(std::wstring(hostName->Data()))
	{
	}


	ARDroneTcpClientWrapper::~ARDroneTcpClientWrapper(void)
	{
	}

	void ARDroneTcpClientWrapper::Start( TcpClientStartedDelegate^ onStarted )
	{
		_wrapped.Start([onStarted](bool result){
			onStarted(result);
		});
	}

	void ARDroneTcpClientWrapper::Stop()
	{
		_wrapped.Stop();
	}

	Windows::Foundation::IAsyncOperation<VideoPacketWrapper^>^ ARDroneTcpClientWrapper::PopVideoPacketAsync()
	{
		auto task = _wrapped.PopVideoPacketAsync().then([](const std::shared_ptr<VideoPacket>& packet){
			return ref new VideoPacketWrapper(packet);
		});
		return concurrency::create_async([task](){return task;});
	}

	VideoPacketWrapper^ ARDroneTcpClientWrapper::PopVideoPacketSync()
	{
		return _wrapped.PopVideoPacketAsync().then([](const std::shared_ptr<VideoPacket>& packet){
			return ref new VideoPacketWrapper(packet);
		}).get();
	}

}