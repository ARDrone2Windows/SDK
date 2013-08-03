#pragma once

#include <memory>
#include <functional>
#include <list>

#include "ARDroneMediaSource.h"
#include "..\ARDrone2VideoTcp\ARDroneTcpClient.h"

const DWORD c_dwARDroneStreamId = 1;

class CFrameGenerator;

class CARDroneMediaStream 
    : public IMFMediaStream
{
public:
    typedef std::function<void()> ARDroneMediaStreamStart;

    static HRESULT CreateInstance(CARDroneMediaSource *pSource, LPCWSTR hostname, CARDroneMediaStream **ppStream);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID iid, void** ppv);
    IFACEMETHOD_ (ULONG, AddRef) ();
    IFACEMETHOD_ (ULONG, Release) ();

    // IMFMediaEventGenerator
    IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback* pCallback,IUnknown* punkState);
    IFACEMETHOD (EndGetEvent) (IMFAsyncResult* pResult, IMFMediaEvent** ppEvent);
    IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent** ppEvent);
    IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue);

    // IMFMediaStream
    IFACEMETHOD (GetMediaSource) (IMFMediaSource** ppMediaSource);
    IFACEMETHOD (GetStreamDescriptor) (IMFStreamDescriptor** ppStreamDescriptor);
    IFACEMETHOD (RequestSample) (IUnknown* pToken);

    // Other public methods
    HRESULT Start();
    HRESULT Stop();
    HRESULT SetRate(float flRate);
    HRESULT Shutdown();
    HRESULT SetDXGIDeviceManager(IMFDXGIDeviceManager *pManager);
    HRESULT CreateSystemMemoryBuffer(BYTE *pSrc, DWORD cbData, IMFMediaBuffer **ppBuffer);
    HRESULT CreateImage(std::shared_ptr<VideoPacket>& videoPacket, IMFSample **ppSample);
    HRESULT HandleError(HRESULT hErrorCode);
    void OnTcpClientVideoPacketsFilled();

protected:
    CARDroneMediaStream();
    CARDroneMediaStream(CARDroneMediaSource *pSource, LPCWSTR hostname);
    ~CARDroneMediaStream(void);

private:
    class CSourceLock;

private:
	int _pendingSampleRequests;
	std::list<Microsoft::WRL::ComPtr<IMFSample>> _sampleBuffer;
    HRESULT Initialize();
    HRESULT CreateMediaType(IMFMediaType **ppMediaType);
    HRESULT DeliverSample(IUnknown *pToken);
    //HRESULT CreateImage(GeometricShape eShape, IMFSample **spSample);
    HRESULT CreateVideoSampleAllocator();

    HRESULT CheckShutdown() const
    {
        if (_eSourceState == SourceState_Shutdown)
        {
            return MF_E_SHUTDOWN;
        }
        else
        {
            return S_OK;
        }
    }

private:
    long                              _cRef;                      // reference count
    SourceState                       _eSourceState;              // Flag to indicate if Shutdown() method was called.
    ComPtr<CARDroneMediaSource>       _spSource;
    ComPtr<IMFMediaEventQueue>        _spEventQueue;              // Event queue
    ComPtr<IMFStreamDescriptor>       _spStreamDescriptor;        // Stream descriptor
    ComPtr<IMFMediaBuffer>            _spPicture;
    LONGLONG                          _llCurrentTimestamp;
    ComPtr<IMFDXGIDeviceManager>       _spDeviceManager;
    LPCWSTR						       _hostname;
    ComPtr<IMFMediaType>              _spMediaType;
    ComPtr<IMFVideoSampleAllocatorEx> _spAllocEx;
    CFrameGenerator*                  _pFrameGenerator;
    float                             _flRate;
    std::unique_ptr<ARDroneTcpClient> _tcpClient;
};

