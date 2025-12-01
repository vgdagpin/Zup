using System.Collections;
using System.Data;
using System.Runtime.InteropServices;

using Microsoft.EntityFrameworkCore;

using Zup.CustomControls;
using Zup.EventArguments;

namespace Zup;

public partial class frmEntryList : Form
{
    const int maxOngoingTaskRow = 4;
    const int maxQueuedTaskRow = 2;
    const int maxRankedTaskRow = 4;

    private readonly SettingHelper settingHelper;
    private frmMain m_FormMain = null!;

    private bool p_OnLoad = true;
    private readonly ZupDbContext m_DbContext;

    private Guid? CurrentRunningTaskID;
    private Guid? LastRunningTaskID;

    private EachEntry? m_SelectedEntryToDelete;


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

    public void MoveToCenterAndBringToFront()
    {
        Left = (Screen.PrimaryScreen!.WorkingArea.Width / 2) - (Width / 2);
        Top = (Screen.PrimaryScreen!.WorkingArea.Height / 2) - (Height / 2);
        BringToFront();
        Activate();
    }

    public void SetFormMain(frmMain frmMain)
    {
        m_FormMain = frmMain;

        m_FormMain.OnNewTask += FormMain_OnNewTask;
        m_FormMain.OnTaskDeleted += M_FormMain_OnTaskDeleted;
        m_FormMain.OnTaskUpdated += M_FormMain_OnTaskUpdated;
    }

    private void M_FormMain_OnTaskUpdated(object? sender, ITask e)
    {
        var eachEntry = flpTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == e.EntryID)
            ?? flpQueuedTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == e.EntryID)
            ?? flpRankedTasks.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == e.EntryID);

        if (eachEntry != null)
        {
            eachEntry.Text = e.Text;
            eachEntry.StartedOn = e.StartedOn;
            eachEntry.EndedOn = e.EndedOn;
            eachEntry.Rank = e.Rank;
        }
    }

    private void M_FormMain_OnTaskDeleted(object? sender, ITask e)
    {
        var entryToRemove = flpTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == e.EntryID);

        if (entryToRemove != null)
        {
            // only hide, remove will auto scroll the list to bottom because the list is in reverse
            entryToRemove.Hide();
        }

        ResizeForm();
    }

    private void FormMain_OnNewTask(object? sender, NewEntryEventArgs e)
    {
        var eachEntry = new EachEntry(e.Task.EntryID, e.Task.Text, e.Task.CreatedOn, e.Task.StartedOn, null);

        eachEntry.GotFocus += (sender, e) => ActiveControl = null;

        eachEntry.OnReRunEvent += EachEntry_OnReRunEventHandler;
        eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
        eachEntry.OnStartEvent += EachEntry_OnStartEvent;
        eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
        eachEntry.OnStartQueueEvent += EachEntry_OnStartQueueEventHandler;
        eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);
        eachEntry.TaskRightClick += EachEntry_TaskRightClick;

        if (eachEntry.TaskStatus == TaskStatus.Queued)
        {
            flpQueuedTaskList.Controls.Add(eachEntry);
        }
        else
        {
            AddEntryToFlowLayoutControl(eachEntry, e);
        }

        SortTasks(flpTaskList.Controls);

        RefreshList();

        if (settingHelper.UsePillTimer)
        {
            Hide();
        }
    }

    private void UpdateFormPosition()
    {
        if (settingHelper.FormLocationX == 0
            && settingHelper.FormLocationY == 0)
        {
            Left = Screen.PrimaryScreen!.WorkingArea.Width - Width - 2;
            Top = Screen.PrimaryScreen!.WorkingArea.Height - Height - 2;
        }
        else
        {
            Left = settingHelper.FormLocationX;
            Top = settingHelper.FormLocationY;
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

    public frmEntryList(ZupDbContext dbContext, SettingHelper settingHelper)
    {
        InitializeComponent();

        m_DbContext = dbContext;
        this.settingHelper = settingHelper;
    }

    private void frmEntryList_Load(object sender, EventArgs e)
    {
        LoadListToControl();

        p_OnLoad = false;

        Opacity = settingHelper.EntryListOpacity;

        RefreshList();
        UpdateFormPosition();

        showQueuedTasksToolStripMenuItem.Checked = settingHelper.ShowQueuedTasks;
        showRankedTasksToolStripMenuItem.Checked = settingHelper.ShowRankedTasks;
        showClosedTasksToolStripMenuItem.Checked = settingHelper.ShowClosedTasks;
    }

    private void RefreshList()
    {
        UpdateTablePanel();
        SetListScrollAndActiveControl();
        ConfigureFlowLayoutPanel();
        ResizeForm();
    }

    private void ConfigureFlowLayoutPanel()
    {
        flpTaskList.AutoScroll = flpTaskList.Controls.Count > 1;
        flpQueuedTaskList.AutoScroll = flpQueuedTaskList.Controls.Count > 1;
        flpRankedTasks.AutoScroll = flpRankedTasks.Controls.Count > 1;
    }

    private void UpdateTablePanel()
    {
        int rowIx = 0;

        tblLayoutPanel.Controls.Clear();
        tblLayoutPanel.RowStyles.Clear();

        tblLayoutPanel.Controls.Add(flpTaskList, 0, rowIx);

        if (flpQueuedTaskList.Controls.Count > 0)
        {
            tblLayoutPanel.Controls.Add(flpQueuedTaskList, 0, ++rowIx);
        }

        if (flpRankedTasks.Controls.Count > 0)
        {
            tblLayoutPanel.Controls.Add(flpRankedTasks, 0, ++rowIx);
        }

        tblLayoutPanel.RowCount = rowIx + 1;

        var oRowHeight = CommonUtility.GetMin((int)EachEntry.OngoingRowHeight * flpTaskList.Controls.Count, (int)EachEntry.OngoingRowHeight * maxOngoingTaskRow);
        tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, oRowHeight));

        if (flpQueuedTaskList.Controls.Count > 0 && flpRankedTasks.Controls.Count > 0)
        {
            if (flpQueuedTaskList.Controls.Count > 0)
            {
                var qRowHeight = CommonUtility.GetMin((int)EachEntry.RowHeight * flpQueuedTaskList.Controls.Count, (int)EachEntry.RowHeight * maxQueuedTaskRow);

                tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, qRowHeight));
            }

            if (flpRankedTasks.Controls.Count > 0)
            {
                var rRowHeight = CommonUtility.GetMin((int)EachEntry.RowHeight * flpRankedTasks.Controls.Count, (int)EachEntry.RowHeight * maxRankedTaskRow);

                tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rRowHeight));
            }
        }
        else if (flpQueuedTaskList.Controls.Count > 0 || flpRankedTasks.Controls.Count > 0)
        {
            if (flpQueuedTaskList.Controls.Count > 0)
            {
                var qRowHeight = CommonUtility.GetMin((int)EachEntry.RowHeight * flpQueuedTaskList.Controls.Count, (int)EachEntry.RowHeight * maxQueuedTaskRow);

                tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, qRowHeight));
            }

            if (flpRankedTasks.Controls.Count > 0)
            {
                var rRowHeight = CommonUtility.GetMin((int)EachEntry.RowHeight * flpRankedTasks.Controls.Count, (int)EachEntry.RowHeight * maxRankedTaskRow);

                tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rRowHeight));
            }
        }
    }

    private void SetListScrollAndActiveControl()
    {
        var firstItem = flpTaskList.Controls.Count > 0
            ? (EachEntry)flpTaskList.Controls[0]
            : null;


        if (firstItem != null)
        {
            flpTaskList.ScrollControlIntoView(firstItem);

            firstItem.IsFirstItem = true;

            ActiveControl = firstItem;
        }
    }

    private void frmEntryList_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        Hide();
    }

    private EachEntry Translate(ITask task)
    {
        var eachEntry = new EachEntry(task.EntryID, task.Text, task.CreatedOn, task.StartedOn, task.EndedOn, task.Reminder)
        {
            Rank = task.Rank,
            TabStop = false,

        };

        eachEntry.GotFocus += (sender, e) => ActiveControl = null;

        eachEntry.OnReRunEvent += EachEntry_OnReRunEventHandler;
        eachEntry.OnStopEvent += EachEntry_OnStopEventHandler;
        eachEntry.OnStartEvent += EachEntry_OnStartEvent;
        eachEntry.OnUpdateEvent += EachEntry_OnUpdateEvent;
        eachEntry.OnStartQueueEvent += EachEntry_OnStartQueueEventHandler;
        eachEntry.TaskMouseDown += new MouseEventHandler(frmEntryList_MouseDown);
        eachEntry.TaskRightClick += EachEntry_TaskRightClick;

        return eachEntry;
    }

    private void LoadListToControl()
    {
        var ongoingTasks = new List<EachEntry>();
        var queuedTasks = new List<EachEntry>();
        var rankedTasks = new List<EachEntry>();

        foreach (var task in m_FormMain.Tasks)
        {
            var eachEntry = Translate(task);

            if (eachEntry.TaskStatus == TaskStatus.Ranked)
            {
                if (!settingHelper.ShowRankedTasks)
                {
                    continue;
                }

                rankedTasks.Add(eachEntry);
                continue;
            }

            if (eachEntry.TaskStatus == TaskStatus.Queued)
            {
                if (!settingHelper.ShowQueuedTasks)
                {
                    continue;
                }

                queuedTasks.Add(eachEntry);
                continue;
            }

            if (eachEntry.TaskStatus == TaskStatus.Closed && !settingHelper.ShowClosedTasks)
            {
                continue;
            }

            ongoingTasks.Add(eachEntry);
        }

        SortTasks(ongoingTasks);

        flpTaskList.SuspendLayout();
        flpTaskList.Controls.Clear();
        flpTaskList.Controls.AddRange(ongoingTasks.ToArray());
        flpTaskList.ResumeLayout();

        flpQueuedTaskList.SuspendLayout();
        flpQueuedTaskList.Controls.Clear();
        flpQueuedTaskList.Controls.AddRange(queuedTasks.OrderBy(a => a.CreatedOn).ToArray());
        flpQueuedTaskList.ResumeLayout();

        flpRankedTasks.SuspendLayout();
        flpRankedTasks.Controls.Clear();
        flpRankedTasks.Controls.AddRange(rankedTasks.OrderBy(a => a.Rank).ToArray());
        flpRankedTasks.ResumeLayout();
    }

    public void SortTasks(IList entryList)
    {
        var stack = new Queue<EachEntry>();
        var list = entryList.OfType<EachEntry>().Where(a => a.Visible).ToList();
        var all = entryList.OfType<EachEntry>().Where(a => a.Visible).ToArray();
        var minDate = DateTime.Now.AddDays(-settingHelper.NumDaysOfDataToLoad);

        // running
        foreach (var item in all.Where(a => a.TaskStatus == TaskStatus.Running).OrderBy(a => a.CreatedOn))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // started but not yet closed
        foreach (var item in all.Where(a => a.TaskStatus == TaskStatus.Unclosed))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // with ranking
        foreach (var item in all.Where(a => a.TaskStatus == TaskStatus.Ranked).OrderBy(a => a.Rank))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // not yet started
        foreach (var item in all.Where(a => a.TaskStatus == TaskStatus.Queued).OrderByDescending(a => a.CreatedOn))
        {
            if (!list.Contains(item))
            {
                continue;
            }

            stack.Enqueue(item);
            list.Remove(item);
        }

        // closed items
        foreach (var item in all.Where(a => a.TaskStatus == TaskStatus.Closed).OrderByDescending(a => a.StartedOn))
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
            // Sorting also called when resuming tasks so the passed parameter entryList is a ControlCollection
            // otherwise it is a list generated upon initialize or refresh
            if (entryList is Control.ControlCollection controls)
            {
                controls.SetChildIndex(entry, i);
            }
            else
            {
                entryList.Remove(entry);
                entryList.Insert(i, entry);
            }

            i++;
        }
    }

    public void ResizeForm()
    {
        var itemCount = CommonUtility.GetMin(flpTaskList.Controls.Count, maxOngoingTaskRow);

        itemCount += CommonUtility.GetMin(flpQueuedTaskList.Controls.Count, maxQueuedTaskRow);
        itemCount += CommonUtility.GetMin(flpRankedTasks.Controls.Count, maxRankedTaskRow);

        // itemCount = settingHelper.ItemsToShow;

        int totalHeight = (int)EachEntry.OngoingRowHeight * CommonUtility.GetMin(flpTaskList.Controls.Count, maxOngoingTaskRow);

        totalHeight += (int)EachEntry.RowHeight * CommonUtility.GetMin(flpQueuedTaskList.Controls.Count, maxQueuedTaskRow);
        totalHeight += (int)EachEntry.RowHeight * CommonUtility.GetMin(flpRankedTasks.Controls.Count, maxRankedTaskRow);

        // margin-bottom
        // totalHeight += 1 * itemCount;

        Height = totalHeight;
    }

    private void EachEntry_OnStartEvent(object? sender, OnStartEventArgs args)
    {
        var eachEntry = (EachEntry)sender!;

        CurrentRunningTaskID = eachEntry.EntryID;
        LastRunningTaskID = eachEntry.EntryID;
    }

    private void EachEntry_OnReRunEventHandler(object? sender, NewEntryEventArgs args)
    {
        EachEntry_NewEntryEventHandler(sender, args);
    }

    private void EachEntry_OnStartQueueEventHandler(object? sender, NewEntryEventArgs args)
    {
        var eachEntryStatus = ((EachEntry)sender!).TaskStatus;

        if (eachEntryStatus == TaskStatus.Queued)
        {
            var queuedEntry = flpQueuedTaskList.Controls.Cast<EachEntry>().Single(a => a.EntryID == ((EachEntry)sender!).EntryID);

            flpQueuedTaskList.Controls.Remove(queuedEntry);
        }

        EachEntry_NewEntryEventHandler(sender, args);
    }

    private void EachEntry_NewEntryEventHandler(object? sender, NewEntryEventArgs args)
    {
        throw new NotImplementedException();
    }

    private void EachEntry_OnUpdateEvent(Guid id)
    {
        var eachEntry = GetEachEntryByID(id);

        m_FormMain.ShowUpdateEntry(id, eachEntry?.TaskStatus == TaskStatus.Closed);
    }

    public EachEntry? GetEachEntryByID(Guid entryID)
    {
        var eachEntry = flpTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == entryID)
            ?? flpQueuedTaskList.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == entryID)
            ?? flpRankedTasks.Controls.Cast<EachEntry>().SingleOrDefault(a => a.EntryID == entryID);

        if (eachEntry == null)
        {
            throw new ArgumentNullException("Entry not found");
        }

        return eachEntry;
    }

    private void AddEntryToFlowLayoutControl(EachEntry newEntry, NewEntryEventArgs args)
    {
        flpTaskList.Controls.Add(newEntry);

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
        m_FormMain.StopTask(id, endOn);

        CurrentRunningTaskID = null;
    }

    public void UpdateCurrentRunningTask()
    {
        if (CurrentRunningTaskID == null)
        {
            return;
        }

        m_FormMain.ShowUpdateEntry(CurrentRunningTaskID.Value);
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

    private void showQueuedTasksToolStripMenuItem_Click(object sender, EventArgs e)
    {
        settingHelper.ShowQueuedTasks = !settingHelper.ShowQueuedTasks;
        settingHelper.Save();

        showQueuedTasksToolStripMenuItem.Checked = settingHelper.ShowQueuedTasks;

        LoadListToControl();

        RefreshList();
    }

    private void showRankedTasksToolStripMenuItem_Click(object sender, EventArgs e)
    {
        settingHelper.ShowRankedTasks = !settingHelper.ShowRankedTasks;
        settingHelper.Save();

        showRankedTasksToolStripMenuItem.Checked = settingHelper.ShowRankedTasks;

        LoadListToControl();

        RefreshList();
    }

    private void showClosedTasksToolStripMenuItem_Click(object sender, EventArgs e)
    {
        settingHelper.ShowClosedTasks = !settingHelper.ShowClosedTasks;
        settingHelper.Save();

        showClosedTasksToolStripMenuItem.Checked = settingHelper.ShowClosedTasks;

        LoadListToControl();

        RefreshList();
    }

    private void reorderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        LoadListToControl();

        RefreshList();
    }

    private async void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (m_SelectedEntryToDelete == null)
        {
            return;
        }

        if (MessageBox.Show("Continue to delete this task?", "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            m_SelectedEntryToDelete = null;

            return;
        }

        var task = await m_DbContext.TaskEntries.SingleOrDefaultAsync(a => a.ID == m_SelectedEntryToDelete.EntryID);

        if (task != null)
        {
            m_DbContext.TaskEntries.Remove(task);
            await m_DbContext.SaveChangesAsync();
        }

        var cms = ((ToolStripMenuItem)sender).Owner as ContextMenuStrip;

        if (cms != null && cms.SourceControl is FlowLayoutPanel flp)
        {
            flp.Controls.Remove(m_SelectedEntryToDelete);
        }

        m_SelectedEntryToDelete = null;
    }

    private void EachEntry_TaskRightClick(object? sender, MouseEventArgs e)
    {
        m_SelectedEntryToDelete = (EachEntry)sender!;
    }
}