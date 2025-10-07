
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public enum NpcAnimation
{
    Idle,
    Right,
    Left,
    Up,
    Down
}

public class Npc : Sprite, IDamageable, IItemUser, IWeaponUser
{

    private Dictionary<string, ActionInteraction> _actions = new();

    private Rectangle _speakArea;


    public bool DialogStarted = false;
    public List<DialogLine> Dialogs = new();

    private float _waitTime;
    private Vector2? _currentTargetPosition = null;
    private float _minMoveDelay = 2f;
    private float _maxMoveDelay = 5f;


    public bool IsIntersectWithPlayer { get; private set; }
    public bool IsInteractingWithPlayer { get; set; }


    public const int CooldownDuration = 10;

    public NpcConfig Config;

    public Inventory Inventory;

    public Npc(Rectangle rect, NpcConfig config, string src, float speed, int health, Map map, bool debug = false) : base(rect, src, speed, health, map, config!=null?config.Name:"", debug)
    {
        _speakArea = Utils.AddToRect(Rect, new Rectangle(-10, -10, 20, 20));
        Config = config;
        _direction = Directions.right;
        SpawnPoint = Position;
        zIndex = 2;

        AddAction(
            name: "speak",
            a: new ActionInteraction(
                "Parler", "Appuyer sur [E] pour parler", key: Keys.E,
                conditionToShow: (player) => IsIntersectWithPlayer,
                conditionToAction: (player) => player.CanInteract && IsIntersectWithPlayer,
                action: player =>
                {
                    IsInteractingWithPlayer = true;
                    player.CanMove = false; player.CanUseItem = false; player.CanSelectItem = false; player.CanInteract = false;
                    DialogManager.Instance.StartDialog(Dialogs);
                    DialogStarted = true;
                }
            )
        );

        Inventory = new Inventory(storageMax: 2, owner: this);
    }

    public Npc() : this(rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), config: null, src:"", speed: 2f, health: 100, map:null, debug:false)
    {

    }

    public void AddAction(string name, ActionInteraction a)
    {
        _actions[name] = a;
    }

    public void RemoveAction(string key)
    {
        if (_actions.ContainsKey(key)) _actions.Remove(key);
    }



    public ActionInteraction GetAction(string key)
    {
        if (_actions.ContainsKey(key))
            return _actions[key];
        return null;
    }

     public void EditAction(string keyName,
        string name = null, string description = null, Keys? key = null,
        Func<Player, bool> conditionToShow = null, Func<Player, bool> conditionToAction = null, Action<Player> action = null
    )
    {
        GetAction(keyName)?.Edit(
            name:name,
            description:description,
            key:key,
            conditionToShow: conditionToShow,
            conditionToAction: conditionToAction,
            action: action
        );
    }

    public override void Load(ContentManager Content)
    {
        CreateBaseAnimations();
        if (Config != null)
        {
            ProfilePicture = Content.Load<Texture2D>(Config.ProfilePicturePath);
            foreach (var conf in Config.Animations)
            {
                var state = conf.Key;
                if (state != "move_left")
                {
                    var a = conf.Value;
                    animation.FromSpritesheet(state, Content.Load<Texture2D>(a.Path), new Size(a.Width, a.Height));
                }
            }
        }

        animation.Play("idle");
    }
    public void Action(Player player)
    {
        player.CanMove = false; player.CanUseItem = false; player.CanSelectItem = false; player.CanInteract = false;
        DialogManager.Instance.StartDialog(Dialogs);
        DialogStarted = true;
    }

    public void Wait(GameTime gameTime)
    {
        _direction = Vector2.Zero;
    }

    public void AutomaticMove(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Velocity = Vector2.Zero;
        if (!IsInteractingWithPlayer)
        {

            if (MovePath != null)
            {
                var v = FollowPath(dt);
                Velocity = v;
            }
            if (_currentTargetPosition != null)
            {
                Vector2 direction = _currentTargetPosition.Value - Position;
                float distance = direction.Length();
                if (distance < 4f)
                {
                    _currentTargetPosition = null;
                    _waitTime = Utils.NextFloat(_minMoveDelay, _maxMoveDelay);
                    MovePath = null;
                    // Console.WriteLine("Arrived at target position");
                }
            }
            else
            {
                _waitTime -= dt;
                if (_waitTime <= 0)
                {
                    Vector2 offset = new Vector2(Utils.Random.Next(-3, 4), Utils.Random.Next(-3, 4)) * Map.TileSize.ToVector2();
                    Vector2 potentialTarget = SpawnPoint + offset;
                    // Console.WriteLine($"[Position:{Position}], [SpawnPoint:{SpawnPoint}], [offset:{offset}] Trying new potential Target Point : {potentialTarget}");
                    var p = Map.GetMapPosFromVector2(potentialTarget, Map.TileSize);
                    if (Map.IsWalkablePoint(new Point(p.Row, p.Col)) && potentialTarget != Position && potentialTarget != _currentTargetPosition)
                    {
                        _currentTargetPosition = potentialTarget;
                        // Console.WriteLine("test");
                        List<Vector2> path = Map.FindPath(Position, _currentTargetPosition.Value, max: new Size(7, 7));
                        // Console.WriteLine("test2");

                        if (path != null && path.Count > 1)
                        {
                            MovePath = path;
                            MovePath.Reverse();
                            // Console.Write($"PotentialTargetPoint accepted: "); Utils.PrintList(MovePath);
                        }
                        else
                        {
                            _currentTargetPosition = null;
                            MovePath = null;
                        }
                    }
                    else
                    {
                        // Console.WriteLine("PotentialTargetPoint rejected");
                        _waitTime = 1f;
                    }
                }
                else
                {
                    Wait(gameTime);
                }
            }
        }
        else
        {
            _currentTargetPosition = null;
            MovePath = null;
            Wait(gameTime);
        }

        Inventory.UpdateSelectedItem(gameTime);

        // Animation
        // Animation
        if (Velocity.X > 0)
        {
            if (animation.CurrentAnimationName != "move_right")
            {
                animation.Play("move_right");
            }
            animation.CurrentAnimation.HorizontalFlip = false;
        }

        else if (Velocity.X < 0)
        {
            if (animation.CurrentAnimationName != "move_right")
            {
                animation.Play("move_right");
            }
            animation.CurrentAnimation.HorizontalFlip = true;
        }
        else if (Velocity.Y > 0)
        {
            if (animation.CurrentAnimationName != "move_down")
            {
                animation.Play("move_down");
            }
        }
        else if (Velocity.Y < 0)
        {
            if (animation.CurrentAnimationName != "move_up")
            {
                animation.Play("move_up");
            }
        }
        else
        {
            if (animation.CurrentAnimationName != "idle")
            {
                animation.Play("idle");
            }
        }

        TryMove(Map, Velocity);
    }

    public void UpdateSpeakArea(GameTime gameTime)
    {
        _speakArea = Utils.AddToRect(base.Rect, new Rectangle(-10, -10, 20, 20));
    }

    public override void UpdateMovement(GameTime gameTime)
    {
        base.UpdateMovement(gameTime);
        AutomaticMove(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            Kill();
        }
        Player player = EntityManager.Player;
        IsIntersectWithPlayer = _speakArea.Intersects(player.Rect);

        foreach (var (name, action) in _actions)
        {
            action.ConditionToShow(player);
            if (action.Show && action.ConditionToAction(player))
            {
                action.Action(player);
                break;
            }
        }
        // if (IsInteracting(player) && IsIntersectWithPlayer)
        // {
        //     Action(player);
        // }

        if (DialogStarted && !DialogManager.Instance.IsActive)
        {
            IsInteractingWithPlayer = false;
            DialogStarted = false;
            player.CanMove = true; player.CanUseItem = true; player.CanSelectItem = true; player.CanInteract = true;
            _waitTime = 1f;
        }

        UpdateMovement(gameTime);
        base.Update(gameTime);
        UpdateSpeakArea(gameTime);
        animation.Update(gameTime);

    }

    public override void UpdateOffscreen(GameTime gameTime)
    {
        base.UpdateOffscreen(gameTime);
        UpdateMovement(gameTime);
        animation.Update(gameTime);
    }

    
    public virtual void DrawIndications(SpriteBatch _spriteBatch)
    {
        if (IsIntersectWithPlayer)
        {
            int space = 0;
            Vector2 p = new Vector2(_rect.X + _rect.Width, _rect.Y + space);
            foreach (var (name, action) in _actions)
            {
                if (action.Show)
                {
                    Size s = Text.GetSize(action.Description, scale: 0.3f);
                    Text.Write(_spriteBatch, action.Description, p, Color.Blue, scale: 0.3f);
                    p.Y += s.Height + space;
                }
            }
        }
    }

    private string GetConditionInstruction()
    {
        return $"Appuyer sur [E] pour parler a {Name}";
    }

    private string GetConditionName()
    {
        return "[SPEAK]";
    }

    public void DrawHealth(SpriteBatch _spriteBatch)
    {
        var a = Utils.AddToRect(_rect, new Rectangle(-3, -3, 6, -_rect.Height + 1));
        var h = a;
        h.Width = Utils.GetValueByPercentage(a.Width, Utils.GetPercentageByValue(MaxHealth, Health));
        Shape.FillRectangle(_spriteBatch, a, Color.Gray);
        Shape.FillRectangle(_spriteBatch, h, Color.Green);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        animation.Draw(_spriteBatch, Rect);
        DrawDebug(_spriteBatch);
        Shape.DrawRectangle(_spriteBatch, _speakArea, Color.Green);
        DrawHealth(_spriteBatch);
        DrawIndications(_spriteBatch);
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
            IsDead = true;
        }
    }
}