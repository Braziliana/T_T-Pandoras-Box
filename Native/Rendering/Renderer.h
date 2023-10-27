#pragma once
#include <d3d11.h>
#include <Windows.h>

#include "../Math/Color.h"
#include "../Math/Vector2.h"


extern "C"{
    typedef void (*RenderCallback)(float deltaTime);
}

class Renderer
{
private:
    int _width = 0;
    int _height = 0;
    Color _clearColor = {0, 0, 0, 0};
    ID3D11Device* _device = nullptr;
    ID3D11DeviceContext* _deviceContext = nullptr;
    IDXGISwapChain* _swapChain = nullptr;
    ID3D11RenderTargetView* _renderTargetView = nullptr;
    ID3D11BlendState* _blendState = nullptr;
    D3D11_VIEWPORT _viewport = {0};
    RenderCallback _renderCallback = nullptr;
    
public:
    ~Renderer();
    bool Init(HWND hWnd, int width, int height);
    void SetRenderCallback(RenderCallback callback);
    void Render(float deltaTime) const;
    void Dispose();

    void SetClearColor(Color color);
    
    void RenderCircle(Vector2 position, float width, Color color);
};

extern "C" {
    __declspec(dllexport) void RegisterRenderCallback(Renderer* renderer, RenderCallback callback);
    __declspec(dllexport) void RenderSetClearColor(Renderer* renderer, Color color);
}