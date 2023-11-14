#include "WindowExtensions.h"

LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg)
    {
    case WM_CLOSE:
        PostQuitMessage(0);
        return 0;
        
    case WM_ACTIVATE:
        if (wParam != WA_INACTIVE) {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            return 0;
        }
        break;
        
    case WM_MOUSEACTIVATE:
        return MA_NOACTIVATE;
    
    default:
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }
    
    return DefWindowProc(hWnd, msg, wParam, lParam);
}


HDC CreateGDI(HWND hWnd)
{
    const HDC hdc = GetDC(hWnd);
    constexpr PIXELFORMATDESCRIPTOR pfd = {
        sizeof(PIXELFORMATDESCRIPTOR),
        1,                                  // Version Number
        PFD_DRAW_TO_WINDOW |              // Format Must Support Window
        PFD_SUPPORT_OPENGL |              // Format Must Support OpenGL
        PFD_SUPPORT_COMPOSITION |         // Format Must Support Composition
        PFD_DOUBLEBUFFER,                 // Must Support Double Buffering
        PFD_TYPE_RGBA,                    // Request An RGBA Format
        32,                               // Select Our Color Depth
        0, 0, 0, 0, 0, 0,                 // Color Bits Ignored
        8,                                // An Alpha Buffer
        0,                                // Shift Bit Ignored
        0,                                // No Accumulation Buffer
        0, 0, 0, 0,                       // Accumulation Bits Ignored
        0 ,                               // No Z-Buffer (Depth Buffer)
        8,                                // Some Stencil Buffer
        0,                                // No Auxiliary Buffer
        PFD_MAIN_PLANE,                   // Main Drawing Layer
        0,                                // Reserved
        0, 0, 0                           // Layer Masks Ignored
    };

    const int pixelFormat = ChoosePixelFormat(hdc, &pfd);
    SetPixelFormat(hdc, pixelFormat, &pfd);
    return hdc;
}

HGLRC CreateOpenGlContext(HDC hdc)
{
    const HGLRC result = wglCreateContext(hdc);
    wglMakeCurrent(hdc, result);
    return result;
}


void WindowExtensions::PrintLastError(const std::wstring& message)
{
    const DWORD dwError = GetLastError();
    LPVOID lpMsgBuf;
    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER | 
        FORMAT_MESSAGE_FROM_SYSTEM |
        FORMAT_MESSAGE_IGNORE_INSERTS,
        nullptr,
        dwError,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        reinterpret_cast<LPTSTR>(&lpMsgBuf),
        0, nullptr);

    std::wcout << message << " code: " << dwError << L" message: " << static_cast<LPTSTR>(lpMsgBuf) << std::endl;

    LocalFree(lpMsgBuf);
}

WindowCreateResult WindowExtensions::Create(const std::wstring& name, const DWORD exStyle, const DWORD style,
    int x, int y, const int width, const int height)
{
    WNDCLASSEX wc;
    ZeroMemory(&wc, sizeof(WNDCLASSEX));
    wc.cbSize = sizeof(WNDCLASSEX);
    wc.style = CS_CLASSDC;
    wc.lpfnWndProc = WndProc;
    wc.hInstance = GetModuleHandle(nullptr);
    wc.lpszClassName = name.c_str();

    if (!RegisterClassEx(&wc)) {
        PrintLastError(L"RegisterClassEx name: " + name + L" failed with error ");
    }
    
    const HWND hWnd = CreateWindowEx(
        exStyle,          // Extended window style
        wc.lpszClassName,         // Window class name
        wc.lpszClassName,  // Window title
        style,    // Window style
        x, y, width, height, // x, y, width, height
        nullptr, nullptr, wc.hInstance, nullptr
    );

    if (!hWnd)
    {
        PrintLastError(L"CreateWindowEx failed with error ");
    }
    
    return {wc, hWnd, nullptr, nullptr, width, height};
}

WindowCreateResult WindowExtensions::CreateOverlay(const std::wstring& name)
{
    //https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/june/windows-with-c-high-performance-window-layering-using-the-windows-composition-engine
    //todo play around code above to check if this will work better if there will be issues with current implementation 
    constexpr DWORD exStyle = WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_NOACTIVATE;
    constexpr DWORD style = WS_POPUP;
    const int width = GetSystemMetrics(SM_CXSCREEN);
    const int height = GetSystemMetrics(SM_CYSCREEN);

    //if you have black screen you can try to use one of bellow
    //auto result = Create(name, exStyle, style, 1, 1, width-1, height-1); 
    //auto result = Create(name, exStyle, style, 1, 1, width-2, height-2);
    auto result = Create(name, exStyle, style, 1, 1, width-1, height-1);
    
    SetLayeredWindowAttributes(result.hWnd, RGB(0, 0, 0), 255, LWA_COLORKEY | LWA_ALPHA);
    SetTransparentBlur(result.hWnd);
    
    result.hdc = CreateGDI(result.hWnd);
    result.hGlRc = CreateOpenGlContext(result.hdc);
    
    ShowWindow(result.hWnd, SW_SHOWNOACTIVATE);
    UpdateWindow(result.hWnd);
    
    return result;
}

DWORD WindowExtensions::GetWindowStyle(const HWND hWnd)
{
    return GetWindowLongW(hWnd, GWL_STYLE);
}

void WindowExtensions::SetTransparentBlur(const HWND hWnd)
{
    BOOL composition;

    if (FAILED(DwmIsCompositionEnabled(&composition)) || !composition)
    {
        PrintLastError(L"DwmIsCompositionEnabled returned false");
        return;
    }

    //Windows7
    /*
        DWM_BLURBEHIND blurBehind = {0};
        blurBehind.dwFlags = DWM_BB_ENABLE;
        DwmEnableBlurBehindWindow(hWnd, &blurBehind);
     */
    
    // Windows >= 8
    const HRGN region = CreateRectRgn(0, 0, -1, -1);
    DWM_BLURBEHIND blurBehind;
    ZeroMemory(&blurBehind, sizeof(DWM_BLURBEHIND));
            
    blurBehind.dwFlags = DWM_BB_ENABLE | DWM_BB_BLURREGION;
    blurBehind.hRgnBlur = region;
    blurBehind.fEnable = TRUE;
            
    if (FAILED(DwmEnableBlurBehindWindow(hWnd, &blurBehind)))
    {
        PrintLastError(L"Failed to enable blur");
    }
    DeleteObject(region);
}