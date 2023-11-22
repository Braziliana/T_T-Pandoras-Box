#pragma once
#include <mutex>
#include <string>
#include <Windows.h>
#include "MemoryBuffer.h"
#include <tlhelp32.h>

class Process
{
private:
    std::wstring _processName;
    DWORD _processId;
    HANDLE _hProcess;
    uintptr_t _moduleBase;
    
    static Process* _instance;
    static std::once_flag _initInstanceFlag;
    Process();

public:
    Process(Process const&) = delete;
    void operator=(Process const&) = delete;
    static Process* GetInstance();

    void SetTargetProcessName(const std::wstring& processName);

    template <typename T>
    bool Read(uintptr_t address, T* result) const;
    bool Read(uintptr_t address, unsigned int size, unsigned char* result) const;
    bool ReadBuffer(uintptr_t address, const MemoryBuffer* memoryBuffer) const;
    template <typename T>
    bool ReadModule(unsigned int offset, T* result) const;
    bool ReadModule(unsigned int offset, unsigned int size, unsigned char* result) const;
    bool ReadModuleBuffer(unsigned int offset, const MemoryBuffer* memoryBuffer) const;

    DWORD GetId() const;
    HANDLE GetHandle() const;
    uintptr_t GetModuleBase() const;
    MODULEENTRY32 GetModuleInfo(const std::wstring& moduleName) const;
    bool IsRunning();
    bool Hook();
    static DWORD GetProcessId(const std::wstring& processName);
    static uintptr_t GetModuleBaseAddress(DWORD processId, const std::wstring& moduleName);

    bool LoadDll(const std::wstring& dllPath) const;
};

template <typename T>
bool Process::Read(const uintptr_t address, T* result) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(address), result, sizeof(T), &bytesRead);
}

template <typename T>
bool Process::ReadModule(const unsigned int offset, T* result) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(_moduleBase + offset), result, sizeof(T), &bytesRead);
}

extern "C" {
    __declspec(dllexport) Process* GetProcess();
    __declspec(dllexport) void ProcessSetTargetProcessName(const wchar_t* processName);
    __declspec(dllexport) bool ProcessRead(uintptr_t address, unsigned int size, unsigned char* result);
    __declspec(dllexport) bool ProcessReadBuffer(uintptr_t address, const MemoryBuffer* memoryBuffer);
    __declspec(dllexport) bool ProcessReadModule(unsigned int offset, unsigned int size, unsigned char* result);
    __declspec(dllexport) bool ProcessReadModuleBuffer(unsigned int offset, const MemoryBuffer* memoryBuffer);

    
    __declspec(dllexport) bool ProcessLoadDll(const wchar_t* processName);
    
    __declspec(dllexport) bool ProcessHook();
    __declspec(dllexport) bool ProcessIsRunning();
    __declspec(dllexport) DWORD ProcessGetId();
    __declspec(dllexport) uintptr_t ProcessGetModuleBase();
}
