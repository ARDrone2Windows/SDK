#pragma once
#include <wrl.h>
#include <wrl/implements.h>
#include <windows.storage.streams.h>
#include <robuffer.h>
#include <vector>

class NativeBuffer : public Microsoft::WRL::RuntimeClass<
	Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >,
	ABI::Windows::Storage::Streams::IBuffer,
	Windows::Storage::Streams::IBufferByteAccess >
{

public:
	virtual ~NativeBuffer()
	{
	}
	NativeBuffer() : _capacity(0), _length(0), _data(nullptr){

	}
	STDMETHODIMP RuntimeClassInitialize(UINT32 capacity)
	{
		if(_data != nullptr){
			delete _data;
		}
		_capacity = capacity;
		_data = new unsigned char[capacity];
		_length = 0;
		return S_OK;
	}

	STDMETHODIMP Buffer( byte **value)
	{
		*value = _data;
		return S_OK;
	}

	STDMETHODIMP get_Capacity(UINT32 *value)
	{
		*value = _capacity;
		return S_OK;
	}

	STDMETHODIMP get_Length(UINT32 *value)
	{
		*value = _length;
		return S_OK;
	}

	STDMETHODIMP put_Length(UINT32 value)
	{
		if(value > _capacity)
			return E_INVALIDARG;
		_length = value;
		return S_OK;
	}
private:
	UINT32 _length; 
	UINT32 _capacity;
	unsigned char* _data;
};

class NativeBufferPool{
private:
	UINT32 _bufferCapacity;
	std::vector<Windows::Storage::Streams::IBuffer^> _buffers;
public:
	NativeBufferPool(UINT32 bufferCapacity);
	Windows::Storage::Streams::IBuffer^ Acquire();
	void Release(Windows::Storage::Streams::IBuffer^ buffer);

};

inline auto AsInspectable(Platform::Object^ const object) -> Microsoft::WRL::ComPtr<IInspectable>
{
	return reinterpret_cast<IInspectable*>(object);
}

inline auto ThrowIfFailedCom(HRESULT const hr) -> void
{
	if (FAILED(hr))
		throw Platform::Exception::CreateException(hr);
}

template <typename T>
inline Microsoft::WRL::ComPtr<T> com_interface(Platform::Object^ object)
{
	Microsoft::WRL::ComPtr<IInspectable> inspectable(AsInspectable(object));
	Microsoft::WRL::ComPtr<T> iPtr;
	ThrowIfFailedCom(inspectable.As(&iPtr));
	return iPtr;
}

inline Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> GetBufferByteAccess(Windows::Storage::Streams::IBuffer^ buffer){
	return com_interface<Windows::Storage::Streams::IBufferByteAccess>(buffer);
}
