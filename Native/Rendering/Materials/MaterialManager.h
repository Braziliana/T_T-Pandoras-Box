#pragma once
#include "Material.h"
#include "../Shaders/Shader.h"
#include "../Shaders/ShaderManager.h"

class MaterialManager
{
private:
    ShaderManager* _shaderManager = nullptr;
    Material* _rectMaterial = nullptr;
    Material* _circleMaterial = nullptr;
public:
    MaterialManager(ShaderManager* shaderManager)
    {
        _shaderManager = shaderManager;
    }
    
    bool Init();
    void AddMaterial(Material* material);
    void Release();
    Material* GetRectMaterial();
    Material* GetCircleMaterial();
};
