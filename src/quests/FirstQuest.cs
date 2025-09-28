using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

public class FirstQuest : Quest
{
    Zombies z = null;
    private TimerHandle _waitHandle = null;
    private bool _cinematicStarted = false;
    private bool _cameraTrackingZombie = false;

    public FirstQuest(Player player) : base("First quest", "Kill a zombie") { }

    public override bool ShouldStart(Player player)
    {
        return player.Inventory.GetAll<Gun>().Any(w => w.Name == "First gun") && player.Map.Scene == MapScene.City1;
    }

    public override void Start(Player player)
    {
        // Optionnel : afficher un message, journal de quête, etc.
        Console.WriteLine("Quête commencée : " + Title);
        Console.WriteLine("Description : " + Description);
        NotificationManager.Add($"Quest '{Title}' started", CFonts.Minecraft_24);
        Status = QuestStatus.InProgress;

        CreateMainZombie(player);

        Vector2 camStart = Camera2D.Position;
        Vector2 camTarget = z.Position - new Vector2(Camera2D.CameraLogicalSize.Width, Camera2D.CameraLogicalSize.Height) / (2 * Camera2D.Zoom * Camera2D.Scale);

        _cinematicStarted = true;
        _cameraTrackingZombie = true;

        Camera2D.FocusOnPlayer = false;
        player.CanMove = false; player.CanUseItem = false;

        CameraCinematicController.Start(
            from: camStart,
            to: camTarget,
            durationSeconds: 1.5f,
            map: player.Map,
            onComplete: () =>
            {
                // Après l'animation vers le zombie, on suit le zombie pendant 5s
                z.CanMove = true; // Laisse le zombie bouger
                _waitHandle = TimerManager.Wait(5f, () =>
                {
                    _cameraTrackingZombie = false; // on arrête de suivre le zombie
                    CameraCinematicController.Start(
                        from: Camera2D.Position,
                        to: player.Position - new Vector2(Camera2D.CameraLogicalSize.Width, Camera2D.CameraLogicalSize.Height) / (2 * Camera2D.Zoom * Camera2D.Scale),
                        durationSeconds: 1.5f,
                        map: player.Map,
                        onComplete: () =>
                        {
                            Camera2D.FocusOnPlayer = true;
                            _cinematicStarted = false; // tout est terminé
                            player.CanMove = true; player.CanUseItem = true;
                        }
                    );
                });
            }
        );

    }

    public void CreateMainZombie(Player player)
    {
        (int, int) pos = (20, 20);
        z = new Zombies(
            rect: new Rectangle(pos.Item1 * player.Map.TileSize.Width, pos.Item2 * player.Map.TileSize.Height, player.Map.TileSize.Width - 3, player.Map.TileSize.Width - 3),
            src: "",
            speed: 2,
            health: 50,
            map: player.Map,
            debug: true
        );
        z.Load(Globals.Content);
        z.CanMove = false;
        // ((GameScene)SceneManager.GetScene(SceneState.Game)).AddEntity(z);
        EntityManager.AddEntity(z);
    }

    public override void Update(Player player)
    {
        if (Status == QuestStatus.InProgress)
        {
            if (_cinematicStarted && _cameraTrackingZombie)
            {
                Camera2D.FocusOnPlayer = true;
                Camera2D.LookAtPosition(z.Position);
                Camera2D.FocusOnPlayer = false;
            }
            if (z.IsDead)
            {
                Complete(player);
            }
        }
    }

    public override void Complete(Player player)
    {
        Status = QuestStatus.Completed;
        IsCompleted = true;
        player.AddMoney(10);
        NotificationManager.Add($"Quest completed", CFonts.Minecraft_24);
    }
}