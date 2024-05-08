using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Zup;

public partial class frmMain : Form
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    extern static bool DestroyIcon(IntPtr handle);


    static class Constants
    {
        public const int ShowUpdateEntry = 1;
        public const int UpdateCurrentRunningTask = 2;
        public const int ToggleLastRunningTask = 3;
        public const int ShowViewAll = 4;
    }

    IServiceProvider? serviceProvider;
    private IServiceProvider m_ServiceProvider
    {
        get
        {
            if (serviceProvider == null)
            {
                serviceProvider = p_ServiceProvider.CreateScope().ServiceProvider;
            }

            return serviceProvider;
        }
    }

    ZupDbContext? dbContext = null;
    private ZupDbContext m_DbContext
    {
        get
        {
            if (dbContext == null)
            {
                dbContext = m_ServiceProvider.GetRequiredService<ZupDbContext>();
            }

            return dbContext;
        }
    }


    frmEntryList? frmEntryList = null;
    private frmEntryList m_FormEntryList
    {
        get
        {
            if (frmEntryList == null || frmEntryList.IsDisposed)
            {
                frmEntryList = m_ServiceProvider.GetRequiredService<frmEntryList>();

                frmEntryList.OnListReadyEvent += FrmEntryList_OnListReadyEvent;
                frmEntryList.OnQueueTaskUpdatedEvent += FrmEntryList_OnQueueTaskUpdatedEvent;
            }

            return frmEntryList;
        }
    }

    private void FrmEntryList_OnQueueTaskUpdatedEvent(int queueCount)
    {
        SetIcon(queueCount);
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

    private void FrmEntryList_OnListReadyEvent(int listCount)
    {
        if (listCount == 0)
        {
            notifIconZup.ShowBalloonTip(1000, "", "It's lonely here, press Shift+Alt+J to start adding task!", ToolTipIcon.Info);
        }
    }

    frmView? frmView = null;
    private frmView m_FormView
    {
        get
        {
            if (frmView == null || frmView.IsDisposed)
            {
                frmView = m_ServiceProvider.GetRequiredService<frmView>();

                frmView.OnSelectedItemEvent += FormView_OnSelectedItemEvent;
            }

            return frmView;
        }
    }


    private readonly IServiceProvider p_ServiceProvider;
    private frmSetting m_FormSetting;

    #region Initialize
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

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

        p_ServiceProvider = serviceProvider;
        m_FormSetting = frmSetting;

        m_FormSetting.OnSettingUpdatedEvent += FormSetting_OnSettingUpdatedEvent;
        m_FormSetting.OnDbTrimEvent += FormSetting_OnDbTrimEvent;
        m_FormSetting.OnDbBackupEvent += FormSetting_OnDbBackupEvent;

        SetIcon();
    }

    private void FormSetting_OnDbBackupEvent()
    {
        //foreach (var log in m_DbContext.TimeLogs)
        //{
        //    var task = new tbl_TaskEntry
        //    {
        //        ID = Guid.NewGuid(),
        //        Task = log.Task,
        //        StartedOn = log.StartedOn,
        //        EndedOn = log.EndedOn
        //    };

        //    m_DbContext.TaskEntries.Add(task);

        //    foreach (var note in m_DbContext.Notes.Where(a => a.LogID == log.ID))
        //    {
        //        var taskNote = new tbl_TaskEntryNote
        //        {
        //            ID = Guid.NewGuid(),
        //            TaskID = task.ID,
        //            Notes = note.Notes,
        //            CreatedOn = note.CreatedOn,
        //            UpdatedOn = note.UpdatedOn,
        //            RTF = note.RTF
        //        };

        //        m_DbContext.TaskEntryNotes.Add(taskNote);
        //    }
        //}

        //m_DbContext.SaveChanges();

        m_DbContext.BackupDb();

        MessageBox.Show("Backup done!", "Zup");
    }

    private void FormSetting_OnDbTrimEvent(int daysToKeep)
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
    }

    private void FormSetting_OnSettingUpdatedEvent(string name, object value)
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
    }

    private void FormView_OnSelectedItemEvent(Guid entryID)
    {
        m_FormEntryList.ShowUpdateEntry(entryID);
    }

    protected bool IsNewWeek()
    {
        var lastRow = m_DbContext.TaskEntries.Where(a => a.StartedOn != null).OrderByDescending(x => x.StartedOn).FirstOrDefault();

        if (lastRow == null)
        {
            return false;
        }

        var lastRowWeekNum = Utility.GetWeekNumber(lastRow.StartedOn!.Value);
        var weekNumNow = Utility.GetWeekNumber(DateTime.Now);

        return lastRowWeekNum < weekNumNow;
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
                        if (m_FormEntryList.ListIsReady)
                        {
                            m_FormEntryList.ShowNewEntry();
                        }
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

    protected override void OnClosing(CancelEventArgs e)
    {
        UnregisterHotKey(this.Handle, Constants.ShowUpdateEntry);
        UnregisterHotKey(this.Handle, Constants.UpdateCurrentRunningTask);
        UnregisterHotKey(this.Handle, Constants.ToggleLastRunningTask);
        UnregisterHotKey(this.Handle, Constants.ShowViewAll);

        base.OnClosing(e);
    }
    #endregion

    private void frmMain_Load(object sender, EventArgs e)
    {
        Visible = false;
    }

    private void tmrDelayShowList_Tick(object sender, EventArgs e)
    {
        m_FormEntryList.Show();
        tmrDelayShowList.Stop();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (m_FormSetting.Visible)
        {
            m_FormSetting.Activate();

            return;
        }

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
    }

    private void notifIconZup_DoubleClick(object sender, EventArgs e)
    {
        if (m_FormView.Visible)
        {
            m_FormView.Activate();

            return;
        }

        m_FormView.Show();
    }

    private void openNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (m_FormEntryList.ListIsReady)
        {
            m_FormEntryList.ShowNewEntry();
        }
    }

    private void updateCurrentRunningTaskToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormEntryList.UpdateCurrentRunningTask();
    }

    private void toggleLastRunningTaskToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormEntryList.ToggleLastRunningTask();
    }
}
