namespace Zup;

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
}

public static class ITaskExtensions
{
    public static TaskStatus GetTaskStatus(this ITask task)
    {
        if (task.IsRunning)
        {
            return TaskStatus.Running;
        }

        if (task.Rank != null)
        {
            return TaskStatus.Ranked;
        }

        if (task.StartedOn == null)
        {
            return TaskStatus.Queued;
        }

        if (task.StartedOn != null && task.EndedOn == null)
        {
            return TaskStatus.Unclosed;
        }

        if (task.StartedOn != null && task.EndedOn != null)
        {
            return TaskStatus.Closed;
        }

        return TaskStatus.Ongoing;
    }
}