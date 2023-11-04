#pragma once
#include "Material.h"

class MaterialManager
{
private:
    MaterialManager()
    {
        CreateMaterial(L"Rect", L"Rect");
    }
    std::unordered_map<std::wstring, Material*> _materials;
    
public:
    static MaterialManager& GetInstance() {
        static MaterialManager instance;
        return instance;
    }

    Material* CreateMaterial(const std::wstring& name, const Shader* shader) {
        auto material = GetMaterial(name);
        if(material == nullptr)
        {
            material = new Material(shader);
            AddMaterial(name, material);
        }
        
        return material;
    }

    Material* CreateMaterial(const std::wstring& name, const std::wstring& shaderName) {
        const auto material = GetMaterial(name);
        if(material != nullptr)
        {
           return material;
        }
        
        const auto shader = ShaderManager::GetInstance().CreateShader(shaderName);
        return CreateMaterial(name, shader);
    }
    
    Material* CreateMaterial(const std::wstring& name, const std::wstring& shaderPath, const std::wstring& shaderName) {
        const auto material = GetMaterial(name);
        if(material != nullptr)
        {
            return material;
        }
        
        const auto shader = ShaderManager::GetInstance().CreateShader(shaderPath, shaderName);
        return CreateMaterial(name, shader);
    }

    void AddMaterial(const std::wstring& name, Material* material)
    {
        _materials[name] = material;
    }
    
    Material* GetMaterial(const std::wstring& name)
    {
        auto it = _materials.find(name);
        if (it != _materials.end()) {
            return it->second;
        }

        return nullptr;
    }
};
