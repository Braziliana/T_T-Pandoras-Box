#include "AppWindow.h"

#include <chrono>
#include <tchar.h>
#include <random>
#include <iostream>
#include <dwmapi.h>

#include "../Input/InputManager.h"
#include "../Rendering/Renderer.h"
#include "WindowExtensions.h"
#include "../Memory/Process.h"

AppWindow* AppWindow::_instance = nullptr;

bool AppWindow::IsRunning() const
{
    return _isRunning;
}

AppWindow* WindowCreate() {
    return AppWindow::CreateInstance();
}

void WindowDestroy()
{
    AppWindow::Destroy();
}

void WindowRun()
{
    const auto window = AppWindow::Instance();
    if(window == nullptr)
    {
        std::cout << "Window doesnt exist. Cant run." << std::endl;
        return;
    }
    
    window->Run();
}

void RegisterAppWindowUpdateCallback(AppWindowUpdateCallback callback)
{
    const auto window = AppWindow::Instance();
    if(window == nullptr)
    {
        std::cout << "Window doesnt exist. Cant add callback." << std::endl;
        return;
    }
    
    window->SetUpdateCallback(callback);
}

bool WindowIsRunning()
{
    return AppWindow::Instance()->IsRunning();
}

void RegisterAppWindowExitCallback(AppWindowExitCallback callback)
{
    const auto window = AppWindow::Instance();
    if(window == nullptr)
    {
        std::cout << "Window doesnt exist. Cant add callback." << std::endl;
        return;
    }
    
    window->SetExitCallback(callback);
}

std::wstring GenerateRandomName(const int length) {
    static constexpr wchar_t Alpha[] = L"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    static constexpr wchar_t Alphanum[] = L"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    static std::random_device rd;
    static std::mt19937 gen(rd());
    static std::uniform_int_distribution<> dist(0, lstrlenW(Alphanum));

    std::wstring result;
    result += Alpha[dist(gen)];
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
    Renderer::CreateInstance(_hdc, createWindowResult.width, createWindowResult.height);
}

auto lastRenderTime = std::chrono::high_resolution_clock::now();
auto processCheckTimer = 0.25f;
void AppWindow::Run()
{
    InputManager::GetInstance()->Start();
    
    _isRunning = true;
    MSG msg;
    ZeroMemory(&msg, sizeof(MSG));
    bool wasRunning = false;
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

            if(processCheckTimer <= 0)
            {
                const auto isRunning = Process::GetInstance()->IsRunning();
                if(!wasRunning && isRunning)
                {
                    wasRunning = true;
                }
                if(!isRunning && wasRunning)
                {
                    _isRunning = false;
                    break;
                }
                processCheckTimer = 0.2f;
            }
            processCheckTimer -= deltaTime;
            
            if(_updateCallback != nullptr)
            {
                _updateCallback(deltaTime);
            }
            
            InputManager::GetInstance()->ProcessInputEvents();
            Renderer::Instance()->Render(deltaTime);
        }
    }

    _isRunning = false;
    Renderer::Destroy();
    wglMakeCurrent(nullptr, nullptr);
    wglDeleteContext(_hGlRc);
    DestroyWindow(_hWnd);
    UnregisterClass(_wc.lpszClassName, _wc.hInstance);
    InputManager::GetInstance()->Stop();

    if(_exitCallback != nullptr)
    {
        _exitCallback();
    }
}

void AppWindow::Close()
{
    _isRunning=false;
}

void AppWindow::SetUpdateCallback(const AppWindowUpdateCallback callback)
{
    _updateCallback = callback;
}

void AppWindow::SetExitCallback(AppWindowExitCallback callback)
{
    _exitCallback = callback;
}

void AppWindow::Release()
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
