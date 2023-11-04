#pragma once
#include <iostream>
#include <string>
#include <map>

#include "../../Materials/Shader.h"
#include "../../Materials/ShaderManager.h"
#include "Font.h"
#include "FontRenderer.h"

class TextRenderer
{
private:
    FontRenderer* _default; 
    std::map<std::string, FontRenderer*> _fonts;
public:
    TextRenderer()
    {
        if(LoadFont("default", "Resources/Fonts/OpenSans-Regular.ttf"))
        {
            _default = _fonts["default"];
        }
    }
    
    bool LoadFont(const std::string& name, const std::string& fontFilePath)
    {
        FT_Library ft;
        if (FT_Init_FreeType(&ft)) {
            std::cout << "ERROR::FREETYPE: Could not init FreeType Library\n" << std::endl;
            return false;
        }

        FT_Face face;
        if (FT_New_Face(ft, fontFilePath.c_str(), 0, &face)) {
            std::cout << "ERROR::FREETYPE: Failed to load font\n" << std::endl;
            return false;
        }

        FT_Set_Pixel_Sizes(face, 0, 26);
        const auto font = new Font(face);
        const auto fontRenderer = new FontRenderer(font);
        _fonts[name] = fontRenderer;
       
        FT_Done_Face(face);
        FT_Done_FreeType(ft);
        
        return  true;
    }

    void Render(const std::string& text, const Vector2& position, float scale, const Color& color)
    {
        _default->Draw(text, position, scale, color);
    }

    void Release()
    {
        _default = nullptr;
        // for (const auto& font : _fonts)
        // {
        //     font.second->Release();
        // }
        _fonts.clear();
    }

    void Flush2D()
    {
        _default->Flush2D();
    }
};
