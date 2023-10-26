#include "Renderer.h"
#include "../Math/Color.h"

void RegisterCallback(Renderer* renderer, const RenderCallback callback) {
    if(renderer != nullptr)
    {
        renderer->AddRenderCallback(callback);
    }
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

    _deviceContext->OMSetRenderTargets(1, &_renderTargetView, nullptr);
    
    return true;
}

void Renderer::AddRenderCallback(const RenderCallback renderCallback)
{
    _renderCallback = renderCallback;
}

void Renderer::Render() const
{
    const auto clearColor = Color(0, 0, 0, 1.0f);

    _deviceContext->ClearRenderTargetView(_renderTargetView, clearColor.rgba);
    
    if(_renderCallback != nullptr)
    {
        _renderCallback();
    }
    
    HRESULT hr = _swapChain->Present(0, 0);
}

void Renderer::Dispose()
{
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
