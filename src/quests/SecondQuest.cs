
using System.Collections.Generic;
using System.Linq;
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