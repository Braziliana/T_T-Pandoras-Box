#pragma once
#include <string>

#include "Shader.h"
#include "ShaderManager.h"

class Material
{
public:
    const Shader* _shader;

    Material(const Shader* shader)
    {
        _shader = shader;
    }
    
    Material(const std::wstring& shaderName)
    {
        _shader = ShaderManager::GetInstance().GetShader(shaderName);
    }

    Material(const std::wstring& shaderPath, const std::wstring& shaderName)
    {
        _shader = ShaderManager::GetInstance().CreateShader(shaderPath, shaderName);
    }

    virtual ~Material()
    {
        Material::Release();
    }

    virtual void Release()
    {
    }
    
    void Use() const
    {
        _shader->Use();
        OnUse();
    }

    virtual void OnUse() const
    {
        
    }

    void SetBool(const std::string& name, bool value) const
    {
        _shader->SetBool(name, value);
    }

    void SetInt(const std::string& name, int value) const
    {
        _shader->SetInt(name, value);
    }

    void SetFloat(const std::string& name, float value) const
    {
        _shader->SetFloat(name, value);
    }

    void SetVec2(const std::string& name, const Vector2& value) const
    {
        _shader->SetVec2(name, value);
    }

    void SetVec3(const std::string& name, const Vector3& value) const
    {
        _shader->SetVec3(name, value);
    }

    void SetColor(const std::string& name, const Color& value) const
    {
        _shader->SetColor(name, value);
    }

    void SetMat4(const std::string& name, const glm::mat4& value) const
    {
        _shader->SetMat4(name, value);
    }

    void SetBool(GLint location, bool value) const
    {
        _shader->SetBool(location, value);
    }

    void SetInt(GLint location, int value) const
    {
        _shader->SetInt(location, value);
    }

    void SetFloat(GLint location, float value) const
    {
        _shader->SetFloat(location, value);
    }

    void SetVec2(GLint location, const Vector2& value) const
    {
        _shader->SetVec2(location, value);
    }

    void SetVec3(GLint location, const Vector3& value) const
    {
        _shader->SetVec3(location, value);
    }

    void SetColor(GLint location, const Color& value) const
    {
        _shader->SetColor(location, value);
    }

    void SetMat4(GLint location, const glm::mat4& value) const
    {
        _shader->SetMat4(location, value);
    }
};
