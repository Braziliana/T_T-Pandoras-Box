﻿#include "Process.h"

#include <iostream>

Process* Process::_instance = nullptr;
std::once_flag Process::_initInstanceFlag;

bool ProcessRead(const uintptr_t address, const unsigned int size, unsigned char* result)
{
    return Process::GetInstance()->Read(address, size, result);
}

bool ProcessReadBuffer(const uintptr_t address, MemoryBuffer* memoryBuffer)
{
    return Process::GetInstance()->Read(address, memoryBuffer);
}

bool ProcessReadModule(const unsigned int offset, const unsigned int size, unsigned char* result)
{
    return Process::GetInstance()->ReadModule(offset, size, result);
}

bool ProcessReadModuleBuffer(const unsigned int offset, MemoryBuffer* memoryBuffer)
{
    return Process::GetInstance()->ReadModuleBuffer(offset, memoryBuffer);
}

void ProcessSetTargetProcessName(const wchar_t* processName)
{
    Process::GetInstance()->SetTargetProcessName(processName);
}

bool ProcessHook()
{
    auto r = Process::GetInstance()->Hook();
    return r;
}

bool ProcessIsRunning()
{
    auto r =  Process::GetInstance()->IsRunning();
    return r;
}

DWORD ProcessGetId()
{
    return Process::GetInstance()->GetId();
}

uintptr_t ProcessGetModuleBase()
{
    return Process::GetInstance()->GetModuleBase();
}


Process::Process(): _processId(0), _hProcess(nullptr), _moduleBase(0)
{
}

Process* Process::GetInstance()
{
    std::call_once(_initInstanceFlag, []() {
        _instance = new Process();
    });
    return _instance;
}

void Process::SetTargetProcessName(const std::wstring& processName)
{
    _processName = processName;
}

bool Process::Read(const uintptr_t address, const unsigned int size, unsigned char* result) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(address), result, size, &bytesRead);
}

bool Process::ReadBuffer(const uintptr_t address, MemoryBuffer* memoryBuffer) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(address), memoryBuffer, memoryBuffer->size, &bytesRead);
}

bool Process::ReadModule(const unsigned int offset, const unsigned int size, unsigned char* result) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(_moduleBase + offset), result, size, &bytesRead);
}

bool Process::ReadModuleBuffer(const unsigned int offset, MemoryBuffer* memoryBuffer) const
{
    size_t bytesRead;
    return ReadProcessMemory(_hProcess, reinterpret_cast<LPCVOID>(_moduleBase + offset), memoryBuffer, memoryBuffer->size, &bytesRead);
}

DWORD Process::GetId() const
{
    return _processId;
}

HANDLE Process::GetHandle() const
{
    return _hProcess;
}

uintptr_t Process::GetModuleBase() const
{
    return _moduleBase;
}

MODULEENTRY32 Process::GetModuleInfo(const std::wstring& moduleName) const
{
    const HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, _processId);
    if (hSnap != INVALID_HANDLE_VALUE)
    {
        MODULEENTRY32 modEntry;
        modEntry.dwSize = sizeof(modEntry);
        if (Module32First(hSnap, &modEntry))
        {
            do
            {
                if (modEntry.szModule == moduleName)
                {
                    return modEntry;
                }
            } while (Module32Next(hSnap, &modEntry));
        }
    }
    
    CloseHandle(hSnap);
    return {};
}

bool Process::IsRunning()
{
    const auto isRunning = WaitForSingleObject(_hProcess, 0) == WAIT_TIMEOUT;
    if(!isRunning)
    {
        _processId = 0;
        _hProcess = nullptr;
    }
    return isRunning;
}

bool Process::Hook()
{
    _processId = GetProcessId(_processName);
    if(_processId == 0)
    {
        return false;
    }
    
    _hProcess = OpenProcess(SYNCHRONIZE | PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, _processId);
    if(_hProcess == nullptr)
    {
        return false;
    }

    if(!IsRunning())
    {
        return false;
    }

    _moduleBase = GetModuleBaseAddress(_processId, _processName);
    
    return true;
}


DWORD Process::GetProcessId(const std::wstring& processName)
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

uintptr_t Process::GetModuleBaseAddress(const DWORD processId, const std::wstring& moduleName)
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

