
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public static class Shape{

    private static Texture2D pixel;

    public static void Init(GraphicsDevice graphicsDevice) {
        pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData([Color.White]);
    }

    public static void DrawRectangle(SpriteBatch _spriteBatch, Rectangle rect, Color color) {
        _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 1), color); // Haut
        _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 1, rect.Height), color); // Gauche
        _spriteBatch.Draw(pixel, new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), color); // Droite
        _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), color); // Bas
    }

    public static void FillRectangle(SpriteBatch _spriteBatch, Rectangle rect, Color color) {
        _spriteBatch.Draw(pixel, rect, color);
    }


    public static void DrawCircle(SpriteBatch _spriteBatch, Vector2 position, int radius, Color color) {
        int outerRadius = radius*2+2;
        Texture2D newpixel = new Texture2D(Globals.Graphics.GraphicsDevice, outerRadius, outerRadius);
        Color[] data = new Color[outerRadius * outerRadius];

        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Transparent;
        
        double angleStep = 1f/radius;

        for (double angle = 0; angle < Math.PI*2; angle+=angleStep) {
            int x = (int)Math.Round(radius+radius*Math.Cos(angle));
            int y = (int)Math.Round(radius+radius*Math.Sin(angle));

            data[y*outerRadius+x+1] = Color.White;
        }

        newpixel.SetData(data);

        _spriteBatch.Draw(newpixel, position, color);
    }
    
    public static void FillCercle(SpriteBatch _spriteBatch, Vector2 position, int radius, Color color) {
        int outerRadius = radius*2+2;
        Texture2D newpixel = new Texture2D(Globals.Graphics.GraphicsDevice, outerRadius, outerRadius);
        Color[] data = new Color[outerRadius * outerRadius];

        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Transparent;

        for (int y = -radius; y <= radius; y++) // Parcours en hauteur
        {
            for (int x = -radius; x <= radius; x++) // Parcours en largeur
            {
                if (x * x + y * y <= radius * radius) // Vérifie si (x,y) est dans le cercle
                {
                    int px = x + radius;
                    int py = y + radius;
                    data[py * outerRadius + px] = Color.White;
                }
            }
        }

        newpixel.SetData(data);

        _spriteBatch.Draw(newpixel, position, color);
    }


    public static void DrawLine(SpriteBatch _spriteBatch, Vector2 start, Vector2 end, Color color, float thickness=1f)
    {
        Vector2 edge = end-start; // Direction de la ligne
        float angle = (float)Math.Atan2(edge.Y, edge.X); // Angle entre les 2 points
        float length = edge.Length(); // Longueur de la ligne
        _spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
    }

    public static Texture2D CreateRoundedRectangleTexture(GraphicsDevice graphics, int width, int height, int borderThickness, int borderRadius, int borderShadow, List<Color> backgroundColors, List<Color> borderColors, float initialShadowIntensity, float finalShadowIntensity)
    {
        if (backgroundColors == null || backgroundColors.Count == 0) throw new ArgumentException("Must define at least one background color (up to four).");
        if (borderColors == null || borderColors.Count == 0) throw new ArgumentException("Must define at least one border color (up to three).");
        if (borderRadius < 1) throw new ArgumentException("Must define a border radius (rounds off edges).");
        if (borderThickness < 1) throw new ArgumentException("Must define border thikness.");
        if (borderThickness + borderRadius > height / 2 || borderThickness + borderRadius > width / 2) throw new ArgumentException("Border will be too thick and/or rounded to fit on the texture.");
        if (borderShadow > borderRadius) throw new ArgumentException("Border shadow must be lesser in magnitude than the border radius (suggeted: shadow <= 0.25 * radius).");

        Texture2D texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
        Color[] color = new Color[width * height];

        for (int x = 0; x < texture.Width; x++)
        {
            for (int y = 0; y < texture.Height; y++)
            {
                switch (backgroundColors.Count)
                {
                    case 4:
                        Color leftColor0 = Color.Lerp(backgroundColors[0], backgroundColors[1], ((float)y / (width - 1)));
                        Color rightColor0 = Color.Lerp(backgroundColors[2], backgroundColors[3], ((float)y / (height - 1)));
                        color[x + width * y] = Color.Lerp(leftColor0, rightColor0, ((float)x / (width - 1)));
                        break;
                    case 3:
                        Color leftColor1 = Color.Lerp(backgroundColors[0], backgroundColors[1], ((float)y / (width - 1)));
                        Color rightColor1 = Color.Lerp(backgroundColors[1], backgroundColors[2], ((float)y / (height - 1)));
                        color[x + width * y] = Color.Lerp(leftColor1, rightColor1, ((float)x / (width - 1)));
                        break;
                    case 2:
                        color[x + width * y] = Color.Lerp(backgroundColors[0], backgroundColors[1], ((float)x / (width - 1)));
                        break;
                    default:
                        color[x + width * y] = backgroundColors[0];
                        break;
                }

                color[x + width * y] = ColorBorder(x, y, width, height, borderThickness, borderRadius, borderShadow, color[x + width * y], borderColors, initialShadowIntensity, finalShadowIntensity);
            }
        }

        texture.SetData<Color>(color);
        return texture;
    }

    private static Color ColorBorder(int x, int y, int width, int height, int borderThickness, int borderRadius, int borderShadow, Color initialColor, List<Color> borderColors, float initialShadowIntensity, float finalShadowIntensity)
    {
        Rectangle internalRectangle = new Rectangle((borderThickness + borderRadius), (borderThickness + borderRadius), width - 2 * (borderThickness + borderRadius), height - 2 * (borderThickness + borderRadius));

        if (internalRectangle.Contains(x, y)) return initialColor;

        Vector2 origin = Vector2.Zero;
        Vector2 point = new Vector2(x, y);

        if (x < borderThickness + borderRadius)
        {
            if (y < borderRadius + borderThickness)
                origin = new Vector2(borderRadius + borderThickness, borderRadius + borderThickness);
            else if (y > height - (borderRadius + borderThickness))
                origin = new Vector2(borderRadius + borderThickness, height - (borderRadius + borderThickness));
            else
                origin = new Vector2(borderRadius + borderThickness, y);
        }
        else if (x > width - (borderRadius + borderThickness))
        {
            if (y < borderRadius + borderThickness)
                origin = new Vector2(width - (borderRadius + borderThickness), borderRadius + borderThickness);
            else if (y > height - (borderRadius + borderThickness))
                origin = new Vector2(width - (borderRadius + borderThickness), height - (borderRadius + borderThickness));
            else
                origin = new Vector2(width - (borderRadius + borderThickness), y);
        }
        else
        {
            if (y < borderRadius + borderThickness)
                origin = new Vector2(x, borderRadius + borderThickness);
            else if (y > height - (borderRadius + borderThickness))
                origin = new Vector2(x, height - (borderRadius + borderThickness));
        }

        if (!origin.Equals(Vector2.Zero))
        {
            float distance = Vector2.Distance(point, origin);

            if (distance > borderRadius + borderThickness + 1)
            {
                return Color.Transparent;
            }
            else if (distance > borderRadius + 1)
            {
                if (borderColors.Count > 2)
                {
                    float modNum = distance - borderRadius;

                    if (modNum < borderThickness / 2)
                    {
                        return Color.Lerp(borderColors[2], borderColors[1], (float)((modNum) / (borderThickness / 2.0)));
                    }
                    else
                    {
                        return Color.Lerp(borderColors[1], borderColors[0], (float)((modNum - (borderThickness / 2.0)) / (borderThickness / 2.0)));
                    }
                }


                if (borderColors.Count > 0)
                    return borderColors[0];
            }
            else if (distance > borderRadius - borderShadow + 1)
            {
                float mod = (distance - (borderRadius - borderShadow)) / borderShadow;
                float shadowDiff = initialShadowIntensity - finalShadowIntensity;
                return DarkenColor(initialColor, ((shadowDiff * mod) + finalShadowIntensity));
            }
        }

        return initialColor;
    }

    private static Color DarkenColor(Color color, float shadowIntensity)
    {
        return Color.Lerp(color, Color.Black, shadowIntensity);
    }

    public static void DrawRoundedRectangle(SpriteBatch _spriteBatch, Rectangle rect, int borderThickness, int borderRadius, List<Color> backgroundsColors, List<Color> borderColors)
    {
        Texture2D texture = CreateRoundedRectangleTexture(Globals.Graphics.GraphicsDevice, rect.Width, rect.Height, borderThickness, borderRadius, 0, backgroundsColors, borderColors, 0, 0);
        _spriteBatch.Draw(texture, new Vector2(rect.X, rect.Y), Color.White);
    }

}
