
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class AnimationSprite
{   
    Texture2D _SpriteSheet;
    private string _spriteSheetAsset;

    private Rectangle[] _SourceRectangles;
    private Rectangle _OffsideSourceRectangles;
    private int _SpriteFrames;
    private int _CurrentFrameIndex;
    
    private int _SpriteWidth;
    private int _SpriteHeight;

    private int _lines;
    private int _columns;

    private float _timer;
    private int _FrameTime;

    private bool _reverseFrame;

    private bool _isFinished;
    public bool IsFinished { get {return _isFinished; }} // Is the animation finished ?

    public int CurrentFrame {get {return _CurrentFrameIndex; }}
    public bool Stop {get; set;}


    private int _lastFrameIndex;
    private int _vf;

    // Contruct the animation with Sprite texture
    public AnimationSprite(
        Rectangle rect,
        string asset,
        int SpriteFrames,
        int FrameTime,
        int width,
        int height,
        int LineNumber,
        int ColumnNumber,
        int LineIndex = 0,
        int ColumnIndex = 0,
        bool reverseFrame = false,
        Rectangle? offset = null
    )
    {   
        // _SpriteSheet = texture;
        _spriteSheetAsset = asset;
        _SpriteFrames = SpriteFrames;

        _SpriteWidth = width;
        _SpriteHeight = height;

        _lines = LineNumber;
        _columns = ColumnNumber;

        _CurrentFrameIndex = 0;
        _OffsideSourceRectangles = offset ?? Rectangle.Empty;
        _SourceRectangles = new Rectangle[_SpriteFrames];
        
        int i = 0, l = LineIndex, c = ColumnIndex;
        while (i < _SourceRectangles.Length && l <= _lines)
        {   
            if (c >= _columns)
            {
                c = 0;
                l++;
            }
            _SourceRectangles[i] = new Rectangle(
                c *_SpriteWidth+_OffsideSourceRectangles.X,
                l *_SpriteHeight+_OffsideSourceRectangles.Y,
                _SpriteWidth +_OffsideSourceRectangles.Width,
                _SpriteHeight +_OffsideSourceRectangles.Height
            );
            c++;
            i++;
        }

        _timer = 0;
        _FrameTime = FrameTime;

        _isFinished = false;
        Stop = false;

        _reverseFrame = reverseFrame;

        _lastFrameIndex = _SpriteFrames-1;
        _vf = 1; // Vector director from frame
    }

    public void Load(ContentManager Content)
    {
        _SpriteSheet = Content.Load<Texture2D>(_spriteSheetAsset);
    }

    public void Pause() { Stop = true; }
    public void Resume() { Stop = false; }



    // Replay Animation to begining
    public void replay()
    {
        if (!_reverseFrame)
        {
            _CurrentFrameIndex = 0;
            _lastFrameIndex = _SpriteFrames-1;
            _vf = 1;
        }else {
            _lastFrameIndex = _CurrentFrameIndex == 0 ? _SpriteFrames-1 : 0;
            _vf = _lastFrameIndex == 0 ? -1 : 1;
        }
        _isFinished = false;
    }

    public void PrintDebug()
    {
        Console.WriteLine($"[AnimationSprite] asset: {_spriteSheetAsset}, duration:{_FrameTime*_SpriteFrames/1000}s, lines:{_lines}, cols:{_columns}");
        Console.WriteLine($"frames:{_SpriteFrames}, frame time:{_FrameTime}ms, current frame index:{_CurrentFrameIndex}");
        Console.WriteLine($"============================================================================");
    }

    // Update Animation Frame
    public void Update(GameTime gameTime)
    {
        if (InputManager.IsPressed(Keys.A) &&  InputManager.IsPressed(Keys.D))
        {
            PrintDebug();
        }
        if (_SpriteFrames > 1)
        {
            if (!_isFinished && !Stop)
            {
                if (_timer > _FrameTime) 
                {                   
                    _timer = 0;
                    _CurrentFrameIndex+=_vf;
                    if (_CurrentFrameIndex == _lastFrameIndex)
                    {
                        _isFinished = true;
                    }
                }
                else {
                    _timer += gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            else if (_isFinished)
            {
                if (!Stop)
                    replay();
            }
        }
        
    }

    // Draw Animation with Source Rectangle
    public void Draw(SpriteBatch _spriteBatch, Rectangle rect)
    {
        _spriteBatch.Draw(_SpriteSheet, rect, _SourceRectangles[_CurrentFrameIndex], Color.White);
        // _spriteBatch.Draw(_SpriteSheet, _rect, Color.White);
    }   

}