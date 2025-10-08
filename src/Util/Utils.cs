

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class Utils
{
    public static float GetValueByPercentage(float value, float percentage)
    {
        return value * (percentage / 100f);
    }

    public static int GetValueByPercentage(int value, int percentage)
    {
        return value * percentage / 100;
    }


    public static float GetPercentageByValue(float a, float b)
    {
        return b * 100 / a;
    }

    public static int GetPercentageByValue(int a, int b)
    {
        return b * 100 / a;
    }

    public static Random Random = new Random();
    public static float NextFloat(float min, float max)
    {
        return (float)(Random.NextDouble() * (max - min) + min);
    }
    public static int NextInt(int min, int max)
    {
        return Random.Next(min, max);
    }
    public static double NextDouble(double min, double max)
    {
        return Random.NextDouble() * (max - min) + min;
    }



    public static Rectangle AddToRect(Rectangle a, Rectangle b)
    {
        return new Rectangle(
            a.X + b.X,
            a.Y + b.Y,
            a.Width + b.Width,
            a.Height + b.Height
        );
    }

    public static Rectangle AddToRect(Rectangle a, int x = 0, int y = 0, int width = 0, int height = 0)
    {
        return AddToRect(a, new Rectangle(x, y, width, height));
    }

    public static Rectangle ExpandRect(Rectangle rect, int expand)
    {
        return new Rectangle(
            rect.X - expand,
            rect.Y - expand,
            rect.Width + expand * 2,
            rect.Height + expand * 2
        );
    }


    public static (int, int) AddTuples((int, int) a, (int, int) b)
    {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2);
    }


    public static (T, T) ReverseTuple<T>((T, T) a)
    {
        return (a.Item2, a.Item1);
    }

    public static string ListToString<T>(List<T> list)
    {
        string s = "[";

        for (int i = 0; i < list.Count; i++)
        {
            s += list[i].ToString();
            if (i < list.Count - 1)
            {
                s += ",";
            }
        }

        s += "]";
        return s;
    }

    public static Dictionary<T1, T2> MergeDictionaries<T1, T2>(
      Dictionary<T1, T2> dict1,
      Dictionary<T1, T2> dict2,
      bool overwrite = true  // optionnel : écraser ou non les clés existantes
      )
    {
        var result = new Dictionary<T1, T2>(dict1);

        foreach (var pair in dict2)
        {
            if (overwrite || !result.ContainsKey(pair.Key))
            {
                result[pair.Key] = pair.Value;
            }
        }

        return result;
    }




    public static MapScene? StringMapNameToMapScene(string name)
    {
        MapScene? s = null;
        switch (name.ToLower())
        {
            case "map":
                s = MapScene.City1;
                break;
            case "home_map":
                s = MapScene.Home;
                break;
            case "grange_inside":
                s = MapScene.Grange1;
                break;
            case "gas_station_map":
                s = MapScene.GasStation1;
                break;
            case "lab":
                s = MapScene.Labo;
                break;
        }
        return s;
    }

    public static bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        float denominator = ((b2.Y - b1.Y) * (a2.X - a1.X)) - ((b2.X - b1.X) * (a2.Y - a1.Y));
        if (denominator == 0)
            return false;

        float numerator1 = ((b2.X - b1.X) * (a1.Y - b1.Y)) - ((b2.Y - b1.Y) * (a1.X - b1.X));
        float numerator2 = ((a2.X - a1.X) * (a1.Y - b1.Y)) - ((a2.Y - a1.Y) * (a1.X - b1.X));
        float r = numerator1 / denominator;
        float s = numerator2 / denominator;
        return r >= 0 && r <= 1 && s >= 0 && s <= 1;
    }

    public static bool SegmentIntersectsRect(Vector2 a1, Vector2 a2, Rectangle rect)
    {
        // Les 4 coins du rectangle
        Vector2 topLeft = new Vector2(rect.Left, rect.Top);
        Vector2 topRight = new Vector2(rect.Right, rect.Top);
        Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);
        Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);

        // Tester contre les 4 côtés
        return
            LineIntersectsLine(a1, a2, topLeft, topRight) || // Haut
            LineIntersectsLine(a1, a2, topRight, bottomRight) || // Droite
            LineIntersectsLine(a1, a2, bottomRight, bottomLeft) || // Bas
            LineIntersectsLine(a1, a2, bottomLeft, topLeft);     // Gauche
    }


    public static void PrintList<T>(List<T> list)
    {
        foreach (var item in list)
        {
            Console.Write($"{item}, ");
        }
        Console.WriteLine();
    }


    public static Texture2D ExtractSubTexture(GraphicsDevice graphics, Texture2D source, Rectangle sourceRect)
    {
        Color[] data = new Color[sourceRect.Width * sourceRect.Height];
        source.GetData(0, sourceRect, data, 0, data.Length);

        Texture2D newTexture = new Texture2D(graphics, sourceRect.Width, sourceRect.Height);
        newTexture.SetData(data);


        return newTexture;
    }
}



public class Size
{
    public int Width { get; set; }
    public int Height { get; set; }

    public static Size Empty => new Size(0, 0);

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public Size(int value) : this(width: value, height: value) { }

    public Size() : this(value: 0) { }

    public Size Add(Size s) { return new Size(Width + s.Width, Height + s.Height); }
    public Size Add(int width = 0, int height = 0) { return new Size(Width + width, Height + height); }

    public static Size operator +(Size a, Size b)
    {
        return new Size(a.Width + b.Width, a.Height + b.Height);
    }

    // Opérateur soustraction
    public static Size operator -(Size a, Size b)
    {
        return new Size(a.Width - b.Width, a.Height - b.Height);
    }

    public static Size operator *(Size a, int b)
    {
        return new Size(a.Width * b, a.Height * b);
    }

    public static Size operator *(Size a, float b)
    {
        return new Size((int)(a.Width * b), (int)(a.Height * b));
    }

    public static Size operator *(Size a, double b)
    {
        return new Size((int)(a.Width * b), (int)(a.Height * b));
    }


    public static Size operator /(Size a, int b)
    {
        return new Size(a.Width / b, a.Height / b);
    }

    public static Size operator /(Size a, float b)
    {
        return new Size((int)(a.Width / b), (int)(a.Height / b));
    }

    public static Size operator /(Size a, double b)
    {
        return new Size((int)(a.Width / b), (int)(a.Height / b));
    }

    public static bool operator ==(Size a, Size b)
    {
        return a.Width == b.Width && a.Height == b.Height;
    }

    public static bool operator !=(Size a, Size b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is not Size) return false;
        var other = (Size)obj;
        return this == other;
    }

    public bool IsEmpty => this == Empty;
    public bool HasZero => Width == 0 || Height == 0;

    public override string ToString()
    {
        return string.Format("[{0}, {1}]", Width, Height);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }


    public Vector2 ToVector2()
    {
        return new Vector2(Width, Height);
    }

    public Point ToPoint() => new Point(Width, Height);    
}