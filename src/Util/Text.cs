using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheEnd;

public static class Text
{
    public static SpriteFont _font = CFonts.DefaultFont;

    public static void Write(SpriteBatch _spriteBatch, string text, Vector2 position, Color color)
    {
        if (_font != null)
        {
            _spriteBatch.DrawString(_font, text, position, color);
        }
    }

    public static void Write(SpriteBatch _spriteBatch, string text, Vector2 position, Color color, SpriteFont font)
    {
        _spriteBatch.DrawString(font, text, position, color);
    }


    public static void Write(SpriteBatch _spriteBatch, string text, Vector2 position, Color color, float scale)
    {
        if (_font != null)
        {
            _spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

    public static Size GetSize(string text)
    {
        Vector2 v = _font.MeasureString(text);
        return new Size((int)v.X, (int)v.Y);
    }

    public static Size GetSize(string text, float scale)
    {
        Vector2 v = _font.MeasureString(text);
        return new Size((int)(v.X*scale), (int)(v.Y*scale));
    }

    public static Size GetSize(string text, SpriteFont font)
    {
        if (text == "")
        {
            return new Size(0, font.LineSpacing);
        }
        Vector2 v = font.MeasureString(text);
        return new Size((int)v.X, (int)v.Y);
    }


    public static int GetMaxCharCount(string sampleText, SpriteFont font, Rectangle rect)
    {
        // Méthode Pas précise
        // Utilise un échantillon pour estimer la largeur moyenne d'un caractère
        if (string.IsNullOrEmpty(sampleText)) sampleText = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        Vector2 size = font.MeasureString(sampleText);
        float avgCharWidth = size.X / sampleText.Length;

        // Nombre max de caractères qui peuvent tenir dans la largeur du rectangle
        int maxChars = (int)(rect.Width / avgCharWidth);

        return maxChars;
    }

    public static string GetFittingText(string text, SpriteFont font, Rectangle rect)
{
    if (string.IsNullOrEmpty(text))
        return "";

    float totalWidth = 0f;
    int charCount = 0;

    foreach (char c in text)
    {
        float charWidth = font.MeasureString(c.ToString()).X;

        if (totalWidth + charWidth > rect.Width)
            break;

        totalWidth += charWidth;
        charCount++;
    }

    // Console.WriteLine($"Total width for string {text} : {totalWidth}, returned_string:{text.Substring(0, charCount)}, char_number:{charCount}, max_width:{rect.Width}");
    return text.Substring(0, charCount);
}
}