using System.Diagnostics.CodeAnalysis;

namespace Zup;

public class ZupTask : ITask
{
    public Guid ID { get; set; }
    public string Task { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
    public DateTime? Reminder { get; set; }
    public byte? Rank { get; set; }
    public bool IsRunning { get; set; }



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
        return x.ID == y.ID;
    }

    public int GetHashCode([DisallowNull] ITask obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return obj.ID.GetHashCode();
    }
}
