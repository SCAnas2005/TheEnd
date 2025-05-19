

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public abstract class Item
{
    protected Rectangle _rect;
    protected string _name;
    protected Texture2D _texture;
    protected string _src;
    public Map Map;
    public MapScene MapScene;

    public bool IsDropped;
    public bool IsIntersectWithPlayer = false;

    public Rectangle Rect { get { return _rect; } set { _rect = value; } }

    public Item(Rectangle rect, string src, string name, Map map, MapScene mapScene)
    {
        _rect = rect;
        _src = src;
        _name = name;

        Map = map;
        MapScene = mapScene;

        IsDropped = true;
    }

    public virtual string GetConditionName() => "Prendre";
    public virtual string GetConditionInstruction() => $"Appuyer sur [E] pour {GetConditionName()}";

    public virtual void Load(ContentManager Content)
    {
        _texture = Content.Load<Texture2D>(_src);
    }

    public virtual void Update(GameTime gameTime, Player player, Map map)
    {
        IsIntersectWithPlayer = _rect.Intersects(player.Rect);
        if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer)
        {
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
        _spriteBatch.Draw(_texture, _rect, Color.White);
        DrawIndications(_spriteBatch);
    }

}