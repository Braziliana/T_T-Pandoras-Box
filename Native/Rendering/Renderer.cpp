#include "Renderer.h"

#include <iostream>

#include "Vertex.h"
#include "../Math/Color.h"

void RegisterRenderCallback(Renderer* renderer, const RenderCallback callback)
{
    if(renderer == nullptr) return;
    
    renderer->SetRenderCallback(callback);
}

void RegisterRenderGuiCallback(Renderer* renderer, const RenderCallback callback)
{
    if(renderer == nullptr) return;
    
    renderer->SetRenderGuiCallback(callback);
}

void RenderSetClearColor(Renderer* renderer, Color color)
{
    if(renderer == nullptr) return;
    
    renderer->SetClearColor(color);
}

Renderer::~Renderer()
{
    Release();
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

    HRESULT hr = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, D3D11_CREATE_DEVICE_DEBUG, nullptr, 0, D3D11_SDK_VERSION, &swapChainDesc, &_swapChain, &_device, nullptr, &_deviceContext);
    if (FAILED(hr)) {
        OutputDebugStringA("Failed to D3D11CreateDeviceAndSwapChain");
        return false;
    }

    ID3D11Texture2D* backBuffer = nullptr;
    hr = _swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), reinterpret_cast<LPVOID*>(&backBuffer));
    if (FAILED(hr)) {
        OutputDebugStringA("Failed to backBuffer GetBuffer");
        return false;
    }

    hr = _device->CreateRenderTargetView(backBuffer, nullptr, &_renderTargetView);
    backBuffer->Release();
    if (FAILED(hr)) {
        OutputDebugStringA("Failed to _renderTargetView CreateRenderTargetView");
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
        OutputDebugStringA("Failed to _blendState CreateBlendState");
        return false;
    }
    
    _deviceContext->OMSetRenderTargets(1, &_renderTargetView, nullptr);
    
    
    D3D11_BUFFER_DESC matrixBufferDesc;
    ZeroMemory(&matrixBufferDesc, sizeof(matrixBufferDesc));
    matrixBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    matrixBufferDesc.ByteWidth = sizeof(MatrixBufferType);
    matrixBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    matrixBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

    hr = _device->CreateBuffer(&matrixBufferDesc, nullptr, &_matrixBuffer);
    
    if (FAILED(hr)) {
        std::cout << "Failed to _matrixBuffer CreateBuffer" << std::endl;
        OutputDebugStringA("Failed to _matrixBuffer CreateBuffer");
        return false;
    }

    _shaderManager = new ShaderManager(_device);
    if(!_shaderManager->LoadDefaultShaders())
    {
        std::cout << "Failed to LoadDefaultShaders" << std::endl;
        OutputDebugStringA("Failed to LoadDefaultShaders");
        _shaderManager->Release();
        delete _shaderManager;
        _shaderManager = nullptr;
        return false;
    }

    _materialManager = new MaterialManager(_shaderManager);
    if(!_materialManager->Init())
    {
        std::cout << "Failed to Init MaterialManager" << std::endl;
        OutputDebugStringA("Failed to Init MaterialManager");
        _materialManager->Release();
        delete _materialManager;
        _materialManager = nullptr;
        return false;
    }

    if(!_rectRenderer.Init(_device, _deviceContext, 100, _materialManager->GetRectMaterial()))
    {
        std::cout << "Failed to Init _rectRenderer" << std::endl;
        OutputDebugStringA("Failed to Init _rectRenderer");
        return false;
    }
    
    if(!_circleRenderer.Init(_device, _deviceContext, 100, _materialManager->GetCircleMaterial(), 64))
    {
        std::cout << "Failed to Init _circleRenderer" << std::endl;
        OutputDebugStringA("Failed to Init _circleRenderer");
        return false;
    }
    
    _viewport.TopLeftX = 0;
    _viewport.TopLeftY = 0;
    _viewport.Width = static_cast<float>(width);
    _viewport.Height = static_cast<float>(height);
    _viewport.MinDepth = -1.0f;
    _viewport.MaxDepth = 1.0f;
    _deviceContext->RSSetViewports(1, &_viewport);
    
    return true;
}

void Renderer::SetRenderCallback(const RenderCallback callback)
{
    _renderCallback = callback;
}

void Renderer::SetRenderGuiCallback(RenderCallback callback)
{
    _renderGuiCallback = callback;
}

void Renderer::Begin3D() const
{
    auto mt = MatrixBufferType{ DirectX::XMMatrixTranspose(_viewProjectionMatrix3D) };
    _deviceContext->UpdateSubresource(_matrixBuffer, 0, nullptr, &mt, 0, 0);
    _deviceContext->VSSetConstantBuffers(0, 1, &_matrixBuffer);
}

void Renderer::Begin2D() const
{
    constexpr float nearPlane = -1.0f;
    constexpr float farPlane = 1.0f;
    const DirectX::XMMATRIX orthoMatrix = DirectX::XMMatrixOrthographicOffCenterLH(0.0f, static_cast<float>(_width), static_cast<float>(_height), 0.0f, nearPlane, farPlane);
    auto mt = MatrixBufferType{ DirectX::XMMatrixTranspose(orthoMatrix) };
    _deviceContext->UpdateSubresource( _matrixBuffer, 0, nullptr, &mt, 0, 0 );
    _deviceContext->VSSetConstantBuffers( 0, 1, &_matrixBuffer );
}

void Renderer::Render(float deltaTime)
{
    const auto blendFactor = Color(0.0f, 0.0f, 0.0f, 0.0f);
    _deviceContext->ClearRenderTargetView(_renderTargetView, _clearColor.rgba);
    _deviceContext->OMSetBlendState(_blendState, blendFactor.rgba, 0xffffffff);

    Begin3D();
    if(_renderCallback != nullptr)
    {
        _renderCallback(deltaTime);
    }
    
    Begin2D();    
    if(_renderGuiCallback != nullptr)
    {
        _renderGuiCallback(deltaTime);
    }

    auto positon = Vector2(5.f, 5.f);
    auto width = 50.0f;
    auto height = 40.f;
    auto color = Color(1.0f, 0.0f, 1.0f, 1.0f);
    
    _rectRenderer.Draw(positon, width, height, color);
    _rectRenderer.Flush2D();
    
    _deviceContext->OMSetBlendState( _blendState, blendFactor.rgba, 0xffffffff );
    HRESULT hr = _swapChain->Present(0, 0);
}

void Renderer::Release()
{
    _rectRenderer.Release();
    _circleRenderer.Release();

    if(_materialManager != nullptr)
    {
        _materialManager->Release();
        delete _materialManager;
        _materialManager = nullptr;
    }
    
    if(_shaderManager != nullptr)
    {
        _shaderManager->Release();
        delete _shaderManager;
        _shaderManager = nullptr;
    }
    
    if(_matrixBuffer != nullptr)
    {
        _matrixBuffer->Release();
    }
    
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

void Renderer::DrawCircle(const Vector2& position, const float radius, const float width, const Color& color)
{
    _circleRenderer.Draw(position, radius, width, color);
}

void Renderer::DrawCircle(const Vector3& position, const float radius, const float width, const Color& color)
{
    _circleRenderer.Draw(position, radius, width, color);
}
