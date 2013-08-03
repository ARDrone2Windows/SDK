

/* this ALWAYS GENERATED file contains the proxy stub code */


 /* File created by MIDL compiler version 8.00.0595 */
/* at Sat Aug 03 03:05:34 2013
 */
/* Compiler settings for C:\Users\aldanvy\AppData\Local\Temp\ARDrone2Video.idl-8c65c850:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0595 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#if !defined(_M_IA64) && !defined(_M_AMD64) && !defined(_ARM_)


#pragma warning( disable: 4049 )  /* more than 64k source lines */
#if _MSC_VER >= 1200
#pragma warning(push)
#endif

#pragma warning( disable: 4211 )  /* redefine extern to static */
#pragma warning( disable: 4232 )  /* dllimport identity*/
#pragma warning( disable: 4024 )  /* array to pointer mapping*/
#pragma warning( disable: 4152 )  /* function/data pointer conversion in expression */
#pragma warning( disable: 4100 ) /* unreferenced arguments in x86 call */

#pragma optimize("", off ) 

#define USE_STUBLESS_PROXY


/* verify that the <rpcproxy.h> version is high enough to compile this file*/
#ifndef __REDQ_RPCPROXY_H_VERSION__
#define __REQUIRED_RPCPROXY_H_VERSION__ 475
#endif


#include "rpcproxy.h"
#ifndef __RPCPROXY_H_VERSION__
#error this stub requires an updated version of <rpcproxy.h>
#endif /* __RPCPROXY_H_VERSION__ */


#include "ARDrone2Video_h.h"

#define TYPE_FORMAT_STRING_SIZE   3                                 
#define PROC_FORMAT_STRING_SIZE   31                                
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   0            

typedef struct _ARDrone2Video_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } ARDrone2Video_MIDL_TYPE_FORMAT_STRING;

typedef struct _ARDrone2Video_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } ARDrone2Video_MIDL_PROC_FORMAT_STRING;

typedef struct _ARDrone2Video_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } ARDrone2Video_MIDL_EXPR_FORMAT_STRING;


static const RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const ARDrone2Video_MIDL_TYPE_FORMAT_STRING ARDrone2Video__MIDL_TypeFormatString;
extern const ARDrone2Video_MIDL_PROC_FORMAT_STRING ARDrone2Video__MIDL_ProcFormatString;
extern const ARDrone2Video_MIDL_EXPR_FORMAT_STRING ARDrone2Video__MIDL_ExprFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_ProxyInfo;



#if !defined(__RPC_WIN32__)
#error  Invalid build platform for this stub.
#endif

#if !(TARGET_IS_NT50_OR_LATER)
#error You need Windows 2000 or later to run this stub because it uses these features:
#error   /robust command line switch.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will fail with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const ARDrone2Video_MIDL_PROC_FORMAT_STRING ARDrone2Video__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure RegisterMFExtensions */

			0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x6 ),	/* 6 */
/*  8 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 10 */	NdrFcShort( 0x0 ),	/* 0 */
/* 12 */	NdrFcShort( 0x8 ),	/* 8 */
/* 14 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x1,		/* 1 */
/* 16 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 18 */	NdrFcShort( 0x0 ),	/* 0 */
/* 20 */	NdrFcShort( 0x0 ),	/* 0 */
/* 22 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Return value */

/* 24 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 26 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 28 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const ARDrone2Video_MIDL_TYPE_FORMAT_STRING ARDrone2Video__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */

			0x0
        }
    };


/* Standard interface: __MIDL_itf_ARDrone2Video_0000_0000, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IInspectable, ver. 0.0,
   GUID={0xAF86E2E0,0xB12D,0x4c6a,{0x9C,0x5A,0xD7,0xAA,0x65,0x10,0x1E,0x90}} */


/* Object interface: __x_ABI_CARDrone2Video_CIARDroneSchemeHandler, ver. 0.0,
   GUID={0xeb16d197,0xe1b2,0x4f41,{0xa6,0x60,0x33,0xd4,0xa7,0xa7,0xb1,0x1c}} */

#pragma code_seg(".orpc")
static const unsigned short __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0
    };

static const MIDL_STUBLESS_PROXY_INFO __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_ProxyInfo =
    {
    &Object_StubDesc,
    ARDrone2Video__MIDL_ProcFormatString.Format,
    &__x_ABI_CARDrone2Video_CIARDroneSchemeHandler_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ARDrone2Video__MIDL_ProcFormatString.Format,
    &__x_ABI_CARDrone2Video_CIARDroneSchemeHandler_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(6) ___x_ABI_CARDrone2Video_CIARDroneSchemeHandlerProxyVtbl = 
{
    0,
    &IID___x_ABI_CARDrone2Video_CIARDroneSchemeHandler,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IInspectable::GetIids */ ,
    0 /* IInspectable::GetRuntimeClassName */ ,
    0 /* IInspectable::GetTrustLevel */
};


static const PRPC_STUB_FUNCTION __x_ABI_CARDrone2Video_CIARDroneSchemeHandler_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION
};

CInterfaceStubVtbl ___x_ABI_CARDrone2Video_CIARDroneSchemeHandlerStubVtbl =
{
    &IID___x_ABI_CARDrone2Video_CIARDroneSchemeHandler,
    &__x_ABI_CARDrone2Video_CIARDroneSchemeHandler_ServerInfo,
    6,
    &__x_ABI_CARDrone2Video_CIARDroneSchemeHandler_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Standard interface: __MIDL_itf_ARDrone2Video_0000_0001, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager, ver. 0.0,
   GUID={0xeb16d197,0xe1b2,0x4f41,{0xa6,0x60,0x33,0xd4,0xa7,0xa7,0xb1,0x1d}} */

#pragma code_seg(".orpc")
static const unsigned short __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0
    };

static const MIDL_STUBLESS_PROXY_INFO __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_ProxyInfo =
    {
    &Object_StubDesc,
    ARDrone2Video__MIDL_ProcFormatString.Format,
    &__x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ARDrone2Video__MIDL_ProcFormatString.Format,
    &__x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(7) ___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerProxyVtbl = 
{
    &__x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_ProxyInfo,
    &IID___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IInspectable::GetIids */ ,
    0 /* IInspectable::GetRuntimeClassName */ ,
    0 /* IInspectable::GetTrustLevel */ ,
    (void *) (INT_PTR) -1 /* __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager::RegisterMFExtensions */
};


static const PRPC_STUB_FUNCTION __x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2
};

CInterfaceStubVtbl ___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerStubVtbl =
{
    &IID___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager,
    &__x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_ServerInfo,
    7,
    &__x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Standard interface: __MIDL_itf_ARDrone2Video_0000_0002, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */

static const MIDL_STUB_DESC Object_StubDesc = 
    {
    0,
    NdrOleAllocate,
    NdrOleFree,
    0,
    0,
    0,
    0,
    0,
    ARDrone2Video__MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x50002, /* Ndr library version */
    0,
    0x8000253, /* MIDL Version 8.0.595 */
    0,
    0,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0
    };

const CInterfaceProxyVtbl * const _ARDrone2Video_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &___x_ABI_CARDrone2Video_CIARDroneSchemeHandlerProxyVtbl,
    ( CInterfaceProxyVtbl *) &___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerProxyVtbl,
    0
};

const CInterfaceStubVtbl * const _ARDrone2Video_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &___x_ABI_CARDrone2Video_CIARDroneSchemeHandlerStubVtbl,
    ( CInterfaceStubVtbl *) &___x_ABI_CARDrone2Video_CIARDroneMFExtensionsManagerStubVtbl,
    0
};

PCInterfaceName const _ARDrone2Video_InterfaceNamesList[] = 
{
    "__x_ABI_CARDrone2Video_CIARDroneSchemeHandler",
    "__x_ABI_CARDrone2Video_CIARDroneMFExtensionsManager",
    0
};

const IID *  const _ARDrone2Video_BaseIIDList[] = 
{
    &IID_IInspectable,
    &IID_IInspectable,
    0
};


#define _ARDrone2Video_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _ARDrone2Video, pIID, n)

int __stdcall _ARDrone2Video_IID_Lookup( const IID * pIID, int * pIndex )
{
    IID_BS_LOOKUP_SETUP

    IID_BS_LOOKUP_INITIAL_TEST( _ARDrone2Video, 2, 1 )
    IID_BS_LOOKUP_RETURN_RESULT( _ARDrone2Video, 2, *pIndex )
    
}

const ExtendedProxyFileInfo ARDrone2Video_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _ARDrone2Video_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _ARDrone2Video_StubVtblList,
    (const PCInterfaceName * ) & _ARDrone2Video_InterfaceNamesList,
    (const IID ** ) & _ARDrone2Video_BaseIIDList,
    & _ARDrone2Video_IID_Lookup, 
    2,
    2,
    0, /* table of [async_uuid] interfaces */
    0, /* Filler1 */
    0, /* Filler2 */
    0  /* Filler3 */
};
#pragma optimize("", on )
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif


#endif /* !defined(_M_IA64) && !defined(_M_AMD64) && !defined(_ARM_) */

