namespace Zup;

public enum TaskStatus
{
    Ongoing,
    Queued,
    Ranked,
    Closed,
    Unclosed,
    Running
}

public interface ITask : IEqualityComparer<ITask>
{
    Guid EntryID { get; set; }

    string Text { get; set; }

    DateTime CreatedOn { get; set; }
    DateTime? StartedOn { get; set; }
    DateTime? EndedOn { get; set; }
    DateTime? Reminder { get; set; }

    bool IsRunning { get; set; }

    byte? Rank { get; set; }

    TaskStatus TaskStatus { get; }
}
