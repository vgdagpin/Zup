using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Runtime.InteropServices;

using Zup.CustomControls;
using Zup.Entities;
using Zup.EventArguments;

namespace Zup;

public partial class frmEntryList : Form
{
    private frmNewEntry m_FormNewEntry;
    private frmUpdateEntry m_FormUpdateEntry;

    private bool p_OnLoad = true;
    private readonly ZupDbContext m_DbContext;

    private Guid? CurrentRunningTaskID;
    private Guid? LastRunningTaskID;

    public bool ListIsReady { get; set; }

    public event EventHandler<ListReadyEventArgs>? OnListReadyEvent;
    public event EventHandler<QueueTaskUpdatedEventArgs>? OnQueueTaskUpdatedEvent;
    public event EventHandler<TokenEventArgs>? OnTokenDoubleClicked;

    #region Draggable Form
    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    private void frmEntryList_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }

    private void frmEntryList_Move(object sender, EventArgs e)
    {
        tmrSaveSetting.Enabled = false;
        tmrSaveSetting.Enabled = true;
    }

    private void tmrSaveSetting_Tick(object sender, EventArgs e)
    {
        Properties.Settings.Default.FormLocationX = Left;
        Properties.Settings.Default.FormLocationY = Top;
        Properties.Settings.Default.Save();

        tmrSaveSetting.Enabled = false;
    }

    private void UpdateFormPosition()
    {
        if (Properties.Settings.Default.FormLocationX == 0
            && Properties.Settings.Default.FormLocationY == 0)
        {
            Left = Screen.PrimaryScreen!.WorkingArea.Width - Width - 2;
            Top = Screen.PrimaryScreen!.WorkingArea.Height - Height - 2;
        }
        else
        {
            Left = Properties.Settings.Default.FormLocationX;
            Top = Properties.Settings.Default.FormLocationY;
        }
    }
    #endregion

    protected override CreateParams CreateParams
    {
        get
        {
            var Params = base.CreateParams;
            Params.ExStyle |= 0x00000080;
            return Params;
        }
    }

    public frmEntryList(ZupDbContext dbContext, frmNewEntry frmNewEntry, frmUpdateEntry frmUpdateEntry)
    {
        InitializeComponent();

        m_DbContext = dbContext;
        m_FormNewEntry = frmNewEntry;
        m_FormUpdateEntry = frmUpdateEntry;

        m_DbContext.Database.Migrate();
    }

    private void frmEntryList_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        Hide();
    }

    public void ShowNewEntry()
    {
        var suggestions = new List<string>();

        var currentList = flpTaskList.Controls.Cast<EachEntry>()
            .Where(a => a.Visible)
            .Select(a => a.Text)
            .Reverse()
            .ToArray();

        if (currentList.Length > 1)
        {
            suggestions.Add(currentList[1]);
        }

        foreach (var item in currentList)
        {
            if (suggestions.Contains(item))
            {
                continue;
            }

            suggestions.Add(item);
        }

        m_FormNewEntry.ShowNewEntryDialog(suggestions.ToArray());
    }

    private void frmEntryList_Load(object sender, EventArgs e)
    {
        m_FormNewEntry.OnNewEntryEvent += EachEntry_NewEntryEventHandler;
        m_FormUpdateEntry.OnDeleteEvent += FormUpdateEntry_OnDeleteEventHandler;
        m_FormUpdateEntry.OnSavedEvent += FormUpdateEntry_OnSavedEventHandler;
        m_FormUpdateEntry.OnTokenDoubleClicked += FormUpdateEntry_OnTokenDoubleClicked;

        var list = LoadListToControl();

        SortTasks();

        ListIsReady = true;

        if (OnListReadyEvent != null)
        {
            OnListReadyEvent(this, new ListReadyEventArgs(list.HasItem));
        }

        if (OnQueueTaskUpdatedEvent != null)
        {
            OnQueueTaskUpdatedEvent(this, new QueueTaskUpdatedEventArgs(list.QueueCount));
        }

        p_OnLoad = false;

        Opacity = Properties.Settings.Default.EntryListOpacity;

        ResizeForm();
        UpdateFormPosition();
    }

    private void FormUpdateEntry_OnTokenDoubleClicked(object? sender, TokenEventArgs e)
    {
        OnTokenDoubleClicked?.Invoke(sender, e);
    }

    private (bool HasItem, int QueueCount) LoadListToControl()
    {
        var queueCount = 0;
        var hasItem = false;

        var minDate = DateTime.Now.AddDays(-Properties.Settings.Default.NumDaysOfDataToLoad);

        foreach (var task in m_DbContext.TaskEntries.Where(a => a.CreatedOn >= minDate))
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.CreatedOn, task.StartedOn, task.EndedOn)
            {
                Rank = task.Rank
            };

            eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
            eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
            eachEntry.OnStartEvent += EachEntry_OnStartEvent;
            eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
            eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
            eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

            flpTaskList.Controls.Add(eachEntry);

            hasItem = true;

            if (task.StartedOn == null)
            {
                queueCount++;
            }
        }

        return (hasItem, queueCount);
    }

    public void SortTasks()
    {
        var stack = new Queue<EachEntry>();
        var list = flpTaskList.Controls.Cast<EachEntry>().Where(a => a.Visible).ToList();
        var all = flpTaskList.Controls.Cast<EachEntry>().Where(a => a.Visible).ToArray();
        var minDate = DateTime.Now.AddDays(-Properties.Settings.Default.NumDaysOfDataToLoad);

        flpTaskList.SuspendLayout();

        // running
        foreach (var item in all.Where(a => a.IsRunning).OrderBy(a => a.CreatedOn))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // with ranking
        foreach (var item in all.Where(a => a.Rank != null).OrderBy(a => a.Rank))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // started but not yet closed
        foreach (var item in all.Where(a => a.StartedOn != null && a.EndedOn == null))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // not yet started
        foreach (var item in all.Where(a => a.StartedOn == null).OrderByDescending(a => a.CreatedOn))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // closed items
        foreach (var item in all.Where(a => a.StartedOn >= minDate && a.EndedOn != null)
            .OrderByDescending(a => a.StartedOn))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        foreach (var item in list)
        {
            stack.Enqueue(item);
        }

        var i = 0;
        while (stack.TryDequeue(out var entry))
        {
            flpTaskList.Controls.SetChildIndex(entry, i);
            i++;
        }

        EachEntry? firstItem = flpTaskList.Controls.Count > 0
            ? (EachEntry)flpTaskList.Controls[flpTaskList.Controls.Count - 1]
            : null;

        if (firstItem != null)
        {
            flpTaskList.ScrollControlIntoView(firstItem);

            firstItem.IsFirstItem = true;
        }

        flpTaskList.ResumeLayout();
    }

    private void ReorderQueuedTask()
    {
        //var queue = new Queue<EachEntry>();

        //foreach (var item in flpTaskList.Controls.Cast<EachEntry>().Where(a => a.StartedOn == null))
        //{
        //    queue.Enqueue(item);
        //}

        //foreach (var item in flpTaskList.Controls.Cast<EachEntry>().Where(a => a.StartedOn != null && a.EndedOn == null))
        //{
        //    queue.Enqueue(item);
        //}

        //var startIx = flpTaskList.Controls.Count - queue.Count;

        //for (int i = startIx; i < flpTaskList.Controls.Count; i++)
        //{
        //    flpTaskList.Controls.SetChildIndex(queue.Dequeue(), i);
        //}
    }

    public void ResizeForm()
    {
        int totalHeight = EachEntry.ExpandedHeight * Properties.Settings.Default.ItemsToShow;

        // margin-bottom
        totalHeight += 1 * Properties.Settings.Default.ItemsToShow;

        Height = totalHeight;
    }

    private void FormUpdateEntry_OnSavedEventHandler(object? sender, SaveEventArgs args)
    {
        foreach (var entry in flpTaskList.Controls.Cast<EachEntry>())
        {
            if (entry.EntryID == args.Task.ID)
            {
                entry.Text = args.Task.Task;
                entry.StartedOn = args.Task.StartedOn;
                entry.EndedOn = args.Task.EndedOn;
                entry.Rank = args.Task.Rank;
            }
        }
    }

    private void EachEntry_OnStartEvent(Guid id)
    {
        CurrentRunningTaskID = id;
        LastRunningTaskID = id;
    }

    private void FormUpdateEntry_OnDeleteEventHandler(Guid entryID)
    {
        DeleteTimeLog(entryID);

        ResizeForm();

        if (OnQueueTaskUpdatedEvent != null)
        {
            OnQueueTaskUpdatedEvent(this, new QueueTaskUpdatedEventArgs(GetQueueCount(false)));
        }
    }

    private void DeleteTimeLog(Guid entryID)
    {
        var entry = m_DbContext.TaskEntries.Find(entryID);

        if (entry != null)
        {
            m_DbContext.TaskEntries.Remove(entry);
            m_DbContext.SaveChanges();
        }

        var entryToRemove = flpTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == entryID);

        if (entryToRemove != null)
        {
            // only hide, remove will auto scroll the list to bottom because the list is in reverse
            entryToRemove.Hide();
        }
    }

    private void EachEntry_NewEntryEventHandler(object? sender, NewEntryEventArgs args)
    {
        var newE = new tbl_TaskEntry
        {
            ID = Guid.NewGuid(),
            Task = args.Entry,
            CreatedOn = DateTime.Now
        };

        if (args.StartNow)
        {
            newE.StartedOn = DateTime.Now;
        }

        m_DbContext.TaskEntries.Add(newE);

        tbl_TaskEntry? parentEntry = args.ParentEntryID != null
            ? m_DbContext.TaskEntries.Find(args.ParentEntryID)
            : null;

        // bring notes, tags and rank from parent, this is when the user started a queued task
        if (parentEntry != null)
        {
            if (args.BringNotes)
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

            if (args.BringTags)
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

            parentEntry.Rank = null;
        }

        if (args.GetTags && parentEntry == null)
        {
            var minDate = DateTime.Now.AddDays(-Properties.Settings.Default.NumDaysOfDataToLoad);

            var tagIDs = (from e in m_DbContext.TaskEntries.Where(a => (a.StartedOn >= minDate && a.EndedOn != null) || a.StartedOn == null || (a.StartedOn != null && a.EndedOn == null))
                          join t in m_DbContext.TaskEntryTags on e.ID equals t.TaskID
                          orderby t.CreatedOn descending
                          where e.Task == args.Entry
                          select t.TagID)
                             .Distinct();

            foreach (var tagID in tagIDs)
            {
                m_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                {
                    CreatedOn = DateTime.Now,
                    TaskID = newE.ID,
                    TagID = tagID
                });
            }
        }

        m_DbContext.SaveChanges();

        var eachEntry = new EachEntry(newE.ID, newE.Task, newE.CreatedOn, newE.StartedOn, null);

        if (newE.Rank != null)
        {
            eachEntry.Rank = newE.Rank;
        }

        eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
        eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
        eachEntry.OnStartEvent += EachEntry_OnStartEvent;
        eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
        eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
        eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

        AddEntryToFlowLayoutControl(eachEntry, args);

        SortTasks();

        if (args.HideParent && args.ParentEntryID != null)
        {
            DeleteTimeLog(args.ParentEntryID.Value);
        }

        if (Properties.Settings.Default.AutoOpenUpdateWindow)
        {
            ShowUpdateEntry(newE.ID);
        }

        ResizeForm();

        if (OnQueueTaskUpdatedEvent != null)
        {
            OnQueueTaskUpdatedEvent(this, new QueueTaskUpdatedEventArgs(GetQueueCount(false)));
        }
    }

    private int GetQueueCount(bool includeHidden)
    {
        return flpTaskList.Controls.Cast<EachEntry>().Count(a => a.StartedOn == null && (a.Visible || includeHidden));
    }

    private void EachEntry_OnUpdateEvent(Guid id)
    {
        ShowUpdateEntry(id);
    }

    public async void ShowUpdateEntry(Guid entryID)
    {
        await m_FormUpdateEntry.ShowUpdateEntry(entryID);
    }

    private void AddEntryToFlowLayoutControl(EachEntry newEntry, NewEntryEventArgs args)
    {
        flpTaskList.Controls.Add(newEntry);

        //// if want it to only queue and there's nothing running
        //// send it to the bottom of the queue
        //if (!args.StartNow)
        //{
        //    var newIx = flpTaskList.Controls.Count - GetQueueCount(true) - 1;

        //    flpTaskList.Controls.SetChildIndex(newEntry, newIx);
        //}

        //ActiveControl = flpTaskList.Controls[flpTaskList.Controls.Count - 1];

        if (!p_OnLoad)
        {
            foreach (EachEntry item in flpTaskList.Controls)
            {
                item.IsFirstItem = false;

                if (args.StopOtherTask)
                {
                    if (item.IsStarted)
                    {
                        item.Stop();
                    }

                    if (item.StartedOn == null)
                    {
                        item.IsExpanded = false;
                    }
                }
            }

            if (newEntry.StartedOn != null)
            {
                newEntry.Start();
            }

            newEntry.IsFirstItem = true;
        }
    }

    private void EachEntry_OnStopEventHandler(Guid id, DateTime endOn)
    {
        var existingE = m_DbContext.TaskEntries.Find(id);

        if (existingE != null)
        {
            existingE.EndedOn = endOn;

            m_DbContext.SaveChanges();
        }

        CurrentRunningTaskID = null;
    }

    public void UpdateCurrentRunningTask()
    {
        if (CurrentRunningTaskID == null)
        {
            return;
        }

        ShowUpdateEntry(CurrentRunningTaskID.Value);
    }

    public void ToggleLastRunningTask()
    {
        if (LastRunningTaskID == null)
        {
            return;
        }

        foreach (EachEntry item in flpTaskList.Controls)
        {
            if (item.EntryID != LastRunningTaskID)
            {
                continue;
            }

            if (item.IsStarted)
            {
                item.Stop();
            }
            else
            {
                item.Start();
            }
        }
    }
}