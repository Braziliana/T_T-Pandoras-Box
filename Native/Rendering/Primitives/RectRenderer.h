#pragma once
#include "../Vertex.h"
#include "../Buffers/InstancedBatchBuffer.h"
#include "../../Math/Color.h"
#include "../../Math/Vector3.h"

struct RectData
{
    Vector3 position;
    Vector3 scale;
    Color color;
};

class RectRenderer
{
private:
    Material* _material = nullptr;
    InstancedBuffer<Vertex, RectData>* _buffer2D = nullptr;
    InstancedBuffer<Vertex, RectData>* _buffer3D = nullptr;
public:
    RectRenderer();
    ~RectRenderer();
    void Release();
    
    void Draw(const Vector2& position, const Vector2& size, const Color& color) const;
    void Draw(const Vector3& position, const Vector2& size, const Color& color) const;
    void Flush2D() const;
    void Flush3D() const;
};
