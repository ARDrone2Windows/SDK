#pragma once
#include <stdint.h>
#include "ARBuffer.h"
#include "VideoPacket.h"
#include <ppltasks.h>
#include "NativeBuffer.h"
#include <mutex>

class nomoredata_in_stream : public std::exception{

};

class ARStream
{
private:
	concurrency::cancellation_token_source _cts;
	std::shared_ptr<std::recursive_mutex> _mutex;
	bool _isFirstFrame;
	unsigned int _firstFrameTimeStamp;
	unsigned int _lastFrameTimeStamp;
	uint32_t _offset;
	uint32_t _bufferSize;
	bool _isFirstFilled;
	bool _isSecondFilled;
	std::shared_ptr<ARBuffer> _first;
	std::shared_ptr<ARBuffer> _second;
	Windows::Storage::Streams::IInputStream^ _inputStream;
	NativeBufferPool _bufferPool;
	void advance1Buffer();
	bool findMagicWord();
	unsigned char at(uint32_t i) const;
	void copyTo(unsigned char* pOut, uint32_t count);
	Concurrency::task<void> ConsumeDataAsync(ARBuffer* buffer, uint32_t alreadyConsumed, Concurrency::cancellation_token cancelToken, std::shared_ptr<std::recursive_mutex> mutex);
	void ConsumeDataSync(ARBuffer* buffer, uint32_t alreadyConsumed);

	std::shared_ptr<VideoPacket> GetVideoPacketSync();
	concurrency::task<void> FillBufferAsync(ARBuffer* buffer, Windows::Storage::Streams::IBuffer^ tmpBuffer, uint32_t alreadyRead, concurrency::cancellation_token cancelToken, std::shared_ptr<std::recursive_mutex> localMutex, int numberOfRead0 = 0);
public:
	Windows::Storage::Streams::IInputStream^ getInputStream() {
		return _inputStream;
	}
	void setInputStream(Windows::Storage::Streams::IInputStream^ stream){
		_inputStream = stream;
	}
	ARStream(uint32_t bufferSize, Windows::Storage::Streams::IInputStream^ inputStream);
	~ARStream(void);
	Concurrency::task<std::shared_ptr<VideoPacket>> GetVideoPacketAsync();
};

