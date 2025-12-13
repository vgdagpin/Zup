using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using Zup.EventArguments;

namespace Zup;

public partial class frmMain : Form
{
    private IServiceProvider serviceProvider;
    private bool listIsReady = false;

    private readonly TaskCollection Tasks;

    public event EventHandler<NewEntryEventArgs>? OnNewTask;
    public event EventHandler<ITask>? OnTaskDeleted;
    public event EventHandler<ITask>? OnTaskUpdated;

    private List<frmFloatingButton> FloatingButtons = new List<frmFloatingButton>();


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    extern static bool DestroyIcon(IntPtr handle);

    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    static class Constants
    {
        public const int ShowUpdateEntry = 1;
        public const int UpdateCurrentRunningTask = 2;
        public const int ToggleLastRunningTask = 3;
        public const int ShowViewAll = 4;
    }

    #region Properties

    ZupDbContext? dbContext = null;
    private ZupDbContext m_DbContext
    {
        get
        {
            if (dbContext == null)
            {
                dbContext = serviceProvider.GetRequiredService<ZupDbContext>();
            }

            return dbContext;
        }
    }

    SettingHelper? settingHelper = null;
    private SettingHelper SettingHelper
    {
        get
        {
            if (settingHelper == null)
            {
                settingHelper = serviceProvider.GetRequiredService<SettingHelper>();
            }

            return settingHelper;
        }
    }


    frmEntryList? frmEntryList = null;
    private frmEntryList m_FormEntryList
    {
        get
        {
            if (frmEntryList == null || frmEntryList.IsDisposed)
            {
                frmEntryList = serviceProvider.GetRequiredService<frmEntryList>();
                frmEntryList.SetFormMain(this);

                frmEntryList.Move += FrmEntryList_Move;
            }

            return frmEntryList;
        }
    }

    private void FrmEntryList_Move(object? sender, EventArgs e)
    {
        tmrSaveSetting.Enabled = false;
        tmrSaveSetting.Enabled = true;
        tmrSaveSetting.Tag = sender;
    }

    frmViewList? frmView = null;
    private frmViewList m_FormView
    {
        get
        {
            if (frmView == null || frmView.IsDisposed)
            {
                frmView = serviceProvider.GetRequiredService<frmViewList>();
                frmView.SetFormMain(this);
            }

            return frmView;
        }
    }

    frmNewEntry? frmNewEntry = null;
    private frmNewEntry m_FormNewEntry
    {
        get
        {
            if (frmNewEntry == null || frmNewEntry.IsDisposed)
            {
                frmNewEntry = serviceProvider.GetRequiredService<frmNewEntry>();

                frmNewEntry.OnNewEntryEvent += FormNewEntry_NewEntryEventHandler;
            }

            return frmNewEntry;
        }
    }

    frmUpdateEntry? frmUpdateEntry = null;
    private frmUpdateEntry m_FormUpdateEntry
    {
        get
        {
            if (frmUpdateEntry == null || frmUpdateEntry.IsDisposed)
            {
                frmUpdateEntry = serviceProvider.GetRequiredService<frmUpdateEntry>();
                frmUpdateEntry.SetFormMain(this);

                frmUpdateEntry.OnTokenDoubleClicked += FrmEntryList_OnTokenDoubleClicked;
                frmUpdateEntry.OnSavedEvent += FrmUpdateEntry_OnSavedEvent;
                frmUpdateEntry.OnReRunEvent += FrmUpdateEntry_OnReRunEvent;
            }
            return frmUpdateEntry;
        }
    }

    private void FrmUpdateEntry_OnReRunEvent(object? sender, NewEntryEventArgs e)
    {
        RunTask(e);
    }

    private void FrmUpdateEntry_OnSavedEvent(object? sender, SaveEventArgs e)
    {
        var task = Tasks.Find(e.Task.ID);

        if (task != null)
        {
            OnTaskUpdated?.Invoke(this, task);
        }
    }

    frmTagEditor? frmTagEditor = null;
    private frmTagEditor m_FormTagEditor
    {
        get
        {
            if (frmTagEditor == null || frmTagEditor.IsDisposed)
            {
                frmTagEditor = serviceProvider.GetRequiredService<frmTagEditor>();
            }

            return frmTagEditor;
        }
    }

    private frmSetting? frmSetting = null;
    private frmSetting m_FormSetting
    {
        get
        {
            if (frmSetting == null || frmSetting.IsDisposed)
            {
                frmSetting = serviceProvider.GetRequiredService<frmSetting>();

                frmSetting.SetFormMain(this);

                frmSetting.OnSettingUpdatedEvent += (name, value) =>
                {
                    if (name == "ItemsToShow")
                    {
                        m_FormEntryList.ResizeForm();
                    }
                    else if (name == "EntryListOpacity" && value is double opacity)
                    {
                        m_FormEntryList.Opacity = opacity;
                    }
                    else if (name == "UpdateDbPath")
                    {
                        m_FormEntryList.Close();

                        dbContext = null;
                        serviceProvider = null;
                        frmEntryList = null;

                        m_FormEntryList.Show();
                    }
                };

                frmSetting.OnDbBackupEvent += () =>
                {
                    m_DbContext.BackupDb();

                    MessageBox.Show("Backup done!", "Zup");
                };
            }

            return frmSetting;
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

    public frmMain(IServiceProvider serviceProvider, frmSetting frmSetting, TaskCollection tasks)
    {
        Tasks = tasks;

        Tasks.OnTaskStarted += Tasks_OnTaskAdded;
        Tasks.OnTaskStopped += Tasks_OnTaskRemoved;


        InitializeComponent();

        this.serviceProvider = serviceProvider.CreateScope().ServiceProvider;

        m_DbContext.Database.Migrate();
        var listResult = LoadList();

        SetIcon(listResult.QueuedTasksCount);

        listIsReady = true;

        /*
          MOD_ALT: 0x0001
          MOD_CONTROL: 0x0002
          MOD_SHIFT: 0x0004
          MOD_WIN: 0x0008
         */

        RegisterHotKey(this.Handle, Constants.ShowUpdateEntry, 5, (int)Keys.J);
        RegisterHotKey(this.Handle, Constants.UpdateCurrentRunningTask, 5, (int)Keys.K);
        RegisterHotKey(this.Handle, Constants.ToggleLastRunningTask, 5, (int)Keys.L);
        RegisterHotKey(this.Handle, Constants.ShowViewAll, 5, (int)Keys.P);



        SetIcon();

    }

    private void Tasks_OnTaskRemoved(object? sender, ITask e)
    {

    }

    private void Tasks_OnTaskAdded(object? sender, NewEntryEventArgs args)
    {
        if (args.HideParent && args.ParentEntryID != null)
        {
            DeleteEntry(args.ParentEntryID.Value);
        }

        if (SettingHelper.UsePillTimer)
        {
            ShowFloatingButton(args.Task);

            if (args.Task.GetTaskStatus() != TaskStatus.Queued
                && args.StopOtherTask
                && args.Task.IsRunning)
            {
                foreach (var runningPills in FloatingButtons.Where(a => ((ITask)a.Tag!).ID != args.Task.ID))
                {
                    runningPills.Stop();
                }
            }

            // removed disposed floating buttons
            FloatingButtons.Where(a => a.IsDisposed).ToList()
                .ForEach(d =>
                {
                    FloatingButtons.Remove(d);
                });
        }
        else
        {
            OnNewTask?.Invoke(this, args);
        }


        if (SettingHelper.AutoOpenUpdateWindow)
        {
            if (SettingHelper.UsePillTimer)
            {
                return;
            }

            ShowUpdateEntry(args.Task.ID);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0312)
        {
            if (m.WParam.ToInt32() == Constants.ShowUpdateEntry
                || m.WParam.ToInt32() == Constants.UpdateCurrentRunningTask
                || m.WParam.ToInt32() == Constants.ToggleLastRunningTask
                || m.WParam.ToInt32() == Constants.ShowViewAll)
            {
                if (IsNewWeek())
                {
                    m_DbContext.BackupDb();
                }

                switch (m.WParam.ToInt32())
                {
                    case Constants.ShowUpdateEntry:
                        ShowNewEntry();
                        break;
                    case Constants.UpdateCurrentRunningTask:
                        m_FormEntryList.UpdateCurrentRunningTask();
                        break;
                    case Constants.ToggleLastRunningTask:
                        m_FormEntryList.ToggleLastRunningTask();
                        break;
                    case Constants.ShowViewAll:
                        if (m_FormView.Visible)
                        {
                            m_FormView.Activate();

                            return;
                        }

                        m_FormView.Show();
                        break;
                }
            }
        }

        base.WndProc(ref m);
    }

    public void TrimDb(int daysToKeep)
    {
        m_DbContext.BackupDb();

        var keepDate = DateTime.Now.AddDays(-daysToKeep);

        (
             from n in m_DbContext.TaskEntryNotes
             join e in m_DbContext.TaskEntries
                 on n.TaskID equals e.ID
             where e.StartedOn < keepDate
             select n
         ).ExecuteDelete();

        var totalDeleted =
        (
            from e in m_DbContext.TaskEntries
            where e.StartedOn < keepDate
            select e
        ).ExecuteDelete();

        m_DbContext.Database.ExecuteSql($"VACUUM;");

        MessageBox.Show($"Trimmed {totalDeleted} record/s.", "Zup");
    }

    public void ShowNewEntry()
    {
        if (!listIsReady)
        {
            return;
        }

        var suggestions = new List<string>();

        var currentList = Tasks
            .ClosedTasks()
            .Select(a => a.Task)
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

    private LoadedListControlDetail LoadList()
    {
        var result = new LoadedListControlDetail();

        var minDate = DateTime.Now.AddDays(-SettingHelper.NumDaysOfDataToLoad);

        foreach (var task in m_DbContext.TaskEntries.Where(a => a.CreatedOn >= minDate || a.StartedOn == null).ToList())
        {
            var eachEntry = new ZupTask
            {
                ID = task.ID,
                Task = task.Task,
                CreatedOn = task.CreatedOn,
                StartedOn = task.StartedOn,
                EndedOn = task.EndedOn,
                Reminder = task.Reminder,
                Rank = task.Rank
            };

            if (eachEntry.GetTaskStatus() == TaskStatus.Ranked)
            {
                if (!SettingHelper.ShowRankedTasks)
                {
                    continue;
                }

                Tasks.Add(eachEntry);
                result.RankedTasksCount++;
                continue;
            }

            if (eachEntry.GetTaskStatus() == TaskStatus.Queued)
            {
                if (!SettingHelper.ShowQueuedTasks)
                {
                    continue;
                }

                Tasks.Add(eachEntry);
                result.QueuedTasksCount++;
                continue;
            }

            if (eachEntry.GetTaskStatus() == TaskStatus.Closed && !SettingHelper.ShowClosedTasks)
            {
                continue;
            }

            Tasks.Add(eachEntry);

            result.OngoingTasksCount++;
        }

        return result;
    }

    public void Notify(string message, string title = "Zup")
    {
        notifIconZup.ShowBalloonTip(0, title, message, ToolTipIcon.Info);
    }

    public void ShowUpdateEntry(Guid taskID, bool canReRun = false)
    {
        var task = Tasks.Find(taskID);

        if (task != null)
        {
            m_FormUpdateEntry.ShowUpdateEntry(task, canReRun);
        }
    }

    public void RunTask(NewEntryEventArgs args)
    {
        throw new NotImplementedException();
    }

    public void DeleteEntry(Guid taskID)
    {
        var entry = m_DbContext.TaskEntries.Find(taskID);

        if (entry != null)
        {
            m_DbContext.TaskEntries.Remove(entry);
            m_DbContext.SaveChanges();
        }

        var task = Tasks.Find(taskID);

        if (task != null)
        {
            OnTaskDeleted?.Invoke(this, task);
        }
    }

    protected bool IsNewWeek()
    {
        if (!m_DbContext.TaskEntries.Any())
        {
            return false;
        }

        var latestCreatedOn = m_DbContext.TaskEntries.OrderByDescending(x => x.CreatedOn).FirstOrDefault()?.CreatedOn;
        var latestStartedOn = m_DbContext.TaskEntries.Where(a => a.StartedOn != null).OrderByDescending(x => x.StartedOn).FirstOrDefault()?.StartedOn;
        var latestEndedOn = m_DbContext.TaskEntries.Where(a => a.EndedOn != null).OrderByDescending(x => x.EndedOn).FirstOrDefault()?.EndedOn;

        var lastRow = new[] { latestCreatedOn.GetValueOrDefault(), latestStartedOn.GetValueOrDefault(), latestEndedOn.GetValueOrDefault() }.Max();

        var lastRowWeekNum = Utility.GetWeekNumber(lastRow);
        var weekNumNow = Utility.GetWeekNumber(DateTime.Now);

        return lastRowWeekNum < weekNumNow;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        UnregisterHotKey(this.Handle, Constants.ShowUpdateEntry);
        UnregisterHotKey(this.Handle, Constants.UpdateCurrentRunningTask);
        UnregisterHotKey(this.Handle, Constants.ToggleLastRunningTask);
        UnregisterHotKey(this.Handle, Constants.ShowViewAll);

        base.OnFormClosing(e);
    }

    private void FrmEntryList_OnTokenDoubleClicked(object? sender, CustomControls.TokenEventArgs e)
    {
        if (m_FormTagEditor.Visible)
        {
            m_FormTagEditor.Activate();

            m_FormTagEditor.SelectTag(e.Text);

            return;
        }

        m_FormTagEditor.Show();

        m_FormTagEditor.SelectTag(e.Text);
    }

    private void FormNewEntry_NewEntryEventHandler(object? sender, NewEntryEventArgs args)
    {
        RunTask(args);
    }

    private void ShowFloatingButton(ITask eachEntry)
    {
        var newFloatingButton = new frmFloatingButton(Tasks)
        {
            StartPosition = FormStartPosition.Manual,
            Tag = eachEntry,
            Left = SettingHelper.FormLocationX,
            Top = SettingHelper.FormLocationY
        };

        newFloatingButton.Move += NewFloatingButton_Move;

        FloatingButtons.Add(newFloatingButton);

        newFloatingButton.OnShowUpdateEntry += (sender, e) =>
        {
            var entry = ((frmFloatingButton)sender!).Tag as ITask;

            if (entry != null)
            {
                ShowUpdateEntry(entry.ID);
            }
        };

        newFloatingButton.OnResetEvent += (sender, e) =>
        {
            var entry = ((frmFloatingButton)sender!).Tag as ITask;

            if (entry != null)
            {
                var existingE = m_DbContext.TaskEntries.Find(entry.ID);

                if (existingE != null)
                {
                    existingE.StartedOn = DateTime.Now;

                    m_DbContext.SaveChanges();
                }
            }
        };

        newFloatingButton.Task = eachEntry.Task;
        newFloatingButton.StartedOn = eachEntry.StartedOn;

        eachEntry.IsRunning = eachEntry.StartedOn != null;

        newFloatingButton.Show();
    }

    private void NewFloatingButton_Move(object? sender, EventArgs e)
    {
        tmrSaveSetting.Enabled = false;
        tmrSaveSetting.Enabled = true;
        tmrSaveSetting.Tag = sender;
    }

    public void SetIcon(int? queueCount = null)
    {
        var inDarkMode = IsUsingDarkMode();

        var bitmapText = queueCount == null || queueCount == 0
            ? new Bitmap(inDarkMode ? Properties.Resources.zup_white_3 : Properties.Resources.zup_black_3)
            : new Bitmap(inDarkMode ? Properties.Resources.zup_white_2 : Properties.Resources.zup_black_2);

        var g = Graphics.FromImage(bitmapText);

        IntPtr hIcon;

        if (queueCount != null && queueCount > 0)
        {
            var str = queueCount > 9 ? "+" : queueCount.ToString();
            var x = queueCount > 9 ? 8 : 9;

            var fontToUse = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Pixel);
            var brushToUse = new SolidBrush(inDarkMode ? Color.Orange : Color.Red);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, x, 2);
        }

        hIcon = bitmapText.GetHicon();

        notifIconZup.Icon = Icon.FromHandle(hIcon);

        DestroyIcon(hIcon);
    }

    private bool IsUsingDarkMode()
    {
        try
        {
            var res = (int?)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);

            if (res == 0)
            {
                return true;
            }
        }
        catch
        {
            //Exception Handling     
        }

        return false;
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
        Visible = false;

        aboutToolStripMenuItem.Text = $"About - {Assembly.GetExecutingAssembly().GetName().Version}";
    }

    private void tmrDelayShowList_Tick(object sender, EventArgs e)
    {
        m_FormView.Show();
        tmrDelayShowList.Stop();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormSetting.Show();
    }

    private void viewToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (m_FormView.Visible)
        {
            m_FormView.Activate();

            return;
        }

        m_FormView.Show();
        m_FormView.Activate();
    }

    private void notifIconZup_DoubleClick(object sender, EventArgs e)
    {
        if (m_FormView.Visible)
        {
            m_FormView.Activate();

            return;
        }

        m_FormView.Show();
        m_FormView.Activate();
    }

    private void openNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowNewEntry();
    }

    private void updateCurrentRunningTaskToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormEntryList.UpdateCurrentRunningTask();
    }

    private void toggleLastRunningTaskToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormEntryList.ToggleLastRunningTask();
    }

    private void tagEditorToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (m_FormTagEditor.Visible)
        {
            m_FormTagEditor.Activate();

            return;
        }

        m_FormTagEditor.Show();
    }

    private void moveToCenterToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormEntryList.MoveToCenterAndBringToFront();
    }

    private void tmrSaveSetting_Tick(object sender, EventArgs e)
    {
        tmrSaveSetting.Enabled = false;

        if (tmrSaveSetting.Tag is frmFloatingButton floatingButton)
        {
            SettingHelper.FormLocationX = floatingButton.Left;
            SettingHelper.FormLocationY = floatingButton.Top;

            SettingHelper.Save();
        }
        else if (tmrSaveSetting.Tag is frmEntryList entryList)
        {
            SettingHelper.FormLocationX = entryList.Left;
            SettingHelper.FormLocationY = entryList.Top;

            SettingHelper.Save();
        }

        tmrSaveSetting.Tag = null;
    }
}
