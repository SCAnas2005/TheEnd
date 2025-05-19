

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using TheEnd;

public class NormalDoorObject : InteractionObject
{
    public bool IsIntersectWithPlayer {get; set;}
    public bool state;
    public NormalDoorObject(
        Rectangle rect, 
        string type, 
        int? l, int? c, bool state, 
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, l, c, actionName, actionInstructions)
    {   
        this.state = state;
        IsIntersectWithPlayer = false;
    }

    public override string GetConditionName() => state ? "[Fermer]" : "[Ouvrir]";
    public override string GetConditionInstruction() => $"Appuyer sur [E] pour {GetConditionName()}";

    public override bool IsConditionDone(Map map, Player player)
    {
        IsIntersectWithPlayer = rect.Intersects(player.Rect);
        if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer && !player.GetMapPositions(map).Contains((l.Value, c.Value)))
        {
            return true;
        }
        return false;
    }

    public override void DoAction(Map map, Player player)
    {
        state = !state;
        
        var obstaclesLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("obstacles");
        var groundLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("ground");
        uint homeDoorClosedId = 179;
        uint homeDoorOpenedId = 180;
        uint newId = state ? homeDoorOpenedId : homeDoorClosedId; 

        if (state)
        {
            groundLayer.SetTile((ushort)c, (ushort)l, newId);
            obstaclesLayer.SetTile((ushort)c, (ushort)l, 0);
        } else {
            groundLayer.SetTile((ushort)c, (ushort)l, 0);
            obstaclesLayer.SetTile((ushort)c, (ushort)l, newId);
        }
        map.UpdateMapRenderer();
        Console.WriteLine($"Editing tile for door: state:{state}, (l,c):({l}, {c}), newId:{newId}");
    }   

    public override void Update(GameTime gametime, Map map, Player player){
        if (IsConditionDone(map, player))
        {
            Console.WriteLine("Doing action");
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