#include "PatternScanner.h"

#include <iostream>
#include <psapi.h>
#include <windows.h>
#include "Process.h"

DWORD64 PatternScanner::FindOffset(const std::string& patternStr, const int pos)
{
    std::vector<BYTE> pattern;
    std::string mask;

    for (size_t i = 0; patternStr[i]; i++) {
        if (patternStr[i] == ' ')
        {
            continue;
        }
        if (patternStr[i] == '?')
        {
            pattern.push_back(0);
            mask.push_back('?');
            if (patternStr[i + 1] == '?') i++;
        }
        else
        {
            const char byte[3] = { patternStr[i], patternStr[i + 1], '\0' };
            pattern.push_back(static_cast<BYTE>(strtol(byte, nullptr, 16)));
            mask.push_back('x');
            i++;
        }
    }
    
    return FindOffset(pattern, mask, pos);
}

DWORD64 PatternScanner::PatternScan(const DWORD64 begin, const DWORD64 end, const std::vector<BYTE>& pattern, const std::string& mask, int pos)
{
    const unsigned int overlapSize = static_cast<unsigned int>(pattern.size()) - 1;
    const unsigned int chunkSize = 4096 - overlapSize;

    const DWORD64 rangeSize = end - begin;
    const auto buffer = new BYTE[chunkSize + overlapSize];
    const DWORD64 scanEnd = begin + rangeSize;

    for (DWORD64 i = begin; i < scanEnd; i += chunkSize)
    {
        
        DWORD oldprotect;
        const unsigned int currentChunkSize = (i + chunkSize + overlapSize > scanEnd) ? static_cast<unsigned int>(scanEnd - i) : chunkSize + overlapSize;
        
        VirtualProtect(reinterpret_cast<LPVOID>(i), currentChunkSize, PROCESS_VM_READ, &oldprotect);
        const bool read = Process::GetInstance()->Read(i, currentChunkSize, buffer); 
	    VirtualProtect(reinterpret_cast<LPVOID>(i), currentChunkSize, oldprotect, &oldprotect);
        if(read)
        {
            for (unsigned int j = 0; j < currentChunkSize; ++j) {
                if (ScanByteArray(buffer + j, pattern, mask)) {
                    delete[] buffer;
                    
                    uint32_t offset;
                    if (Process::GetInstance()->Read(i + j + pos, &offset)) {
                        return (i + j + pos + offset + 4) - begin;
                    }

                    return (i + j) - begin;
                }
            }
        }
    }
    
    delete[] buffer;
    return 0;
}

DWORD64 PatternScanner::FindOffset(const std::vector<BYTE>& pattern, const std::string& mask, const int pos)
{
    if(!Process::GetInstance()->IsRunning())
    {
        std::cout << "Process is not running or you forgot to use Process::Hook() before using pattern scan" << std::endl;
        return 0;
    }
    
    const auto moduleBase = Process::GetInstance()->GetModuleBase();
    MODULEINFO modInfo;
    if (GetModuleInformation(Process::GetInstance()->GetHandle(), reinterpret_cast<HMODULE>(moduleBase), &modInfo, sizeof(modInfo)))
    {
        const DWORD64 begin = moduleBase;
        const DWORD64 end = begin + modInfo.SizeOfImage;

        return PatternScan(begin, end, pattern, mask, pos);
    }

    std::cout << "Failed to get process module info." << std::endl;
    return 0;
}

bool PatternScanner::ScanByteArray(const BYTE* data, const std::vector<BYTE>& pattern, const std::string& mask) {
    for (size_t i = 0; i < pattern.size(); i++) {
        if (mask[i] != '?' && data[i] != pattern[i]) {
            return false;
        }
    }
    return true;
}

DWORD64 PatternScannerFindOffset(const char* patternStr, const int pos)
{
    return PatternScanner::FindOffset(patternStr, pos);
}

DWORD64 PatternScannerFindOffsetWithMask(const BYTE* pattern, const char* mask, const unsigned int patternSize, const int pos)
{
    const std::vector<BYTE> patternVec(pattern, pattern + patternSize);
    const std::string maskStr(mask);
    
    return PatternScanner::FindOffset(patternVec, maskStr, pos);
}
