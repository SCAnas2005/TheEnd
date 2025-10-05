
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using TheEnd;

public class ReadablePaperObject : InteractionObject
{
    private PaperReaderWidget _reader;
    private bool _showReader;
    public string Content;
    public ReadablePaperObject(
        Rectangle rect,
        string type,
        Map map,
        string name,
        int? l, int? c,
        string content = "",
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, map, name, l, c, actionName, actionInstructions)
    {
        IsIntersectWithPlayer = false;
        Content = content;
        Size ca = new Size((int)(Camera2D.CameraLogicalSize.Width), (int)(Camera2D.CameraLogicalSize.Height));
        Size s = new Size(ca.Width * 4 / 5, ca.Height * 4 / 5);
        Console.WriteLine("tesqlfkdjmsqlfkjsmlfkdjsmlfkqsdjml : " + s);
        _reader = new PaperReaderWidget(
            rect: new Rectangle(
                (int)((ca.Width - s.Width) / 2),
                (int)((ca.Height - s.Height) / 2),
                s.Width,
                s.Height
            ),
            text: Content
        );

        _reader.Load(Globals.Content);
        _showReader = false;

        EditAction(
            "interact",
            name: "Action",
            action: (player) => { _showReader = true; }
        );
    }

    // public override string GetConditionName() => ActionName == null ? "[ACTION]" : ActionName?.Invoke();
    // public override string GetConditionInstruction() => ActionInstructions == null ? $"Appuyer sur [E] pour {GetConditionName()}" : ActionInstructions?.Invoke();

    // public override bool IsConditionDone(Map map, Player player)
    // {
    //     IsIntersectWithPlayer = Rect.Intersects(player.Rect);
    //     if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    // public override void DoAction(Map map, Player player)
    // {
    //     _showReader = true;
    // }

    public override void Update(GameTime gameTime, Map map, Player player){
        base.Update(gameTime, map, player);

        if (IsIntersectWithPlayer && _showReader)
        {
            _reader.Update(gameTime);
        } else {
            _showReader = false;
        }
    }

    public override void Draw(SpriteBatch _spriteBatch) {

        // if (IsIntersectWithPlayer)
        // {
        //     Size s = Text.GetSize(GetConditionName(), scale:0.3f);
        //     Vector2 p = new Vector2(Rect.X+Rect.Width, Rect.Y+(Rect.Height-s.Height)/2);
        //     Text.Write(_spriteBatch, GetConditionName(), p, Color.Blue, scale: 0.3f);
        //     p.Y+=s.Height;
        //     Text.Write(_spriteBatch, GetConditionInstruction(), p, Color.Blue, scale:0.3f);
        // }
        base.Draw(_spriteBatch);

        if (IsIntersectWithPlayer && _showReader)
        {
            _reader.Draw(_spriteBatch);
        }
    }

}