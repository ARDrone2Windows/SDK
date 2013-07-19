#pragma once
#include "AsyncCB.h"
#include "ARDrone2Video_h.h"

class CARDroneSchemeHandler
    : public Microsoft::WRL::RuntimeClass<
    Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
    ABI::Windows::Media::IMediaExtension,
    IMFSchemeHandler >
{
    InspectableClass(RuntimeClass_ARDrone2Video_ARDroneSchemeHandler, BaseTrust)

public:
    CARDroneSchemeHandler(void);
    ~CARDroneSchemeHandler(void);

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

    // IMFSchemeHandler
    IFACEMETHOD (BeginCreateObject) ( 
        _In_ LPCWSTR pwszURL,
        _In_ DWORD dwFlags,
        _In_ IPropertyStore *pProps,
        _Out_opt_  IUnknown **ppIUnknownCancelCookie,
        _In_ IMFAsyncCallback *pCallback,
        _In_ IUnknown *punkState);

    IFACEMETHOD (EndCreateObject) ( 
        _In_ IMFAsyncResult *pResult,
        _Out_  MF_OBJECT_TYPE *pObjectType,
        _Out_  IUnknown **ppObject);

    IFACEMETHOD (CancelObjectCreation) ( 
        _In_ IUnknown *pIUnknownCancelCookie);

private:
    HRESULT OnSourceOpen(_In_ IMFAsyncResult *pResult);

private:
    AsyncCallback<CARDroneSchemeHandler> _OnSourceOpenCB;
};

class ARDroneMFExtensionsManager : public Microsoft::WRL::RuntimeClass<ABI::ARDrone2Video::IARDroneMFExtensionsManager>{
	InspectableClass(RuntimeClass_ARDrone2Video_ARDroneMFExtensionsManager, BaseTrust);
public:
	ARDroneMFExtensionsManager(){}
	~ARDroneMFExtensionsManager(){}

	virtual HRESULT STDMETHODCALLTYPE RegisterMFExtensions( void) override{
		Windows::Media::MediaExtensionManager^ m = ref new Windows::Media::MediaExtensionManager();

			m->RegisterSchemeHandler(L"ARDrone2Video.ARDroneSchemeHandler", L"ardrone:");
			return S_OK;
		
	}
};
