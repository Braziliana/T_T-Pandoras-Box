#pragma once
#include <string>

#include "Font.h"
#include "../Vertex.h"
#include "../Buffers/InstancedBatchBuffer.h"
#include "../Materials/TexturedMaterial.h"
#include <string>

class FontRenderer
{
private:
    TexturedMaterial* _fontMaterial;
    
    Font* _font;
    InstancedBuffer<VertexPositionUv, FontCharacterInstance>* _buffer2D = nullptr;
    InstancedBuffer<VertexPositionUv, FontCharacterInstance>* _buffer3D = nullptr;
    
public:
    FontRenderer(Font* font);

    void Draw(std::string text, const Vector2& position, const float& scale, const Color& color);
    void Draw(std::string text, const Vector3& position, const Vector3& scale, const Color& color);
    void Flush2D();
    void Flush3D();

    void Release();
};