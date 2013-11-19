#include "stdafx.h"
#include "TestClass.h"

STDMETHODIMP TestClass::get_Name(BSTR* pName)
{
	_bstr_t buf("world");
	*pName = buf.Detach();
	return S_OK;
}