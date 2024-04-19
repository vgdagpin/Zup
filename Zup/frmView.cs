using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Zup;

public partial class frmView : Form
{
    private readonly ZupDbContext p_DbContext;

    public delegate void OnSelectedItem(int entryID);

    public event OnSelectedItem? OnSelectedItemEvent;

    public frmView(ZupDbContext dbContext)
    {
        InitializeComponent();
        p_DbContext = dbContext;
    }

    private void frmView_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        Hide();
    }

    private void frmView_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible)
        {
            dgView.DataSource = p_DbContext.TimeLogs
                .OrderByDescending(a => a.ID)
                .ToList()
                .Select(a =>
                {
                    string? duration = null;
                    TimeSpan? durationData = null;

                    if (a.EndedOn != null)
                    {
                        durationData = a.EndedOn!.Value - a.StartedOn;

                        duration = $"{durationData.Value.Hours:00}:{durationData.Value.Minutes:00}:{durationData.Value.Seconds:00}";
                    }

                    return new TimeLogSummary
                    {
                        ID = a.ID,
                        Task = a.Task,
                        StartedOn = a.StartedOn.ToString("MM/dd hh:mm tt"),
                        EndedOn = a.EndedOn?.ToString("MM/dd hh:mm tt"),
                        Duration = duration,
                        DurationData = durationData
                    };
                })
                .ToList();
        }
    }

    private void dgView_DoubleClick(object sender, EventArgs e)
    {
        dgView.SelectedRows.Cast<DataGridViewRow>().Select(a => (TimeLogSummary)a.DataBoundItem)
            .ToList()
            .ForEach(a =>
            {
                if (OnSelectedItemEvent != null)
                {
                    OnSelectedItemEvent(a.ID);
                }
            });
    }

    private void dgView_SelectionChanged(object sender, EventArgs e)
    {
        TimeSpan ts = new TimeSpan();

        dgView.SelectedRows.Cast<DataGridViewRow>().Select(a => (TimeLogSummary)a.DataBoundItem)
            .Where(a => a.DurationData != null)
            .ToList()
            .ForEach(a =>
            {
                ts += a!.DurationData!.Value;
            });

        lblSelectedTotal.Text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
    }
}

public class TimeLogSummary
{
    public int ID { get; set; }

    [MaxLength(255)]
    public string Task { get; set; } = null!;


    public string StartedOn { get; set; } = null!;
    public string? EndedOn { get; set; }
    public string? Duration { get; set; }

    public TimeSpan? DurationData { get; set; }
}