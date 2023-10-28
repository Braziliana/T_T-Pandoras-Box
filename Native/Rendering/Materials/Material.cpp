#include "Material.h"

Material::Material(const unsigned int id, Shader* shader)
{
    _id = id;
    _shader = shader;
}

void Material::Use(ID3D11DeviceContext* deviceContext) const
{
    if(_shader == nullptr)
    {
        return;
    }

    _shader->Use(deviceContext);
}

ID3D11InputLayout* Material::GetInputLayout() const
{
    return _shader->GetInputLayout();
}
