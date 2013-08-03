#include "pch.h"
#include "ARStream.h"
#include "VideoEncapsulation.h"
#include "VideoEncapsulationFrameType.h"
#include <assert.h>
#include <sstream>

using namespace Windows::Storage::Streams;
using namespace Concurrency;

const unsigned char c_magicWord [] = { 'P', 'a', 'V', 'E' };

ARStream::ARStream(uint32_t bufferSize, IInputStream^ inputStream) :
	_offset(0),
	_isFirstFilled(false),
	_isSecondFilled(false),
	_bufferSize(bufferSize),
	_first(new ARBuffer(bufferSize)),
	_second(new ARBuffer(bufferSize)),
	_inputStream(inputStream),
	_bufferPool(bufferSize),
	_isFirstFrame(true),
	_mutex(new std::recursive_mutex())
{
}


ARStream::~ARStream(void)
{
	std::lock_guard<std::recursive_mutex> lg(*_mutex);
	_cts.cancel();
}

inline const int64 getInt64Now()
{
	FILETIME now;
	GetSystemTimeAsFileTime(&now);

	ULARGE_INTEGER largetInteger;
	largetInteger.HighPart = now.dwHighDateTime;
	largetInteger.LowPart = now.dwLowDateTime;

	return largetInteger.QuadPart;
}

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
	default:
		return Unknown;
		//throw Exception(0, "frame_type");
	}
}

std::shared_ptr<VideoPacket> ARStream::GetVideoPacketSync()
{
	if (!_isFirstFilled){
		auto buffer = _bufferPool.Acquire();
		IBuffer^ buffer2 = create_task(_inputStream->ReadAsync(buffer, _bufferSize, InputStreamOptions::None)).get();

		assert(buffer2->Length == _bufferSize);
		auto bufferByteAccess = GetBufferByteAccess(buffer2);
		byte* data;
		bufferByteAccess->Buffer(&data);
		memcpy_s(_first->getData(), _first->getSize(), data, buffer2->Length);
		_isFirstFilled = true;

		_bufferPool.Release(buffer);
	}
	if (!_isSecondFilled){
		auto buffer = _bufferPool.Acquire();
		IBuffer^ buffer2 = create_task(_inputStream->ReadAsync(buffer, _bufferSize, InputStreamOptions::None)).get();

		assert(buffer2->Length == _bufferSize);
		auto bufferByteAccess = GetBufferByteAccess(buffer2);
		byte* data;
		bufferByteAccess->Buffer(&data);
		memcpy_s(_second->getData(), _second->getSize(), data, buffer2->Length);
		_isSecondFilled = true;

		_bufferPool.Release(buffer);

	}
	if (!findMagicWord()){
		advance1Buffer();
		return GetVideoPacketSync();
	}

	if (_offset >= _bufferSize){
		advance1Buffer();
		return GetVideoPacketSync();
	}

	video_encapsulation_t ve;
	copyTo((unsigned char*) &ve, sizeof(video_encapsulation_t));
	/*
	this->packetData = ref new Array<uint8>(ve.payload_size);
	packet = std::make_shared<VideoPacket>();
	packet->Timestamp = getInt64Now();
	packet->FrameNumber = ve.frame_number;
	packet->Width = ve.display_width;
	packet->Height = ve.display_height;
	packet->FrameTypev = convert(ve.frame_type);
	packet->DataLength = ve.payload_size;
	frameStart = i + ve.header_size;
	frameEnd = frameStart + packetData->Length;
	hasVideoPacket = true;
	*/
	_offset += ve.header_size;
	if (_isFirstFrame){
		_firstFrameTimeStamp = ve.timestamp;
		_lastFrameTimeStamp = 0;
		_isFirstFrame = false;
	}
	auto ts = ve.timestamp > _firstFrameTimeStamp ? ve.timestamp - _firstFrameTimeStamp : 0;
	std::shared_ptr<VideoPacket> packet(new VideoPacket(ts + 0, ts - _lastFrameTimeStamp, ve.frame_number, ve.display_height, ve.display_width, convert(ve.frame_type), ve.payload_size));
	_lastFrameTimeStamp = ts;
	ConsumeDataSync(&packet->Buffer, 0);
	return packet;

}

Concurrency::task<std::shared_ptr<VideoPacket>> ARStream::GetVideoPacketAsync()
{
	auto cancelToken = _cts.get_token();
	auto localMutex = _mutex;

	std::lock_guard<std::recursive_mutex> lg(*localMutex);
	if (cancelToken.is_canceled()){
		return create_task([]{return std::shared_ptr<VideoPacket>(nullptr); });
	}

	//return create_task([this](){return GetVideoPacketSync();});
	if (!_isFirstFilled){
		auto buffer = _bufferPool.Acquire();
		return  FillBufferAsync(_first.get(), buffer, 0, cancelToken, localMutex)
			.then([this, buffer, localMutex, cancelToken]{
				std::lock_guard<std::recursive_mutex> lg(*localMutex);
				if (cancelToken.is_canceled()){
					return create_task([]{return std::shared_ptr<VideoPacket>(nullptr); });
				}
				_isFirstFilled = true;
				_bufferPool.Release(buffer);
				return GetVideoPacketAsync();
		}, cancelToken);
	}
	if (!_isSecondFilled){
		auto buffer = _bufferPool.Acquire();
		return FillBufferAsync(_second.get(), buffer, 0, cancelToken, localMutex)
			.then([this, buffer, localMutex, cancelToken]{
				std::lock_guard<std::recursive_mutex> lg(*localMutex);
				if (cancelToken.is_canceled()){
					return create_task([]{return std::shared_ptr<VideoPacket>(nullptr); });
				}
				_isSecondFilled = true;
				_bufferPool.Release(buffer);
				return GetVideoPacketAsync();
		}, cancelToken);
	}
	if (!findMagicWord()){
		advance1Buffer();
		return GetVideoPacketAsync();
	}

	if (_offset >= _bufferSize){
		advance1Buffer();
		return GetVideoPacketAsync();
	}

	video_encapsulation_t ve;
	copyTo((unsigned char*) &ve, sizeof(video_encapsulation_t));

	_offset += ve.header_size;
	if (_isFirstFrame){
		_firstFrameTimeStamp = ve.timestamp;
		_lastFrameTimeStamp = 0;
		_isFirstFrame = false;
	}
	auto ts = ve.timestamp > _firstFrameTimeStamp ? ve.timestamp - _firstFrameTimeStamp : 0;
	std::shared_ptr<VideoPacket> packet(new VideoPacket(ts + 250, ts - _lastFrameTimeStamp, ve.frame_number, ve.display_height, ve.display_width, convert(ve.frame_type), ve.payload_size));
	_lastFrameTimeStamp = ts;
	return ConsumeDataAsync(&packet->Buffer, 0, cancelToken, localMutex).then([packet](){

		return std::shared_ptr<VideoPacket>(packet);
	}, cancelToken);
}

void ARStream::advance1Buffer()
{
	assert(_offset >= _bufferSize);
	std::swap(_first, _second);
	_offset -= _bufferSize;
	_isFirstFilled = _isSecondFilled;
	_isSecondFilled = false;

}

bool ARStream::findMagicWord()
{
	for (uint32_t i = _offset; i < 2 * _bufferSize - ARRAYSIZE(c_magicWord); i++){
		if (at(i) == c_magicWord[0]
			&& at(i + 1) == c_magicWord[1]
			&& at(i + 2) == c_magicWord[2]
			&& at(i + 3) == c_magicWord[3])
		{
			_offset = i;
			return true;
		}
	}
	_offset = 2 * _bufferSize - ARRAYSIZE(c_magicWord);
	return false;
}

unsigned char ARStream::at(uint32_t i) const
{
	if (i < _bufferSize){
		return _first->getData()[i];
	}
	else{
		return _second->getData()[i - _bufferSize];
	}
}

void ARStream::copyTo(unsigned char* pOut, uint32_t count)
{
	uint32_t sizeInFirstBuffer = count;
	uint32_t sizeInSecondBuffer = 0;
	if (count + _offset > _bufferSize){
		sizeInFirstBuffer = _bufferSize - _offset;
		sizeInSecondBuffer = count - sizeInFirstBuffer;
	}
	memcpy_s(pOut, count, _first->getData() + _offset, sizeInFirstBuffer);
	if (sizeInSecondBuffer > 0){

		memcpy_s(pOut + sizeInFirstBuffer, count - sizeInFirstBuffer, _second->getData(), sizeInSecondBuffer);
	}
}

Concurrency::task<void> ARStream::ConsumeDataAsync(ARBuffer* arbuffer, uint32_t alreadyConsumed, cancellation_token cancelToken, std::shared_ptr<std::recursive_mutex> localMutex)
{
	std::lock_guard<std::recursive_mutex> lg(*localMutex);
	if (cancelToken.is_canceled()){
		return task<void>([]{return; });
	}
	if (!_isFirstFilled){
		auto buffer = _bufferPool.Acquire();
		return FillBufferAsync(_first.get(), buffer, 0, cancelToken, localMutex)
			.then([this, buffer, arbuffer, alreadyConsumed, cancelToken, localMutex]{
				std::lock_guard<std::recursive_mutex> lg(*localMutex);
				if (cancelToken.is_canceled()){
					return create_task([]{return; });
				}
				_isFirstFilled = true;
				_bufferPool.Release(buffer);
				return ConsumeDataAsync(arbuffer, alreadyConsumed, cancelToken, localMutex);
		}, cancelToken);
	}
	if (_offset >= _bufferSize)
	{
		advance1Buffer();
		return ConsumeDataAsync(arbuffer, alreadyConsumed, cancelToken, localMutex);
	}
	uint32_t toConsume = min(arbuffer->getSize() - alreadyConsumed, _bufferSize - _offset);
	memcpy_s(arbuffer->getData() + alreadyConsumed, arbuffer->getSize() - alreadyConsumed, _first->getData() + _offset, toConsume);
	_offset += toConsume;
	alreadyConsumed += toConsume;
	if (alreadyConsumed == arbuffer->getSize()){
		return create_task([](){});
	}
	else{
		return ConsumeDataAsync(arbuffer, alreadyConsumed, cancelToken, localMutex);
	}
}

void ARStream::ConsumeDataSync(ARBuffer* arbuffer, uint32_t alreadyConsumed)
{
	if (_offset >= _bufferSize)
	{
		advance1Buffer();
	}
	if (!_isFirstFilled){
		auto buffer = _bufferPool.Acquire();
		IBuffer^ buffer2 = create_task(_inputStream->ReadAsync(buffer, _bufferSize, InputStreamOptions::None)).get();
		assert(buffer2->Length == _bufferSize);
		auto bufferByteAccess = GetBufferByteAccess(buffer2);
		byte* data;
		bufferByteAccess->Buffer(&data);
		memcpy_s(_first->getData(), _first->getSize(), data, buffer2->Length);
		_isFirstFilled = true;
		_bufferPool.Release(buffer);

	}

	uint32_t toConsume = min(arbuffer->getSize() - alreadyConsumed, _bufferSize - _offset);
	memcpy_s(arbuffer->getData() + alreadyConsumed, arbuffer->getSize() - alreadyConsumed, _first->getData() + _offset, toConsume);
	_offset += toConsume;
	alreadyConsumed += toConsume;
	if (alreadyConsumed == arbuffer->getSize()){
		return;
	}
	else{
		ConsumeDataSync(arbuffer, alreadyConsumed);
	}
}


concurrency::task<void> ARStream::FillBufferAsync(ARBuffer* buffer, Windows::Storage::Streams::IBuffer^ tmpBuffer, uint32_t alreadyRead, cancellation_token cancelToken, std::shared_ptr<std::recursive_mutex> localMutex, int numberOfRead0){
	uint32_t toRead = _bufferSize - alreadyRead;
	try{
		return create_task(_inputStream->ReadAsync(tmpBuffer, toRead, Windows::Storage::Streams::InputStreamOptions::None)).then([this, buffer, tmpBuffer, alreadyRead, cancelToken, localMutex, numberOfRead0]
			(concurrency::task<Windows::Storage::Streams::IBuffer^> returnedBufferTask){
				try{
					Windows::Storage::Streams::IBuffer ^ returnedBuffer = returnedBufferTask.get();
					std::lock_guard<std::recursive_mutex> lg(*localMutex);
					if (cancelToken.is_canceled()){
						return concurrency::create_task([](){});
					}
					auto bufferByteAccess = GetBufferByteAccess(returnedBuffer);
					byte* data;
					bufferByteAccess->Buffer(&data);
					memcpy_s(buffer->getData() + alreadyRead, buffer->getSize() - alreadyRead, data, returnedBuffer->Length);
					auto totalRead = alreadyRead + returnedBuffer->Length;
					int newNumberOfRead0 = numberOfRead0;
					if (returnedBuffer->Length == 0){
						newNumberOfRead0++;
						if (newNumberOfRead0 > 10){

							throw nomoredata_in_stream();
						}
					}
					else{
						newNumberOfRead0 = 0;
					}
					if (totalRead == _bufferSize){
						return concurrency::create_task([](){});
					}
					else {
						return FillBufferAsync(buffer, tmpBuffer, totalRead, cancelToken, localMutex, newNumberOfRead0);
					}
				}
				catch (Platform::ObjectDisposedException^){
					throw nomoredata_in_stream();
				}
		});
	}
	catch (Platform::ObjectDisposedException^){
		return create_task([](){
			throw nomoredata_in_stream();
		});
	}
}