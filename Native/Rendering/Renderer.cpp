#include "Renderer.h"

#include "Vertex.h"
#include "../Math/Color.h"

void RegisterRenderCallback(Renderer* renderer, const RenderCallback callback)
{
    if(renderer == nullptr) return;
    
    renderer->SetRenderCallback(callback);
}

void RenderSetClearColor(Renderer* renderer, Color color)
{
    if(renderer == nullptr) return;
    
    renderer->SetClearColor(color);
}

void Renderer::InitShapes()
{
    _rectVertex = new Vertex[]
    {
        // Triangle 1
        {Vector3(-0.5f,  0.5f, 0.0f), Vector2(0.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-left
        {Vector3( 0.5f,  0.5f, 0.0f), Vector2(1.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3(-0.5f, -0.5f, 0.0f), Vector2(0.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left

        // Triangle 2
        {Vector3(-0.5f, -0.5f, 0.0f), Vector2(0.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left
        {Vector3( 0.5f,  0.5f, 0.0f), Vector2(1.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3( 0.5f, -0.5f, 0.0f), Vector2(1.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-right
    };

    _planeVertex = new Vertex[]
    {
        // Triangle 1
        {Vector3(-0.5f,  0.0f, 0.5f), Vector2(0.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-left
        {Vector3( 0.5f,  0.0f, 0.5f), Vector2(1.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3(-0.5f, 0.0f, -0.5f), Vector2(0.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left

        // Triangle 2
        {Vector3(-0.5f, 0.0f, -0.5f), Vector2(0.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left
        {Vector3( 0.5f,  0.0f, 0.5f), Vector2(1.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3( 0.5f, 0.0f, -0.5f), Vector2(1.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-right
    };

    SetCircleSegments(64);
}

Renderer::~Renderer()
{
    Dispose();
}

bool Renderer::Init(const HWND hWnd, const int width, const int height)
{
    _width = width;
    _height = height;

    DXGI_MODE_DESC bufferDesc;
    ZeroMemory(&bufferDesc, sizeof(DXGI_MODE_DESC));

    bufferDesc.Width = width;
    bufferDesc.Height = height;
    bufferDesc.RefreshRate.Numerator = 60;
    bufferDesc.RefreshRate.Denominator = 1;
    bufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    bufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
    bufferDesc.Scaling = DXGI_MODE_SCALING_UNSPECIFIED;

    DXGI_SWAP_CHAIN_DESC swapChainDesc;
    ZeroMemory(&swapChainDesc, sizeof(swapChainDesc));
    
    swapChainDesc.BufferDesc = bufferDesc;
    swapChainDesc.BufferCount = 1;
    swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    swapChainDesc.OutputWindow = hWnd;
    swapChainDesc.SampleDesc.Count = 1;
    swapChainDesc.Windowed = TRUE;
    swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

    HRESULT hr = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, nullptr, 0, D3D11_SDK_VERSION, &swapChainDesc, &_swapChain, &_device, nullptr, &_deviceContext);
    if (FAILED(hr)) {
        return false;
    }

    ID3D11Texture2D* backBuffer = nullptr;
    hr = _swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), reinterpret_cast<LPVOID*>(&backBuffer));
    if (FAILED(hr)) {
        return false;
    }

    hr = _device->CreateRenderTargetView(backBuffer, nullptr, &_renderTargetView);
    backBuffer->Release();
    if (FAILED(hr)) {
        return false;
    }

    D3D11_BLEND_DESC blendDesc;
    ZeroMemory(&blendDesc, sizeof(D3D11_BLEND_DESC));

    blendDesc.RenderTarget[0].BlendEnable = TRUE;
    blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    hr =_device->CreateBlendState(&blendDesc, &_blendState);
    if (FAILED(hr)) {
        return false;
    }
    
    _deviceContext->OMSetRenderTargets(1, &_renderTargetView, nullptr);

    _viewport.TopLeftX = 0;
    _viewport.TopLeftY = 0;
    _viewport.Width = static_cast<float>(width);
    _viewport.Height = static_cast<float>(height);
    _viewport.MinDepth = 0.0f;
    _viewport.MaxDepth = 1.0f;
    _deviceContext->RSSetViewports(1, &_viewport);

    InitShapes();
    
    return true;
}

void Renderer::SetRenderCallback(const RenderCallback callback)
{
    _renderCallback = callback;
}

void Renderer::Render(float deltaTime) const
{
    const auto blendFactor = Color(0.0f, 0.0f, 0.0f, 0.0f);
    
    _deviceContext->ClearRenderTargetView(_renderTargetView, _clearColor.rgba);
    _deviceContext->OMSetBlendState(_blendState, blendFactor.rgba, 0xffffffff);
    
    if(_renderCallback != nullptr)
    {
        _renderCallback(deltaTime);
    }
    
    _deviceContext->OMSetBlendState( _blendState, blendFactor.rgba, 0xffffffff );
    HRESULT hr = _swapChain->Present(0, 0);
}

void Renderer::Dispose()
{
    delete _rectVertex;
    delete _planeVertex;
    delete _circle2DVertex;
    delete _circle3DVertex;
    
    if(_blendState != nullptr)
    {
        _blendState->Release();
        _blendState = nullptr;
    }
    
    if(_renderTargetView != nullptr)
    {
        _renderTargetView->Release();
        _renderTargetView = nullptr;
    }

    if(_swapChain != nullptr)
    {
        _swapChain->Release();
        _swapChain = nullptr;
    }

    if(_deviceContext != nullptr)
    {
        _deviceContext->Release();
        _deviceContext = nullptr;
    }
    
    if(_device != nullptr)
    {
        _device->Release();
        _device = nullptr;
    }
}

void Renderer::SetClearColor(const Color color)
{
    _clearColor = color;
}

void Renderer::SetCircleSegments(int segments)
{
    _circle2DVertex = new Vertex[segments+2];
    _circle3DVertex = new Vertex[segments+2];
    _circle2DVertex[0] = {Vector3(0.0f, 0.0f, 0.0f), Vector2(0.5f, 0.5f), Color(1.0f, 1.0f, 1.0f, 1.0f)};
    _circle3DVertex[0] = {Vector3(0.0f, 0.0f, 0.0f), Vector2(0.5f, 0.5f), Color(1.0f, 1.0f, 1.0f, 1.0f)};

    for (int i = 0; i <= segments; ++i) {
        const float theta = 2.0f * 3.1415926f * static_cast<float>(i) / static_cast<float>(segments);
        const float dx = cosf(theta);
        const float dy = sinf(theta);
        
        _circle2DVertex[i+1] = {Vector3( dx, dy, 0.0f), Vector2(0.5f + 0.5f * cosf(theta), 0.5f + 0.5f * sinf(theta)), Color(1.0f, 1.0f, 1.0f, 1.0f)};
        _circle3DVertex[i+1] = {Vector3( dx, 0.0f, dy), Vector2(0.5f + 0.5f * cosf(theta), 0.5f + 0.5f * sinf(theta)), Color(1.0f, 1.0f, 1.0f, 1.0f)};
    }
    
}

void Renderer::RenderCircle(Vector2 position, float width, Color color)
{
    
}
