#pragma once
#include <Windows.h>
#include <string>
#include "Rendering/Renderer.h"

extern "C"{
    typedef void (*AppWindowUpdateCallback)(float deltaTime);
}

struct WindowSettings
{
    int width;
    int height;
    int positionX;
    int positionY;
    int targetFps;
};

class AppWindow
{
private:
    std::wstring _name;
    WNDCLASSEX _wc;
    HWND _hWnd;
    HGLRC _hGlRc;
    Renderer* _renderer;
    bool _isRunning;
    WindowSettings _windowSettings;
    AppWindowUpdateCallback _updateCallback = nullptr;
    
public:
    AppWindow(const WindowSettings& windowSettings);
    ~AppWindow();

    void Run();
    void Close();
    Renderer* GetRenderer() const;
    void SetUpdateCallback(AppWindowUpdateCallback callback);
};

extern "C" {
    __declspec(dllexport) AppWindow* WindowCreate(const WindowSettings& windowSettings);
    __declspec(dllexport) void WindowDestroy(AppWindow* window);
    __declspec(dllexport) void WindowRun(AppWindow* window);
    __declspec(dllexport) void WindowClose(AppWindow* window);
    __declspec(dllexport) Renderer* WindowGetRenderer(AppWindow* appWindow);
    
    __declspec(dllexport) void RegisterAppWindowUpdateCallback(AppWindow* appWindow, AppWindowUpdateCallback callback);
}