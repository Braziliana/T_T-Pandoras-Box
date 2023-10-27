#include "ShaderManager.h"

ShaderManager::ShaderManager(ID3D11Device* device)
{
    _device = device;
}

ShaderManager::~ShaderManager()
{
    Dispose();
}

bool ShaderManager::LoadShader(const std::string& name, const std::wstring& shaderPath)
{
    const auto shader = new Shader();
    if(!shader->Initialize(_device, shaderPath))
    {
        delete shader;
        return false;
    }

    _shaders.insert({name, shader});
    
    return true;
}

Shader* ShaderManager::GetShader(const std::string& name)
{
    const auto shaderIter = _shaders.find(name);
    if (shaderIter != _shaders.end())
    {
        return shaderIter->second;
    }

    return nullptr;
}

void ShaderManager::Dispose()
{
    for (const auto& shaderItem : _shaders)
    {
        delete shaderItem.second;
    }

    _shaders.clear();
}
