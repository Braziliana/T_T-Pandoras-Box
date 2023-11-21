#include <iostream>
#include <vector>
#include <Windows.h>
#include <tlhelp32.h>

/*
typedef void(__thiscall* fnPrintChat)(QWORD* chatClient, const char* message, int colorId);
        fnPrintChat _fnSendChat = (fnPrintChat)(globals::moduleBase + oPrintChat);
        std::string timeMarkString = "[" + ConvertTime(GetGameTime()) + "] ";
        std::string coloredTimeMarkString = CHAT_COLOR_DT("#7ce9ff", timeMarkString);
        std::string formattedText = coloredTimeMarkString + text;
        _fnSendChat((QWORD*)(*(QWORD*)(globals::moduleBase + oChatInstance)), formattedText.c_str(), 4);
 */

typedef void(__thiscall* PrintChatFunc)(void* thisPtr, const char* msg, int type);
// DWORD64 funcPtr = 0xDEADBEEF1;
// DWORD64 thisPtr = 0xDEADBEEF2;
// DWORD64 msgPtr = 0xDEADBEEF3;

struct PrintChatParams
{
    DWORD64 funcPtr;
    DWORD64 thisPtr;
    char msg[255];

    PrintChatParams(DWORD64 func, DWORD64 chatInstance, const char* msg, int msgLen)
    {
        funcPtr = func;
        thisPtr = chatInstance;

        ZeroMemory(this->msg, 255);
        memcpy_s(this->msg, 254, msg, msgLen);

        if(msgLen < 255)
        {
            this->msg[msgLen] = '\0';
        }
        else
        {
            this->msg[254] = '\0';
        }
    }
};

void PrintChat(LPVOID lpParam)
{
    auto* pParams = static_cast<PrintChatParams*>(lpParam);

    const auto myPrintChat = reinterpret_cast<PrintChatFunc>(pParams->funcPtr);
    myPrintChat(reinterpret_cast<void*>(pParams->thisPtr), reinterpret_cast<const char*>(pParams->msg), 0);
}

int GetSize(byte* func) {
    int size = 0;
    while (*func != 0xCC)
    {
        //std::cout << std::hex << (int)*func << std::endl;
        size++;
        func++;
    }
    size--;

    return size;
}

byte* GetFunctionBytes(byte* func, int size) {
    auto result = new byte[size];

    for (auto i = 0; i < size; i++) {
        result[i] = func[i];
    }

    return result;
}

DWORD GetProcessId(const std::wstring& processName)
{
    DWORD processId = 0;
    const HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hSnapshot != INVALID_HANDLE_VALUE) {
        PROCESSENTRY32 pe;
        pe.dwSize = sizeof(PROCESSENTRY32);
        if (Process32First(hSnapshot, &pe)) {
            do {
                if (processName == pe.szExeFile) {
                    processId = pe.th32ProcessID;
                    break;
                }
            } while (Process32Next(hSnapshot, &pe));
        }
        CloseHandle(hSnapshot);
    }
    return processId;
}

uintptr_t GetModuleBaseAddress(const DWORD processId, const std::wstring& moduleName)
{
    if(processId == 0)
    {
        return 0;
    }
    
    uintptr_t modBaseAddr = 0;
    const HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, processId);
    if (hSnap != INVALID_HANDLE_VALUE) {
        MODULEENTRY32 modEntry;
        modEntry.dwSize = sizeof(modEntry);
        if (Module32First(hSnap, &modEntry)) {
            do {
                if (modEntry.szModule == moduleName) {
                    modBaseAddr = reinterpret_cast<uintptr_t>(modEntry.modBaseAddr);
                    break;
                }
            } while (Module32Next(hSnap, &modEntry));
        }
    }
    
    CloseHandle(hSnap);
    return modBaseAddr;
}


int main(int argc, char* argv[])
{
    auto processId = GetProcessId(L"League of Legends.exe");
    if(processId == 0)
    {
        return false;
    }
    
    auto hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processId);
    if(hProcess == nullptr)
    {
        return false;
    }
    std::cout << "processId: " << processId << std::endl;
    std::cout << "hProcess: " << hProcess << std::endl;
    void* printChatAddress = reinterpret_cast<void*>(&PrintChat);
    
    auto size = GetSize((byte*)printChatAddress);
    auto funcBytes = GetFunctionBytes((byte*)printChatAddress, size);
    LPVOID codeCave = VirtualAllocEx(hProcess, NULL, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

    std::cout << "codeCave: " << codeCave << std::endl;
    // Step 4: Copy the method to the code cave
    auto r = WriteProcessMemory(hProcess, codeCave, funcBytes, GetSize((byte*)printChatAddress), NULL);

    std::cout << "result: " << r << std::endl;
    // Step 5: Execute the copied method

    SIZE_T paramSize = sizeof(PrintChatParams); // Size of your parameter structure

    LPVOID pRemoteParam = VirtualAllocEx(hProcess, NULL, paramSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

    std::cout << "codeCave: " << pRemoteParam << std::endl;
    auto testMessage = std::string("Test print chat");
    DWORD64 moduleBase = GetModuleBaseAddress(processId, L"League of Legends.exe");
    PrintChatParams params = PrintChatParams(moduleBase + 0x86ACB0, moduleBase + 0x2211A60, testMessage.c_str(), testMessage.length()); // Your parameters

    BOOL result = WriteProcessMemory(hProcess, pRemoteParam, &params, paramSize, NULL);
    std::cout << "result: " << result << std::endl;
    HANDLE hThread = CreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)codeCave, pRemoteParam, 0, NULL);
    std::cout << "hThread: " << result << std::endl;
    //0x0 Start
    //0x8 End
    // Test* t = new Test();
    // std::string str = "Test";
    // int type = 0;
    //
    // void* printChatAddress = reinterpret_cast<void*>(&PrintChat);
    // std::cout << "0x" << std::hex << printChatAddress << " " << GetSize((byte*)printChatAddress) << std::endl;
    // t->Print(str.c_str(), type);


    //PrintChat();
    while (true)
    {
        Sleep(1);
    }
    return 0;
}
