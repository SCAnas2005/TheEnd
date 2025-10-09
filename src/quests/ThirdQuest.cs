
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;

public class ThirdQuest : Quest
{
    private Npc marc;
    public ThirdQuest() : base("Third Quest", "Find an apple and give it to Marc")
    {
        // marc = (Npc)((GameScene)SceneManager.GetScene(SceneState.Game))._entities.FirstOrDefault(e => e is Npc n && n.Name == "Marc");
        marc = (Npc)EntityManager.GetFirst(e=> e is Npc n && n.Name == "Marc");
    }

    public override bool ShouldStart(Player player)
    {
        marc ??= NpcManager.GetFirst(n=> n.Name == "Marc");
        Console.WriteLine("Marc : " + marc);
        
        if (QuestManager.Get(QuestName.SecondQuest).IsCompleted && player.Map.Scene == MapScene.City1 && marc != null && marc.GetAction("speak").ConditionToAction(player))
            return true;
        return false;
    }

    public override void Start(Player player)
    {
        Status = QuestStatus.InProgress;
        marc.Dialogs = [
            new DialogLine(marc.Name, "Bonjour etranger", marc.ProfilePicture),
            new DialogLine(marc.Name, "Cela fait plusieurs jours que je n'ai pas mange, j'ai terriblement faim, amene moi quelque chose a manger", marc.ProfilePicture),
        ];
        marc.AddAction(
            name: "give",
            a: new ActionInteraction(
                name: "Donner",
                description: "Appuyer sur [G] pour donner",
                key: Microsoft.Xna.Framework.Input.Keys.G,
                conditionToShow: (player) => Status == QuestStatus.InProgress && player.Inventory.SelectedItem is AppleItem,
                conditionToAction: (player) => marc.IsIntersectWithPlayer && player.Inventory.SelectedItem is AppleItem,
                action: (player) =>
                { 
                    Item i = player.DropItem(); marc.Inventory.AddItem(i);
                    marc.IsInteractingWithPlayer = true;
                    player.CanMove = false; player.CanUseItem = false; player.CanSelectItem = false; player.CanInteract = false;
                    marc.Dialogs = [
                        new DialogLine(marc.Name, "Merci, etranger... c'est la premiere fois depuis des jours que je vais pouvoir manger a ma faim.", marc.ProfilePicture),
                        new DialogLine(marc.Name, "Tu m'as sauve la vie. Tiens, prends ca en signe de gratitude.", marc.ProfilePicture),
                    ];
                    DialogManager.Instance.StartDialog(marc.Dialogs);
                    marc.DialogStarted = true;
                }
            )
        );
        NotificationManager.Add($"Quest '{Title}' started", CFonts.Minecraft_24);
    }

    public override void Update(Player player)
    {
        if (!marc.DialogStarted)
        {
            if (marc.IsInteractingWithPlayer && marc.GetAction("speak").ConditionToAction(player))
            {
                List<DialogLine> newDialogs = [];
                if (Status == QuestStatus.InProgress)
                {
                    newDialogs = new List<DialogLine>
                    {
                        new DialogLine(marc.Name, "Tu n'as toujours rien trouve ?... J'ai l'estomac vide et je sens mes forces me quitter...", marc.ProfilePicture),
                        new DialogLine(marc.Name, "Ecoute... chaque minute compte. Trouve-moi quelque chose a manger.", marc.ProfilePicture),
                        new DialogLine(marc.Name, "J'vais finir par grignoter ma botte si ca continue.", marc.ProfilePicture),
                        new DialogLine(marc.Name, "J'ai l'impression que mon ventre crie plus fort que les infectes dehors...", marc.ProfilePicture),
                        new DialogLine(marc.Name, "Ne tarde pas trop... les rues ne sont pas sures la nuit.", marc.ProfilePicture),
                        new DialogLine(marc.Name, "Mes mains tremblent... c'est la faim. Et aussi un peu la peur.", marc.ProfilePicture)
                    };

                    // Choisit un dialogue au hasard
                    var randomDialog = newDialogs[Utils.Random.Next(newDialogs.Count)];

                    // Assigne à marc.Dialogs un tableau/list contenant juste ce dialogue
                    marc.Dialogs = [ randomDialog ];
                }
            }
        }
        if (marc.Inventory.GetFirst<AppleItem>() is Item i)
        {
            i.Kill();
            Complete(player);
        }
    }

    public override void Complete(Player player)
    {
        IsCompleted = true;
        Status = QuestStatus.Completed;
        marc.RemoveAction("give");
        player.AddMoney(50);
        NotificationManager.Add(text: "Vous avez gagne 50$", font: CFonts.Minecraft_24);
        marc.Dialogs = [
            new DialogLine(marc.Name, "Je te remercie encore!", marc.ProfilePicture)
        ];
        NotificationManager.Add($"Quest completed", CFonts.Minecraft_24);
    }

    public override void OnFailed(Player player)
    {
        throw new NotImplementedException();
    }

    public override void ShouldFailed(Player player)
    {
        throw new NotImplementedException();
    }
}