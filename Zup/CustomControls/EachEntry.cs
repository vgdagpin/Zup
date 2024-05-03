using Microsoft.EntityFrameworkCore.Sqlite.Storage.Json.Internal;

namespace Zup.CustomControls;

public partial class EachEntry : UserControl
{
    const string StartChar = "►";
    const string StopChar = "■";

    Color RunningColor = Color.LightPink;

    public bool IsStarted { get; private set; }

    public delegate void OnResume(string entry, bool stopOtherTask, bool startNow, int? parentEntryID = null);
    public delegate void OnStop(int id, DateTime endOn);
    public delegate void OnUpdate(int id);
    public delegate void OnStart(int id);

    public event OnResume? OnResumeEvent;
    public event OnStop? OnStopEvent;
    public event OnUpdate? OnUpdateEvent;
    public event OnStart? OnStartEvent;

    public event MouseEventHandler? TaskMouseDown;

    public override string Text
    {
        get => lblText.Text;
        set
        {
            lblText.Text = value;

            if (!string.IsNullOrWhiteSpace(value))
            {
                toolTip.SetToolTip(lblText, value);
            }
        }
    }

    public const int ExpandedHeight = 35;
    public const int CollapsedHeight = 22;

    bool isExpanded = false;
    public bool IsExpanded
    {
        get
        {
            return isExpanded;
        }
        set
        {
            isExpanded = value;

            if (isExpanded)
            {
                Height = ExpandedHeight;
                lblTimeInOut.Visible = true;
                lblDuration.Visible = true;
            }
            else
            {
                Height = CollapsedHeight;
                lblTimeInOut.Visible = false;
                lblDuration.Visible = false;
            }
        }
    }

    public bool IsFirstItem { get; set; }

    public int EntryID { get; set; }

    DateTime? startedOn;
    public DateTime? StartedOn
    {
        get
        {
            return startedOn;
        }
        set
        {
            startedOn = value;

            WriteTime();
        }
    }

    DateTime? endedOn;
    public DateTime? EndedOn
    {
        get
        {
            return endedOn;
        }
        set
        {
            endedOn = value;

            WriteTime();
        }
    }

    public EachEntry(string text)
    {
        InitializeComponent();

        Text = text;

        WriteTime();
    }

    public EachEntry(int entryID, string text, DateTime startedOn, DateTime? endedOn = null)
    {
        InitializeComponent();

        EntryID = entryID;
        Text = text;
        StartedOn = startedOn;
        EndedOn = endedOn;

        WriteTime();

        BackColor = DefaultBackColor;
        btnToggleStartStop.ForeColor = DefaultForeColor;

        VisibleChanged += EachEntry_VisibleChanged;

        RegisterMouseDownAndDoubleClick();

        IsExpanded = !Properties.Settings.Default.AutoFold;
    }

    private void RegisterMouseDownAndDoubleClick()
    {
        //MouseDown += (sender, args) =>
        //{
        //    if (args.Button == MouseButtons.Left)
        //    {
        //        if (args.Clicks == 1) // mousedown
        //        {
        //            TaskMouseDown?.Invoke(sender, args);
        //        }
        //        else // double click
        //        {
        //            OnUpdateEvent?.Invoke(EntryID);
        //        }
        //    }
        //};

        //lblText.MouseDown += (sender, args) =>
        //{
        //    if (args.Button == MouseButtons.Left)
        //    {
        //        if (args.Clicks == 1) // mousedown
        //        {
        //            TaskMouseDown?.Invoke(sender, args);
        //        }
        //        else // double click
        //        {
        //            OnUpdateEvent?.Invoke(EntryID);
        //        }
        //    }
        //};

        //lblTimeInOut.MouseDown += (sender, args) =>
        //{
        //    if (args.Button == MouseButtons.Left)
        //    {
        //        if (args.Clicks == 1) // mousedown
        //        {
        //            TaskMouseDown?.Invoke(sender, args);
        //        }
        //        else // double click
        //        {
        //            OnUpdateEvent?.Invoke(EntryID);
        //        }
        //    }
        //};

        //lblDuration.MouseDown += (sender, args) =>
        //{
        //    if (args.Button == MouseButtons.Left)
        //    {
        //        if (args.Clicks == 1) // mousedown
        //        {
        //            TaskMouseDown?.Invoke(sender, args);
        //        }
        //        else // double click
        //        {
        //            OnUpdateEvent?.Invoke(EntryID);
        //        }
        //    }
        //};
    }

    private void EachEntry_VisibleChanged(object? sender, EventArgs e)
    {
        if (!Visible)
        {
            tmr.Enabled = false;
        }
    }

    private void WriteTime()
    {
        if (EndedOn == null)
        {
            lblTimeInOut.Text = $"{StartedOn:hh:mmtt}";
            lblDuration.Text = "";

            return;
        }

        lblTimeInOut.Text = $"{StartedOn:hh:mmtt} - {EndedOn:hh:mmtt}";

        var diff = EndedOn.Value - StartedOn!.Value;

        lblDuration.Text = $"{diff.Hours:00}:{diff.Minutes:00}:{diff.Seconds:00}";
    }

    public void Stop()
    {
        tmr.Stop();
        EndedOn = DateTime.Now;
        btnToggleStartStop.Text = StartChar;
        btnToggleStartStop.ForeColor = DefaultForeColor;
        IsStarted = false;

        WriteTime();

        if (OnStopEvent != null)
        {
            OnStopEvent(EntryID, EndedOn.Value);
        }

        BackColor = DefaultBackColor;

        if (!IsFirstItem && Properties.Settings.Default.AutoFold)
        {
            IsExpanded = false;
        }
    }

    public void Start()
    {
        if (EndedOn != null)
        {
            if (OnResumeEvent != null)
            {
                // if shift is pressed, create a new entry running in parallel
                OnResumeEvent(Text, !ModifierKeys.HasFlag(Keys.Shift), true, EntryID);

                return;
            }
        }

        if (StartedOn == null)
        {
            StartedOn = DateTime.Now;
        }

        tmr.Start();

        btnToggleStartStop.Text = StopChar;
        btnToggleStartStop.ForeColor = Color.Red;
        IsStarted = true;

        EndedOn = null;

        BackColor = RunningColor;

        IsExpanded = true;

        if (OnStartEvent != null)
        {
            OnStartEvent(EntryID);
        }
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

    private void tmrDuration_Tick(object sender, EventArgs e)
    {
        var diff = DateTime.Now - StartedOn!.Value;

        lblDuration.Text = $"{diff.Hours:00}:{diff.Minutes:00}:{diff.Seconds:00}";
    }

    private void lblText_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (e.Clicks == 1) // mousedown
            {
                TaskMouseDown?.Invoke(sender, e);
            }
            else // double click
            {
                OnUpdateEvent?.Invoke(EntryID);
            }
        }
    }

    private void lblTimeInOut_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (e.Clicks == 1) // mousedown
            {
                TaskMouseDown?.Invoke(sender, e);
            }
            else // double click
            {
                OnUpdateEvent?.Invoke(EntryID);
            }
        }
    }

    private void lblDuration_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (e.Clicks == 1) // mousedown
            {
                TaskMouseDown?.Invoke(sender, e);
            }
            else // double click
            {
                OnUpdateEvent?.Invoke(EntryID);
            }
        }
    }

    private void EachEntry_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (e.Clicks == 1) // mousedown
            {
                TaskMouseDown?.Invoke(sender, e);
            }
            else // double click
            {
                OnUpdateEvent?.Invoke(EntryID);
            }
        }
    }
}
