#pragma once
#include <Windows.h>
#include <string>
#include <vector>

class PatternScanner
{
public:
    static DWORD64 FindOffset(const std::string& patternStr, int pos);
    static DWORD64 FindOffset(const std::vector<BYTE>& pattern, const std::string& mask, int pos);
    static DWORD64 PatternScan(DWORD64 begin, DWORD64 end, const std::vector<BYTE>& pattern, const std::string& mask, int pos);
    static bool ScanByteArray(const BYTE* data, const std::vector<BYTE>& pattern, const std::string& mask);
};

// Assuming that these are part of the PatternScanner class, you need to create C-compatible wrappers.
extern "C" {
    __declspec(dllexport) DWORD64 PatternScannerFindOffset(const char* patternStr, int pos);
    __declspec(dllexport) DWORD64 PatternScannerFindOffsetWithMask(const BYTE* pattern, const char* mask, unsigned int patternSize, int pos);
}