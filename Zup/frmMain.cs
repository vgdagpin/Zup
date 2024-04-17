using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Zup;

public partial class frmMain : Form
{
    private frmEntryList m_FormEntryList;
    private frmSetting m_FormSetting;
    private frmView m_FormView;

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

    public frmMain(ZupDbContext dbContext, frmEntryList frmEntryList, frmSetting frmSetting, frmView frmView)
    {
        InitializeComponent();

        /*
          MOD_ALT: 0x0001
          MOD_CONTROL: 0x0002
          MOD_SHIFT: 0x0004
          MOD_WIN: 0x0008
         */

        RegisterHotKey(this.Handle, 1, 5, (int)Keys.J);
        RegisterHotKey(this.Handle, 2, 5, (int)Keys.K);
        RegisterHotKey(this.Handle, 3, 5, (int)Keys.L);

        dbContext.Database.Migrate();

        m_FormEntryList = frmEntryList;
        m_FormSetting = frmSetting;
        m_FormView = frmView;

        m_FormView.OnSelectedItemEvent += FormView_OnSelectedItemEvent;
    }

    private void FormView_OnSelectedItemEvent(int entryID)
    {
        m_FormEntryList.ShowUpdateEntry(entryID);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
        {
            m_FormEntryList.ShowNewEntry();
        }
        else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 2)
        {
            m_FormEntryList.UpdateCurrentRunningTask();
        }
        else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 3)
        {
            m_FormEntryList.ToggleLastRunningTask();
        }

        base.WndProc(ref m);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        UnregisterHotKey(this.Handle, 1);

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
        m_FormEntryList.ShowNewEntry();
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
