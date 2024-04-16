namespace Zup.CustomControls;

public partial class EachEntry : UserControl
{
    const string StartChar = "►";
    const string StopChar = "■";

    Color RunningColor = Color.LightPink;

    public bool IsStarted { get; private set; }

    public delegate void OnResume(string entry);
    public delegate void OnStop(int id, DateTime endOn);
    public delegate void OnUpdate(int id);
    public delegate void OnStart(int id);

    public event OnResume? OnResumeEvent;
    public event OnStop? OnStopEvent;
    public event OnUpdate? OnUpdateEvent;
    public event OnStart? OnStartEvent;

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

    public int EntryID { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? EndedOn { get; set; }

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
    }

    private void WriteTime()
    {
        if (EndedOn == null)
        {
            lblStart.Text = $"{StartedOn:hh:mmtt}";
            lblDuration.Text = "";

            return;
        }

        lblStart.Text = $"{StartedOn:hh:mmtt} - {EndedOn:hh:mmtt}";

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
                OnResumeEvent(Text);

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

    private void timer1_Tick(object sender, EventArgs e)
    {
        var diff = DateTime.Now - StartedOn!.Value;

        lblDuration.Text = $"{diff.Hours:00}:{diff.Minutes:00}:{diff.Seconds:00}";
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (OnUpdateEvent != null)
        {
            OnUpdateEvent(EntryID);
        }
    }
}
