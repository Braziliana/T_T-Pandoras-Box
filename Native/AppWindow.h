#pragma once
#include <Windows.h>
#include <string>
#include "Rendering/Renderer.h"

extern "C"{
    typedef void (*AppWindowUpdateCallback)(float deltaTime);
}

class AppWindow
{
private:
    std::wstring _name;
    WNDCLASSEX _wc;
    HWND _hWnd;
    HGLRC _hGlRc;
    HDC _hdc;
    Renderer* _renderer;
    bool _isRunning;
    AppWindowUpdateCallback _updateCallback = nullptr;

    static AppWindow* _instance;
    
public:
    AppWindow();
    ~AppWindow();

    void Run();
    void Close();
    Renderer* GetRenderer() const;
    void SetUpdateCallback(AppWindowUpdateCallback callback);

    static AppWindow* Instance()
    {
        if(_instance == nullptr)
        {
            _instance = new AppWindow();
        }
        
        return _instance;
    }
};

extern "C" {
    __declspec(dllexport) AppWindow* WindowCreate();
    __declspec(dllexport) void WindowDestroy(AppWindow* window);
    __declspec(dllexport) void WindowRun(AppWindow* window);
    __declspec(dllexport) void WindowClose(AppWindow* window);
    __declspec(dllexport) Renderer* WindowGetRenderer(AppWindow* appWindow);
    
    __declspec(dllexport) void RegisterAppWindowUpdateCallback(AppWindow* appWindow, AppWindowUpdateCallback callback);
}