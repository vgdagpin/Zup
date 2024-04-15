namespace Zup.CustomControls;

public partial class EachEntry : UserControl
{
    const int wrapLength = 30;
    const string StartChar = "►";
    const string StopChar = "■";

    public bool IsStarted { get; private set; }

    private string txt = null!;

    public delegate void OnResume(string entry);
    public delegate void OnStop(int id, DateTime endOn);
    public delegate void OnUpdate(int id);

    public event OnResume? OnResumeEvent;
    public event OnStop? OnStopEvent;
    public event OnUpdate? OnUpdateEvent;

    public override string Text
    {
        get => txt;
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
        btnToggleStartStop.ForeColor = Color.Black;
        IsStarted = false;

        WriteTime();

        if (OnStopEvent != null)
        {
            OnStopEvent(EntryID, EndedOn.Value);
        }
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
