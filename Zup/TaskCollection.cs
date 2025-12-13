using System.Collections;
using System.Data;

using Microsoft.EntityFrameworkCore;

using Zup.Entities;
using Zup.EventArguments;

namespace Zup;

public class TaskCollection : IEnumerable<ITask>
{
    private readonly ZupDbContext m_DbContext;
    private readonly SettingHelper p_SettingHelper;

    public Guid[] RunningIDs => runningIDs.ToArray();

    private HashSet<Guid> runningIDs { get; set; } = new HashSet<Guid>();

    private HashSet<ITask> tasks { get; set; } = new HashSet<ITask>();

    public event EventHandler<NewEntryEventArgs>? OnTaskStarted;
    public event EventHandler<ITask>? OnTaskStopped;

    public TaskCollection(ZupDbContext dbContext, SettingHelper settingHelper)
    {
        m_DbContext = dbContext;
        p_SettingHelper = settingHelper;
    }

    public ITask? Find(Guid id)
    {
        return tasks.SingleOrDefault(a => a.ID == id);
    }

    public IEnumerable<ITask> ClosedTasks()
    {
        return tasks.Where(t => t.GetTaskStatus() == TaskStatus.Closed);
    }

    public void Add(ITask task)
    {
        tasks.Add(task);
    }

    public void Start(Guid id)
    {

    }

    public void Start(string text, bool startNow, bool stopOtherTasks, bool hideParent, bool bringNotes, bool bringTags, Guid? parentEntryID = null)
    {
        var newE = new tbl_TaskEntry
        {
            ID = Guid.NewGuid(),
            Task = text,
            CreatedOn = DateTime.Now
        };

        m_DbContext.BackupDb();

        if (startNow)
        {
            newE.StartedOn = DateTime.Now;
        }

        m_DbContext.TaskEntries.Add(newE);

        var parentEntry = parentEntryID != null
            ? m_DbContext.TaskEntries.Find(parentEntryID)
            : null;

        // bring notes, tags and rank from parent, this is when the user started a queued task
        if (parentEntry != null)
        {
            if (bringNotes)
            {
                foreach (var note in m_DbContext.TaskEntryNotes.Where(a => a.TaskID == parentEntry.ID).ToList())
                {
                    m_DbContext.TaskEntryNotes.Add(new tbl_TaskEntryNote
                    {
                        ID = Guid.NewGuid(),
                        TaskID = newE.ID,
                        CreatedOn = note.CreatedOn,
                        Notes = note.Notes,
                        RTF = note.RTF,
                        UpdatedOn = note.UpdatedOn
                    });
                }
            }

            if (bringTags)
            {
                foreach (var tag in m_DbContext.TaskEntryTags.Where(a => a.TaskID == parentEntry.ID).ToList())
                {
                    m_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                    {
                        CreatedOn = tag.CreatedOn,
                        TaskID = newE.ID,
                        TagID = tag.TagID
                    });
                }
            }

            if (hideParent)
            {
                m_DbContext.TaskEntries.Remove(parentEntry);
            }
        }

        //if (args.GetTags && parentEntry == null)
        //{
        //    var minDate = DateTime.Now.AddDays(-p_SettingHelper.NumDaysOfDataToLoad);

        //    var tagIDs =
        //        (
        //            from e in m_DbContext.TaskEntries.Where(a => (a.StartedOn >= minDate && a.EndedOn != null) || a.StartedOn == null || (a.StartedOn != null && a.EndedOn == null))
        //            join t in m_DbContext.TaskEntryTags on e.ID equals t.TaskID
        //            orderby t.CreatedOn descending
        //            where e.Task == args.Entry
        //            select t.TagID
        //        ).Distinct();

        //    foreach (var tagID in tagIDs)
        //    {
        //        m_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
        //        {
        //            CreatedOn = DateTime.Now,
        //            TaskID = newE.ID,
        //            TagID = tagID
        //        });
        //    }
        //}

        m_DbContext.SaveChanges();

        runningIDs.Add(newE.ID);

        var task = new ZupTask
        {
            ID = newE.ID,
            Task = newE.Task,
            CreatedOn = newE.CreatedOn,
            StartedOn = newE.StartedOn,
            EndedOn = newE.EndedOn,
            Reminder = newE.Reminder,
            Rank = newE.Rank
        };

        var args = new NewEntryEventArgs(text)
        {
            Task = task,
            StopOtherTask = stopOtherTasks,
            StartNow = startNow,
            ParentEntryID = parentEntryID,
            HideParent = hideParent,
            BringNotes = bringNotes,
            BringTags = bringTags
        };

        tasks.Add(task);

        OnTaskStarted?.Invoke(this, args);
    }

    public void Stop(Guid taskId, DateTime? endOn = null)
    {
        var task = Find(taskId);

        if (task != null)
        {
            task.EndedOn = endOn ?? DateTime.Now;
            runningIDs.Remove(taskId);

            var taskEntity = m_DbContext.TaskEntries.Find(taskId);

            if (taskEntity != null)
            {
                taskEntity.EndedOn = task.EndedOn;
                m_DbContext.SaveChanges();
            }

            OnTaskStopped?.Invoke(this, task);
        }
    }

    public IEnumerator<ITask> GetEnumerator() => tasks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
