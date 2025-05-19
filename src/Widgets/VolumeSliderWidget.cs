
using System;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class VolumeSliderWidget : Widget
{
    private float _currentVolume;
    private float _maxVolume;

    private Rectangle _rectVolume;
    private Rectangle _rectVolumeMax;

    private Color _color;
    Color _borderColor;
    public Action<float> OnChanged = null;

    private bool _disabled = false;

    public override int Y {get {return _rect.Y;} set {_rect.Y = value;}}

    public VolumeSliderWidget(
        Rectangle rect, float maxVolume
        = 1f, float volume = 1f, 
        bool disabled = false, 
        Color? borderColor = null,
        Action<float> OnChanged = null
        ) : base(rect) {
        _currentVolume = volume;
        _maxVolume = maxVolume;
        _disabled = disabled;

        _rectVolume = new Rectangle(_rect.X+2, _rect.Y+2, (int)((float)_rect.Width*_currentVolume), _rect.Height-4);
        _rectVolumeMax = _rectVolume;
        _rectVolumeMax.Width = _rect.Width-4;
        CalculateColor(_currentVolume);
        _borderColor = borderColor ?? Color.Red;


        this.OnChanged = OnChanged;
    }

    public VolumeSliderWidget(
        Size size, float maxVolume = 1f, 
        float volume = 1f, 
        bool disabled = false, 
        Color? borderColor = null,
        Action<float> OnChanged = null
    ) : this(new Rectangle(0,0,size.Width,size.Height),maxVolume, volume,disabled,borderColor, OnChanged)  
    {
    }


    public override void Load(ContentManager Content) {

    }

    public void CalculateColor(float volume)
    {
        float percentage_volume = volume*100;
        if (percentage_volume >= 60) _color = new Color(0,175,165);
        else if (percentage_volume >= 30) _color = Color.Yellow;
        else if (percentage_volume >= 0) _color = Color.Red;
    }

    public override void Update(GameTime gameTime)
    {
        if (!_disabled)
        {
            if ((InputManager.LeftHold) && InputManager.Hover(_rectVolumeMax))
            {
                Vector2 pos = InputManager.GetMousePosition();
                float vx = pos.X-_rect.X;
                float vx_percentage = vx*100/_rect.Width;
                _rectVolume.Width = (int)vx;
                _currentVolume = vx_percentage/100;
                if (OnChanged != null)
                {
                    OnChanged?.Invoke(_currentVolume);
                    CalculateColor(_currentVolume);
                }
            }
        }

        _rectVolume = new Rectangle(_rect.X+2, _rect.Y+2, (int)((float)_rect.Width*_currentVolume)-4, _rect.Height-4);
        _rectVolumeMax = _rectVolume;
        _rectVolumeMax.Width = _rect.Width;
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        Shape.DrawRectangle(_spriteBatch, _rect, !_disabled ? _borderColor : Color.DarkGray);
        Shape.FillRectangle(_spriteBatch, _rectVolume, !_disabled ? _color : Color.Gray);
    }
}
