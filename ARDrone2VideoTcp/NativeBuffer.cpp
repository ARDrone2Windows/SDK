#include "pch.h"
#include "NativeBuffer.h"

using namespace Microsoft::WRL;

NativeBufferPool::NativeBufferPool( UINT32 bufferCapacity )
	: _bufferCapacity(bufferCapacity), _buffers()
{

}

Windows::Storage::Streams::IBuffer^ NativeBufferPool::Acquire()
{
	if(_buffers.size() == 0){
		ComPtr<NativeBuffer> nativeBuffer;
		MakeAndInitialize<NativeBuffer>(&nativeBuffer, _bufferCapacity);
		auto iinspectable = (IInspectable*)reinterpret_cast<IInspectable*>(nativeBuffer.Get());
		return reinterpret_cast<Windows::Storage::Streams::IBuffer^>(iinspectable);
	}
	auto buffer = _buffers.at(_buffers.size()-1);
	_buffers.erase(_buffers.end()-1);
	return buffer;
}

void NativeBufferPool::Release( Windows::Storage::Streams::IBuffer^ buffer )
{
	buffer->Length = 0;
	_buffers.push_back(buffer);
}
