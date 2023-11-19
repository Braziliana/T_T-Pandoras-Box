#include <windows.h>
#include <cstdlib>
#define GLEW_STATIC
#include <GL/glew.h>

#pragma comment(lib, "opengl32.lib")
#pragma comment(lib, "glu32.lib")
//#pragma comment(lib, "glew32s.lib")

#ifdef _DEBUG
#pragma comment(lib, "freetyped.lib")
#pragma comment(lib, "glew32sd.lib")
#else
#pragma comment(lib, "freetype.lib")
#pragma comment(lib, "glew32s.lib")
#endif


#pragma comment(lib, "Dwmapi.lib")

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ulReasonForCall, LPVOID lpReserved)
{
    switch (ulReasonForCall)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
	
    return TRUE;
}

