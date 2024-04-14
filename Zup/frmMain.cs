using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Zup;

public partial class frmMain : Form
{
    private frmNewEntry m_FormNewEntry = new frmNewEntry();
    private frmEntryList m_FormEntryList = new frmEntryList();

    private List<frmEachEntry> m_EachEntry = new List<frmEachEntry>();

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

    public frmMain()
    {
        InitializeComponent();

        // RegisterHotKey for Win+Shift+Z
        RegisterHotKey(this.Handle, 1, 12, (int)Keys.Z);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
        {
            m_FormNewEntry.Show();
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
}
