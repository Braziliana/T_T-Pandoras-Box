#include "RenderBatch.h"

RenderBatch::RenderBatch(Material* material, const D3D11_PRIMITIVE_TOPOLOGY topology, const size_t size)
{
    _material = material;
    _topology = topology;
    _vertices.reserve(size);
}
