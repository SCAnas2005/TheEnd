

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public abstract class Item
{
    protected Rectangle _rect;
    protected Rectangle _zoneRect;
    protected string _name;
    protected Texture2D _texture;
    protected string _src;
    public Map Map;
    public MapScene MapScene;

    public bool IsDropped;
    public bool IsIntersectWithPlayer = false;
    public string Name {get{ return _name; }}

    public Texture2D Texture { get { return _texture; } }

    public Rectangle Rect { get { return _rect; } set { _rect = value;_zoneRect = GetInteractionZone(_rect, 15); } }

    public Item(Rectangle rect, string src, string name, Map map, MapScene mapScene, bool dropped = true)
    {
        _rect = rect;
        _zoneRect = GetInteractionZone(_rect, 15);
        _src = src;
        _name = name;

        Map = map;
        MapScene = mapScene;

        IsDropped = dropped;
    }

    

    public virtual string GetConditionName() => "Prendre";
    public virtual string GetConditionInstruction() => $"Appuyer sur [E] pour {GetConditionName()}";

    public virtual void Load(ContentManager Content)
    {
        _texture = Content.Load<Texture2D>(_src);
    }

    public Rectangle GetInteractionZone(Rectangle baseRect, int padding)
    {
        return new Rectangle(
            baseRect.X - padding,
            baseRect.Y - padding,
            baseRect.Width + padding * 2,
            baseRect.Height + padding * 2
        );
    }

    public virtual void Update(GameTime gameTime, Player player, Map map)
    {
        IsIntersectWithPlayer = _zoneRect.Intersects(player.Rect);
        if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer)
        {
            Console.WriteLine("pressed [E]");
            Action(player, map);
        }
    }

    public abstract void Action(Player player, Map map);

    public virtual void DrawIndications(SpriteBatch _spriteBatch)
    {
        if (IsIntersectWithPlayer && IsDropped)
        {
            Size s = Text.GetSize(GetConditionName(), scale: 0.3f);
            Vector2 p = new Vector2(_rect.X + _rect.Width, _rect.Y + (_rect.Height - s.Height) / 2);
            Text.Write(_spriteBatch, GetConditionName(), p, Color.Blue, scale: 0.3f);
            p.Y += s.Height;
            Text.Write(_spriteBatch, GetConditionInstruction(), p, Color.Blue, scale: 0.3f);
        }
    }

    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        if (IsDropped)
        {
            _spriteBatch.Draw(_texture, _rect, Color.White);

            Shape.DrawRectangle(_spriteBatch, _rect, Color.Blue);
            Shape.DrawRectangle(_spriteBatch, _zoneRect, Color.Purple);
            DrawIndications(_spriteBatch);
        }
    }

}