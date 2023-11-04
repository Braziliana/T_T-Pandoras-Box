#pragma once
#include <dwmapi.h>
#include <Windows.h>
#include <iostream>
#include <string>

struct WindowCreateResult
{
    WNDCLASSEX wc;
    HWND hWnd;
    HGLRC hGlRc;
    HDC hdc;
    int width;
    int height;
};

class WindowExtensions
{
public:
    static void PrintLastError(std::wstring message);
    static WindowCreateResult Create(const std::wstring& name, DWORD exStyle, DWORD style, int x, int y, int width, int height);
    static WindowCreateResult CreateOverlay(const std::wstring& name);
    static DWORD GetWindowStyle(HWND hWnd);
    static void SetTransparentBlur(HWND hWnd);
};
