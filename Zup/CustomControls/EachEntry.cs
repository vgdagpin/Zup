using Zup.EventArguments;

namespace Zup.CustomControls;

public enum TaskStatus
{
    Ongoing,
    Queued,
    Ranked,
    Closed,
    Unclosed,
    Running
}

public partial class EachEntry : UserControl
{
    public const string StartChar = "►";
    public const string StopChar = "■";
    public const string ReminderChar = "Ѧ";

    Color RunningColor = Color.LightPink;

    public const float OngoingRowHeight = 37;
    public const float RowHeight = 26;

    public const int ExpandedHeight = 35;
    public const int CollapsedHeight = 22;

    public bool IsStarted { get; private set; }

    public delegate void OnStop(Guid id, DateTime endOn);
    public delegate void OnUpdate(Guid id);

    public event EventHandler<NewEntryEventArgs>? OnResumeEvent;
    public event OnStop? OnStopEvent;
    public event OnUpdate? OnUpdateEvent;
    public event EventHandler<OnStartEventArgs>? OnStartEvent;
    public event EventHandler<NewEntryEventArgs>? OnStartQueueEvent;

    public event MouseEventHandler? TaskMouseDown;
    public event MouseEventHandler? TaskRightClick;

    public TaskStatus TaskStatus
    {
        get
        {
            if (tmr.Enabled)
            {
                return TaskStatus.Running;
            }

            if (Rank != null)
            {
                return TaskStatus.Ranked;
            }

            if (StartedOn == null)
            {
                return TaskStatus.Queued;
            }

            if (StartedOn != null && EndedOn == null)
            {
                return TaskStatus.Unclosed;
            }

            if (StartedOn != null && EndedOn != null)
            {
                return TaskStatus.Closed;
            }

            return TaskStatus.Ongoing;
        }
    }

    public override string Text
    {
        get => lblText.Text;
        set
        {
            lblText.Text = value;

            if (string.IsNullOrEmpty(value))
            {
                lblText.Text = "Blank Task";
                lblText.ForeColor = Color.Gray;
            }
            else
            {
                lblText.ForeColor = SystemColors.ControlText;
            }

            if (!string.IsNullOrWhiteSpace(lblText.Text))
            {
                toolTip.SetToolTip(lblText, lblText.Text);
            }
        }
    }

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
    private DateTime? taskReminder;

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

    public DateTime? TaskReminder 
    { 
        get => taskReminder; 
        set
        {
            taskReminder = value;
            btnReminder.Visible = value.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue;
        }
    }

    public EachEntry(string text)
    {
        InitializeComponent();

        Text = text;

        WriteTime();
    }

    public EachEntry(Guid entryID, string text, DateTime createdOn, DateTime? startedOn, DateTime? endedOn = null, DateTime? taskReminder = null)
    {
        InitializeComponent();

        EntryID = entryID;
        Text = text;
        CreatedOn = createdOn;
        StartedOn = startedOn;
        EndedOn = endedOn;
        Rank = null;
        TaskReminder = taskReminder;

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
        var curStatus = TaskStatus;

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
                var args = new NewEntryEventArgs(Text)
                {
                    StopOtherTask = !ModifierKeys.HasFlag(Keys.Shift),
                    StartNow = true,
                    ParentEntryID = EntryID,
                    HideParent = TaskStatus == TaskStatus.Queued, // only hide if it is started from queued tasks
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
            OnStartEvent(this, new OnStartEventArgs
            {
                PreviousStatus = curStatus
            });
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
        else if (e.Button == MouseButtons.Right)
        {
            TaskRightClick?.Invoke(this, e);
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
        else if (e.Button == MouseButtons.Right)
        {
            TaskRightClick?.Invoke(this, e);
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
        else if (e.Button == MouseButtons.Right)
        {
            TaskRightClick?.Invoke(this, e);
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
        else if (e.Button == MouseButtons.Right)
        {
            TaskRightClick?.Invoke(this, e);
        }
    }
    #endregion

    private void btnReminder_Click(object sender, EventArgs e)
    {

    }
}
