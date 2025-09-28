using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

record PlayerState(int health, int monney, int zombieKilled, int? ammo);

public class UserInfoWidget : StatefulWidget
{
    private Player _player;
    private ContainerWidget _container;

    private PlayerState _lastPlayerState;

    public UserInfoWidget(
        Rectangle rect,
        Player player,
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick: OnClick, debug: debug)
    {
        _player = player;
        _lastPlayerState = new PlayerState(player.Health, player.Money, player.ZombieKilled, _player.Inventory.SelectedItem is Gun ? ((Gun)_player.Inventory.SelectedItem).Ammo : null);
        _debug = debug;
        this.OnClick = OnClick;

        Build();
    }


    public UserInfoWidget(
        Size size,
        Player player,
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0, 0, size.Width, size.Height), player, debug, OnClick)
    { }

    public UserInfoWidget(
        Player player,
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, player, debug, OnClick)
    { }

    public override int X
    {
        get { return _rect.X; }
        set
        {
            _rect.X = value; _container.X = _rect.X;
        }
    }
    public override int Y
    {
        get { return _rect.Y; }
        set
        {
            _rect.Y = value; _container.Y = _rect.Y;
        }
    }

    public override void Build()
    {
        _container = new ContainerWidget(
            rect: _rect,
            alignItem: Align.Vertical,
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,
            widgets: [
                new AvatarWidget(
                    size: new Size(_rect.Width, 60),
                    image: new ImageWidget(size: new Size(60, 60), texture: _player.ProfilePicture),
                    text: new TextWidget(_player.Name, font: CFonts.GetClosestFont(60)),
                    debug:true
                ),
                new TextWidget($"Health : {_player.Health}", font: CFonts.GetClosestFont(60)),
                new TextWidget($"Money: ${_player.Money}", font: CFonts.GetClosestFont(60)),
                new TextWidget($"Zombie killed : {_player.ZombieKilled}", font: CFonts.GetClosestFont(60)),
                _player.Inventory.SelectedItem is Gun ? new TextWidget($"Ammo : {((Gun)_player.Inventory.SelectedItem).Ammo}", font: CFonts.GetClosestFont(60)) : new SizedBox()
            ]
        );


        if (_loaded)
        {
            Load(Globals.Content);
        }
    }

    public override void Load(ContentManager Content)
    {
        _container.Load(Content);
        _loaded = true;
    }

    public override void Update(GameTime gameTime)
    {

        _container.Update(gameTime);
        var _currentPlayerState = new PlayerState(_player.Health, _player.Money, _player.ZombieKilled, _player.Inventory.SelectedItem is Gun ? ((Gun)_player.Inventory.SelectedItem).Ammo : null);
        if (_currentPlayerState != _lastPlayerState)
        {
            _lastPlayerState = _currentPlayerState;
            SetState();
        }
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _container.Draw(_spriteBatch);
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }
}