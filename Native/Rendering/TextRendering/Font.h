#pragma once
#include <string>
#include <freetype/freetype.h>
#include <map>

#include "../Materials/Shader.h"
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
    TextureAtlas* _textureAtlas = nullptr;
    std::string _name;
    std::map<char, Character> _characters;

    static unsigned int NextPowerOfTwo(const unsigned int number) {
        unsigned int result = 1;
        while (result < number) result <<= 1;
        return result;
    }
    
public:
    Font(FT_Face face);
    FontCharacterInstance GetInstance(char c, Vector2& position, float scale, Color color);
    FontCharacterInstance GetInstance(char c, Vector3& position, float scale, Color color);
    Vector2 GetTextSize(const std::string& text, float scale);
    void Release();
    TextureAtlas* GetFontTexture() const;
    std::string GetName();
};