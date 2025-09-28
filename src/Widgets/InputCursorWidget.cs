
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class CusorInputWidget : Widget
{
    private Texture2D _texture;
    private int _position;
    private bool _blink;
    private Chrono _timer;
    private int _dashLen;

    private Color _color;

    public CusorInputWidget(Rectangle rect, int position = 0, int dashLen = 5, Color? color = null) : base(rect)
    {
        _position = position;
        _blink = false;
        _timer = new Chrono();
        _dashLen = dashLen;

        _color = color ?? Color.Black;
    }

    public CusorInputWidget(Size size, int position = 0, int dashLen = 5) : this(new Rectangle(0,0,size.Width, size.Height), position, dashLen) {

    }

    public override void Load(ContentManager Content)
    {
        // _texture = Content.Load<Texture2D>("Menu/inputs/text-cursor");
    }

    public void Blink()
    {
        if (_timer.Wait(0.5))
        {
            _blink = !_blink;
        }
    }

    public override void Update(GameTime gameTime)
    {
        _timer.Update(gameTime.ElapsedGameTime.TotalSeconds);
        Blink();
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (!_blink)
        {
            // _spriteBatch.Draw(_texture, _rect, Color.White);
            Shape.DrawLine(_spriteBatch, Position, Position+new Vector2(0,Size.Height), _color, 2);
        }
    }

}