using System.Data;
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

        //Left = Screen.PrimaryScreen!.WorkingArea.Width - Width - 2;
        Left = 2;
        Top = Screen.PrimaryScreen!.WorkingArea.Height - Height - 2;

        m_DbContext = dbContext;
        m_FormNewEntry = frmNewEntry;
        m_FormUpdateEntry = frmUpdateEntry;
    }

    private void frmEntryList_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        Hide();
    }

    public void ShowNewEntry()
    {
        var suggestions = flowLayoutPanel1.Controls.Cast<EachEntry>()
            .Select(a => a.Text)
            .Distinct().ToArray();

        m_FormNewEntry.ShowNewEntryDialog(suggestions);
    }

    private void frmEntryList_Load(object sender, EventArgs e)
    {
        m_FormNewEntry.OnNewEntryEvent += EachEntry_NewEntryEventHandler;
        m_FormUpdateEntry.OnDeleteEvent += FormUpdateEntry_OnDeleteEventHandler;

        var tasks = m_DbContext.TimeLogs.ToList();

        foreach (var task in tasks)
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.StartedOn, task.EndedOn);

            eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
            eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
            eachEntry.OnStartEvent += EachEntry_OnStartEvent;
            eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;

            AddEntryToFlowLayoutControl(eachEntry);
        }

        p_OnLoad = false;
    }

    private void EachEntry_OnStartEvent(int id)
    {
        CurrentRunningTaskID = id;
        LastRunningTaskID = id;
    }

    private void FormUpdateEntry_OnDeleteEventHandler(int entryID)
    {
        var entryToRemove = flowLayoutPanel1.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == entryID);

        if (entryToRemove != null)
        {
            flowLayoutPanel1.Controls.Remove(entryToRemove);
        }
    }

    private void EachEntry_NewEntryEventHandler(string entry)
    {
        var newE = new tbl_TimeLog
        {
            Task = entry,
            StartedOn = DateTime.Now
        };

        m_DbContext.TimeLogs.Add(newE);

        m_DbContext.SaveChanges();

        var eachEntry = new EachEntry(newE.ID, newE.Task, newE.StartedOn, null);

        eachEntry.OnResumeEvent += EachEntry_NewEntryEventHandler;
        eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
        eachEntry.OnStartEvent += EachEntry_OnStartEvent;
        eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;

        AddEntryToFlowLayoutControl(eachEntry);

        EachEntry_OnUpdateEvent(newE.ID);
    }

    private void EachEntry_OnUpdateEvent(int id)
    {
        m_FormUpdateEntry.ShowUpdateEntry(id);
    }

    private void AddEntryToFlowLayoutControl(EachEntry newEntry)
    {
        flowLayoutPanel1.Controls.Add(newEntry);

        ActiveControl = newEntry;

        if (!p_OnLoad)
        {
            StopAll();
            newEntry.Start();
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

    private void StopAll()
    {
        foreach (EachEntry item in flowLayoutPanel1.Controls)
        {
            if (item.IsStarted)
            {
                item.Stop();
            }
        }
    }

    public void UpdateCurrentRunningTask()
    {
        if (CurrentRunningTaskID == null)
        {
            return;
        }

        EachEntry_OnUpdateEvent(CurrentRunningTaskID.Value);
    }

    public void ToggleLastRunningTask()
    {
        if (LastRunningTaskID == null)
        {
            return;
        }

        foreach (EachEntry item in flowLayoutPanel1.Controls)
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