using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Runtime.InteropServices;

using Zup.CustomControls;
using Zup.Entities;

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

    public delegate void OnListReady(int listCount);
    public delegate void OnQueueTaskUpdated(int queueCount);

    public event OnListReady? OnListReadyEvent;
    public event OnQueueTaskUpdated? OnQueueTaskUpdatedEvent;

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

        LoadListToControl();
        ReorderQueuedTask();

        p_OnLoad = false;

        Opacity = Properties.Settings.Default.EntryListOpacity;

        ResizeForm();
        UpdateFormPosition();
    }

    private void LoadListToControl()
    {
        var count = 0;
        var queueCount = 0;

        var minDate = DateTime.Now.AddDays(-Properties.Settings.Default.NumDaysOfDataToLoad);

        // closed items
        foreach (var task in m_DbContext.TaskEntries
            .Where(a => a.StartedOn >= minDate && a.EndedOn != null)
            .OrderBy(a => a.StartedOn))
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.CreatedOn, task.StartedOn, task.EndedOn);

            eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
            eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
            eachEntry.OnStartEvent += EachEntry_OnStartEvent;
            eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
            eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
            eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

            AddEntryToFlowLayoutControl(eachEntry, new NewEntryEventArgs(task.Task)
            {
                StopOtherTask = true,
                StartNow = true
            });

            count++;
        }

        // not yet started
        foreach (var task in m_DbContext.TaskEntries.Where(a => a.StartedOn == null).OrderByDescending(a => a.CreatedOn))
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.CreatedOn, task.StartedOn, task.EndedOn);

            eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
            eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
            eachEntry.OnStartEvent += EachEntry_OnStartEvent;
            eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
            eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
            eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

            AddEntryToFlowLayoutControl(eachEntry, new NewEntryEventArgs(task.Task)
            {
                StopOtherTask = true,
                StartNow = true
            });

            count++;
            queueCount++;
        }

        // started but not yet closed
        foreach (var task in m_DbContext.TaskEntries.Where(a => a.StartedOn != null && a.EndedOn == null))
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.CreatedOn, task.StartedOn, task.EndedOn);

            eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
            eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
            eachEntry.OnStartEvent += EachEntry_OnStartEvent;
            eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
            eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
            eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

            AddEntryToFlowLayoutControl(eachEntry, new NewEntryEventArgs(task.Task)
            {
                StopOtherTask = true,
                StartNow = true
            });

            count++;
        }

        ListIsReady = true;

        if (OnListReadyEvent != null)
        {
            OnListReadyEvent(count);
        }

        if (OnQueueTaskUpdatedEvent != null)
        {
            OnQueueTaskUpdatedEvent(queueCount);
        }
    }

    private void ReorderQueuedTask()
    {
        var queue = new Queue<EachEntry>();

        foreach (var item in flpTaskList.Controls.Cast<EachEntry>().Where(a => a.StartedOn == null))
        {
            queue.Enqueue(item);
        }

        foreach (var item in flpTaskList.Controls.Cast<EachEntry>().Where(a => a.StartedOn != null && a.EndedOn == null))
        {
            queue.Enqueue(item);
        }

        var startIx = flpTaskList.Controls.Count - queue.Count;

        for (int i = startIx; i < flpTaskList.Controls.Count; i++)
        {
            //flpTaskList.Controls.SetChildIndex(queue.Dequeue(), i);
        }        
    }

    public void ResizeForm()
    {
        int totalHeight = EachEntry.ExpandedHeight * Properties.Settings.Default.ItemsToShow;

        // margin-bottom
        totalHeight += 1 * Properties.Settings.Default.ItemsToShow;

        Height = totalHeight;
    }

    public void ExpandFirstEntryOnly()
    {
        var lastControl = flpTaskList.Controls.Cast<EachEntry>().LastOrDefault();

        if (lastControl != null)
        {
            lastControl.IsExpanded = true;
        }
    }

    private void FormUpdateEntry_OnSavedEventHandler(tbl_TaskEntry log)
    {
        foreach (var entry in flpTaskList.Controls.Cast<EachEntry>())
        {
            if (entry.EntryID == log.ID)
            {
                entry.Text = log.Task;
                entry.StartedOn = log.StartedOn;
                entry.EndedOn = log.EndedOn;
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
            OnQueueTaskUpdatedEvent(GetQueueCount(false));
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

    // private void EachEntry_NewEntryEventHandler(string entry, bool stopOtherTask, bool startNow, Guid? parentEntryID = null, bool hideParent = false, bool bringNotesAndTags = false)
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

        if (args.BringNotes && args.ParentEntryID != null)
        {
            foreach (var note in m_DbContext.TaskEntryNotes.Where(a => a.TaskID == args.ParentEntryID).ToList())
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

        if (args.BringTags && args.ParentEntryID != null)
        {
            foreach (var tag in m_DbContext.TaskEntryTags.Where(a => a.TaskID == args.ParentEntryID).ToList())
            {
                m_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                {
                    CreatedOn = tag.CreatedOn,
                    TaskID = newE.ID,
                    TagID = tag.TagID
                });
            }
        }

        m_DbContext.SaveChanges();

        var eachEntry = new EachEntry(newE.ID, newE.Task, newE.CreatedOn, newE.StartedOn, null);

        eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
        eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
        eachEntry.OnStartEvent += EachEntry_OnStartEvent;
        eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
        eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
        eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

        AddEntryToFlowLayoutControl(eachEntry, args);

        ReorderQueuedTask();

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
            OnQueueTaskUpdatedEvent(GetQueueCount(false));
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

    // private void AddEntryToFlowLayoutControl(EachEntry newEntry, bool stopOthers = true, bool startNow = true, Guid? parentEntryID = null, bool hideParent = false)
    private void AddEntryToFlowLayoutControl(EachEntry newEntry, NewEntryEventArgs args)
    {
        flpTaskList.Controls.Add(newEntry);

        // if want it to only queue and there's nothing running
        // send it to the bottom of the queue
        if (!args.StartNow)
        {
            var newIx = flpTaskList.Controls.Count - GetQueueCount(true) - 1;

            flpTaskList.Controls.SetChildIndex(newEntry, newIx);
        }

        ActiveControl = flpTaskList.Controls[flpTaskList.Controls.Count - 1];

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