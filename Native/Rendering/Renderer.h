#pragma once
#include <d3d11.h>
#include <Windows.h>

#include "Vertex.h"
#include "../Math/Color.h"
#include "../Math/Vector2.h"
#include "Materials/MaterialManager.h"
#include "Renderers/CircleRenderer.h"
#include "Renderers/RectRenderer.h"
#include "Shaders/ShaderManager.h"


extern "C"{
    typedef void (*RenderCallback)(float deltaTime);
}

struct MatrixBufferType
{
    DirectX::XMMATRIX viewProjectionMatrix;
};

class Renderer
{
private:
    int _width = 0;
    int _height = 0;
    
    ID3D11Buffer* _matrixBuffer = nullptr;
    DirectX::XMMATRIX _viewProjectionMatrix3D = DirectX::XMMatrixIdentity();
    Color _clearColor = {0, 0, 0, 1.0};
    ID3D11Device* _device = nullptr;
    ID3D11DeviceContext* _deviceContext = nullptr;
    IDXGISwapChain* _swapChain = nullptr;
    ID3D11RenderTargetView* _renderTargetView = nullptr;
    ID3D11BlendState* _blendState = nullptr;
    D3D11_VIEWPORT _viewport = {0};
    RenderCallback _renderCallback = nullptr;
    RenderCallback _renderGuiCallback = nullptr;

    RectRenderer _rectRenderer = {};
    CircleRenderer _circleRenderer = {};

    ShaderManager* _shaderManager = nullptr;
    MaterialManager* _materialManager = nullptr;
    
public:
    ~Renderer();
    bool Init(HWND hWnd, int width, int height);
    void SetRenderCallback(RenderCallback callback);
    void SetRenderGuiCallback(RenderCallback callback);
    void Render(float deltaTime);
    void Release();

    void SetClearColor(Color color);

    void Begin3D() const;
    void Begin2D() const;
    
    void DrawCircle(const Vector2& position, float radius, float width, const Color& color);
    void DrawCircle(const Vector3& position, float radius, float width, const Color& color);
};

extern "C" {
    __declspec(dllexport) void RegisterRenderCallback(Renderer* renderer, RenderCallback callback);
    __declspec(dllexport) void RegisterRenderGuiCallback(Renderer* renderer, RenderCallback callback);
    __declspec(dllexport) void RenderSetClearColor(Renderer* renderer, Color color);
}