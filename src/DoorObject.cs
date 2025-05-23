
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class TransitionDoorObject : InteractionObject
{
    public bool IsIntersectWithPlayer {get; set;}
    public MapScene? destinationMap;
    public TransitionDoorObject(
        Rectangle rect, 
        string type, 
        MapScene mapScene,
        string name,
        int? l, int? c, 
        MapScene? destinationMap,
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, mapScene, name, l, c, actionName, actionInstructions)
    {   
        this.destinationMap = destinationMap;
        IsIntersectWithPlayer = false;
    }

    public override string GetConditionName() => ActionName == null ? "[Entrer]" : ActionName?.Invoke();
    public override string GetConditionInstruction() => ActionInstructions == null ? $"Appuyer sur [E] pour {GetConditionName()}" : ActionInstructions?.Invoke();

    public override bool IsConditionDone(Map map, Player player)
    {
        IsIntersectWithPlayer = rect.Intersects(player.Rect);
        if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer)
        {
            return true;
        }
        return false;
    }

    public override void DoAction(Map map, Player player)
    {
        GameScene s = (GameScene)SceneManager.Scenes[SceneState.Game];
        s.ChangeMapScene(destinationMap.Value, (l != null && c !=null) ? Map.GetPosFromMap((l.Value, c.Value), map.TileSize) : null);
    }

    public override void Update(GameTime gametime, Map map, Player player){
        if (IsConditionDone(map, player))
        {
            DoAction(map, player);
        }
    }

    public override void Draw(SpriteBatch _spriteBatch) {
        if (IsIntersectWithPlayer)
        {
            Size s = Text.GetSize(GetConditionName(), scale:0.3f);
            Vector2 p = new Vector2(rect.X+rect.Width, rect.Y+(rect.Height-s.Height)/2);
            Text.Write(_spriteBatch, GetConditionName(), p, Color.Blue, scale: 0.3f);
            p.Y+=s.Height;
            Text.Write(_spriteBatch, GetConditionInstruction(), p, Color.Blue, scale:0.3f);
        }
    }

}