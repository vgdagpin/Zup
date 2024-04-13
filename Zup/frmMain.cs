using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Zup;

public partial class frmMain : Form
{
    protected virtual frmNewEntry m_FormNewEntry { get; set; } = new frmNewEntry();

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

        // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
        // Compute the addition of each combination of the keys you want to be pressed
        // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...
        //RegisterHotKey(this.Handle, 1, 6, (int)Keys.F12);


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

    private void frmMain_Load(object sender, EventArgs e)
    {

    }

    
}
