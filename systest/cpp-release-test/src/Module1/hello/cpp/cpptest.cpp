// cpptest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"


int _tmain(int argc, _TCHAR* argv[])
{
#ifdef NDEBUG
#ifdef TEST
	std::cout << "Test C++ executable running";

	return 13;
#else
	return 1;
#endif
#else 
	return 2;
#endif
}

