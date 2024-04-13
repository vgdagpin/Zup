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


public partial class frmNewEntry : Form
{
    public frmNewEntry()
    {
        InitializeComponent();
    }

    private void frmNewEntry_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        txtEntry.Text = "";

        Hide();
    }

    private void frmNewEntry_Load(object sender, EventArgs e)
    {
        txtEntry.Focus();
    }

    private void txtEntry_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            e.SuppressKeyPress = true;
            Close();

            return;
        }

        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            Close();
        }
    }
}
