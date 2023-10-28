#include "MaterialManager.h"

bool MaterialManager::Init()
{
    _rectMaterial = new Material(0, _shaderManager->GetShader("Rect"));
    _circleMaterial = new Material(1, _shaderManager->GetShader("Circle"));
    return true;
}

void MaterialManager::AddMaterial(Material* material)
{
    if(material == nullptr)
    {
        return;
    }
}

void MaterialManager::Release()
{
    delete _rectMaterial;
    _rectMaterial = nullptr;
    delete _circleMaterial;
    _circleMaterial = nullptr;
}

Material* MaterialManager::GetRectMaterial()
{
    return _rectMaterial;
}

Material* MaterialManager::GetCircleMaterial()
{
    return _circleMaterial;
}
