#pragma once
#include <Windows.h>
#include <string>

extern "C"{
    typedef void (*AppWindowUpdateCallback)(float deltaTime);
    typedef void (*AppWindowExitCallback)();
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
    AppWindowExitCallback _exitCallback = nullptr;

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
    void SetExitCallback(AppWindowExitCallback callback);

    void Release();

    static AppWindow* Instance()
    {
        return _instance;
    }

    static AppWindow* CreateInstance()
    {
        if(_instance != nullptr)
        {
            return _instance;
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

    bool IsRunning() const;
};

extern "C" {
    __declspec(dllexport) AppWindow* WindowCreate();
    __declspec(dllexport) void WindowDestroy();
    __declspec(dllexport) void WindowRun();
    __declspec(dllexport) void RegisterAppWindowUpdateCallback(AppWindowUpdateCallback callback);
    __declspec(dllexport) void RegisterAppWindowExitCallback(AppWindowExitCallback callback);
    __declspec(dllexport) bool WindowIsRunning();
}