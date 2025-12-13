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
    public event EventHandler<ITask>? OnTaskDeleted;
    public event EventHandler<ITask>? OnTaskUpdated;

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

    public void Delete(Guid taskId)
    {
        m_DbContext.TaskEntries.Where(a => a.ID == taskId).ExecuteDelete();

        var task = Find(taskId);

        if (task != null)
        {
            tasks.Remove(task);
            OnTaskDeleted?.Invoke(this, task);
            runningIDs.Remove(taskId);
        }
    }

    public void Update(Guid taskId, ZupTask task)
    {
        var taskEntity = m_DbContext.TaskEntries.Find(taskId);

        if (taskEntity != null)
        {
            taskEntity.Task = task.Task;
            taskEntity.StartedOn = task.StartedOn;
            taskEntity.EndedOn = task.EndedOn;
            taskEntity.Rank = task.Rank;

            SaveTags(taskId, task.Tags);

            m_DbContext.SaveChanges();

            task.ID = taskId;

            OnTaskUpdated?.Invoke(this, task);
        }
    }

    private void SaveTags(Guid taskID, string[] tags)
    {
        if (tags == null)
        {
            return;
        }

        var allTagsNameIDDictionary = m_DbContext.Tags.Where(a => tags.Contains(a.Name))
            .ToList()
            .ToDictionary(a => a.Name, a => a.ID);

        var query = from tet in m_DbContext.TaskEntryTags
                    join t in m_DbContext.Tags
                        on tet.TagID equals t.ID
                    where tet.TaskID == taskID
                    orderby tet.CreatedOn
                    select new
                    {
                        t.ID,
                        t.Name
                    };

        var existing = query.ToArray();


        #region Tags to remove
        var tagIDsToRemove = new List<Guid>();

        foreach (var item in existing)
        {
            if (!tags.Contains(item.Name))
            {
                tagIDsToRemove.Add(item.ID);
            }
        }

        if (tagIDsToRemove.Any())
        {
            var tagEToRem = m_DbContext.TaskEntryTags.Where(a => a.TaskID == taskID && tagIDsToRemove.Contains(a.TagID))
            .ToList();

            m_DbContext.TaskEntryTags.RemoveRange(tagEToRem);
        }
        #endregion

        #region Tags to add
        var tagNamesToAdd = new List<string>();

        foreach (var newTag in tags)
        {
            if (!existing.Any(a => a.Name == newTag))
            {
                tagNamesToAdd.Add(newTag);
            }
        }

        foreach (var tag in tagNamesToAdd.Distinct())
        {
            if (allTagsNameIDDictionary.ContainsKey(tag))
            {
                m_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                {
                    TagID = allTagsNameIDDictionary[tag],
                    TaskID = taskID,
                    CreatedOn = DateTime.Now
                });
            }
            else
            {
                var newTag = new tbl_Tag
                {
                    ID = Guid.NewGuid(),
                    Name = tag
                };

                m_DbContext.Tags.Add(newTag);

                m_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                {
                    TagID = newTag.ID,
                    TaskID = taskID,
                    CreatedOn = DateTime.Now
                });
            }
        }
        #endregion
    }

    public IEnumerator<ITask> GetEnumerator() => tasks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
