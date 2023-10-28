#pragma once
#include <map>
#include <string>

#include "Shader.h"

class ShaderManager
{
private:
    ID3D11Device* _device;
    std::map<std::string, Shader*> _shaders;
public:
    ShaderManager(ID3D11Device* device);
    ~ShaderManager();
    bool LoadDefaultShaders();
    bool LoadShader(const std::string& name, const std::wstring& shaderPath, const std::wstring& fileName, D3D11_INPUT_ELEMENT_DESC* vertexLayoutDesc, size_t size);
    Shader* GetShader(const std::string& name);
    void Release();
};
