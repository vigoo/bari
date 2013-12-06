// StaticLibTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "..\..\StaticLib\cpp\staticlib.h"

int main(int argc, wchar_t* argv[])
{
	std::wcout << get_message().c_str() << std::endl;
	return 10;
}

