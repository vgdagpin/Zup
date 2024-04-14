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
using Zup.Entities;
using static Zup.frmNewEntry;

namespace Zup;
public partial class frmEntryList : Form
{
    private frmNewEntry m_FormNewEntry;
    private bool p_OnLoad = true;
    private readonly ZupDbContext m_DbContext;

    protected override CreateParams CreateParams
    {
        get
        {
            var Params = base.CreateParams;
            Params.ExStyle |= 0x00000080;
            return Params;
        }
    }

    public frmEntryList(ZupDbContext dbContext, frmNewEntry frmNewEntry)
    {
        InitializeComponent();

        Left = Screen.PrimaryScreen!.WorkingArea.Width - Width - 5;
        Top = Screen.PrimaryScreen!.WorkingArea.Height - Height - 5;

        m_DbContext = dbContext;
        m_FormNewEntry = frmNewEntry;
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
        m_FormNewEntry.OnNewEntryEvent += M_FormNewEntry_OnNewEntryEvent;

        var tasks = m_DbContext.TimeLogs.ToList();

        foreach (var task in tasks)
        {
            var eachEntry = new EachEntry(task.ID, task.Task, task.StartedOn, task.EndedOn);

            eachEntry.OnResumeEvent += NewEntry_OnResumeEvent;
            eachEntry.OnStopEvent += Ee_OnStopEvent;

            FormNewEntry_OnNewEntryEvent(eachEntry);
        }

        p_OnLoad = false;
    }

    private void M_FormNewEntry_OnNewEntryEvent(string entry)
    {
        var newE = new tbl_TimeLog
        {
            Task = entry,
            StartedOn = DateTime.Now
        };

        m_DbContext.TimeLogs.Add(newE);

        m_DbContext.SaveChanges();

        var ee = new EachEntry(newE.ID, newE.Task, newE.StartedOn, null);

        ee.OnResumeEvent += NewEntry_OnResumeEvent;
        ee.OnStopEvent += Ee_OnStopEvent;

        FormNewEntry_OnNewEntryEvent(ee);
    }

    private void FormNewEntry_OnNewEntryEvent(EachEntry newEntry)
    {
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
        var newE = new tbl_TimeLog
        {
            Task = entry,
            StartedOn = DateTime.Now
        };

        m_DbContext.TimeLogs.Add(newE);

        m_DbContext.SaveChanges();

        var ee = new EachEntry(newE.ID, newE.Task, newE.StartedOn, null);

        ee.OnResumeEvent += NewEntry_OnResumeEvent;
        ee.OnStopEvent += Ee_OnStopEvent;

        FormNewEntry_OnNewEntryEvent(ee);
    }

    private void Ee_OnStopEvent(int id, DateTime endOn)
    {
        var existingE = m_DbContext.TimeLogs.Find(id);

        if (existingE != null)
        {
            existingE.EndedOn = endOn;

            m_DbContext.SaveChanges();
        }
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