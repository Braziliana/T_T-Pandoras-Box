#include <windows.h>

#include "Functions.h"
#include "Offsets.h"


HMODULE localModule;
// bool WINAPI HideThread(const HANDLE hThread) noexcept
// {
//     __try {
//         using FnSetInformationThread = NTSTATUS(NTAPI*)(HANDLE ThreadHandle, UINT ThreadInformationClass, PVOID ThreadInformation, ULONG ThreadInformationLength);
//         const auto NtSetInformationThread{ reinterpret_cast<FnSetInformationThread>(::GetProcAddress(::GetModuleHandle(L"ntdll.dll"), "NtSetInformationThread")) };
//
//         if (!NtSetInformationThread)
//             return false;
//
//         if (const auto status{ NtSetInformationThread(hThread, 0x11u, nullptr, 0ul) }; status == 0x00000000)
//             return true;
//     }
//     __except (TRUE) {
//         return false;
//     }
// }

DWORD WINAPI Run(LPVOID lpReserved)
{
    //HideThread(::GetCurrentThread());
    auto offsets = Offsets::GetInstance();
    while (true)
    {
        const float gameTime = *reinterpret_cast<float*>(offsets->GameTime);
        if (gameTime > 3.0f) break;
        Sleep(300);
    }

    Functions::PrintChat("Hello world");
    
    FreeLibraryAndExitThread(localModule, 0);
    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        localModule = hModule;
		DisableThreadLibraryCalls(hModule);
		CreateThread(nullptr, 0, Run, hModule, 0, nullptr);
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
	
    return TRUE;
}

