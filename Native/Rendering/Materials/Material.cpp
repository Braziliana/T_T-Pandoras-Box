#include "Material.h"

Material::Material(const Shader* shader)
{
    _shader = shader;
}

Material::~Material()
{
    Material::Release();
}

void Material::Release()
{
}

void Material::Begin() const
{
    _shader->Use();
}

void Material::End() const
{
}

void Material::SetBool(const std::string& name, const bool value) const
{
    _shader->Use();
    _shader->SetBool(name, value);
}

void Material::SetInt(const std::string& name, const int value) const
{
    _shader->Use();
    _shader->SetInt(name, value);
}

void Material::SetFloat(const std::string& name, const float value) const
{
    _shader->Use();
    _shader->SetFloat(name, value);
}

void Material::SetVec2(const std::string& name, const Vector2& value) const
{
    _shader->Use();
    _shader->SetVec2(name, value);
}

void Material::SetVec3(const std::string& name, const Vector3& value) const
{
    _shader->Use();
    _shader->SetVec3(name, value);
}

void Material::SetColor(const std::string& name, const Color& value) const
{
    _shader->Use();
    _shader->SetColor(name, value);
}

void Material::SetMat4(const std::string& name, const glm::mat4& value) const
{
    _shader->Use();
    _shader->SetMat4(name, value);
}

void Material::SetBool(const GLint location, const bool value) const
{
    _shader->Use();
    Shader::SetBool(location, value);
}

void Material::SetInt(const GLint location, const int value) const
{
    _shader->Use();
    Shader::SetInt(location, value);
}

void Material::SetFloat(const GLint location, const float value) const
{
    _shader->Use();
    Shader::SetFloat(location, value);
}

void Material::SetVec2(const GLint location, const Vector2& value) const
{
    _shader->Use();
    Shader::SetVec2(location, value);
}

void Material::SetVec3(const GLint location, const Vector3& value) const
{
    _shader->Use();
    Shader::SetVec3(location, value);
}

void Material::SetColor(const GLint location, const Color& value) const
{
    _shader->Use();
    Shader::SetColor(location, value);
}

void Material::SetMat4(const GLint location, const glm::mat4& value) const
{
    _shader->Use();
    Shader::SetMat4(location, value);
}

void Material::BindShader() const
{
    _shader->Use();
}
