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
    Guid ID { get; set; }

    string Task { get; set; }

    DateTime CreatedOn { get; set; }
    DateTime? StartedOn { get; set; }
    DateTime? EndedOn { get; set; }
    DateTime? Reminder { get; set; }

    bool IsRunning { get; set; }

    byte? Rank { get; set; }

    public TaskStatus TaskStatus
    {
        get
        {
            if (IsRunning)
            {
                return TaskStatus.Running;
            }

            if (Rank != null)
            {
                return TaskStatus.Ranked;
            }

            if (StartedOn == null)
            {
                return TaskStatus.Queued;
            }

            if (StartedOn != null && EndedOn == null)
            {
                return TaskStatus.Unclosed;
            }

            if (StartedOn != null && EndedOn != null)
            {
                return TaskStatus.Closed;
            }

            return TaskStatus.Ongoing;
        }
    }
}
