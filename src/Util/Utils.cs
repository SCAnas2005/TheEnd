

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public static class Utils
{
    public static float GetPercentage(float value, float percentage)
    {
        return value * (percentage / 100f);
    }

    public static int GetPercentageInt(int value, int percentage)
    {
        return value * percentage / 100;
    }

    public static Random Random = new Random();


    public static Rectangle AddToRect(Rectangle a, Rectangle b)
    {
        return new Rectangle(
            a.X + b.X,
            a.Y + b.Y,
            a.Width + b.Width,
            a.Height + b.Height
        );
    }

    public static Rectangle AddToRect(Rectangle a, int x=0, int y=0, int width=0, int height=0) 
    {
        return AddToRect(a, new Rectangle(x, y, width, height));
    }


    public static (int, int) AddTuples((int, int)a, (int, int)b)
    {
        return (a.Item1+b.Item1, a.Item2+b.Item2);
    }


    public static (T, T) ReverseTuple<T>((T, T)a)
    {
        return (a.Item2, a.Item1);
    }

    public static string ListToString<T>(List<T> list)
    {
        string s = "[";

        for (int i = 0; i < list.Count; i++)
        {   
            s+=list[i].ToString();
            if (i < list.Count-1) {
                s+=",";
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
        }
        return s;
    }
}



public class Size
{
    public int Width { get; set; }
    public int Height { get; set; }

    public static Size Empty => new Size(0,0);

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public Size(int value) : this(width: value, height: value) {}

    public Size() : this(value: 0) {}

    public Size Add(Size s) { return new Size(Width+s.Width, Height+s.Height); }
    public Size Add(int width=0, int height=0) { return new Size(Width+width, Height+height); }

    public static Size operator +(Size a, Size b)
    {
        return new Size(a.Width + b.Width, a.Height + b.Height);
    }

    // Opérateur soustraction
    public static Size operator -(Size a, Size b)
    {
        return new Size(a.Width - b.Width, a.Height - b.Height);
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
    
    public override string ToString() {
        return string.Format("[{0}, {1}]", Width, Height);
    }
}