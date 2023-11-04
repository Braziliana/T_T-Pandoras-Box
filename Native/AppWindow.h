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
    bool _isRunning;
    AppWindowUpdateCallback _updateCallback = nullptr;

    static AppWindow* _instance;
    
public:
    AppWindow();
    ~AppWindow()
    {
        Release();
        delete _instance;
        _instance = nullptr;
    }

    void Run();
    void Close();
    void SetUpdateCallback(AppWindowUpdateCallback callback);

    void Release()
    {
        _isRunning = false;
        Renderer::Destroy();
        wglMakeCurrent(nullptr, nullptr);
        wglDeleteContext(_hGlRc);
        DestroyWindow(_hWnd);
        UnregisterClass(_wc.lpszClassName, _wc.hInstance);
        delete _instance;
        _instance = nullptr;
    }
    
    static AppWindow* Instance()
    {
        return _instance;
    }

    static AppWindow* CreateInstance()
    {
        if(_instance != nullptr)
        {
        }
        
        _instance = new AppWindow();
        return _instance;
    }
    
    static void Destroy()
    {
        if(_instance != nullptr)
        {
            _instance->Release();
            delete _instance;
            _instance = nullptr;
        }
    }
};

extern "C" {
    __declspec(dllexport) AppWindow* WindowCreate();
    __declspec(dllexport) void WindowDestroy();
    __declspec(dllexport) void WindowRun();
    __declspec(dllexport) void RegisterAppWindowUpdateCallback(AppWindowUpdateCallback callback);
}