// MixedCppCli.cpp : main project file.

#include "stdafx.h"

using namespace System;

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

    Console::WriteLine(L"Hello World");
    return code;
}
