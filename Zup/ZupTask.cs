using System.Diagnostics.CodeAnalysis;

namespace Zup;

public class ZupTask : ITask
{
    public Guid EntryID { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
    public DateTime? Reminder { get; set; }
    public byte? Rank { get; set; }
    public bool IsRunning { get; set; }


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


    public bool Equals(ITask? x, ITask? y)
    {
        if (x == null && y == null)
        {
            return true;
        }
        if (x == null || y == null)
        {
            return false;
        }
        return x.EntryID == y.EntryID;
    }

    public int GetHashCode([DisallowNull] ITask obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return obj.EntryID.GetHashCode();
    }
}
