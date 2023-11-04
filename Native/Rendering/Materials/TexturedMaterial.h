#pragma once
#include "Material.h"
#include "../Textures/Texture.h"

class TexturedMaterial : Material
{
private:
    const Texture* _texture;
public:

    TexturedMaterial(const Texture* texture, const Shader* shader) : Material(shader)
    {
        _texture = texture;
        shader->Use();
        shader->SetInt("textureSampler", 0);
    }
    
    ~TexturedMaterial() override
    {
        TexturedMaterial::Release();
    }

    void Begin() const override
    {
        Material::Begin();
        _texture->Bind();
    }

    void End() const override
    {
        Material::End();
        _texture->UnBind();
    }

    void Release() override
    {
        Material::Release();
    }
};
