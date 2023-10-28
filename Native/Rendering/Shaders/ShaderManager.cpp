#include "ShaderManager.h"

#include <iostream>

ShaderManager::ShaderManager(ID3D11Device* device)
{
    _device = device;
}

ShaderManager::~ShaderManager()
{
    Release();
}

bool LoadRectShader(ShaderManager* shaderManager)
{
    D3D11_INPUT_ELEMENT_DESC layoutDesc[] =
    {
        { "POSITION",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "TEXCOORD",  0, DXGI_FORMAT_R32G32_FLOAT,    0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "COLOR",     0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0 },
                
        { "INSTANCEPOSITION",     0, DXGI_FORMAT_R32G32B32_FLOAT, 1, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_INSTANCE_DATA, 1 },
        { "INSTANCESCALE",     0, DXGI_FORMAT_R32G32B32_FLOAT, 1, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_INSTANCE_DATA, 1 },
        { "INSTANCECOLOR",     0, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_INSTANCE_DATA, 1 },
    };

    return shaderManager->LoadShader("Rect", L"Shaders/", L"Rect", layoutDesc, ARRAYSIZE(layoutDesc));
}

bool ShaderManager::LoadDefaultShaders()
{
    if(!LoadRectShader(this))
    {
        return false;
    }
    
    return true;
}

bool ShaderManager::LoadShader(const std::string& name, const std::wstring& shaderPath, const std::wstring& fileName, D3D11_INPUT_ELEMENT_DESC* vertexLayoutDesc, size_t size)
{
    const auto shader = new Shader();
    std::wcout << "Load shader: " << fileName << std::endl;
    if(!shader->Initialize(_device, shaderPath, fileName, vertexLayoutDesc, size))
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

void ShaderManager::Release()
{
    for (const auto& shaderItem : _shaders)
    {
        delete shaderItem.second;
    }

    _shaders.clear();
}
