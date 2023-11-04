#pragma once

#include <unordered_map>
#include <string>
#include <glm/glm.hpp>
#include "Shader.h"

class ShaderManager {
public:
    static ShaderManager& GetInstance();

    ~ShaderManager();

    Shader* CreateShader(const std::wstring& shaderPath, const std::wstring& shaderName);
    Shader* CreateShader(const std::wstring& shaderName);

    Shader* GetShader(const std::wstring& shaderName);

    void SetViewProjectionMatrix(const glm::mat4& matrix) const;

    void Release();

private:
    ShaderManager();

    void UpdateViewProjectionMatrixInShaders(const glm::mat4& matrix) const;

    std::unordered_map<std::wstring, Shader*> _shaders;
};