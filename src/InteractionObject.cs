

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class InteractionObject
{
    public Rectangle rect;
    public string type;
    public int? l;
    public int? c;
    public string name;

    public MapScene MapScene; 
    
    public Func<string> ActionName;
    public Func<string> ActionInstructions;


    public InteractionObject(Rectangle rect, string type, MapScene mapScene, string name, int? l, int? c, Func<string> actionName = null, Func<string> actionInstructions = null)
    {
        this.rect = rect;
        this.type = type;
        this.name = name;
        this.l = l; this.c = c;
        ActionName = actionName;
        ActionInstructions = actionInstructions;
    } 

    public abstract string GetConditionName();
    public abstract string GetConditionInstruction();
    public abstract bool IsConditionDone(Map map, Player player);
    public abstract void DoAction(Map map, Player player);

    public abstract void Update(GameTime gameTime, Map map, Player player);
    public abstract void Draw(SpriteBatch _spriteBatch);
} 
