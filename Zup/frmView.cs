using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

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
                .AsNoTracking()
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
                        StartedOn = a.StartedOn,
                        EndedOn = a.EndedOn,
                        Duration = durationData,
                        DurationString = duration
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
        var ts = new TimeSpan();

        dgView.SelectedRows.Cast<DataGridViewRow>().Select(a => (TimeLogSummary)a.DataBoundItem)
            .Where(a => a.Duration != null)
            .ToList()
            .ForEach(a =>
            {
                ts += a!.Duration!.Value;
            });

        lblSelectedTotal.Text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
    }

    private void btnBrowseTimesheetFolder_Click(object sender, EventArgs e)
    {
        var result = fbdTimesheetFolder.ShowDialog();

        if (result == DialogResult.OK)
        {
            Properties.Settings.Default.TimesheetsFolder = fbdTimesheetFolder.SelectedPath;
            Properties.Settings.Default.Save();

            txtTimesheetFolder.Text = fbdTimesheetFolder.SelectedPath;
        }
    }

    private void frmView_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.TimesheetsFolder))
        {
            txtTimesheetFolder.Text = Properties.Settings.Default.TimesheetsFolder;
        }
    }

    private void btnExportTimesheet_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTimesheetFolder.Text))
        {
            MessageBox.Show("Timesheet directory is empty!");
            return;
        }

        if (!Path.Exists(txtTimesheetFolder.Text))
        {
            MessageBox.Show("Timesheet directory doesn't exist!");
            return;
        }

        var path = Path.Combine(txtTimesheetFolder.Text, $"{dtTimesheetDate.Value:MM-dd-yyyy}.fd");        

        var content = new StringBuilder();

        dgView.SelectedRows.Cast<DataGridViewRow>()
            .Select(a => (TimeLogSummary)a.DataBoundItem)
            .Where(a => a.Duration != null)
            .ToList()
            .ForEach(a =>
            {
                content.AppendLine($"{a.StartedOn.Ticks}^{a.Task}^{ExtractComments(a.ID)}^{GetClients(a.ID)}^{a.DurationString}^False^False");
            });

        var confirm = MessageBox.Show("Exporting to: \n\n" + path + "\n\nThis will replace existing records.", "Export", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

        if (confirm == DialogResult.OK)
        {
            File.WriteAllText(path, content.ToString());
        }
    }

    private string ExtractComments(int taskID)
    {
        var notes = p_DbContext.Notes.Where(a => a.LogID == taskID).ToList();

        if (!notes.Any())
        {
            return string.Empty;
        }

        var str = new List<string>();

        foreach (var note in notes)
        {
            str.Add(CleanNotes(note.Notes));
        }

        if (!str.Any())
        {
            return string.Empty;
        }

        return string.Join(';', str.Where(a => !string.IsNullOrWhiteSpace(a)));
    }

    private string CleanNotes(string notes)
    {
        notes = notes.Replace('^', ' ');
        notes = notes.Replace(Environment.NewLine, " ");
        notes = notes.Replace("\n\r", " ");
        notes = notes.Replace("\n", " ");
        notes = notes.Replace("\r", " ");

        if (notes.Length > 100)
        {
            notes = notes.Substring(0, 100) + "...";
        }

        return notes;
    }

    private string GetClients(int taskID)
    {
        return string.Empty;
    }

    /*
    // from Fuse
    public void SaveTimesheet(string currentFile)
    {
        try
        {
            List<String> lines = new List<String>();
            foreach (var task in Tasks)
            {
                lines.Add(task.TaskId + "^" + task.TaskDesc + "^" + task.Comments + "^" + task.Client + "^" + task.Duration + "^" + task.Running + "^" + task.Delete);
            }

            if (currentFile == DateTime.Now.Date.ToString("MM-dd-yyyy"))
            {
                endTime = Functions.GetDateTime("{0:MM/dd/yyyy h:mm:ss tt}");
            }

            lines.Add("FileEnd^" + MiscTimer.TaskSeconds + "^" + startTime + "^" + endTime + "^" + totalTimeClosed);
            //start saving away data        
            foreach (var item in awayData)
            {
                lines.Add("AwayData^" + item.Lock + "^" + item.Unlock + "^" + item.Add + "^" + item.Remove);
            }

            System.IO.File.WriteAllLines(Utility.CurrentStageUserDataTimesheetFolder + currentFile + ".fd", lines);
        }
        catch { }
    }
     */
}