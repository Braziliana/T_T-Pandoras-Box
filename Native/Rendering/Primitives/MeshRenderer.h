#pragma once
#include "../Vertex.h"
#include "../Buffers/InstancedBatchBuffer.h"
#include "../../Math/Color.h"
#include "../../Math/Vector3.h"

struct MeshData
{
    Vector3 position;
    Vector3 scale;
    Color color;
};

class MeshRenderer
{
private:
    Material* _material = nullptr;
    InstancedBuffer<Vertex, MeshData>* _buffer2D = nullptr;
    InstancedBuffer<Vertex, MeshData>* _buffer3D = nullptr;
};
