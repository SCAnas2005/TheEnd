

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using TheEnd;

public class NormalDoorObject : InteractionObject
{
    public bool state;
    public bool State {get{ return state; } set{ state = value; }}

    public bool locked;
    public bool Locked {get{ return locked; } set{ locked = value; }}
    public string key;

    private SoundEffect _openSound;
    private SoundEffect _closeSound;
    private SoundEffect _unlockSound;

    public NormalDoorObject(
        Rectangle rect,
        string type,
        string name,
        MapScene mapScene,
        int? l, int? c, bool state, bool locked, string key_name = null,
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, mapScene, name, l, c, actionName, actionInstructions)
    {
        this.state = state;
        this.locked = locked;
        IsIntersectWithPlayer = false;
        key = key_name;


        EditAction(
            "interact",
            name: Locked ? "Locked" : state ? "Fermer" : "Ouvrir",
            conditionToAction: (player) =>
            {
                Console.WriteLine("Locked is : " + Locked);
                if (Locked) return false;
                return IsIntersectWithPlayer && !player.GetMapPositions(player.Map).Contains((l.Value, c.Value));
            },
            action: (player) =>
            {
                if (Locked) { return;}
                state = !state;


                var obstaclesLayer = player.Map.TiledMap.GetLayer<TiledMapTileLayer>("obstacles");
                var groundLayer = player.Map.TiledMap.GetLayer<TiledMapTileLayer>("ground");
                uint homeDoorClosedId = 179;
                uint homeDoorOpenedId = 180;
                uint newId = state ? homeDoorOpenedId : homeDoorClosedId;

                if (state)
                {
                    groundLayer.SetTile((ushort)c, (ushort)l, newId);
                    obstaclesLayer.SetTile((ushort)c, (ushort)l, 0);
                    AudioManager.Play(_openSound);
                }
                else
                {
                    groundLayer.SetTile((ushort)c, (ushort)l, 0);
                    obstaclesLayer.SetTile((ushort)c, (ushort)l, newId);
                    AudioManager.Play(_closeSound);
                }
                EditAction("interact", name: locked ? "Locked" : state ? "Fermer" : "Ouvrir");
                player.Map.UpdateMapRenderer();
            }
        );
    }

    // public override string GetConditionName() => locked ? "[Locked]" : state ? "[Fermer]" : "[Ouvrir]";
    // public override string GetConditionInstruction() => locked ? "" : $"Appuyer sur [E] pour {GetConditionName()}";

    // public override bool IsConditionDone(Map map, Player player)
    // {
    //     if (locked) return false;
    //     if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer && !player.GetMapPositions(map).Contains((l.Value, c.Value)))
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _openSound = Content.Load<SoundEffect>("sounds/entities/door/open");
        _closeSound = Content.Load<SoundEffect>("sounds/entities/door/close");
        _unlockSound = Content.Load<SoundEffect>("sounds/entities/door/unlock");
    }

    public void Unlock()
    {
        locked = false;
        EditAction("interact", name: locked ? "Locked" : state ? "Fermer" : "Ouvrir");
        AudioManager.Play(_unlockSound, volume: 1f);
    }

    // public override void DoAction(Map map, Player player)
    // {

    //     if (locked)
    //     {
    //         return;
    //     }
    //     state = !state;


    //     var obstaclesLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("obstacles");
    //     var groundLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("ground");
    //     uint homeDoorClosedId = 179;
    //     uint homeDoorOpenedId = 180;
    //     uint newId = state ? homeDoorOpenedId : homeDoorClosedId;

    //     if (state)
    //     {
    //         groundLayer.SetTile((ushort)c, (ushort)l, newId);
    //         obstaclesLayer.SetTile((ushort)c, (ushort)l, 0);
    //         AudioManager.Play(_openSound);
    //         Console.WriteLine("test");
    //     }
    //     else
    //     {
    //         groundLayer.SetTile((ushort)c, (ushort)l, 0);
    //         obstaclesLayer.SetTile((ushort)c, (ushort)l, newId);
    //         AudioManager.Play(_closeSound);
    //         Console.WriteLine("test");
    //     }
    //     map.UpdateMapRenderer();
    //     // Console.WriteLine($"Editing tile for door: state:{state}, (l,c):({l}, {c}), newId:{newId}");
    // }

    public override void Update(GameTime gametime, Map map, Player player)
    {
        base.Update(gametime, map, player);
        if (!locked)
        {
        }
        else
        {
            if (IsIntersectWithPlayer)
            {
                if (!player.Inventory.IsEmpty && player.Inventory.SelectedItem != null && player.Inventory.SelectedItem.Name == key)
                {
                    KeyItem key = (KeyItem)player.Inventory.SelectedItem;
                    // (KeyItem)player.Inventory.Items.FirstOrDefault(
                    //     item => item is KeyItem && item.Name == this.key
                    // );
                    if (key != null)
                    {
                        key.Unlock(this);
                        player.Inventory.RemoveSelectedItem();
                        key.KillMe = true;
                    }
                }
            }
        } 
        

    }

    // public override void Draw(SpriteBatch _spriteBatch) {
    //     if (IsIntersectWithPlayer)
    //     {
    //         Size s = Text.GetSize(GetConditionName(), scale:0.3f);
    //         Vector2 p = new Vector2(Rect.X+Rect.Width, Rect.Y+(Rect.Height-s.Height)/2);
    //         Text.Write(_spriteBatch, GetConditionName(), p, Color.Blue, scale: 0.3f);
    //         p.Y+=s.Height;
    //         Text.Write(_spriteBatch, GetConditionInstruction(), p, Color.Blue, scale:0.3f);
    //     }
    // }

}