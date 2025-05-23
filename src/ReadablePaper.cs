
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class ReadablePaperObject : InteractionObject
{
    private PaperReaderWidget _reader;
    private bool _showReader;
    public string Content;
    public bool IsIntersectWithPlayer {get; set;}
    public ReadablePaperObject(
        Rectangle rect, 
        string type, 
        MapScene mapScene,
        string name,
        int? l, int? c, 
        string content = "",
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, mapScene, name, l, c, actionName, actionInstructions)
    {   
        IsIntersectWithPlayer = false;
        Content = content;
        Size s = new Size(Globals.ScreenSize.Width*4/5, Globals.ScreenSize.Height*4/5);
        _reader = new PaperReaderWidget(
            rect: new Rectangle((Globals.ScreenSize.Width-s.Width)/2,(Globals.ScreenSize.Height-s.Height)/2, s.Width, s.Height),
            text: Content
        );
        _reader.Load(Globals.Content);
        _showReader = false;
    }

    public override string GetConditionName() => ActionName == null ? "[ACTION]" : ActionName?.Invoke();
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
        _showReader = true;
    }

    public override void Update(GameTime gameTime, Map map, Player player){
        if (IsConditionDone(map, player))
        {
            DoAction(map, player);
        }

        if (IsIntersectWithPlayer && _showReader)
        {
            _reader.Update(gameTime);
        } else {
            _showReader = false;
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

        if (IsIntersectWithPlayer && _showReader)
        {
            _reader.Draw(_spriteBatch);
        }
    }

}