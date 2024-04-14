using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Zup.CustomControls;

namespace Zup;
public partial class frmEntryList : Form
{
    private frmNewEntry m_FormNewEntry = new frmNewEntry();
    private bool p_OnLoad = true;

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

    public void ShowNewEntry()
    {
        m_FormNewEntry.ShowNewEntryDialog(flowLayoutPanel1.Controls.Cast<EachEntry>()
            .Select(a => a.Text)
            .Distinct().ToArray());
    }

    private void frmEntryList_Load(object sender, EventArgs e)
    {
        m_FormNewEntry.OnNewEntryEvent += FormNewEntry_OnNewEntryEvent;

        FormNewEntry_OnNewEntryEvent("Vincent");
        FormNewEntry_OnNewEntryEvent("Dagpin");
        FormNewEntry_OnNewEntryEvent("Task 101");
        FormNewEntry_OnNewEntryEvent("Task 102");
        FormNewEntry_OnNewEntryEvent("Task 502");
        FormNewEntry_OnNewEntryEvent("Task 605");
        FormNewEntry_OnNewEntryEvent("The quick brown fox jumps over the lazy dog");

        p_OnLoad = false;
    }

    private void FormNewEntry_OnNewEntryEvent(string entry)
    {
        var newEntry = new EachEntry(entry, DateTime.Now, null);

        newEntry.OnResumeEvent += NewEntry_OnResumeEvent;

        flowLayoutPanel1.Controls.Add(newEntry);

        ActiveControl = newEntry;

        if (!p_OnLoad)
        {
            StopAll();
            newEntry.Start();
        }
    }

    private void NewEntry_OnResumeEvent(string entry)
    {
        FormNewEntry_OnNewEntryEvent(entry);
    }

    private void StopAll()
    {
        foreach (EachEntry item in flowLayoutPanel1.Controls)
        {
            if (item.IsStarted)
            {
                item.Stop();
            }
        }
    }
}