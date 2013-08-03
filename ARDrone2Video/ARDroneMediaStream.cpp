#include "StdAfx.h"
#include "ARDroneMediaStream.h"
#include "..\ARDrone2VideoTcp\ARDroneTcpClient.h"

#include <initguid.h>
#include <math.h>
#include <stdlib.h>
#include <wrl\module.h>
#include <sstream>

namespace
{
	const DWORD c_dwOutputImageHeight = 360;
	const DWORD c_dwOutputImageWidth = 640;
	const DWORD c_cbOutputSamplenNumPixels = c_dwOutputImageWidth * c_dwOutputImageHeight;
	const DWORD c_cbOutputSampleSize = c_cbOutputSamplenNumPixels * 4;
	const DWORD c_dwOutputFrameRateNumerator = 10;
	const DWORD c_dwOutputFrameRateDenominator = 1;
	//const LONGLONG c_llOutputFrameDuration = 1000000ll;
}

class CARDroneMediaStream::CSourceLock
{
public:
	_Acquires_lock_(_spSource)
		CSourceLock(CARDroneMediaSource *pSource)
		: _spSource(pSource)
	{
		if (_spSource)
		{
			_spSource->Lock();
		}
	}

	_Releases_lock_(_spSource)
		~CSourceLock()
	{
		if (_spSource)
		{
			_spSource->Unlock();
		}
	}

private:
	ComPtr<CARDroneMediaSource> _spSource;
};

BYTE Clip(int i)
{
	return (i > 255 ? 255 : (i < 0 ? 0 : i));
}

DWORD YUVToRGB( BYTE Y, BYTE U, BYTE V)
{
	int C = INT(Y) - 16;
	int D = INT(U) - 128;
	int E = INT(V) - 128;

	INT R = ( 298 * C           + 409 * E + 128) >> 8;
	INT G = ( 298 * C - 100 * D - 208 * E + 128) >> 8;
	INT B = ( 298 * C + 516 * D           + 128) >> 8;

	DWORD ret = 0xff000000;
	BYTE *bCols = reinterpret_cast<BYTE*>(&ret);

	bCols[2] = Clip(R);
	bCols[1] = Clip(G);
	bCols[0] = Clip(B);

	return ret;
}

class CFrameGenerator
{
public:
	CFrameGenerator() { }
	virtual ~CFrameGenerator() { }

	HRESULT PrepareFrame(BYTE *pBuf, LONGLONG llTimestamp, LONG lPitch)
	{
		ZeroMemory(pBuf, lPitch * c_dwOutputImageHeight);

		//sin((llTimestamp/100000.0)*3.1415);
		return DrawFrame(pBuf, YUVToRGB(128, 128 + BYTE(127 * sin(llTimestamp/10000000.0)), 128 + BYTE(127 * cos(llTimestamp/3300000.0))), lPitch);
	}

protected:
	virtual HRESULT DrawFrame(BYTE *pBuf, DWORD dwColor, LONG lPitch) = 0;

	HRESULT SetColor(_In_reads_bytes_(cElements) DWORD *pBuf, DWORD dwColor, DWORD cElements)
	{
		for(DWORD nIndex = 0; nIndex < cElements; ++nIndex)
		{
			pBuf[nIndex] = dwColor;
		}
		return S_OK;
	}

private:
	CFrameGenerator(const CFrameGenerator& nonCopyable);
	CFrameGenerator& operator=(const CFrameGenerator& nonCopyable);
};

HRESULT CARDroneMediaStream::CreateInstance(CARDroneMediaSource *pSource, LPCWSTR hostname, CARDroneMediaStream **ppStream)
{
	if (pSource == nullptr || ppStream == nullptr)
	{
		return E_INVALIDARG;
	}

	HRESULT hr = S_OK;
	ComPtr<CARDroneMediaStream> spStream;
	spStream.Attach(new(std::nothrow) CARDroneMediaStream(pSource, hostname));
	if (!spStream)
	{
		hr = E_OUTOFMEMORY;
	}

	if (SUCCEEDED(hr))
	{
		hr = spStream->Initialize();
	}

	if (SUCCEEDED(hr))
	{
		(*ppStream) = spStream.Detach();
	}

	return hr;
}

CARDroneMediaStream::CARDroneMediaStream(CARDroneMediaSource *pSource, LPCWSTR hostname)
	: _cRef(1)
	, _spSource(pSource)
	, _eSourceState(SourceState_Invalid)
	, _hostname(hostname)
	, _flRate(1.0f), _pendingSampleRequests(0)
	, _tcpClient(new ARDroneTcpClient(hostname))
{
	auto module = ::Microsoft::WRL::GetModuleBase();
	if (module != nullptr)
	{
		module->IncrementObjectCount();
	}
}


CARDroneMediaStream::~CARDroneMediaStream(void)
{
	auto module = ::Microsoft::WRL::GetModuleBase();
	if (module != nullptr)
	{
		module->DecrementObjectCount();
	}
}

// IUnknown methods

IFACEMETHODIMP CARDroneMediaStream::QueryInterface(REFIID riid, void** ppv)
{
	if (ppv == nullptr)
	{
		return E_POINTER;
	}
	HRESULT hr = E_NOINTERFACE;
	(*ppv) = nullptr;
	if (riid == IID_IUnknown || 
		riid == IID_IMFMediaEventGenerator ||
		riid == IID_IMFMediaStream)
	{
		(*ppv) = static_cast<IMFMediaStream *>(this);
		AddRef();
		hr = S_OK;
	}

	return hr;
}

IFACEMETHODIMP_(ULONG) CARDroneMediaStream::AddRef()
{
	return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CARDroneMediaStream::Release()
{
	long cRef = InterlockedDecrement(&_cRef);
	if (cRef == 0)
	{
		delete this;
	}
	return cRef;
}

// IMFMediaEventGenerator methods.
// Note: These methods call through to the event queue helper object.

IFACEMETHODIMP CARDroneMediaStream::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
{
	HRESULT hr = S_OK;

	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
	}

	return hr;
}

IFACEMETHODIMP CARDroneMediaStream::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
	HRESULT hr = S_OK;

	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
	}

	return hr;
}

IFACEMETHODIMP CARDroneMediaStream::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
	// NOTE:
	// GetEvent can block indefinitely, so we don't hold the lock.
	// This requires some juggling with the event queue pointer.

	HRESULT hr = S_OK;

	ComPtr<IMFMediaEventQueue> spQueue;

	{
		CSourceLock lock(_spSource.Get());

		// Check shutdown
		hr = CheckShutdown();

		// Get the pointer to the event queue.
		if (SUCCEEDED(hr))
		{
			spQueue = _spEventQueue;
		}
	}

	// Now get the event.
	if (SUCCEEDED(hr))
	{
		hr = spQueue->GetEvent(dwFlags, ppEvent);
	}

	return hr;
}

IFACEMETHODIMP CARDroneMediaStream::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
{
	HRESULT hr = S_OK;

	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
	}

	return hr;
}

// IMFMediaStream methods

IFACEMETHODIMP CARDroneMediaStream::GetMediaSource(IMFMediaSource** ppMediaSource)
{
	if (ppMediaSource == nullptr)
	{
		return E_INVALIDARG;
	}

	HRESULT hr = S_OK;

	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		*ppMediaSource = _spSource.Get();
		(*ppMediaSource)->AddRef();
	}

	return hr;
}

IFACEMETHODIMP CARDroneMediaStream::GetStreamDescriptor(IMFStreamDescriptor** ppStreamDescriptor)
{
	if (ppStreamDescriptor == nullptr)
	{
		return E_INVALIDARG;
	}

	HRESULT hr = S_OK;

	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		*ppStreamDescriptor = _spStreamDescriptor.Get();
		(*ppStreamDescriptor)->AddRef();
	}

	return hr;
}

IFACEMETHODIMP CARDroneMediaStream::RequestSample(IUnknown* pToken)
{
	HRESULT hr = S_OK;

	CSourceLock lock(_spSource.Get());
	hr = CheckShutdown();

	if (SUCCEEDED(hr) && _eSourceState != SourceState_Started)
	{
		// We cannot be asked for a sample unless we are in started state
		hr = MF_E_INVALIDREQUEST;
	}

	if (SUCCEEDED(hr))
	{
		// Trigger sample delivery
		hr = DeliverSample(pToken);
	}

	if (FAILED(hr))
	{
		hr = HandleError(hr);
	}

	return hr;
}


concurrency::task<void> DoAsyncWhile(std::function<concurrency::task<void>(void)> todo, std::function<bool(void)> toCkeck ){
	if(toCkeck()){
		return todo().then([todo, toCkeck]()
		{ 
			return DoAsyncWhile(todo, toCkeck);
		});
	}
	else {
		return concurrency::create_task([](){});
	}
}

HRESULT CARDroneMediaStream::Start()
{
	HRESULT hr = S_OK;
	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();
	if (_eSourceState == SourceState_Started){
		HRESULT hr = QueueEvent(MEStreamStarted, GUID_NULL, S_OK, nullptr);

		return S_OK;
	}
	if (SUCCEEDED(hr))
	{
		if (_eSourceState == SourceState_Stopped ||
			_eSourceState == SourceState_Started)
		{
			if (SUCCEEDED(hr))
			{
				if (_eSourceState == SourceState_Stopped)
				{
					_llCurrentTimestamp = 0;
				}
				_eSourceState = SourceState_Started;

				_tcpClient->Start([this](bool succeeded) -> void
				{
					if(succeeded){
						// Inform the client that we've started
						HRESULT hr = QueueEvent(MEStreamStarted, GUID_NULL, S_OK, nullptr);


						if (FAILED(hr))
						{
							hr = HandleError(hr);
						}

						DoAsyncWhile([this](){
							return _tcpClient->PopVideoPacketAsync().then([this](const std::shared_ptr<VideoPacket>& videoPacket){

								ComPtr<IMFSample> spSample;
								ComPtr<IMFMediaBuffer> spOutputBuffer;
								if (videoPacket == nullptr){
									return;
								}
								HRESULT hr = CreateSystemMemoryBuffer(videoPacket->Buffer.getData(), videoPacket->Buffer.getSize(), &spOutputBuffer);

								if (SUCCEEDED(hr))
								{
									hr = MFCreateSample(&spSample);
								}

								if (SUCCEEDED(hr))
								{
									hr = spSample->AddBuffer(spOutputBuffer.Get());
								}
								LONGLONG nsTime = videoPacket->Timestamp;
								nsTime*=10000;
								if (SUCCEEDED(hr))
								{
									hr = spSample->SetSampleTime(nsTime);
								}
								LONGLONG nsDuration = videoPacket->Duration;
								nsDuration *= 10000;
								if (SUCCEEDED(hr))
								{
									//hr = spSample->SetSampleDuration(nsDuration);
								}
								if(SUCCEEDED(hr)){
									hr = spSample->SetUINT32(MFSampleExtension_CleanPoint, videoPacket->FrameTypev == FrameType::I ? 1: 0);
									//hr = spSample->SetUINT32(MFSampleExtension_CleanPoint, 1);
								}

								//if (SUCCEEDED(hr) && pToken != nullptr)
								//{
								//	// If token was not null set the sample attribute.
								//	hr = spSample->SetUnknown(MFSampleExtension_Token, pToken);
								//}

								if (SUCCEEDED(hr))
								{
									//hr = _spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, spSample.Get());
									//OutputDebugStringW(L"Frame submitted\n");
									CSourceLock lock(_spSource.Get());
									if (_eSourceState != SourceState_Started){
										return;
									}
									if(_pendingSampleRequests>0){
										hr = _spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, spSample.Get());
										_pendingSampleRequests--;
									}else{
										_sampleBuffer.push_back(spSample);
										/*while (_sampleBuffer.size() > 10){
											_sampleBuffer.erase(_sampleBuffer.begin());
										}*/
									}
								}

							})
								.then([this](concurrency::task<void> t){
									try{
										t.get();
									}
									catch (Platform::Exception^){

										Stop();
									}
							});
						}, [this](){return _eSourceState == SourceState_Started;});
					} else {

						Stop();
					}
				});

			}
		}
		else
		{
			hr = MF_E_INVALID_STATE_TRANSITION;    
		}
	}

	if (FAILED(hr))
	{
		hr = HandleError(hr);
	}

	return hr;
}

HRESULT CARDroneMediaStream::Stop()
{
	HRESULT hr = S_OK;
	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		if (_eSourceState == SourceState_Started)
		{
			_eSourceState = SourceState_Stopped;
			this->_tcpClient->Stop();
			// Inform the client that we've stopped.
			hr = QueueEvent(MEStreamStopped, GUID_NULL, S_OK, nullptr);
		}
		else
		{
			hr = MF_E_INVALID_STATE_TRANSITION;    
		}
	}

	if (FAILED(hr))
	{
		hr = HandleError(hr);
	}

	return hr;
}

HRESULT CARDroneMediaStream::SetRate(float flRate)
{
	HRESULT hr = S_OK;
	CSourceLock lock(_spSource.Get());

	hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		_flRate = flRate;
	}

	if (FAILED(hr))
	{
		hr = HandleError(hr);
	}

	return hr;
}

HRESULT CARDroneMediaStream::Shutdown()
{
	CSourceLock lock(_spSource.Get());

	HRESULT hr = CheckShutdown();

	if (SUCCEEDED(hr))
	{
		if (_spEventQueue)
		{
			_spEventQueue->Shutdown();
			_spEventQueue.ReleaseAndGetAddressOf();
		}

		if (_spAllocEx != nullptr)
		{
			_spAllocEx->UninitializeSampleAllocator();
		}

		_spStreamDescriptor.ReleaseAndGetAddressOf();
		_spDeviceManager.ReleaseAndGetAddressOf();
		_eSourceState = SourceState_Shutdown;

		_pFrameGenerator = nullptr;
	}

	return hr;
}

HRESULT CARDroneMediaStream::SetDXGIDeviceManager(IMFDXGIDeviceManager *pManager)
{
	HRESULT hr = S_OK;

	/*_spDeviceManager = pManager;

	if (_spDeviceManager)
	{
	hr = CreateVideoSampleAllocator();
	}*/

	return hr;
}

HRESULT CARDroneMediaStream::Initialize()
{
	// Create the media event queue.
	HRESULT hr = MFCreateEventQueue(&_spEventQueue);
	ComPtr<IMFStreamDescriptor> spSD;
	ComPtr<IMFMediaTypeHandler> spMediaTypeHandler;

	if (SUCCEEDED(hr))
	{
		// Create a media type object.
		hr = CreateMediaType(&_spMediaType);
	}

	if (SUCCEEDED(hr))
	{
		// Now we can create MF stream descriptor.
		hr = MFCreateStreamDescriptor(c_dwARDroneStreamId, 1, _spMediaType.GetAddressOf(), &spSD);
	}

	if (SUCCEEDED(hr))
	{
		hr = spSD->GetMediaTypeHandler(&spMediaTypeHandler);
	}

	if (SUCCEEDED(hr))
	{
		// Set current media type
		hr = spMediaTypeHandler->SetCurrentMediaType(_spMediaType.Get());
	}

	if (SUCCEEDED(hr))
	{
		_spStreamDescriptor = spSD;
		// State of the stream is started.
		_eSourceState = SourceState_Stopped;
	}

	if (SUCCEEDED(hr))
	{
		// hr = CreateFrameGenerator(_eShape, &_pFrameGenerator);
	}

	return hr;
}

HRESULT CARDroneMediaStream::CreateMediaType(IMFMediaType **ppMediaType)
{
	ComPtr<IMFMediaType> spOutputType;
	HRESULT hr = MFCreateMediaType(&spOutputType);

	if (SUCCEEDED(hr))
	{
		hr = spOutputType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
	}

	if (SUCCEEDED(hr))
	{
		hr = spOutputType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_H264_ES);
	}

	if (SUCCEEDED(hr))
	{
		hr = spOutputType->SetUINT32(MF_MT_FIXED_SIZE_SAMPLES, FALSE);
	}

	if (SUCCEEDED(hr))
	{
		hr = spOutputType->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, FALSE);
	}

	if (SUCCEEDED(hr))
	{
		//hr = spOutputType->SetUINT32(MF_MT_SAMPLE_SIZE, c_cbOutputSampleSize);
	}

	if (SUCCEEDED(hr))
	{
		//hr = MFSetAttributeSize(spOutputType.Get(), MF_MT_FRAME_SIZE, c_dwOutputImageWidth, c_dwOutputImageHeight);
	}

	if (SUCCEEDED(hr))
	{
		//hr = MFSetAttributeRatio(spOutputType.Get(), MF_MT_FRAME_RATE, c_dwOutputFrameRateNumerator, c_dwOutputFrameRateDenominator);
	}


	if (SUCCEEDED(hr))
	{
		hr = spOutputType->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_MixedInterlaceOrProgressive);
	}

	if (SUCCEEDED(hr))
	{
		hr = MFSetAttributeRatio(spOutputType.Get(), MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
	}

	if (SUCCEEDED(hr))
	{
		*ppMediaType = spOutputType.Detach();
	}

	return hr;
}

HRESULT CARDroneMediaStream::DeliverSample(IUnknown *pToken)
{

	HRESULT hr = S_OK;

	/*if (!_tcpClient->PopVideoPacket(&videoPacket))
	{
	return S_QUEUEEMPTY;
	}*/



	{
	CSourceLock lock(_spSource.Get());
	if(!_sampleBuffer.empty()){
	ComPtr<IMFSample> sample = *_sampleBuffer.begin();
	_sampleBuffer.pop_front();

	hr = _spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, sample.Get());
	} else {
	_pendingSampleRequests++;
	return S_QUEUEEMPTY;
	}

	}

	

	return hr;
}

HRESULT CARDroneMediaStream::CreateSystemMemoryBuffer(BYTE *pSrc, DWORD cbData, IMFMediaBuffer **ppBuffer)
{
	HRESULT hr = S_OK;
	BYTE *pData = NULL;

	IMFMediaBuffer *pBuffer = NULL;

	// Create the media buffer.
	//MFCreateVideoSampleAllocatorEx()
	hr = MFCreateAlignedMemoryBuffer(cbData, MF_128_BYTE_ALIGNMENT, &pBuffer);
	//hr = MFCreateMemoryBuffer(
	//	cbData,   // Amount of memory to allocate, in bytes.
	//	&pBuffer        
	//	);

	// Lock the buffer to get a pointer to the memory.
	if (SUCCEEDED(hr))
	{
		hr = pBuffer->Lock(&pData, NULL, NULL);
	}

	if (SUCCEEDED(hr))
	{
		memcpy_s(pData, cbData, pSrc, cbData);
	}

	// Update the current length.
	if (SUCCEEDED(hr))
	{
		hr = pBuffer->SetCurrentLength(cbData);
	}

	// Unlock the buffer.
	if (pData)
	{
		hr = pBuffer->Unlock();
	}

	if (SUCCEEDED(hr))
	{
		*ppBuffer = pBuffer;
		(*ppBuffer)->AddRef();
	}

	return hr;
}


HRESULT CARDroneMediaStream::CreateImage(std::shared_ptr<VideoPacket>& videoPacket, IMFSample **ppSample)
{
	ComPtr<IMFMediaBuffer> spOutputBuffer;
	ComPtr<IMF2DBuffer2> sp2DBuffer;
	ComPtr<IMFSample> spSample;
	ComPtr<ID3D11Texture2D> spTexture2D;
	BYTE *pBuf = nullptr;
	LONG pitch = c_dwOutputImageWidth * sizeof(DWORD);

	HRESULT hr = S_OK;

	if (_pFrameGenerator == nullptr)
	{
		hr = E_UNEXPECTED;
	}

	if (SUCCEEDED(hr))
	{
		if (_spDeviceManager)
		{
			hr = _spAllocEx->AllocateSample(&spSample);

			if (SUCCEEDED(hr))
			{
				hr = spSample->GetBufferByIndex(0, &spOutputBuffer);
			}
		}
		else
		{
			hr = MFCreateMemoryBuffer(c_cbOutputSampleSize, &spOutputBuffer);

			if (SUCCEEDED(hr))
			{
				hr = MFCreateSample(&spSample);
			}

			if (SUCCEEDED(hr))
			{
				hr = spSample->AddBuffer(spOutputBuffer.Get());
			}
		}
	}

	if (SUCCEEDED(hr))
	{
		spOutputBuffer.As(&sp2DBuffer);
	}

	if (SUCCEEDED(hr))
	{
		BYTE* pBufferStart;
		DWORD cLen;
		if (sp2DBuffer)
		{
			hr = sp2DBuffer->Lock2DSize(MF2DBuffer_LockFlags_Write, &pBuf, &pitch, &pBufferStart, &cLen);
		}
		else
		{
			hr = spOutputBuffer->Lock(&pBuf, nullptr, nullptr);
		}
	}

	if (SUCCEEDED(hr))
	{
		hr = _pFrameGenerator->PrepareFrame(pBuf, _llCurrentTimestamp, pitch);
	}

	if (SUCCEEDED(hr) && !sp2DBuffer)
	{
		hr = spOutputBuffer->SetCurrentLength(c_cbOutputSampleSize);
	}

	if (pBuf != nullptr)
	{
		if (sp2DBuffer)
		{
			sp2DBuffer->Unlock2D();
		}
		else
		{
			spOutputBuffer->Unlock();
		}
	}

	if (SUCCEEDED(hr))
	{
		*ppSample = spSample.Detach();
	}

	return hr;
}

HRESULT CARDroneMediaStream::HandleError(HRESULT hErrorCode)
{
	if (hErrorCode != MF_E_SHUTDOWN)
	{
		// Send MEError to the client
		hErrorCode = QueueEvent(MEError, GUID_NULL, hErrorCode, nullptr);
	}

	return hErrorCode;
}

HRESULT CARDroneMediaStream::CreateVideoSampleAllocator()
{
	HRESULT hr = S_OK;
	ComPtr<IMFAttributes> spAttributes;

	hr = MFCreateAttributes(&spAttributes, 1);

	if (SUCCEEDED(hr))
	{
		spAttributes->SetUINT32( MF_SA_D3D11_BINDFLAGS, D3D11_BIND_SHADER_RESOURCE );
		hr = MFCreateVideoSampleAllocatorEx(IID_IMFVideoSampleAllocatorEx, (void**)&_spAllocEx);
	}
	if (SUCCEEDED(hr))
	{
		hr = _spAllocEx->SetDirectXManager(_spDeviceManager.Get());
	}
	if (SUCCEEDED(hr))
	{
		hr = _spAllocEx->InitializeSampleAllocatorEx(1, 8, spAttributes.Get(), _spMediaType.Get());
	}

	return hr;
}
