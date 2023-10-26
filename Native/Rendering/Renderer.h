#pragma once
#include <d3d11.h>
#include <Windows.h>

extern "C"{
    typedef void (*RenderCallback)();
}

class Renderer
{
private:
    int _width = 0;
    int _height = 0;
    ID3D11Device* _device = nullptr;
    ID3D11DeviceContext* _deviceContext = nullptr;
    IDXGISwapChain* _swapChain = nullptr;
    ID3D11RenderTargetView* _renderTargetView = nullptr;
    RenderCallback _renderCallback = nullptr;
    
public:
    ~Renderer();
    bool Init(HWND hWnd, int width, int height);
    void AddRenderCallback(RenderCallback renderCallback);
    void Render() const;
    void Dispose();
};

extern "C" {
    __declspec(dllexport) void RegisterCallback(Renderer* renderer, RenderCallback callback);
}

/*
 C#
[DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
public static extern void RegisterCallback(IntPtr renderer, RenderDelegate callback);
RegisterCallback(renderer, Render);
 */