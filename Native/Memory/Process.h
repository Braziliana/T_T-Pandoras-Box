#pragma once
#include <mutex>
#include <string>
#include <Windows.h>

#include "MemoryBuffer.h"

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
    bool Read(uintptr_t address, size_t size, unsigned char* result) const;
    bool ReadBuffer(uintptr_t address, MemoryBuffer* memoryBuffer) const;
    template <typename T>
    bool ReadModule(size_t offset, T* result) const;
    bool ReadModule(size_t offset, size_t size, unsigned char* result) const;
    bool ReadModuleBuffer(size_t offset, MemoryBuffer* memoryBuffer) const;

    DWORD GetId() const;
    HANDLE GetHandle() const;
    uintptr_t GetModuleBase() const;
    
    bool IsRunning();
    bool GetProcess();
    static DWORD GetProcessId(const std::wstring& processName);
    static uintptr_t GetModuleBaseAddress(DWORD processId, const std::wstring& moduleName);
};

template <typename T>
bool Process::Read(const uintptr_t address, T* result) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(address), result, sizeof(T), &bytesRead);
}

template <typename T>
bool Process::ReadModule(const size_t offset, T* result) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(_moduleBase + offset), result, sizeof(T), &bytesRead);
}

extern "C" {
    __declspec(dllexport) Process* GetProcess();
    __declspec(dllexport) void ProcessSetTargetProcessName(const std::wstring& processName);
    __declspec(dllexport) bool ProcessRead(uintptr_t address, size_t size, unsigned char* result);
    __declspec(dllexport) bool ProcessReadBuffer(uintptr_t address, MemoryBuffer* memoryBuffer);
    __declspec(dllexport) bool ProcessReadModule(size_t offset, size_t size, unsigned char* result);
    __declspec(dllexport) bool ProcessReadModuleBuffer(size_t offset, MemoryBuffer* memoryBuffer);
    
    __declspec(dllexport) bool ProcessIsRunning();
    __declspec(dllexport) bool ProcessGetId();
    __declspec(dllexport) uintptr_t ProcessGetModuleBase();
}
