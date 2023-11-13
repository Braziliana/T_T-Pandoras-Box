#pragma once
#include "Material.h"
#include "../Textures/Texture.h"

class TexturedMaterial : public Material
{
private:
    const Texture* _texture;
public:

    TexturedMaterial(const Texture* texture, const Shader* shader);

    ~TexturedMaterial() override;

    void Begin() const override;

    void End() const override;

    void Release() override;
};
