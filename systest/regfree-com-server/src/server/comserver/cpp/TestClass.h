#pragma once
#include "Resource.h"

[
	object,
	uuid("58F73285-E3AD-4DDC-AC6E-93223CDF4F61"),
	dual
]
__interface ITestClass: IDispatch 
{
	[propget, id(1)] HRESULT Name([out, retval] BSTR* pName);
};

[
	coclass,
	default(ITestClass),
	threading(apartment),
	uuid("B3C970F4-4A3D-46EB-9A93-676F2C59D833"),
	version(1.0)
]
class ATL_NO_VTABLE TestClass: public ITestClass
{
public:
	STDMETHOD(get_Name)(BSTR* pName);
};
