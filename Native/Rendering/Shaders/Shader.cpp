#include "Shader.h"

#include <d3d11.h>
#include <D3dcompiler.h>

Shader::~Shader()
{
    Dispose();
}

bool Shader::Initialize(ID3D11Device* device, const std::wstring& shaderPath)
{
    ID3D10Blob* errorMessage = nullptr;
    ID3D10Blob* vertexShaderBuffer = nullptr;
    ID3D10Blob* pixelShaderBuffer = nullptr;

    HRESULT result = D3DCompileFromFile(
        shaderPath.c_str(),
        nullptr,
        nullptr,
        "main",
        "vs_5_0",
        D3D10_SHADER_ENABLE_STRICTNESS,
        0,
        &vertexShaderBuffer,
        &errorMessage);
    
    if (FAILED(result))
    {
        return false;
    }

    result = device->CreateVertexShader(
        vertexShaderBuffer->GetBufferPointer(),
        vertexShaderBuffer->GetBufferSize(),
        nullptr,
        &_vertexShader);
    
    if (FAILED(result))
    {
        return false;
    }

    constexpr D3D11_INPUT_ELEMENT_DESC vertexLayoutDesc[] = {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,    0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "COLOR",    0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 20, D3D11_INPUT_PER_VERTEX_DATA, 0 },
    };

    result = device->CreateInputLayout(
        vertexLayoutDesc,
        3,
        vertexShaderBuffer->GetBufferPointer(),
        vertexShaderBuffer->GetBufferSize(),
        &_inputLayout);
    
    if (FAILED(result))
    {
        return false;
    }
    
    vertexShaderBuffer->Release();
    pixelShaderBuffer->Release();

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
