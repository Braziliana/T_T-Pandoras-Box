#include "TexturedMaterial.h"

TexturedMaterial::TexturedMaterial(const Texture* texture, const Shader* shader): Material(shader)
{
    _texture = texture;
    shader->Use();
    shader->SetInt("textureSampler", 0);
}

TexturedMaterial::~TexturedMaterial()
{
    TexturedMaterial::Release();
}

void TexturedMaterial::Begin() const
{
    Material::Begin();
    _texture->Bind();
}

void TexturedMaterial::End() const
{
    Material::End();
    Texture::UnBind();
}

void TexturedMaterial::Release()
{
    Material::Release();
}
