using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zup.CustomControls;

public partial class EachEntry : UserControl
{
    const int wrapLength = 30;

    public bool IsStarted { get; private set; }

    private DateTime? startedOn = null;
    private DateTime? endedOn = null;
    private string txt = null!;

    public delegate void OnResume(string entry);

    public event OnResume? OnResumeEvent;

    public override string Text
    {
        get
        {
            return txt;
        }
        set
        {
            txt = value;

            if (value != null)
            {
                if (value.Length > wrapLength)
                {
                    lblText.Text = value.Substring(0, wrapLength) + "..";
                }
                else
                {
                    lblText.Text = value;
                }
            }
        }
    }

    public DateTime StartedOn
    {
        get
        {
            return startedOn ?? DateTime.MinValue;
        }
        set
        {
            startedOn = value;
            //lblStart.Text = value.ToString("hh:mmtt");
        }
    }

    public DateTime? EndedOn
    {
        get
        {
            return endedOn;
        }
        set
        {
            endedOn = value;
            //lblEnd.Text = value?.ToString("hh:mmtt");
        }
    }

    public EachEntry(string text)
    {
        InitializeComponent();

        Text = text;

        WriteTime();
    }

    public EachEntry(string text, DateTime startedOn, DateTime? endedOn)
    {
        InitializeComponent();

        Text = text;
        StartedOn = startedOn;
        EndedOn = endedOn;

        WriteTime();
    }

    private void WriteTime()
    {
        if (EndedOn == null)
        {
            lblStart.Text = $"{StartedOn:hh:mmtt}";

            return;
        }

        lblStart.Text = $"{StartedOn:hh:mmtt} - {EndedOn:hh:mmtt}";

        var diff = EndedOn.Value - StartedOn;

        lblDuration.Text = $"{diff.Hours:00}:{diff.Minutes:00}:{diff.Seconds:00}";
    }

    public void Stop()
    {
        tmr.Stop();
        EndedOn = DateTime.Now;
        btnToggleStartStop.Text = "►";
        btnToggleStartStop.ForeColor = Color.Black;
        IsStarted = false;

        WriteTime();
    }

    public void Start()
    {
        if (EndedOn != null)
        {
            if (OnResumeEvent != null)
            {
                OnResumeEvent(Text);

                return;
            }
        }
        else
        {
            StartedOn = DateTime.Now;
        }

        tmr.Start();

        btnToggleStartStop.Text = "■";
        btnToggleStartStop.ForeColor = Color.Red;
        IsStarted = true;

        EndedOn = null;
    }

    private void btnToggleStartStop_Click(object sender, EventArgs e)
    {
        if (!IsStarted)
        {
            Start();
        }
        else
        {
            Stop();
        }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        var diff = DateTime.Now - StartedOn;

        lblDuration.Text = $"{diff.Hours:00}:{diff.Minutes:00}:{diff.Seconds:00}";
    }
}
