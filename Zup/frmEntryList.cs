using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zup;
public partial class frmEntryList : Form
{
    protected override CreateParams CreateParams
    {
        get
        {
            var Params = base.CreateParams;
            Params.ExStyle |= 0x00000080;
            return Params;
        }
    }

    public frmEntryList()
    {
        InitializeComponent();

        Left = Screen.PrimaryScreen!.WorkingArea.Width - Width - 5;
        Top = Screen.PrimaryScreen!.WorkingArea.Height - Height - 5;
    }

    private void frmEntryList_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        Hide();
    }

    private void frmEntryList_Load(object sender, EventArgs e)
    {
        flowLayoutPanel1.Controls.Add(new EachEntry());
        flowLayoutPanel1.Controls.Add(new EachEntry());
    }
}
