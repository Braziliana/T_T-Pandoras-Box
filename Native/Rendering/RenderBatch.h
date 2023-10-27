#pragma once
#include <d3d11.h>
#include <vector>

#include "Vertex.h"
#include "Materials/Material.h"

class RenderBatch
{
private:
    Material* _material;
    D3D11_PRIMITIVE_TOPOLOGY _topology;
    std::vector<Vertex>	_vertices;

public:
    RenderBatch() = delete;
    RenderBatch(Material* material, D3D11_PRIMITIVE_TOPOLOGY topology, size_t size);
    
};
