using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheEnd;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Size _screenSize;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        // Configuration de la fenêtre de jeu
        _screenSize = new Size(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        _graphics.PreferredBackBufferWidth = _screenSize.Width;
        _graphics.PreferredBackBufferHeight = _screenSize.Height;
        
        _graphics.IsFullScreen = true;
        IsMouseVisible = true;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Crée un SpriteBatch pour dessiner les objets 2D
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.Init(_graphics, Content, _screenSize);
        Shape.Init(_graphics.GraphicsDevice);
        CFonts.Init();
        CFonts.Load(Content);
        SceneManager.Init(SceneState.Game);
        SceneManager.LoadAllScreens(Content);
        

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Gère les entrées du joueur ou autres logiques de jeu ici
        InputManager.Update();
        AudioStreamPlayerManager.Update();

        SceneManager.CurrentScene.Update(gameTime);

        if (InputManager.AreKeysPressedTogether(Keys.LeftShift, Keys.Escape))
        {
            Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Transparent);

        // Commence à dessiner avec le SpriteBatch
        var state = new SpriteBatchState();
        SpriteBatchContext.Push(state);
        SpriteBatchContext.ApplyToContext(_spriteBatch, SpriteBatchContext.Top);

        SceneManager.CurrentScene.Draw(_spriteBatch);
        // Fin du dessin
        _spriteBatch.End();
        SpriteBatchContext.Pop();

        base.Draw(gameTime);
    }
}
