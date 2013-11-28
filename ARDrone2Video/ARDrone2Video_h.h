

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0595 */
/* at Wed Nov 27 01:22:14 2013
 */
/* Compiler settings for C:\Users\arfontai\AppData\Local\Temp\ARDrone2Video.idl-532fad83:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0595 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __ARDrone2Video_h_h__
#define __ARDrone2Video_h_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

#if defined(__cplusplus)
#if defined(__MIDL_USE_C_ENUM)
#define MIDL_ENUM enum
#else
#define MIDL_ENUM enum class
#endif
#endif


/* Forward Declarations */ 

#ifndef ____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_FWD_DEFINED__
#define ____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_FWD_DEFINED__
typedef interface __x_ABI_CARDrone2Video_CIARDroneSchemeHandler __x_ABI_CARDrone2Video_CIARDroneSchemeHandler;

#ifdef __cplusplus
namespace ABI {
    namespace ARDrone2Video {
        interface IARDroneSchemeHandler;
    } /* end namespace */
} /* end namespace */

#endif /* __cplusplus */

#endif 	/* ____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_FWD_DEFINED__ */


#ifndef ____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_FWD_DEFINED__
#define ____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_FWD_DEFINED__
typedef interface __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager;

#ifdef __cplusplus
namespace ABI {
    namespace ARDrone2Video {
        interface IARDroneMFExtensionsManager;
    } /* end namespace */
} /* end namespace */

#endif /* __cplusplus */

#endif 	/* ____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_FWD_DEFINED__ */


/* header files for imported files */
#include "Windows.Media.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ARDrone2Video_0000_0000 */
/* [local] */ 

#pragma warning(push)
#pragma warning(disable:4001) 
#pragma once
#pragma warning(pop)
#ifdef __cplusplus
namespace ABI {
namespace ARDrone2Video {
class ARDroneSchemeHandler;
} /*ARDrone2Video*/
}
#endif

#if !defined(____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_INTERFACE_DEFINED__)
extern const __declspec(selectany) WCHAR InterfaceName_ARDrone2Video_IARDroneSchemeHandler[] = L"ARDrone2Video.IARDroneSchemeHandler";
#endif /* !defined(____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_INTERFACE_DEFINED__) */


/* interface __MIDL_itf_ARDrone2Video_0000_0000 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_ARDrone2Video_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ARDrone2Video_0000_0000_v0_0_s_ifspec;

#ifndef ____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_INTERFACE_DEFINED__
#define ____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_INTERFACE_DEFINED__

/* interface __x_ABI_CARDrone2Video_CIARDroneSchemeHandler */
/* [uuid][object] */ 



/* interface ABI::ARDrone2Video::IARDroneSchemeHandler */
/* [uuid][object] */ 


EXTERN_C const IID IID___x_ABI_CARDrone2Video_CIARDroneSchemeHandler;

#if defined(__cplusplus) && !defined(CINTERFACE)
    } /* end extern "C" */
    namespace ABI {
        namespace ARDrone2Video {
            
            MIDL_INTERFACE("eb16d197-e1b2-4f41-a660-33d4a7a7b11c")
            IARDroneSchemeHandler : public IInspectable
            {
            public:
            };

            extern const __declspec(selectany) IID & IID_IARDroneSchemeHandler = __uuidof(IARDroneSchemeHandler);

            
        }  /* end namespace */
    }  /* end namespace */
    extern "C" { 
    
#else 	/* C style interface */

    typedef struct __x_ABI_CARDrone2Video_CIARDroneSchemeHandlerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __x_ABI_CARDrone2Video_CIARDroneSchemeHandler * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __x_ABI_CARDrone2Video_CIARDroneSchemeHandler * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __x_ABI_CARDrone2Video_CIARDroneSchemeHandler * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetIids )( 
            __x_ABI_CARDrone2Video_CIARDroneSchemeHandler * This,
            /* [out] */ ULONG *iidCount,
            /* [size_is][size_is][out] */ IID **iids);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeClassName )( 
            __x_ABI_CARDrone2Video_CIARDroneSchemeHandler * This,
            /* [out] */ HSTRING *className);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrustLevel )( 
            __x_ABI_CARDrone2Video_CIARDroneSchemeHandler * This,
            /* [out] */ TrustLevel *trustLevel);
        
        END_INTERFACE
    } __x_ABI_CARDrone2Video_CIARDroneSchemeHandlerVtbl;

    interface __x_ABI_CARDrone2Video_CIARDroneSchemeHandler
    {
        CONST_VTBL struct __x_ABI_CARDrone2Video_CIARDroneSchemeHandlerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_GetIids(This,iidCount,iids)	\
    ( (This)->lpVtbl -> GetIids(This,iidCount,iids) ) 

#define __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_GetRuntimeClassName(This,className)	\
    ( (This)->lpVtbl -> GetRuntimeClassName(This,className) ) 

#define __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_GetTrustLevel(This,trustLevel)	\
    ( (This)->lpVtbl -> GetTrustLevel(This,trustLevel) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ____x_ABI_CARDrone2Video_CIARDroneSchemeHandler_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_ARDrone2Video_0000_0001 */
/* [local] */ 

#ifndef RUNTIMECLASS_ARDrone2Video_ARDroneSchemeHandler_DEFINED
#define RUNTIMECLASS_ARDrone2Video_ARDroneSchemeHandler_DEFINED
extern const __declspec(selectany) WCHAR RuntimeClass_ARDrone2Video_ARDroneSchemeHandler[] = L"ARDrone2Video.ARDroneSchemeHandler";
#endif

#ifdef __cplusplus
namespace ABI {
namespace ARDrone2Video {
class ARDroneMFExtensionsManager;
} /*ARDrone2Video*/
}
#endif
#if !defined(____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_INTERFACE_DEFINED__)
extern const __declspec(selectany) WCHAR InterfaceName_ARDrone2Video_IARDroneMFExtensionsManager[] = L"ARDrone2Video.IARDroneMFExtensionsManager";
#endif /* !defined(____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_INTERFACE_DEFINED__) */


/* interface __MIDL_itf_ARDrone2Video_0000_0001 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_ARDrone2Video_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ARDrone2Video_0000_0001_v0_0_s_ifspec;

#ifndef ____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_INTERFACE_DEFINED__
#define ____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_INTERFACE_DEFINED__

/* interface __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager */
/* [uuid][object] */ 



/* interface ABI::ARDrone2Video::IARDroneMFExtensionsManager */
/* [uuid][object] */ 


EXTERN_C const IID IID___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    } /* end extern "C" */
    namespace ABI {
        namespace ARDrone2Video {
            
            MIDL_INTERFACE("eb16d197-e1b2-4f41-a660-33d4a7a7b11d")
            IARDroneMFExtensionsManager : public IInspectable
            {
            public:
                virtual HRESULT STDMETHODCALLTYPE RegisterMFExtensions( void) = 0;
                
            };

            extern const __declspec(selectany) IID & IID_IARDroneMFExtensionsManager = __uuidof(IARDroneMFExtensionsManager);

            
        }  /* end namespace */
    }  /* end namespace */
    extern "C" { 
    
#else 	/* C style interface */

    typedef struct __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetIids )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This,
            /* [out] */ ULONG *iidCount,
            /* [size_is][size_is][out] */ IID **iids);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeClassName )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This,
            /* [out] */ HSTRING *className);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrustLevel )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This,
            /* [out] */ TrustLevel *trustLevel);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterMFExtensions )( 
            __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager * This);
        
        END_INTERFACE
    } __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerVtbl;

    interface __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager
    {
        CONST_VTBL struct __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_GetIids(This,iidCount,iids)	\
    ( (This)->lpVtbl -> GetIids(This,iidCount,iids) ) 

#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_GetRuntimeClassName(This,className)	\
    ( (This)->lpVtbl -> GetRuntimeClassName(This,className) ) 

#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_GetTrustLevel(This,trustLevel)	\
    ( (This)->lpVtbl -> GetTrustLevel(This,trustLevel) ) 


#define __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_RegisterMFExtensions(This)	\
    ( (This)->lpVtbl -> RegisterMFExtensions(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ____x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_ARDrone2Video_0000_0002 */
/* [local] */ 

#ifndef RUNTIMECLASS_ARDrone2Video_ARDroneMFExtensionsManager_DEFINED
#define RUNTIMECLASS_ARDrone2Video_ARDroneMFExtensionsManager_DEFINED
extern const __declspec(selectany) WCHAR RuntimeClass_ARDrone2Video_ARDroneMFExtensionsManager[] = L"ARDrone2Video.ARDroneMFExtensionsManager";
#endif


/* interface __MIDL_itf_ARDrone2Video_0000_0002 */
/* [local] */ 



extern RPC_IF_HANDLE __MIDL_itf_ARDrone2Video_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ARDrone2Video_0000_0002_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


