using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class PauseScene : Scene
{
    private Widget _itemsListWidget = null;
    private string _text;
    public PauseScene(SceneState screenState, Rectangle rect, bool debug = false, Action OnClose = null) : base(screenState: screenState, rect: rect, debug: debug, OnClose: OnClose)
    {
        _text = "Reprendre [ENTER]";
    }

    public override void Load(ContentManager Content) {

    }
    public override void Update(GameTime gameTime)
    {
        if (InputManager.IsPressed(Keys.Enter))
        {
            OnClose?.Invoke();
        }
        else if (InputManager.IsPressed(Keys.F1))
        {
            if (_itemsListWidget == null)
            {
                var items = ItemManager.Items;
                _itemsListWidget = new ItemsListWidget(rect: new Rectangle(500, 100, 500, 700), items: items);
                _itemsListWidget.Load(Globals.Content);
            }
            else
            {
                _itemsListWidget = null;
            }
        }

        _itemsListWidget?.Update(gameTime);
    }
    public override void Draw(SpriteBatch _spriteBatch)
    {
        var s = Text.GetSize(_text, font: CFonts.Minecraft_48);
        Text.Write(_spriteBatch, _text, new Vector2((Globals.ScreenSize.Width - s.Width) / 2, (Globals.ScreenSize.Height - s.Height) / 2), Color.White, font: CFonts.Minecraft_48);

        _itemsListWidget?.Draw(_spriteBatch);
    } 
}