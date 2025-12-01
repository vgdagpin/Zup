using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using Zup.CustomControls;
using Zup.Entities;
using Zup.EventArguments;

namespace Zup;

public partial class frmMain : Form
{
    private IServiceProvider serviceProvider;
    private bool listIsReady = false;

    public event EventHandler<NewEntryEventArgs>? OnNewTask;
    public event EventHandler<ITask>? OnTaskDeleted;
    public event EventHandler<ITask>? OnTaskUpdated;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HashSet<ITask> Tasks { get; set; } = new HashSet<ITask>();

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
        FormNewEntry_NewEntryEventHandler(sender, e);
    }

    private void FrmUpdateEntry_OnSavedEvent(object? sender, SaveEventArgs e)
    {
        var task = Tasks.SingleOrDefault(a => a.EntryID == e.Task.ID);

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
                frmSetting = serviceProvider.CreateScope()
                    .ServiceProvider.GetRequiredService<frmSetting>();

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

                frmSetting.OnDbTrimEvent += (daysToKeep) =>
                {
                    m_DbContext.BackupDb();

                    var keepDate = DateTime.Now.AddDays(-daysToKeep);

                    var toDel = m_DbContext.TaskEntries
                        .Where(a => a.StartedOn < keepDate)
                        .ToList();

                    var toDelNotes = m_DbContext.TaskEntryNotes
                        .Where(a => toDel.Select(b => b.ID).Contains(a.TaskID))
                        .ToList();

                    m_DbContext.TaskEntryNotes.RemoveRange(toDelNotes);
                    m_DbContext.TaskEntries.RemoveRange(toDel);

                    m_DbContext.SaveChanges();

                    MessageBox.Show($"Trimmed {toDel.Count} record/s.", "Zup");
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

    public frmMain(IServiceProvider serviceProvider, frmSetting frmSetting)
    {
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

    public void ShowNewEntry()
    {
        if (!listIsReady)
        {
            return;
        }

        var suggestions = new List<string>();

        var currentList = Tasks
            .Where(a => a.TaskStatus == TaskStatus.Closed)
            .Select(a => a.Text)
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
                EntryID = task.ID,
                Text = task.Task,
                CreatedOn = task.CreatedOn,
                StartedOn = task.StartedOn,
                EndedOn = task.EndedOn,
                Reminder = task.Reminder,
                Rank = task.Rank
            };

            if (eachEntry.TaskStatus == TaskStatus.Ranked)
            {
                if (!SettingHelper.ShowRankedTasks)
                {
                    continue;
                }

                Tasks.Add(eachEntry);
                result.RankedTasksCount++;
                continue;
            }

            if (eachEntry.TaskStatus == TaskStatus.Queued)
            {
                if (!SettingHelper.ShowQueuedTasks)
                {
                    continue;
                }

                Tasks.Add(eachEntry);
                result.QueuedTasksCount++;
                continue;
            }

            if (eachEntry.TaskStatus == TaskStatus.Closed && !SettingHelper.ShowClosedTasks)
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
        var task = Tasks.SingleOrDefault(a => a.EntryID == taskID);

        if (task != null)
        {
            m_FormUpdateEntry.ShowUpdateEntry(task, canReRun);
        }
    }

    public void DeleteEntry(Guid taskID)
    {
        var entry = m_DbContext.TaskEntries.Find(taskID);

        if (entry != null)
        {
            m_DbContext.TaskEntries.Remove(entry);
            m_DbContext.SaveChanges();
        }

        var task = Tasks.SingleOrDefault(a => a.EntryID == taskID);

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
        m_DbContext.BackupDb();

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

        var parentEntry = args.ParentEntryID != null
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
        }

        if (args.GetTags && parentEntry == null)
        {
            var minDate = DateTime.Now.AddDays(-SettingHelper.NumDaysOfDataToLoad);

            var tagIDs =
                (
                    from e in m_DbContext.TaskEntries.Where(a => (a.StartedOn >= minDate && a.EndedOn != null) || a.StartedOn == null || (a.StartedOn != null && a.EndedOn == null))
                    join t in m_DbContext.TaskEntryTags on e.ID equals t.TaskID
                    orderby t.CreatedOn descending
                    where e.Task == args.Entry
                    select t.TagID
                ).Distinct();

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

        var eachEntry = new ZupTask
        {
            EntryID = newE.ID,
            Text = newE.Task,
            CreatedOn = newE.CreatedOn,
            StartedOn = newE.StartedOn,
            EndedOn = newE.EndedOn,
            Reminder = newE.Reminder,
            Rank = newE.Rank
        };

        Tasks.Add(eachEntry);

        args.Task = eachEntry;

        if (args.HideParent && args.ParentEntryID != null)
        {
            DeleteEntry(args.ParentEntryID.Value);
        }

        if (SettingHelper.UsePillTimer)
        {
            if (eachEntry.TaskStatus != TaskStatus.Queued)
            {
                if (args.StopOtherTask && eachEntry.IsRunning)
                {
                    var pillTimer = FloatingButtons.Single(a => ((EachEntry)a.Tag!).EntryID == eachEntry.EntryID);

                    pillTimer.Stop();
                }
            }

            ShowFloatingButton(eachEntry);
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

            ShowUpdateEntry(eachEntry.EntryID);
        }
    }

    private void ShowFloatingButton(ITask eachEntry)
    {
        var newFloatingButton = new frmFloatingButton
        {
            StartPosition = FormStartPosition.Manual,
            Tag = eachEntry,
            Left = Left,
            Top = Top
        };

        newFloatingButton.Move += NewFloatingButton_Move;

        FloatingButtons.Add(newFloatingButton);

        newFloatingButton.OnStopEvent += (sender, ts) =>
        {
            if (ts.IsClosed && FloatingButtons.Count == 1)
            {
                Show();
            }

            var entry = ((frmFloatingButton)sender!).Tag as EachEntry;

            entry?.Stop();
        };

        newFloatingButton.FormClosed += (sender, e) =>
        {
            FloatingButtons.Remove((frmFloatingButton)sender!);
        };

        newFloatingButton.OnTaskTextDoubleClick += (sender, e) =>
        {
            var entry = ((frmFloatingButton)sender!).Tag as EachEntry;

            if (entry != null)
            {
                ShowUpdateEntry(entry.EntryID);
            }
        };

        newFloatingButton.OnResetEvent += (sender, e) =>
        {
            var entry = ((frmFloatingButton)sender!).Tag as EachEntry;

            if (entry != null)
            {
                var existingE = m_DbContext.TaskEntries.Find(entry.EntryID);

                if (existingE != null)
                {
                    existingE.StartedOn = DateTime.Now;

                    m_DbContext.SaveChanges();
                }

                entry.Reset();
            }
        };

        newFloatingButton.Text = eachEntry.Text;
        newFloatingButton.StartedOn = eachEntry.StartedOn;

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
