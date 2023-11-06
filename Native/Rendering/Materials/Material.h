#pragma once
#include <string>

#include "Shader.h"
#include "ShaderManager.h"

class Material
{
private:
    const Shader* _shader = nullptr;
    
public:

    Material(const Shader* shader);
    virtual ~Material();
    virtual void Release();
    virtual void Begin() const;
    virtual void End() const;
    void SetBool(const std::string& name, bool value) const;
    void SetInt(const std::string& name, int value) const;
    void SetFloat(const std::string& name, float value) const;
    void SetVec2(const std::string& name, const Vector2& value) const;
    void SetVec3(const std::string& name, const Vector3& value) const;
    void SetColor(const std::string& name, const Color& value) const;
    void SetMat4(const std::string& name, const glm::mat4& value) const;
    void SetBool(GLint location, bool value) const;
    void SetInt(GLint location, int value) const;
    void SetFloat(GLint location, float value) const;
    void SetVec2(GLint location, const Vector2& value) const;
    void SetVec3(GLint location, const Vector3& value) const;
    void SetColor(GLint location, const Color& value) const;
    void SetMat4(GLint location, const glm::mat4& value) const;
    void BindShader() const;
};
