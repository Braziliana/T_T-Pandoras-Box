#include "Shader.h"

#include <d3d11.h>
#include <D3dcompiler.h>
#include <iostream>


std::wstring GetHRErrorString(HRESULT hr)
{
    wchar_t* errorStr = nullptr;
    FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER, 
                   nullptr, 
                   hr, 
                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), 
                   (LPWSTR)&errorStr, 
                   0, 
                   nullptr);

    std::wstring errorMessage = errorStr;
    LocalFree(errorStr);
    return errorMessage;
}

Shader::~Shader()
{
    Dispose();
}

bool Shader::Initialize(ID3D11Device* device, const std::wstring& shaderPath, const std::wstring& shaderName, D3D11_INPUT_ELEMENT_DESC* vertexLayoutDesc, size_t size)
{
    ID3DBlob* vsBlob = nullptr;
    ID3DBlob* psBlob = nullptr;
    ID3DBlob* errorMessage = nullptr;

    // Compile the vertex shader
    HRESULT hr = D3DCompileFromFile((shaderPath + shaderName + L"VS.hlsl").c_str(), nullptr, nullptr, "main", "vs_5_0", D3DCOMPILE_ENABLE_STRICTNESS, 0, &vsBlob, &errorMessage);
    if (FAILED(hr))
    {
        std::wcout << "failed to Compile the vertex shader: " << (shaderPath + shaderName + L"VS.hlsl").c_str() << std::endl;
        // Handle the error appropriately.
        if (errorMessage)
        {
            // Output the shader compile errors
            std::cout << reinterpret_cast<const char*>(errorMessage->GetBufferPointer()) << std::endl;
            OutputDebugStringA(reinterpret_cast<const char*>(errorMessage->GetBufferPointer()));
            errorMessage->Release();
        }
        return false;
    }

    // Compile the pixel shader
    hr = D3DCompileFromFile((shaderPath + shaderName + L"PS.hlsl").c_str(), nullptr, nullptr, "main", "ps_5_0", D3DCOMPILE_ENABLE_STRICTNESS, 0, &psBlob, &errorMessage);
    if (FAILED(hr))
    {
        std::wcout << "failed to Compile the pixel shader: " << (shaderPath + shaderName + L"PS.hlsl").c_str() << std::endl;
        if (errorMessage)
        {
            OutputDebugStringA(reinterpret_cast<const char*>(errorMessage->GetBufferPointer()));
            errorMessage->Release();
        }
        vsBlob->Release();
        return false;
    }

    // Create the vertex shader
    hr = device->CreateVertexShader(vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), nullptr, &_vertexShader);
    if (FAILED(hr))
    {
        std::cout << "Failed to CreateVertexShader" << std::endl;
        OutputDebugStringA("Failed to CreateVertexShader");
        vsBlob->Release();
        psBlob->Release();
        return false;
    }

    // Create the pixel shader
    hr = device->CreatePixelShader(psBlob->GetBufferPointer(), psBlob->GetBufferSize(), nullptr, &_pixelShader);
    if (FAILED(hr))
    {
        std::cout << "Failed to CreatePixelShader" << std::endl;
        OutputDebugStringA("Failed to CreatePixelShader");
        vsBlob->Release();
        psBlob->Release();
        return false;
    }


    // Create the vertex input layout
    
    std::wcerr << std::hex << vsBlob->GetBufferPointer() << std::endl;
    std::wcerr << std::dec << vsBlob->GetBufferSize() << std::endl;
    std::wcerr << std::dec << size << std::endl;
    std::wcerr << std::hex << vertexLayoutDesc << std::endl;
    std::wcerr << std::dec;
    
    hr = device->CreateInputLayout(vertexLayoutDesc, static_cast<UINT>(size), vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), &_inputLayout);
    if (FAILED(hr))
    {
        std::wcerr << L"Error: " << GetHRErrorString(hr) << std::endl;
        std::cout << "Failed to CreateInputLayout" << std::endl;
        OutputDebugStringA("Failed to CreateInputLayout");
        vsBlob->Release();
        psBlob->Release();
        return false;
    }

    vsBlob->Release();
    psBlob->Release();
    std::wcout << "Shader loaded: " << shaderName << std::endl;
    return true;
}

void Shader::Use(ID3D11DeviceContext* deviceContext) const
{
    deviceContext->VSSetShader(_vertexShader, nullptr, 0);
    deviceContext->PSSetShader(_pixelShader, nullptr, 0);
    deviceContext->IASetInputLayout(_inputLayout);
}

void Shader::Dispose()
{
    if (_inputLayout)
    {
        _inputLayout->Release();
        _inputLayout = nullptr;
    }

    if (_pixelShader)
    {
        _pixelShader->Release();
        _pixelShader = nullptr;
    }

    if (_vertexShader)
    {
        _vertexShader->Release();
        _vertexShader = nullptr;
    }
}

ID3D11InputLayout* Shader::GetInputLayout() const
{
    return _inputLayout;
}
