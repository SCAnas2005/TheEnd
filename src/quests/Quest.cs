public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed
}

public abstract class Quest
{
    public string Title { get; protected set; }
    public string Description { get; protected set; }
    public QuestStatus Status;

    public int Rewards { get; protected set; }

    public Quest(string title, string desc, QuestStatus status = QuestStatus.NotStarted)
    {
        Title = title;
        Description = desc;
        Status = status;

        IsCompleted = false;
        Rewards = 5;
    }

    public bool IsCompleted { get; protected set; }

    public abstract bool ShouldStart(Player player);
    public abstract void Start(Player player);
    public abstract void Update(Player player);
    public abstract void Complete(Player player);
}
