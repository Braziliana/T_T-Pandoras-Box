#pragma once
#include "../Shaders/Shader.h"

class Material
{
private:
    unsigned int _id = 0;
    Shader* _shader = nullptr;
public:
    Material() = delete;
    Material(unsigned int id, Shader* shader);
    void Use(ID3D11DeviceContext* deviceContext) const;
    ID3D11InputLayout* GetInputLayout() const;
};
