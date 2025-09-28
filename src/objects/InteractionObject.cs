

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public abstract class InteractionObject
{
    public Rectangle Rect;
    public string type;
    public int? l;
    public int? c;
    public string name;

    public MapScene MapScene;

    public Func<string> ActionName;
    public Func<string> ActionInstructions;

    protected Dictionary<string, ActionInteraction> _actions = new();
    public bool IsIntersectWithPlayer { get; set; }

    public bool _destroy = false;
    public bool IsDestroyed { get => _destroy; }

    public InteractionObject(Rectangle rect, string type, MapScene mapScene, string name, int? l, int? c, Func<string> actionName = null, Func<string> actionInstructions = null, Keys key = Keys.E)
    {
        this.Rect = rect;
        this.type = type;
        this.name = name;
        this.l = l; this.c = c;
        ActionName = actionName;
        ActionInstructions = actionInstructions;

        AddAction(
            name: "interact",
            a: new ActionInteraction(
                name: "Interagir",
                description: "Appuyer pour interagir",
                key: key,
                conditionToShow: (player) => IsIntersectWithPlayer,
                conditionToAction: (player) => IsIntersectWithPlayer,
                action: (player) => { }
            )
        );
    }

    public void AddAction(string name, ActionInteraction a)
    {
        _actions[name] = a;
    }

    public void RemoveAction(string key)
    {
        if (_actions.ContainsKey(key)) _actions.Remove(key);
    }

    public void EditAction(string keyName,
        string name = null, string description = null, Keys? key = null,
        Func<Player, bool> conditionToShow = null, Func<Player, bool> conditionToAction = null, Action<Player> action = null
    )
    {
        GetAction(keyName)?.Edit(
            name: name,
            description: description,
            key: key,
            conditionToShow: conditionToShow,
            conditionToAction: conditionToAction,
            action: action
        );
    }

    public ActionInteraction GetAction(string key)
    {
        if (_actions.ContainsKey(key))
            return _actions[key];
        return null;
    }


    public virtual void Load(ContentManager Content) { }
    public virtual void Update(GameTime gameTime, Map map, Player player)
    {
        IsIntersectWithPlayer = Rect.Intersects(player.Rect);
        foreach (var (name, action) in _actions)
        {
            action.ConditionToShow(player);
            if (action.Show && action.ConditionToAction(player))
            {
                action.Action(player);
            }
        }
    }
    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        if (IsIntersectWithPlayer)
        {
            int space = 0;
            Vector2 p = new Vector2(Rect.X + Rect.Width, Rect.Y + space);
            foreach (var (name, action) in _actions)
            {
                if (action.Show)
                {
                    Size s = Text.GetSize(action.Name, scale: 0.3f);
                    Text.Write(_spriteBatch, action.Name, p, Color.Blue, scale: 0.3f);
                    p.Y += s.Height;
                    Text.Write(_spriteBatch, action.Description, p, Color.Blue, scale: 0.3f);
                    s = Text.GetSize(action.Description, scale: 0.3f);
                    p.Y += s.Height;
                }
            }
        }
    }


    public virtual void Destroy()
    {
        _destroy = true;
    }
} 
