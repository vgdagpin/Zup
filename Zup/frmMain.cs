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

        // RegisterHotKey for Win+Shift+Z
        // RegisterHoykey for Shift+Alt+J


        RegisterHotKey(this.Handle, 1, 12, (int)Keys.J);
        RegisterHotKey(this.Handle, 2, 12, (int)Keys.K);
        RegisterHotKey(this.Handle, 3, 12, (int)Keys.L);

        dbContext.Database.Migrate();

        m_FormEntryList = frmEntryList;
        m_FormSetting = frmSetting;
        m_FormView = frmView;
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
        m_FormSetting.Show();
    }

    private void viewToolStripMenuItem_Click(object sender, EventArgs e)
    {
        m_FormView.Show();
    }

    private void notifIconZup_DoubleClick(object sender, EventArgs e)
    {
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
