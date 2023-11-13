#include "Shader.h"
#include <glm/gtc/type_ptr.hpp>

#include <string>
#include <iostream>
#include <fstream>
#include <sstream>
#include <unordered_map>

Shader::Shader(const std::wstring& shaderPath, const std::wstring& shaderName)
{
    std::wstring vertexPath = shaderPath + L"/" + shaderName + L".vert";
    std::wstring fragmentPath = shaderPath + L"/" + shaderName + L".frag";

    std::string vertexCode;
    std::string fragmentCode;
    std::ifstream vShaderFile;
    std::ifstream fShaderFile;

    vShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);
    fShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);

    try
    {
        vShaderFile.open(vertexPath);
        fShaderFile.open(fragmentPath);
        std::stringstream vShaderStream, fShaderStream;

        vShaderStream << vShaderFile.rdbuf();
        fShaderStream << fShaderFile.rdbuf();

        vShaderFile.close();
        fShaderFile.close();

        vertexCode = vShaderStream.str();
        fragmentCode = fShaderStream.str();
    }
    catch (std::ifstream::failure& e)
    {
        std::cerr << "ERROR::SHADER::FILE_NOT_SUCCESSFULLY_READ" << std::endl;
    }

    const char* vShaderCode = vertexCode.c_str();
    const char* fShaderCode = fragmentCode.c_str();

    // 2. Compile shaders
    GLuint vertex, fragment;
    GLint success;
    GLchar infoLog[512];

    vertex = glCreateShader(GL_VERTEX_SHADER);
    glShaderSource(vertex, 1, &vShaderCode, nullptr);
    glCompileShader(vertex);

    glGetShaderiv(vertex, GL_COMPILE_STATUS, &success);
    if (!success)
    {
        glGetShaderInfoLog(vertex, 512, nullptr, infoLog);
        std::cerr << "ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" << infoLog << std::endl;
    }

    fragment = glCreateShader(GL_FRAGMENT_SHADER);
    glShaderSource(fragment, 1, &fShaderCode, nullptr);
    glCompileShader(fragment);

    glGetShaderiv(fragment, GL_COMPILE_STATUS, &success);
    if (!success)
    {
        glGetShaderInfoLog(fragment, 512, nullptr, infoLog);
        std::cerr << "ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" << infoLog << std::endl;
    }

    _program = glCreateProgram();
    glAttachShader(_program, vertex);
    glAttachShader(_program, fragment);
    glLinkProgram(_program);

    glGetProgramiv(_program, GL_LINK_STATUS, &success);
    if (!success)
    {
        glGetProgramInfoLog(_program, 512, nullptr, infoLog);
        std::cerr << "ERROR::SHADER::PROGRAM::LINKING_FAILED\n" << infoLog << std::endl;
    }

    glDeleteShader(vertex);
    glDeleteShader(fragment);
}

Shader::~Shader()
{
    Release();
}

void Shader::Release()
{
    if(_program > 0)
    {
        glDeleteProgram(_program);
        _program = 0;
    }
}

void Shader::Use() const
{
    glUseProgram(_program);
}

GLint Shader::GetUniformLocation(const std::string& name) const
{
    if (_uniformLocations.find(name) != _uniformLocations.end())
    {
        return _uniformLocations[name];
    }

    const GLint location = glGetUniformLocation(_program, name.c_str());
    if (location != -1)
    {
        _uniformLocations[name] = location;
    }
    else
    {
        std::cerr << "Warning: Uniform '" << name << "' not found in shader program!" << std::endl;
    }
    return location;
}

void Shader::SetBool(const std::string& name, const bool value) const
{
    glUniform1i(GetUniformLocation(name), static_cast<int>(value));
}

void Shader::SetInt(const std::string& name, const int value) const
{
    glUniform1i(GetUniformLocation(name), value);
}

void Shader::SetFloat(const std::string& name, const float value) const
{
    glUniform1f(GetUniformLocation(name), value);
}

void Shader::SetVec2(const std::string& name, const Vector2& value) const
{
    glUniform2fv(GetUniformLocation(name), 1, value.xy);
}

void Shader::SetVec3(const std::string& name, const Vector3& value) const
{
    glUniform3fv(GetUniformLocation(name), 1, value.xyz);
}

void Shader::SetColor(const std::string& name, const Color& color) const
{
    glUniform4fv(GetUniformLocation(name), 1, color.rgba);
}

void Shader::SetMat4(const std::string& name, const glm::mat4& mat) const
{
    glUniformMatrix4fv(GetUniformLocation(name), 1, GL_FALSE, glm::value_ptr(mat));
}

void Shader::SetBool(const GLint location, const bool value)
{
    glUniform1i(location, static_cast<int>(value));
}

void Shader::SetInt(const GLint location, const int value)
{
    glUniform1i(location, value);
}

void Shader::SetFloat(const GLint location, const float value)
{
    glUniform1f(location, value);
}

void Shader::SetVec2(const GLint location, const Vector2& value)
{
    glUniform2fv(location, 1, value.xy);
}

void Shader::SetVec3(const GLint location, const Vector3& value)
{
    glUniform3fv(location, 1, value.xyz);
}

void Shader::SetColor(const GLint location, const Color& color)
{
    glUniform4fv(location, 1, color.rgba);
}

void Shader::SetMat4(const GLint location, const glm::mat4& mat)
{
    glUniformMatrix4fv(location, 1, GL_FALSE, glm::value_ptr(mat));
}
