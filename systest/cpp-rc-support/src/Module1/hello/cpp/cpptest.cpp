// cpptest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "resource.h"

int _tmain(int argc, _TCHAR* argv[])
{
	char buffer[256];
	::LoadString(::GetModuleHandle(nullptr), IDS_TEST, buffer, 255);

	std::cout << buffer;

	return 13;
}

