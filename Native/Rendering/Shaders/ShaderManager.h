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
    bool LoadShader(const std::string& name, const std::wstring& shaderPath);
    Shader* GetShader(const std::string& name);
    void Dispose();
};
