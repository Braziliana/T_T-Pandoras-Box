#include "Material.h"

Material::Material(Shader* shader)
{
    _shader = shader;
}

void Material::Use(ID3D11DeviceContext* deviceContext) const
{
    _shader->Use(deviceContext);
}
