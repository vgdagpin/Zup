using System.Collections;

using Zup.EventArguments;

namespace Zup;

public class TaskCollection : IEnumerable<ITask>
{
    private HashSet<ITask> Tasks { get; set; } = new HashSet<ITask>();

    public event EventHandler<NewEntryEventArgs>? OnTaskAdded;
    public event EventHandler<ITask>? OnTaskRemoved;

    public ITask? Find(Guid id)
    {
        return Tasks.SingleOrDefault(a => a.ID == id);
    }

    public IEnumerable<ITask> ClosedTasks()
    {
        return Tasks.Where(t => t.GetTaskStatus() == TaskStatus.Closed);
    }

    public void Add(ITask task, bool runEvent = false)
    {
        Tasks.Add(task);

        if (runEvent)
        {
            OnTaskAdded?.Invoke(this, new NewEntryEventArgs(task.Task)
            {
                Task = task
            });
        }
    }

    public void Remove(Guid taskId, bool runEvent = false)
    {
        var task = Find(taskId);

        if (task != null)
        {
            Tasks.Remove(task);

            if (runEvent)
            {
                OnTaskRemoved?.Invoke(this, task);
            }
        }
    }

    public IEnumerator<ITask> GetEnumerator() => Tasks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
