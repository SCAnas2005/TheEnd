
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class TransitionDoorObject : InteractionObject
{
    public MapScene? destinationMap;
    private bool _state;
    public bool State { get{ return _state; } set { _state = value; EditAction("interact", name: State ? $"Entrer" : "[Locked]", description: State ? $"[{_actions["interact"]?.Key}] Entrer" : "[Locked]"); } }
    public TransitionDoorObject(
        Rectangle rect,
        string type,
        MapScene mapScene,
        string name,
        int? l, int? c,
        MapScene? destinationMap,
        bool state,
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, mapScene, name, l, c, actionName, actionInstructions)
    {
        this.destinationMap = destinationMap;
        IsIntersectWithPlayer = false;
        this.State = state;

        EditAction(
            "interact",
            name: state ? $"Entrer" : "[Locked]",
            description: state ? $"[{_actions["interact"]?.Key}] Entrer" : "[Locked]",
            conditionToAction: (player) => IsIntersectWithPlayer && state,
            action: (player) =>
            {
                GameScene s = (GameScene)SceneManager.Scenes[SceneState.Game];
                s.ChangeMapScene(destinationMap.Value, (l != null && c !=null) ? Map.GetPosFromMap((l.Value, c.Value), player.Map.TileSize) : null);
            }
        );
    }

    // public override string GetConditionName() => state ? ActionName == null ? "[Entrer]" : ActionName?.Invoke() : "[Locked]";
    // public override string GetConditionInstruction() => state ? ActionInstructions == null ? $"Appuyer sur [E] pour {GetConditionName()}" : ActionInstructions?.Invoke() : "";

    // public override bool IsConditionDone(Map map, Player player)
    // {
    //     IsIntersectWithPlayer = Rect.Intersects(player.Rect);
    //     if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer && state)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    // public override void DoAction(Map map, Player player)
    // {
    //     GameScene s = (GameScene)SceneManager.Scenes[SceneState.Game];
    //     s.ChangeMapScene(destinationMap.Value, (l != null && c !=null) ? Map.GetPosFromMap((l.Value, c.Value), map.TileSize) : null);
    // }

    public override void Update(GameTime gametime, Map map, Player player)
    {
        // if (IsConditionDone(map, player))
        // {
        //     DoAction(map, player);
        // }
        base.Update(gametime, map, player);
    }

    public override void Draw(SpriteBatch _spriteBatch) {
        if (IsIntersectWithPlayer)
        {
            foreach (var (name, action) in _actions)
            {
                Size s = Text.GetSize(action.Name, scale: 0.3f);
                Vector2 p = new Vector2(Rect.X + Rect.Width, Rect.Y + (Rect.Height - s.Height) / 2);
                Text.Write(_spriteBatch, action.Name, p, Color.Blue, scale: 0.3f);
                p.Y += s.Height;
                if (State)
                {
                    s = Text.GetSize(action.Description, scale: 0.3f);
                    Text.Write(_spriteBatch, action.Description, p, Color.Blue, scale: 0.3f);
                    p.Y += s.Height;
                }
            }
        }
    }

}