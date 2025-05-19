
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public enum Icons{
    Add, 
    Minus, 
    Close, 
    Audio, 
    Music,
    Twitter,
    Settings,
    Star,
    Trash,
    Checked,
    CheckboxChecked,
    CheckboxUnChecked,
    Dropdown,
    Dropup,
    Infinity
}

public static class IconsManager
{
    private static Dictionary<Icons, string> _iconsPath;

    public static void Init()
    {
        _iconsPath = new Dictionary<Icons, string>();
        List<Icons> listIcons = Enum.GetValues(typeof(Icons)).Cast<Icons>().ToList();
        foreach (Icons i in listIcons)
        {
            _iconsPath[i] = $"Menu/icons/{i.ToString().ToLower()}";
        }
    }

    public static string GetIconPath(Icons i)
    {
        return _iconsPath[i];
    }
}