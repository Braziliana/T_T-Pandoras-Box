#pragma once
#include <d3d11.h>
#include <string>

class Shader
{
private:
    ID3D11VertexShader* _vertexShader = nullptr;
    ID3D11PixelShader* _pixelShader = nullptr;
    ID3D11InputLayout* _inputLayout = nullptr;
    
public:
    ~Shader();
    
    bool Initialize(ID3D11Device* device, const std::wstring& shaderPath);
    void Use(ID3D11DeviceContext* deviceContext) const;
    void Dispose();
    ID3D11InputLayout* GetInputLayout() const;
};
