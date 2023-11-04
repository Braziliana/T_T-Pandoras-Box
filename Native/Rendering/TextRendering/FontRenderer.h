#pragma once
#include <iostream>
#include <string>
#include <ft2build.h>
#include <map>

#include <glm/vec2.hpp>

#include "Font.h"
#include "../Vertex.h"
#include "../../Buffers/InstancedBatchBuffer.h"
#include "../../Materials/Shader.h"
#include "../../Materials/ShaderManager.h"
#include "GL/glew.h"
#include <string>

class FontRenderer
{
private:
    unsigned int _textureId;
    Font* _font;
    InstancedBuffer<VertexPositionUv, FontCharacterInstance>* _buffer2D = nullptr;
    InstancedBuffer<VertexPositionUv, FontCharacterInstance>* _buffer3D = nullptr;
    
public:
    FontRenderer(Font* font);

    void Draw(std::string text, const Vector2& position, const float& scale, const Color& color);
    void Draw(std::string text, const Vector3& position, const Vector3& scale, const Color& color);
    void Flush2D();
    void Flush3D();
};