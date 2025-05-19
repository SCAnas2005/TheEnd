
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheEnd;


public enum SceneState {
    Menu,
    Game
}


public abstract class Scene
{
    protected Rectangle _rect;
    protected SceneState _screenState;
    protected Action OnClose;
    protected bool _debug;

    public Scene(SceneState screenState, Rectangle rect, bool debug = false, Action  OnClose = null)
    {
        _rect = rect;
        _debug = debug;
        _screenState = screenState;
        this.OnClose = OnClose;
    }
    public bool IsDebug() { return _debug; }
    
    public void ToggleDebug() { _debug = !_debug; }
    public void DrawDebug(SpriteBatch _spriteBatch) {
        Text.Write(_spriteBatch, $"Screen: {_screenState}", new Vector2(_rect.X, _rect.Y), Color.White);
    }
    public abstract void Load(ContentManager Content);
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch _spriteBatch); 

    public void Close()
    {
        OnClose?.Invoke();
    }
}