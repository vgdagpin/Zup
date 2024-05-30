namespace Zup.CustomControls;

public partial class EachEntry : UserControl
{
    public const string StartChar = "►";
    public const string StopChar = "■";

    Color RunningColor = Color.LightPink;

    public bool IsStarted { get; private set; }

    public delegate void OnStop(Guid id, DateTime endOn);
    public delegate void OnUpdate(Guid id);
    public delegate void OnStart(Guid id);

    public event EventHandler<NewEntryEventArgs>? OnResumeEvent;
    public event OnStop? OnStopEvent;
    public event OnUpdate? OnUpdateEvent;
    public event OnStart? OnStartEvent;
    public event EventHandler<NewEntryEventArgs>? OnStartQueueEvent;

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

    byte? rank;
    public byte? Rank
    {
        get => rank;
        set
        {
            rank = value;

            if (rank == null)
            {
                lblRank.Text = string.Empty;
            }
            else
            {
                lblRank.Text = $"#{rank}";
            }
        }
    }

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

    public Guid EntryID { get; set; }

    public DateTime CreatedOn { get; set; }

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

    public bool IsRunning => tmr.Enabled;

    public EachEntry(string text)
    {
        InitializeComponent();

        Text = text;

        WriteTime();
    }

    public EachEntry(Guid entryID, string text, DateTime createdOn, DateTime? startedOn, DateTime? endedOn = null)
    {
        InitializeComponent();

        EntryID = entryID;
        Text = text;
        CreatedOn = createdOn;
        StartedOn = startedOn;
        EndedOn = endedOn;
        Rank = null;

        WriteTime();

        BackColor = DefaultBackColor;
        btnToggleStartStop.ForeColor = DefaultForeColor;

        VisibleChanged += EachEntry_VisibleChanged;

        IsExpanded = StartedOn != null;

        SetStyle(ControlStyles.Selectable, false);
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
        if (StartedOn != null && EndedOn == null)
        {
            lblTimeInOut.Text = $"{StartedOn:hh:mmtt}";
            lblDuration.Text = "";

            return;
        }
        else if (StartedOn == null || EndedOn == null)
        {
            return;
        }

        lblTimeInOut.Text = $"{StartedOn:hh:mmtt} - {EndedOn:hh:mmtt}";

        toolTip.SetToolTip(lblTimeInOut, $"{StartedOn:MM/dd/yy hh:mm:tt} - {EndedOn:MM/dd/yy hh:mm:tt}");

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
    }

    public void Start()
    {
        if (EndedOn != null)
        {
            if (OnResumeEvent != null)
            {
                // we need to set this to null
                // because we are moving this rank to the new entry
                Rank = null;

                // if shift is pressed, create a new entry running in parallel
                var args = new NewEntryEventArgs(Text)
                {
                    StopOtherTask = !ModifierKeys.HasFlag(Keys.Shift),
                    StartNow = true,
                    ParentEntryID = EntryID,
                    BringTags = true
                };

                OnResumeEvent(this, args);

                return;
            }
        }

        if (StartedOn == null)
        {
            if (OnStartQueueEvent != null)
            {
                // we need to set this to null
                // because we are moving this rank to the new entry
                Rank = null;

                var args = new NewEntryEventArgs(Text)
                {
                    StopOtherTask = !ModifierKeys.HasFlag(Keys.Shift),
                    StartNow = true,
                    ParentEntryID = EntryID,
                    HideParent = true,
                    BringNotes = true,
                    BringTags = true
                };

                OnStartQueueEvent(this, args);

                return;
            }
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

    #region Register Mouse DoubleClick
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
    #endregion
}
