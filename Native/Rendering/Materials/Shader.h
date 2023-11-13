#pragma once

#define GLEW_STATIC
#include <GL/glew.h>
#include <glm/glm.hpp>

#include <string>
#include <fstream>
#include <unordered_map>

#include "../../Math/Color.h"
#include "../../Math/Vector2.h"
#include "../../Math/Vector3.h"


class Shader
{
private:
    GLuint _program;
    mutable std::unordered_map<std::string, GLint> _uniformLocations;

public:

    Shader(const std::wstring& shaderPath, const std::wstring& shaderName);
    ~Shader();
    void Release();
    void Use() const;
    GLint GetUniformLocation(const std::string& name) const;
    void SetBool(const std::string& name, bool value) const;
    void SetInt(const std::string& name, int value) const;
    void SetFloat(const std::string& name, float value) const;
    void SetVec2(const std::string& name, const Vector2& value) const;
    void SetVec3(const std::string& name, const Vector3& value) const;
    void SetColor(const std::string& name, const Color& color) const;
    void SetMat4(const std::string& name, const glm::mat4& mat) const;
    static void SetBool(GLint location, bool value);
    static void SetInt(GLint location, int value);
    static void SetFloat(GLint location, float value);
    static void SetVec2(GLint location, const Vector2& value);
    static void SetVec3(GLint location, const Vector3& value);
    static void SetColor(GLint location, const Color& color);
    static void SetMat4(GLint location, const glm::mat4& mat);
};
