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

    private int? CurrentRunningTaskID;
    private int? LastRunningTaskID;

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

        p_OnLoad = false;

        Opacity = Properties.Settings.Default.EntryListOpacity;

        ResizeForm();
        UpdateFormPosition();
    }

    private void LoadListToControl()
    {
        var count = 0;
        var queueCount = 0;

        foreach (var task in m_DbContext.TimeLogs.ToList())
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.StartedOn, task.EndedOn);

            eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
            eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
            eachEntry.OnStartEvent += EachEntry_OnStartEvent;
            eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
            eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
            eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

            AddEntryToFlowLayoutControl(eachEntry);

            count++;

            if (eachEntry.StartedOn == null)
            {
                queueCount++;
            }
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

    private void FormUpdateEntry_OnSavedEventHandler(tbl_TimeLog log)
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

    private void EachEntry_OnStartEvent(int id)
    {
        CurrentRunningTaskID = id;
        LastRunningTaskID = id;
    }

    private void FormUpdateEntry_OnDeleteEventHandler(int entryID)
    {
        DeleteTimeLog(entryID);

        ResizeForm();

        if (OnQueueTaskUpdatedEvent != null)
        {
            OnQueueTaskUpdatedEvent(GetQueueCount(false));
        }
    }

    private void DeleteTimeLog(int entryID)
    {
        var entry = m_DbContext.TimeLogs.Find(entryID);

        if (entry != null)
        {
            m_DbContext.TimeLogs.Remove(entry);
            m_DbContext.SaveChanges();
        }

        var entryToRemove = flpTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == entryID);

        if (entryToRemove != null)
        {
            // only hide, remove will auto scroll the list to bottom because the list is in reverse
            entryToRemove.Hide();
        }
    }

    private void EachEntry_NewEntryEventHandler(string entry, bool stopOtherTask, bool startNow, int? parentEntryID = null, bool hideParent = false, bool bringNotes = false)
    {
        var newE = new tbl_TimeLog
        {
            Task = entry
        };

        if (startNow)
        {
            newE.StartedOn = DateTime.Now;
        }

        m_DbContext.TimeLogs.Add(newE);

        m_DbContext.SaveChanges();

        if (bringNotes && parentEntryID != null)
        {
            foreach (var note in m_DbContext.Notes.Where(a => a.LogID == parentEntryID).ToList())
            {
                m_DbContext.Notes.Add(new tbl_Note
                {
                    LogID = newE.ID,
                    CreatedOn = note.CreatedOn,
                    Notes = note.Notes,
                    RTF = note.RTF,
                    UpdatedOn = note.UpdatedOn
                });
            }

            m_DbContext.SaveChanges();
        }

        var eachEntry = new EachEntry(newE.ID, newE.Task, newE.StartedOn, null);

        eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
        eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
        eachEntry.OnStartEvent += EachEntry_OnStartEvent;
        eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
        eachEntry.OnStartQueueEvent += EachEntry_NewEntryEventHandler;
        eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);

        AddEntryToFlowLayoutControl(eachEntry, stopOtherTask, startNow, parentEntryID, hideParent);

        if (hideParent && parentEntryID != null)
        {
            DeleteTimeLog(parentEntryID.Value);
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

    private void EachEntry_OnUpdateEvent(int id)
    {
        ShowUpdateEntry(id);
    }

    public void ShowUpdateEntry(int entryID)
    {
        m_FormUpdateEntry.ShowUpdateEntry(entryID);
    }

    private void AddEntryToFlowLayoutControl(EachEntry newEntry, bool stopOthers = true, bool startNow = true, int? parentEntryID = null, bool hideParent = false)
    {
        flpTaskList.Controls.Add(newEntry);

        // if want it to only queue and there's nothing running
        // go to the bottom of the queue
        if (!startNow && CurrentRunningTaskID != null)
        {
            flpTaskList.Controls.SetChildIndex(newEntry, flpTaskList.Controls.Count - GetQueueCount(true) - 1);
        }

        ActiveControl = newEntry;

        if (!p_OnLoad)
        {
            foreach (EachEntry item in flpTaskList.Controls)
            {
                item.IsFirstItem = false;

                if (stopOthers)
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

    private void EachEntry_OnStopEventHandler(int id, DateTime endOn)
    {
        var existingE = m_DbContext.TimeLogs.Find(id);

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