// MixedCppCli.cpp : main project file.

#include "stdafx.h"

using namespace System;

[assembly:System::Reflection::AssemblyVersionAttribute(BARI_PROJECT_VERSION)];

class NativeClass 
{
private:
	int exitCode;

public:
	NativeClass(int c) 
		: exitCode(c)
	{}

	inline int getExitCode() const throw() { return exitCode; };
};

int main(array<System::String ^> ^args)
{
	NativeClass* pNative = new NativeClass(11);
	int code = pNative->getExitCode();
	delete pNative;
	
	Console::WriteLine(System::Reflection::Assembly::GetExecutingAssembly()->GetName()->Version->ToString());    

	wchar_t modPath[MAX_PATH];
	::GetModuleFileName(nullptr, modPath, sizeof(modPath));
	DWORD dwHandle;
	DWORD dwSize = ::GetFileVersionInfoSize(modPath, &dwHandle);

	if (dwSize > 0)
	{
		BYTE* pBuf = static_cast<BYTE*>(::_alloca(dwSize));
		if (::GetFileVersionInfo(modPath, dwHandle, dwSize, pBuf))
		{
			UINT uiSize;
			BYTE* lpb;
			if (::VerQueryValue(pBuf,  
								L"\\VarFileInfo\\Translation",
								reinterpret_cast<void**>(&lpb),
								&uiSize))
			{
				WORD* lpw = reinterpret_cast<WORD*>(lpb);
				wchar_t key[256];
				wsprintf(key, L"\\StringFileInfo\\%04x%04x\\ProductVersion", lpw[0], lpw[1]);
				if (::VerQueryValue(pBuf, key, reinterpret_cast<void**>(&lpb), &uiSize))
				{					
					wprintf(reinterpret_cast<wchar_t*>(lpb));
					wprintf(L"\n");
				}
			}
		}
	}

    return code;
}
