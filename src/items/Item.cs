

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public abstract class Item : Entity
{
    protected Rectangle _zoneRect;
    protected string _name;

    public bool IsDropped;
    public bool IsIntersectWithPlayer = false;
    public string Name {get{ return _name; }}


    public override Rectangle Rect { get { return _rect; } set { _rect = value; _zoneRect = GetInteractionZone(_rect, 15); } }
    public override Vector2 Position { get { return new Vector2(_rect.X, _rect.Y); } set { _rect.X = (int)value.X; _rect.Y = (int)value.Y; _zoneRect = GetInteractionZone(_rect, 15); } }

    public SoundEffect _dropSound;
    public SoundEffect _onPickUpSound;

    protected Dictionary<string, ActionInteraction> _actions = new();

    public Item(Rectangle rect, string src, string name, Map map, bool dropped = true, bool debug = false) : base(rect: rect, src: src, map: map, debug: debug)
    {
        _zoneRect = GetInteractionZone(_rect, 15);
        _name = name;
        Map = map;
        IsDropped = dropped;

        AddAction(
            name: "pick",
            a: new ActionInteraction(
                name: "Prendre",
                description: "Appuyer sur pour prendre",
                key: Keys.E,
                conditionToShow: (player) => true,
                conditionToAction: (player) => IsIntersectWithPlayer && player.CanMove && player.CanUseItem,
                action: (player) =>
                {
                    if (IsDropped)
                    {
                        IsDropped = false;
                        player.Inventory.AddItem(this);
                    }
                }
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
            name:name,
            description:description,
            key:key,
            conditionToShow: conditionToShow,
            conditionToAction: conditionToAction,
            action: action
        );
    }


    public void AddToExistingAction(string keyName,
        string name = null, string description = null,
        Func<Player, bool> conditionToShow = null, Func<Player, bool> conditionToAction = null, Action<Player> action = null
    )
    {
        ActionInteraction a = GetAction(keyName);
        if (a == null) return;
        a.Edit(
            name: a.Name + name ?? "", description: a.Description + description ?? "",
            conditionToShow: (p) => !a.ConditionToShow(p) || conditionToShow == null || conditionToShow(p),
            conditionToAction: (p) => !a.ConditionToAction(p) || conditionToAction == null || conditionToAction(p)
        );
    }

    public ActionInteraction GetAction(string key)
    {
        if (_actions.ContainsKey(key))
            return _actions[key];
        return null;
    }


    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _dropSound = Content.Load<SoundEffect>("sounds/items/drop-item");
        // _onPickUpSound = Content.Load<SoundEffect>("sounds/items/pickup-item");
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

    public virtual bool IsInteracting()
    {
        return InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer;
    }

    public virtual bool IsUsing()
    {
        return InputManager.IsPressed(Keys.J) && IsIntersectWithPlayer;
    }

    public override void Update(GameTime gameTime)
    {
        Player player = EntityManager.Player;

        if (KillMe) return;
        IsIntersectWithPlayer = _zoneRect.Intersects(player.Rect);
        foreach (var (name, action) in _actions)
        {
            if (action.ConditionToShow(player) && action.ConditionToAction(player))
            {
                // Action(player, Map);
                action.Action(player);
            }
            // if (IsDropped)
            // {
            // }
            // else
            // {
            //     if (IsUsing())
            //     {
            //         action.Action(player);
            //     }
            // }
        }

    }


    public virtual void DrawIndications(SpriteBatch _spriteBatch)
    {
        if (IsIntersectWithPlayer)
        {
            int space = 0;
            Vector2 p = new Vector2(_rect.X + _rect.Width, _rect.Y + space);
            foreach (var (name, action) in _actions)
            {
                if (action.Show)
                {
                    Size s = Text.GetSize(action.Description, scale: 0.3f);
                    Text.Write(_spriteBatch, action.Description, p, Color.Blue, scale: 0.3f);
                    p.Y += s.Height + space;
                }
            }
        }
    }

    public virtual void OnPickUp(IItemUser user)
    {
        IsDropped = false;
        AudioManager.Play(_onPickUpSound); 
    }

    public virtual void OnSelect()
    {
        
    }

    public virtual void OnDrop()
    {
        IsDropped = true;
        AudioManager.Play(_dropSound);
        Console.WriteLine("on drop");
    }

    public virtual void OnChange()
    {
        
    }

    public void DrawZones(SpriteBatch _spriteBatch)
    {
        Shape.DrawRectangle(_spriteBatch, _rect, Color.Blue);
        Shape.DrawRectangle(_spriteBatch, _zoneRect, Color.Purple);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (IsDropped)
        {
            _spriteBatch.Draw(_texture, _rect, Color.White);

            DrawZones(_spriteBatch);
            DrawIndications(_spriteBatch);
        }
    }

}