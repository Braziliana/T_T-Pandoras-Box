#pragma once
#include "../Shaders/Shader.h"

class Material
{
private:
    Shader* _shader = nullptr;
public:
    Material() = delete;
    Material(Shader* shader);
    void Use(ID3D11DeviceContext* deviceContext) const;
};
