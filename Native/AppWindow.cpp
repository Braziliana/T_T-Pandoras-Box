#include "AppWindow.h"

#include <chrono>
#include <tchar.h>
#include <random>
#include <iostream>

AppWindow* WindowCreate(const WindowSettings& windowSettings) {
    return new AppWindow(windowSettings);
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


LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg)
    {
        case WM_CLOSE:
            PostQuitMessage(0);
            return 0;

        default:
            return DefWindowProc(hwnd, msg, wParam, lParam);
    }
}

std::wstring GenerateRandomName(const int length) {
    static constexpr wchar_t Alphanum[] = L"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    static std::random_device rd;
    static std::mt19937 gen(rd());
    static std::uniform_int_distribution<> dist(0, lstrlenW(Alphanum));

    std::wstring result;
    for (size_t i = 0; i < length; ++i) {
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

AppWindow::AppWindow(const WindowSettings& windowSettings) : _isRunning(false), _windowSettings(windowSettings) {
    _name = GenerateRandomName(8, 14);

    _wc= { sizeof(WNDCLASSEX), CS_CLASSDC, WndProc, 0L, 0L, GetModuleHandle(nullptr), nullptr, nullptr, nullptr, nullptr, _name.c_str(), nullptr};
    RegisterClassEx(&_wc);

    if(_windowSettings.positionX < 1)
    {
        _windowSettings.positionX = 1;
    }
    if(_windowSettings.positionY < 1)
    {
        _windowSettings.positionY = 1;
    }
    
    //_hWnd = CreateWindow(_wc.lpszClassName, _wc.lpszClassName, WS_OVERLAPPEDWINDOW, windowSettings.positionX, windowSettings.positionY, windowSettings.width, windowSettings.height, nullptr, nullptr, _wc.hInstance, nullptr);

    _hWnd = CreateWindowEx(
        WS_EX_LAYERED,          // Extended window style
        _wc.lpszClassName,         // Window class name
        _wc.lpszClassName,  // Window title
        WS_OVERLAPPEDWINDOW,    // Window style
        windowSettings.positionX, windowSettings.positionY, windowSettings.width, windowSettings.height, // x, y, width, height
        nullptr, nullptr, _wc.hInstance, nullptr
    );

    SetLayeredWindowAttributes(_hWnd, 0, 255, LWA_COLORKEY);

    HDC hdc = GetDC(_hWnd);
    PIXELFORMATDESCRIPTOR pfd = {
        sizeof(PIXELFORMATDESCRIPTOR),
        1,
        PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,
        PFD_TYPE_RGBA,
        32,    // 32-bit color
        0, 0, 0, 0, 0, 0,
        8,     // 8-bit alpha
        0,
        0,
        0, 0, 0, 0,
        24,    // 24-bit depth buffer
        8,     // 8-bit stencil buffer
        0,
        PFD_MAIN_PLANE,
        0,
        0, 0, 0
    };

    int pixelFormat = ChoosePixelFormat(hdc, &pfd);
    SetPixelFormat(hdc, pixelFormat, &pfd);

    _hGlRc = wglCreateContext(hdc);
    wglMakeCurrent(hdc, _hGlRc);
    
    ShowWindow(_hWnd, SW_NORMAL);
    UpdateWindow(_hWnd);

    // MARGINS margins = {-1};
    // auto hr = DwmExtendFrameIntoClientArea(_hWnd, &margins);
    
    _renderer = new Renderer(hdc);
    auto res = _renderer->Init(windowSettings.width, windowSettings.height);
    std::cout << "_renderer init " << res << std::endl;
}

AppWindow::~AppWindow() {
    _renderer->Release();
    delete _renderer;
}

auto lastRenderTime = std::chrono::high_resolution_clock::now();
void AppWindow::Run()
{
    OutputDebugString(L"Run");
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
