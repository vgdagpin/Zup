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

                    if (a.EndedOn != null)
                    {
                        var diff = a.EndedOn!.Value - a.StartedOn;

                        duration = $"{diff.Hours:00}:{diff.Minutes:00}:{diff.Seconds:00}";
                    }

                    return new TimeLogSummary
                    {
                        ID = a.ID,
                        Task = a.Task,
                        StartedOn = a.StartedOn.ToString("MM/dd hh:mm tt"),
                        EndedOn = a.EndedOn?.ToString("MM/dd hh:mm tt"),
                        Duration = duration
                    };
                })
                .ToList();
        }
    }

    private void dgView_DoubleClick(object sender, EventArgs e)
    {
        dgView.SelectedRows.Cast<DataGridViewRow>().ToList().ForEach(a =>
        {
            var id = (int)a.Cells["ID"].Value;

            if (OnSelectedItemEvent != null)
            {
                OnSelectedItemEvent(id);
            }
        });
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
}