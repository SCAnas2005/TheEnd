
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;


public enum Fonts 
{
    Arial_12,
    Arial_36,
    Arial_48,

    Minecraft_12,
    Minecraft_24,
    Minecraft_36,
    Minecraft_48,
}

public static class CFonts
{
    public static Dictionary<Fonts, SpriteFont> _fonts;

    public static void Init()
    {
        _fonts = [];
    }
    public static void Load(ContentManager Content){
        _fonts[Fonts.Arial_12] = Content.Load<SpriteFont>("Fonts/Arial");
        _fonts[Fonts.Arial_36] = Content.Load<SpriteFont>("Fonts/Arial-36");
        _fonts[Fonts.Arial_48] = Content.Load<SpriteFont>("Fonts/Arial-48");
        
        _fonts[Fonts.Minecraft_12] = Content.Load<SpriteFont>("Fonts/Minecraft");
        _fonts[Fonts.Minecraft_24] = Content.Load<SpriteFont>("Fonts/Minecraft-24");
        _fonts[Fonts.Minecraft_36] = Content.Load<SpriteFont>("Fonts/Minecraft-36");
        _fonts[Fonts.Minecraft_48] = Content.Load<SpriteFont>("Fonts/Minecraft-48");

        Arial_12 = _fonts[Fonts.Arial_12];
        Arial_36 = _fonts[Fonts.Arial_36];
        Arial_48 = _fonts[Fonts.Arial_48];


        Minecraft_12 = _fonts[Fonts.Minecraft_12];
        Minecraft_24 = _fonts[Fonts.Minecraft_24];
        Minecraft_36 = _fonts[Fonts.Minecraft_36];
        Minecraft_48 = _fonts[Fonts.Minecraft_48];


        DefaultFont = Minecraft_12;
    }   


    public static SpriteFont Arial_12 {get; private set; }
    public static SpriteFont Arial_36 {get; private set; }
    public static SpriteFont Arial_48 {get; private set; }

    public static SpriteFont Minecraft_12 {get; private set; }
    public static SpriteFont Minecraft_24 {get; private set; }
    public static SpriteFont Minecraft_36 {get; private set; }
    public static SpriteFont Minecraft_48 {get; private set; }

    public static SpriteFont DefaultFont {get; private set; }


    public static SpriteFont GetClosestFontFromSize(int size)
    {
        if (size <= 12)
            return Minecraft_12;
        if (size <= 24)
            return Minecraft_24;
        if (size <= 36)
            return Minecraft_36;
        if (size <= 48)
            return Minecraft_48;
        return Minecraft_48;
    }

    public static float GetScaleFromFontSize(SpriteFont font, int fontSize)
    {
        return 0f;
    }


    public static SpriteFont GetClosestFont(int desiredHeight)
    {
        if (_fonts == null || !_fonts.Any())
            throw new InvalidOperationException("No fonts have been loaded into CFonts.");
        return _fonts
            .OrderBy(pair => Math.Abs(pair.Value.LineSpacing - desiredHeight))
            .First()
            .Value;
    }


}