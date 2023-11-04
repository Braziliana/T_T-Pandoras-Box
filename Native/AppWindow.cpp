#include "AppWindow.h"

#include <chrono>
#include <tchar.h>
#include <random>
#include <iostream>
#include <dwmapi.h>
#include "Windows/WindowExtensions.h"

AppWindow* WindowCreate() {
    return new AppWindow();
}

void WindowDestroy(const AppWindow* window)
{
    delete window;
}

void WindowRun(AppWindow* window)
{
    if(window == nullptr) return;
    window->Run();
}

void WindowClose(AppWindow* window)
{
    if(window == nullptr) return;
    window->Close();
}

Renderer* WindowGetRenderer(const AppWindow* appWindow)
{
    if(appWindow == nullptr)
    {
        return nullptr;
    }
    
    return appWindow->GetRenderer();
}


void RegisterAppWindowUpdateCallback(AppWindow* appWindow, AppWindowUpdateCallback callback)
{
    appWindow->SetUpdateCallback(callback);
}


std::wstring GenerateRandomName(const int length) {
    static constexpr wchar_t Alphanum[] = L"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    static std::random_device rd;
    static std::mt19937 gen(rd());
    static std::uniform_int_distribution<> dist(0, lstrlenW(Alphanum));

    std::wstring result;
    for (int i = 0; i < length; i++) {
        result += Alphanum[dist(gen)];
    }

    return result;
}

std::wstring GenerateRandomName(const int minLength, const int maxLength) {
    
    static std::random_device rd;
    static std::mt19937 gen(rd());
    std::uniform_int_distribution<> distLength(minLength, maxLength);
    
    return GenerateRandomName(distLength(gen));
}

AppWindow::AppWindow() : _isRunning(false) {
    _name = GenerateRandomName(8, 14);

    const auto createWindowResult = WindowExtensions::CreateOverlay(_name);
    _hWnd = createWindowResult.hWnd;
    _wc = createWindowResult.wc;
    _hGlRc = createWindowResult.hGlRc;
    _hdc = createWindowResult.hdc;
    
    _renderer = new Renderer(createWindowResult.hdc);
    auto res = _renderer->Init(createWindowResult.width, createWindowResult.height);
}

AppWindow::~AppWindow() {
    _renderer->Release();
    delete _renderer;
}

auto lastRenderTime = std::chrono::high_resolution_clock::now();
void AppWindow::Run()
{
    _isRunning = true;
    MSG msg = { 0 };
    while (msg.message != WM_QUIT && _isRunning)
    {
        if (PeekMessage(&msg, nullptr, 0U, 0U, PM_REMOVE))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
        else
        {
            auto currentTime = std::chrono::high_resolution_clock::now();
            const auto deltaTime = static_cast<float>(std::chrono::duration<double, std::milli>(currentTime - lastRenderTime).count());
            lastRenderTime = currentTime;

            if(_updateCallback != nullptr)
            {
                _updateCallback(deltaTime);
            }
            
            _renderer->Render(deltaTime);
        }
    }
    
    _isRunning = false;
    _renderer->Release();
    wglMakeCurrent(nullptr, nullptr);
    wglDeleteContext(_hGlRc);
    DestroyWindow(_hWnd);
    UnregisterClass(_wc.lpszClassName, _wc.hInstance);
}

void AppWindow::Close()
{
    _isRunning=false;
}

Renderer* AppWindow::GetRenderer() const {
    return _renderer;
}

void AppWindow::SetUpdateCallback(const AppWindowUpdateCallback callback)
{
    _updateCallback = callback;
}
