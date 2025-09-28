

// using System;
// using System.Linq;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;
// using MonoGame.Extended.Tiled;
// using TheEnd;

// public class GunObject : InteractionObject
// {
//     private string _name;
//     public bool IsIntersectWithPlayer { get; set; }
//     public GunObject(
//         Rectangle rect, 
//         string type, 
//         string name,
//         int? l, int? c, 
//         Func<string> actionName = null, Func<string> actionInstructions = null
//     ) : base(rect, type, l, c, actionName, actionInstructions)
//     {
//         _name = name;
//         IsIntersectWithPlayer = false;
//     }

//     public override string GetConditionName() => "Prendre";
//     public override string GetConditionInstruction() => $"Appuyer sur [E] pour {GetConditionName()}";

//     public override bool IsConditionDone(Map map, Player player)
//     {
//         IsIntersectWithPlayer = rect.Intersects(player.Rect);
//         if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer && !player.GetMapPositions(map).Contains((l.Value, c.Value)))
//         {
//             return true;
//         }
//         return false;
//     }

//     public override void DoAction(Map map, Player player)
//     {
//         var itemsLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("items");
//         uint newId = 0; // Empty
//         itemsLayer.SetTile((ushort)c, (ushort)l, newId);

//         Weapon gun = new Weapon(
//             rect: new Rectangle(
//                 x: player.Rect.X + Utils.GetPercentageInt(player.Rect.Width, -7),
//                 y: player.Rect.Y + Utils.GetPercentageInt(player.Rect.Height, 38),
//                 width: Utils.GetPercentageInt(player.Rect.Width, 114),
//                 height: Utils.GetPercentageInt(player.Rect.Height, 31)
//             ),
//             src: "Weapons/Guns/gun",
//             name: "Gun",
//             map: player.Map,
//             dx: player.DirectionX
//         );
//         gun.Load(Globals.Content); 

//         player.gun = gun;
//         Console.WriteLine("player has a gun");

//         var objectLayer = map.TiledMap.GetLayer<TiledMapObjectLayer>("Interaction");

//         // Convertir en liste modifiable
//         var objectsList = objectLayer.Objects.ToList();

//         // Trouver l'objet à supprimer
//         var targetObject = objectsList.FirstOrDefault(o => o.Name == _name);

//         if (targetObject != null)
//         {
//             objectsList.Remove(targetObject);
//         }

//     }   

//     public override void Update(GameTime gametime, Map map, Player player){
//         if (IsConditionDone(map, player))
//         {
//             DoAction(map, player);
//         }

//     }
//     public override void Draw(SpriteBatch _spriteBatch)
//     {
//         if (IsIntersectWithPlayer)
//         {
//             Size s = Text.GetSize(GetConditionName(), scale: 0.3f);
//             Vector2 p = new Vector2(rect.X + rect.Width, rect.Y + (rect.Height - s.Height) / 2);
//             Text.Write(_spriteBatch, GetConditionName(), p, Color.Blue, scale: 0.3f);
//             p.Y += s.Height;
//             Text.Write(_spriteBatch, GetConditionInstruction(), p, Color.Blue, scale: 0.3f);
//         }
//     }

// }