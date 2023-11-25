#include <windows.h>

#include "Offsets.h"
#include "SpoofCall.h"
#include "Communication/IPCServer.h"
#include "Game/Functions.h"
#include "Game/Hero.h"

HMODULE localModule;
bool WINAPI HideThread(const HANDLE hThread) noexcept
{
    __try {
        using FnSetInformationThread = NTSTATUS(NTAPI*)(HANDLE ThreadHandle, UINT ThreadInformationClass, PVOID ThreadInformation, ULONG ThreadInformationLength);
        const auto NtSetInformationThread{ reinterpret_cast<FnSetInformationThread>(::GetProcAddress(::GetModuleHandle(L"ntdll.dll"), "NtSetInformationThread")) };

        if (!NtSetInformationThread)
            return false;

        const auto status = NtSetInformationThread(hThread, 0x11u, nullptr, 0ul);
        if (status == 0x00000000)
        {
            return true;
        }
    }
    __except (TRUE) {
        return false;
    }
}

DWORD WINAPI Run(LPVOID lpReserved)
{
    HideThread(::GetCurrentThread());
    auto offsets = Offsets::GetInstance();
    auto ipcServer = IPCServer(L"HelloWorld");
    while (true)
    {
        const float gameTime = *reinterpret_cast<float*>(offsets->GameTime);
        if (gameTime > 3.0f)
        {
            if(!ipcServer.IsRunning())
            {
                if(!ipcServer.Start())
                {
                    Sleep(1);
                }
                
            }
            else
            {
                ipcServer.Run();
            }
            
            if(GetAsyncKeyState(VK_F11))
            {
                break;
            }
        }

        //Sleep(1);
    }

    // std::string(SP_STRING("<font color='") + std::string(SP_STRING(color)) + SP_STRING("'>") + std::string(text) + SP_STRING("</font>"))
    //Functions::PrintChat("<font color=#FC6A03>T_T Pandora's box</font>");
    //Functions::MoveTo(8000, 8000);
    //Functions::MoveToMouse();
    //Functions::CastSpell(1);
    //Functions::CastSpellClick(1, Vector2());
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

