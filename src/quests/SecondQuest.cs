
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Xna.Framework;

public class SecondQuest : Quest
{
    private Item i = null;
    public SecondQuest() : base("Second Quest", "Find ammo and medkit")
    {

    }

    public override bool ShouldStart(Player player)
    {
        if (QuestManager.Get(QuestName.FirstQuest).IsCompleted && player.Map.Scene == MapScene.Home)
            return true;
        return false;
    }

    public override void Start(Player player)
    {
        // List<ArmoireObject> armoires = ((GameScene)SceneManager.GetScene(SceneState.Game)).InteractionObjects[MapScene.Home].OfType<ArmoireObject>().ToList();
        // ArmoireObject a = armoires[0];
        ArmoireObject a = InteractionObjectsManager.GetFirst<ArmoireObject>(MapScene.Home);
        i = new MedkitItem(
            rect: new Rectangle(0, 0, player.Map.TileSize.Width, player.Map.TileSize.Height),
            name: "Medkit quest 2",
            map: player.Map
        );
        i.Load(Globals.Content);

        // gameScene.AddEntity(i);
        ItemManager.AddItem(i);
        a.AddItem(i);
        Status = QuestStatus.InProgress;
        NotificationManager.Add($"Quest '{Title}' started", CFonts.Minecraft_24);
    }

    public override void Update(Player player)
    {
        if (Status == QuestStatus.InProgress)
        {
            if (i != null && player.Inventory.Has(i))
            {
                Complete(player);
            }
        }
    }

    public override void Complete(Player player)
    {
        IsCompleted = true;
        Status = QuestStatus.Completed;
        player.AddMoney(5);
        NotificationManager.Add($"Quest completed", CFonts.Minecraft_24);

        Event cinematicEvent = null;
        cinematicEvent = new Event(
            shouldStart: () => player.Map.Scene == MapScene.City1,
            start: () =>
            {
                var marc = NpcManager.CreateBasicNpcFromType(
                    typeof(Npc),
                    Map.GetPosFromMap((28, 38), player.Map.TileSize),
                    map: player.Map,
                    config: NpcManager.GetConfigByName("Marc")
                );

                NpcManager.AddNpc(marc);

                var v = marc.MoveTo(Map.GetPosFromMap((18, 31), marc.Map.TileSize));
                Console.WriteLine("marc will move ? : " + v);

                Vector2 camStart = Camera2D.Position;
                Vector2 camTarget = marc.Position - new Vector2(Camera2D.CameraLogicalSize.Width, Camera2D.CameraLogicalSize.Height) / (2 * Camera2D.Zoom * Camera2D.Scale);

                Camera2D.SetTarget(null);

                CameraCinematicController.Start(
                    from: camStart,
                    to: camTarget,
                    durationSeconds: 1.5f,
                    map: player.Map,
                    onComplete: () =>
                    {
                        // Après l'animation vers le zombie, on suit le zombie pendant 5s
                        marc.CanMove = true; // Laisse le zombie bouger
                        Camera2D.SetTarget(marc);
                        TimerManager.Wait(5f, () =>
                        {
                            CameraCinematicController.Start(
                                from: Camera2D.Position,
                                to: player.Position - new Vector2(Camera2D.CameraLogicalSize.Width, Camera2D.CameraLogicalSize.Height) / (2 * Camera2D.Zoom * Camera2D.Scale),
                                durationSeconds: 2f,
                                map: player.Map,
                                onComplete: () =>
                                {
                                    CameraCinematicController.Stop(); // tout est terminé
                                    player.CanMove = true; player.CanUseItem = true;
                                    Camera2D.SetTarget(player);
                                    cinematicEvent.Finished = true;
                                }
                            );
                        });
                    }
                );

            },
            shouldEnd: () => cinematicEvent != null && cinematicEvent.Finished,
            end: () =>
            {
                Console.WriteLine("Event finished");
            },
            update: (gameTime) =>
            {

            }
        );
        EventsManager.Instance.Add(
            e: cinematicEvent
        );
    }

    public override void OnFailed(Player player)
    {
        throw new System.NotImplementedException();
    }

    public override void ShouldFailed(Player player)
    {
        throw new System.NotImplementedException();
    }
}