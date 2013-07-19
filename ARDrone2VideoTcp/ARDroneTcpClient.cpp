#include "pch.h"
#include "VideoEncapsulation.h"
#include "VideoEncapsulationFrameType.h"
#include "VideoPacket.h"
#include "ARDroneTcpClient.h"
#include "ARStream.h"
#include <mutex>

using namespace Concurrency;
using namespace Windows::Networking::Sockets;

using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Storage::Streams;

#define ARDRONE_PORT_NUMBER "5555"

#define FRAME_BUFFER_SIZE 0x1000000
#define NETWORK_STREAM_SIZE 0x10000

class ARDroneTcpClient::Impl
{
private:
	std::mutex _mutex;
    bool videoPackersQueueNotifier;
    bool isStopped;
    std::wstring hostname;
    int offset;
    int frameStart;
    int frameEnd;
    Array<uint8>^ packetData;
    Array<uint8>^ buffer;
	bool hasVideoPacket;
    StreamSocket^ socket;
    cancellation_token_source cts;
    std::function<void(bool)> callback;
	std::unique_ptr<ARStream> _arStream;
private:
    inline FrameType convert(byte frameType)
    {
        video_encapsulation_frametypes_t vframeType = (video_encapsulation_frametypes_t) frameType;

        switch (vframeType)
        {
        case FRAME_TYPE_IDR_FRAME:
        case FRAME_TYPE_I_FRAME:
            return I;
        case FRAME_TYPE_P_FRAME:
            return P;
        case FRAME_TYPE_UNKNNOWN:
        case FRAME_TYPE_HEADERS:
            return Unknown;
        default:break;
            //throw Exception(0, "frame_type");
        }
    }

    inline const int64 getInt64Now()
    {
        FILETIME now;
        GetSystemTimeAsFileTime(&now);

        ULARGE_INTEGER largetInteger ;
        largetInteger.HighPart=now.dwHighDateTime;
        largetInteger.LowPart=now.dwLowDateTime;

        return largetInteger.QuadPart;
    }

	task<std::shared_ptr<VideoPacket>> PullNextFrame(Concurrency::cancellation_token cancelToken){
		if(_arStream->getInputStream() == nullptr){
			String^ hostnameStr = ref new String(this->hostname.c_str());
			HostName^ arDroneHostname = ref new HostName(hostnameStr);
			this->socket = ref new StreamSocket();
			return create_task(this->socket->ConnectAsync(arDroneHostname, ARDRONE_PORT_NUMBER, SocketProtectionLevel::PlainSocket)).then([this, cancelToken] (task<void> previousTask)
			{
				try
				{
					previousTask.get();
					_arStream->setInputStream(socket->InputStream);
				}
				catch(Exception^ exception)
				{
				}
				
				return PullNextFrame(cancelToken);
			});
		}
		return _arStream->GetVideoPacketAsync().then([this, cancelToken](task<std::shared_ptr<VideoPacket>> t){
			if (cancelToken.is_canceled()){
				return concurrency::create_task([](){ return std::shared_ptr<VideoPacket>(nullptr); });
			}
			try{
				auto res = t.get();
				return t;
			}
			catch (concurrency::task_canceled& ){
				if (cancelToken.is_canceled()){
					return concurrency::create_task([](){ return std::shared_ptr<VideoPacket>(nullptr); });
				}
				this->_arStream->setInputStream(nullptr);
				String^ hostnameStr = ref new String(this->hostname.c_str());
				HostName^ arDroneHostname = ref new HostName(hostnameStr);
				this->socket = ref new StreamSocket();
				return create_task(this->socket->ConnectAsync(arDroneHostname, ARDRONE_PORT_NUMBER, SocketProtectionLevel::PlainSocket)).then([this, cancelToken](task<void> previousTask)
				{
					try
					{
						previousTask.get();
						_arStream->setInputStream(socket->InputStream);
					}
					catch (Exception^ exception)
					{
					}
					return PullNextFrame(cancelToken);
				});
			}
			catch (nomoredata_in_stream& ){
				if (cancelToken.is_canceled()){
					return concurrency::create_task([](){ return std::shared_ptr<VideoPacket>(nullptr); });
				}
				this->_arStream->setInputStream(nullptr);
				String^ hostnameStr = ref new String(this->hostname.c_str());
				HostName^ arDroneHostname = ref new HostName(hostnameStr);
				this->socket = ref new StreamSocket();
				return create_task(this->socket->ConnectAsync(arDroneHostname, ARDRONE_PORT_NUMBER, SocketProtectionLevel::PlainSocket)).then([this, cancelToken](task<void> previousTask)
				{
					try
					{
						previousTask.get();
						_arStream->setInputStream(socket->InputStream);
					}
					catch (Exception^ exception)
					{
					}
					return PullNextFrame(cancelToken);
				});
			}
			catch (Platform::ObjectDisposedException^ ex){
				if (cancelToken.is_canceled()){
					return concurrency::create_task([](){ return std::shared_ptr<VideoPacket>(nullptr); });
				}
				this->_arStream->setInputStream(nullptr);
				String^ hostnameStr = ref new String(this->hostname.c_str());
				HostName^ arDroneHostname = ref new HostName(hostnameStr);
				this->socket = ref new StreamSocket();
				return create_task(this->socket->ConnectAsync(arDroneHostname, ARDRONE_PORT_NUMBER, SocketProtectionLevel::PlainSocket)).then([this, cancelToken](task<void> previousTask)
				{
					try
					{
						previousTask.get();
						_arStream->setInputStream(socket->InputStream);
					}
					catch (Exception^ exception)
					{
					}
					return PullNextFrame(cancelToken);
				});
			}
			catch(Platform::Exception^ ex){
				if (cancelToken.is_canceled()){
					return concurrency::create_task([](){ return std::shared_ptr<VideoPacket>(nullptr); });
				}
				this->_arStream->setInputStream(nullptr);
				String^ hostnameStr = ref new String(this->hostname.c_str());
				HostName^ arDroneHostname = ref new HostName(hostnameStr);
				this->socket = ref new StreamSocket();
				return create_task(this->socket->ConnectAsync(arDroneHostname, ARDRONE_PORT_NUMBER, SocketProtectionLevel::PlainSocket)).then([this, cancelToken] (task<void> previousTask)
				{
					try
					{
						previousTask.get();
						_arStream->setInputStream(socket->InputStream);
					}
					catch(Exception^ exception)
					{
					}
					return PullNextFrame(cancelToken);
				});

				
			}
			
		});
		//return create_task(streamReader->LoadAsync(NETWORK_STREAM_SIZE)).then([this, cancelToken] (unsigned int actualLength)
		//{
		//	if(cancelToken.is_canceled()){
		//		return std::shared_ptr<VideoPacket>();
		//	}
		//	std::lock_guard<std::mutex> lg(_mutex);
		//	//if(done)return;
		//	Array<uint8>^ tmpBuffer = ref new Array<uint8>(actualLength);
		//	streamReader->ReadBytes(tmpBuffer);
		//	for (int i = offset; i < actualLength; i++)
		//	{
		//		this->buffer[i] = tmpBuffer[i];
		//	}


		//	//tmpBuffer->Dispose();
		//	delete tmpBuffer;
		//	std::shared_ptr<VideoPacket> packet;
		//	this->offset += actualLength;
		//	if (this->packetData == nullptr)
		//	{
		//		int maxSearchIndex = offset - sizeof(video_encapsulation_t);
		//		for (int i = 0; i < maxSearchIndex; i++)
		//		{
		//			if (this->buffer[i] == 'P' &&
		//				this->buffer[i + 1] == 'a' &&
		//				this->buffer[i + 2] == 'V' &&
		//				this->buffer[i + 3] == 'E')
		//			{
		//				video_encapsulation_t ve = *(video_encapsulation_t*) (this->buffer->Data + i);
		//				
		//				
		//				this->packetData = ref new Array<uint8>(ve.payload_size);
		//				packet = std::make_shared<VideoPacket>();
		//				packet->Timestamp = getInt64Now();
		//				packet->FrameNumber = ve.frame_number;
		//				packet->Width = ve.display_width;
		//				packet->Height = ve.display_height;
		//				packet->FrameTypev = convert(ve.frame_type);
		//				packet->DataLength = ve.payload_size;
		//				frameStart = i + ve.header_size;
		//				frameEnd = frameStart + packetData->Length;
		//				hasVideoPacket = true;
		//				break;
		//			}
		//		}

		//		if (this->packetData == nullptr)
		//		{
		//			// frame is not detected
		//			offset -= maxSearchIndex;

		//			for (int i = maxSearchIndex, j = 0; j < offset; i++, j++)
		//			{
		//				this->buffer[j] = this->buffer[i];
		//			}
		//		}
		//	}

		//	if (this->packetData != nullptr && offset >= frameEnd)
		//	{
		//		// frame acquired
		//		for (int i = frameStart, j = 0; j < packet->DataLength; i++, j++)
		//		{
		//			this->packetData[j] = this->buffer[i];
		//		}

		//		packet->SetData(this->packetData->Data, packet->DataLength);


		//		if (!this->videoPackersQueueNotifier)
		//		{
		//			this->videoPackersQueueNotifier = true;
		//			//this->callback();
		//		}

		//		// clean up acquired frame

		//		delete this->packetData;
		//		this->packetData = nullptr;
		//		offset -= frameEnd;

		//		for (int i = frameEnd, j = 0; j < offset-1; i++, j++)
		//		{
		//			this->buffer[j] = this->buffer[i];
		//		}
		//	}
		//	return packet;

		//	//this->packetData->Dispose();
		//	//delete this->packetData;
		//}).then([this,cancelToken](task<std::shared_ptr<VideoPacket>> t){
		//	if(t.get() != nullptr){
		//		return t;
		//	}
		//	else{
		//		return PullNextFrame(cancelToken);
		//	}
		//});   
	}
public:
    Impl(const std::wstring& hostname) : hostname(hostname), hasVideoPacket(false)
    {
    }
	~Impl(){
		cts.cancel();
	}

    void Start(std::function<void(bool)> callback)
    {
        this->offset = 0;
        this->frameStart = 0;
        this->frameEnd = 0;
        this->packetData = nullptr;
        this->videoPackersQueueNotifier = false;
        this->buffer = ref new Array<uint8>(FRAME_BUFFER_SIZE);
        this->callback = callback;

        String^ hostnameStr = ref new String(this->hostname.c_str());
        HostName^ arDroneHostname = ref new HostName(hostnameStr);

        this->socket = ref new StreamSocket();

        create_task(this->socket->ConnectAsync(arDroneHostname, ARDRONE_PORT_NUMBER, SocketProtectionLevel::PlainSocket)).then([this] (task<void> previousTask)
        {
            try
            {
                previousTask.get();
				this->_arStream.reset(new ARStream(4096, this->socket->InputStream));
				this->callback(true);
            }
            catch(Exception^ exception)
            {
				this->callback(false);
            }
        });
    }

    void Stop()
    {
		_arStream = nullptr;
		this->socket = nullptr;
    }



	Concurrency::task<std::shared_ptr<VideoPacket>> PopVideoPacketAsync() 
	{
		return PullNextFrame(this->cts.get_token());
	}


};

ARDroneTcpClient::ARDroneTcpClient(const std::wstring& hostname) : impl(new Impl(hostname))
{
}

ARDroneTcpClient::~ARDroneTcpClient()
{
    this->impl->Stop();
}

void ARDroneTcpClient::Start(std::function<void(bool)> callback)
{
    this->impl->Start(callback);
}

void ARDroneTcpClient::Stop()
{
    this->impl->Stop();
}


Concurrency::task<std::shared_ptr<VideoPacket>> ARDroneTcpClient::PopVideoPacketAsync()
{
	return impl->PopVideoPacketAsync();
}
