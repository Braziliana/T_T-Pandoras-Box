#pragma once
#include <iostream>
#include <string>
#include <ft2build.h>
#include <freetype/freetype.h>
#include <map>

#include "../Materials/Shader.h"
#include "../Materials/ShaderManager.h"
#include "../Textures/TextureAtlas.h"


struct Character {
    Vector2 size;
    Vector2 bearing;
    Vector2 uv;
    Vector2 uvSize;
    float advance;
};

struct FontCharacterInstance 
{
    Vector3 position;
    Vector3 scale;
    Vector2 uv;
    Vector2 uvSize;
    Color color;
};


class Font
{
private:
    TextureAtlas* _textureAtlas;
    std::string _name;
    std::map<char, Character> _characters;

    static unsigned int NextPowerOfTwo(const unsigned int number) {
        unsigned int result = 1;
        while (result < number) result <<= 1;
        return result;
    }
    
public:
    Font(FT_Face face)
    {
        int maxWidth = 0;
        int maxHeight = 0;
        int numGlyphs = 0;
        for (unsigned char c = 32; c < 127; ++c) {
            if (FT_Load_Char(face, c, FT_LOAD_RENDER)) {
                std::cerr << "Failed to load Glyph" << std::endl;
                continue;
            }
            maxWidth = std::max(maxWidth, static_cast<int>(face->glyph->bitmap.width));
            maxHeight = std::max(maxHeight, static_cast<int>(face->glyph->bitmap.rows));
            numGlyphs++;
        }

        const unsigned int itemSize = std::max(maxWidth, maxHeight);
        const unsigned int side = static_cast<unsigned int>(std::ceil(std::sqrt(numGlyphs)));
        const unsigned int atlasSize = NextPowerOfTwo(itemSize * side);
        const unsigned int dataSize = atlasSize*atlasSize;
        auto* data = new unsigned char[dataSize];
        memset(data, 0, dataSize);
        
        unsigned int x = 0, y = 0;
        for (char c = 32; c < 127; ++c) {
            if (FT_Load_Char(face, c, FT_LOAD_RENDER)) continue;

            const auto& glyph = face->glyph;

            const FT_Bitmap bitmap = glyph->bitmap;
            for (unsigned int i = 0; i < bitmap.rows; i++)
            {
                for (unsigned int j = 0; j < bitmap.width; j++) {
                    const unsigned int index = i * abs(bitmap.pitch) + j;
                    const unsigned char pixelValue = bitmap.buffer[index];

                    const unsigned int atlasRow = y * itemSize + i;
                    const unsigned int atlasCol = x * itemSize + j;
                    const unsigned int dataIndex = atlasRow * atlasSize + atlasCol;

                    data[dataIndex] = pixelValue;
                }
            }

            _characters[c] = {
                //size
                {static_cast<float>(glyph->bitmap.width), static_cast<float>(glyph->bitmap.rows)},
                //bearing
                {static_cast<float>(glyph->bitmap_left), static_cast<float>(glyph->bitmap_top)},
                // UV (bottom-left of the glyph in the atlas)
                 {
                     static_cast<float>(x * itemSize) / static_cast<float>(atlasSize),
                     static_cast<float>(y * itemSize) / static_cast<float>(atlasSize)
                 },
                 // UV Size (how big the glyph is in the atlas)
                 {
                     static_cast<float>(glyph->bitmap.width) / static_cast<float>(atlasSize),
                     static_cast<float>(glyph->bitmap.rows) / static_cast<float>(atlasSize)
                 },
                //advance
                static_cast<float>(glyph->advance.x >> 6)
            };

            x++;
            if (x >= side) {
                x = 0;
                y++;
            }
        }
        
        glPixelStorei(GL_PACK_ALIGNMENT, 1);
        _textureAtlas = new TextureAtlas(static_cast<int>(atlasSize), static_cast<int>(atlasSize), TextureType::Single, static_cast<int>(side), static_cast<int>(side), data);
        _textureAtlas->SaveToFile();
    }

    FontCharacterInstance GetInstance(char c, Vector2& position, float scale, Color color)
    {
        if(c < 32 || c >= 127)
        {
            c = '?';
        }
        
        const Character ch = _characters[c];

        float xPos = position.x + (ch.bearing.x * scale);
        float yPos = position.y - ch.bearing.y * scale;

        position.x += (ch.advance * scale); 
        return  {
            {xPos, yPos, 0.0f},
            {ch.size.x * scale, ch.size.y * scale, 1.0f},
            ch.uv,
            ch.uvSize,
            color
        };

    }
    
    void Release()
    {
        _characters.clear();
        if(_textureAtlas != nullptr)
        {
            _textureAtlas->Release();
            delete _textureAtlas;
            _textureAtlas = nullptr;
        }
    }

    TextureAtlas* GetFontTexture() const
    {
        return _textureAtlas;
    }
    
    std::string GetName()
    {
        return _name;
    }
};