

using System.Collections.Generic;

public enum QuestName
{
    FirstQuest,
    SecondQuest,
    ThirdQuest,
    FourthQuest,
}

public static class QuestManager
{
    public static Dictionary<QuestName, Quest> Quests = new();
    public static QuestName? CurrentQuestName = null;
    public static Quest CurrentQuest => IsPlayingQuest ? Quests[CurrentQuestName.Value] : null;
    public static bool IsPlayingQuest => CurrentQuestName != null;

    public static Quest Get(QuestName name)
    {
        return Quests[name];
    }

    public static void CreateAllQuests(Player player)
    {
        Quests[QuestName.FirstQuest] = new FirstQuest();
        Quests[QuestName.SecondQuest] = new SecondQuest();
        Quests[QuestName.ThirdQuest] = new ThirdQuest();
    }

    public static void Start(QuestName quest, Player player)
    {
        if (!IsPlayingQuest)
        {
            CurrentQuestName = quest;
            CurrentQuest.Start(player);
        }
    }

    public static void Update(Player player)
    {
        if (IsPlayingQuest && CurrentQuest.Status == QuestStatus.InProgress)
        {
            CurrentQuest?.Update(player);
            if (CurrentQuest.IsCompleted)
            {
                End();
            }
        }
        else
        {
            // Cherche des quests qui sont dispo
            foreach (var quest in Quests)
            {
                if (quest.Value.Status == QuestStatus.NotStarted && quest.Value.ShouldStart(player))
                {
                    Start(quest.Key, player);
                    break;
                }
            }
        }
    }

    public static void End()
    {
        if (IsPlayingQuest)
        {
            CurrentQuestName = null;
        }
    }
}