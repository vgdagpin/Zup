using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Microsoft.EntityFrameworkCore;

using Zup.Entities;

namespace Zup;

public partial class frmViewList : Form
{
    private readonly ZupDbContext p_DbContext;
    private readonly SettingHelper settingHelper;
    private readonly TaskCollection p_Tasks;
    private frmMain m_FormMain = null!;
    private Font? tagFont = null;

    public delegate void OnSelectedItem(Guid entryID);
    public delegate void OnExported();

    public event OnExported? OnExportedEvent;


    private IEnumerable<WeekData> WeekDataList = null!;

    public frmViewList(ZupDbContext dbContext, SettingHelper settingHelper, TaskCollection tasks)
    {
        InitializeComponent();
        p_DbContext = dbContext;
        this.settingHelper = settingHelper;
        p_Tasks = tasks;
        p_DbContext.Database.Migrate();

        p_Tasks.OnTaskStarted += P_Tasks_OnTaskAdded;
        p_Tasks.OnTaskStopped += P_Tasks_OnTaskRemoved;
        p_Tasks.OnTaskDeleted += P_Tasks_OnTaskDeleted;
        p_Tasks.OnTaskUpdated += P_Tasks_OnTaskUpdated;
    }

    private void P_Tasks_OnTaskUpdated(object? sender, ITask e)
    {
        RefreshList();
    }

    private void P_Tasks_OnTaskDeleted(object? sender, ITask e)
    {
        RefreshList();
    }

    private void P_Tasks_OnTaskRemoved(object? sender, ITask e)
    {
        RefreshList();
    }

    private void P_Tasks_OnTaskAdded(object? sender, EventArguments.NewEntryEventArgs e)
    {
        RefreshList();
    }

    private void frmView_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(settingHelper.TimesheetsFolder))
        {
            txtTimesheetFolder.Text = settingHelper.TimesheetsFolder;
        }

        txtRowFormat.Text = settingHelper.ExportRowFormat;
        txtExtension.Text = settingHelper.ExportFileExtension;

        SetLabelOutput();

        typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dgView, new object[] { true });

        TagHelper.Register<ZupTask>("StartedOnTicks", (tagKey, dicAction) => dicAction.StartedOn!.Value.Ticks.ToString());
        TagHelper.Register<ZupTask>("Task", (tagKey, dicAction) => NoteSummary.CleanString(dicAction.Task, 200));
        TagHelper.Register<ZupTask>("Tags", (tagKey, dicAction) => ExtractTags(dicAction.ID));
        TagHelper.Register<ZupTask>("Tag", (tagKey, dicAction) => ExtractTag(tagKey, dicAction.ID));
        TagHelper.Register<ZupTask>("Comments", (tagKey, dicAction) => ExtractComments(dicAction.ID));
        TagHelper.Register<ZupTask>("Duration", (tagKey, dicAction) => dicAction.DurationString!);
        TagHelper.Register<ZupTask>("TimesheetDate", (tagKey, dicAction) => dtTimesheetDate.Value.ToString(dtTimesheetDate.CustomFormat));
    }

    private void frmView_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible)
        {
            LoadWeekData();
        }
    }

    public void SetFormMain(frmMain frmMain)
    {
        m_FormMain = frmMain;
    }

    private (DateTime Start, DateTime End) LoadWeekData()
    {
        var curWeekNum = Utility.GetWeekNumber(DateTime.Now);

        // if weekdata list is not yet initialized or its not for this year
        if (WeekDataList == null || WeekDataList.Skip(1).First().Start.Year != DateTime.Now.Year)
        {
            WeekDataList = Utility.GetWeekData(DateTime.Now.Year)
                .OrderByDescending(a => a.WeekNumber);

            lbWeek.DataSource = WeekDataList.Where(a => a.WeekNumber <= curWeekNum).ToArray();
        }

        var curWeekData = WeekDataList.Single(a => a.WeekNumber == curWeekNum);

        if (lbWeek.SelectedIndices.Count > 0)
        {
            var selData = lbWeek.SelectedItems.Cast<WeekData>().ToArray();

            return (selData.Min(a => a.Start), selData.Max(a => a.End));
        }

        return (curWeekData.Start, curWeekData.End);
    }

    private void LoadListData(DateTime from, DateTime to, string? search = null)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            search = null;
        }

        // between the date range or not yet started
        Expression<Func<tbl_TaskEntry, bool>> filter = a => (from <= a.StartedOn && a.StartedOn <= to) || a.StartedOn == null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            // search filter, if search provided then ignore date range
            filter = a => a.Task.ToLower().Contains(search.ToLower());
        }

        var runningIDs = p_Tasks.RunningIDs;
        var ds = (from te in p_DbContext.TaskEntries.Where(filter)
                  join tet in p_DbContext.TaskEntryTags on te.ID equals tet.TaskID into tet
                  from tet2 in tet.DefaultIfEmpty()
                  join tag in p_DbContext.Tags on tet2.TagID equals tag.ID into tag
                  from tag2 in tag.DefaultIfEmpty()
                  select new
                  {
                      TaskEntry = te,
                      Tag = tet2 == null
                        ? null
                        : new { tet2.CreatedOn, tag2.Name }
                  })
                        .AsEnumerable()
                        .GroupBy(x => x.TaskEntry)
                        .OrderByDescending(a => a.Key.StartedOn)
                        .Select(a =>
                        {
                            string? duration = null;
                            TimeSpan? durationData = null;

                            if (a.Key.EndedOn != null && a.Key.StartedOn != null)
                            {
                                durationData = a.Key.EndedOn!.Value - a.Key.StartedOn;

                                duration = $"{durationData!.Value.Hours:00}:{durationData.Value.Minutes:00}:{durationData.Value.Seconds:00}";
                            }

                            return new ZupTask
                            {
                                ID = a.Key.ID,
                                Task = a.Key.Task,
                                StartedOn = a.Key.StartedOn,
                                EndedOn = a.Key.EndedOn,
                                Duration = durationData,
                                DurationString = duration,
                                IsRunning = runningIDs.Contains(a.Key.ID),
                                Tags = a.Where(a => a.Tag != null)
                                    .OrderByDescending(a => a.Tag.CreatedOn)
                                    .Select(x => x.Tag.Name)
                                    .ToArray()
                            };
                        })
                        .ToList();

        foreach (var item in ds)
        {
            item.DayOfWeek = Utility.GetDayOfWeek(item.StartedOn, settingHelper.DayStart, settingHelper.DayEnd);
        }

        dgView.DataSource = ds;
    }

    public void RefreshList()
    {
        var currentWeekData = LoadWeekData();
        LoadListData(currentWeekData.Start, currentWeekData.End, txtSearch.Text);
    }

    #region dgView Events
    private void dgView_DoubleClick(object sender, EventArgs e)
    {
        dgView.SelectedRows.Cast<DataGridViewRow>()
            .Select(a => a.DataBoundItem)
            .Cast<ZupTask>()
            .ToList()
            .ForEach(a =>
            {
                m_FormMain.ShowUpdateEntry(a.ID);
            });
    }

    private void dgView_SelectionChanged(object sender, EventArgs e)
    {
        var ts = new TimeSpan();

        dgView.SelectedRows.Cast<DataGridViewRow>()
            .Select(a => a.DataBoundItem)
            .Cast<ZupTask>()
            .Where(a => a.Duration != null)
            .ToList()
            .ForEach(a =>
            {
                ts += a.Duration!.Value;
            });

        lblSelectedTotal.Text = $"{ts.Days:00}:{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
    }

    private void dgView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        var column = dgView.Columns[e.ColumnIndex];

        if (column.Name == "PlayAction" && column is DataGridViewButtonColumn buttonColumn)
        {
            var dataRow = (ZupTask)dgView.Rows[e.RowIndex].DataBoundItem!;
            var cell = dgView.Rows[e.RowIndex].Cells[e.ColumnIndex];

            // Ensure the column uses cell values instead of column text
            if (buttonColumn.UseColumnTextForButtonValue)
            {
                buttonColumn.UseColumnTextForButtonValue = false;
            }

            var rowStatus = dataRow.GetTaskStatus();

            if (rowStatus == TaskStatus.Running)
            {
                e.Value = Constants.Controls.Stop;
                cell.ReadOnly = false;
            }
            else if (rowStatus == TaskStatus.Unclosed)
            {
                e.Value = string.Empty;
                cell.ReadOnly = true; // Disable clicking
                e.CellStyle.BackColor = dgView.DefaultCellStyle.BackColor;
                e.CellStyle.ForeColor = dgView.DefaultCellStyle.BackColor;
            }
            else
            {
                e.Value = Constants.Controls.Play;
                cell.ReadOnly = false;
            }
        }
    }

    private void dgView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        var column = dgView.Columns[e.ColumnIndex];

        if (column.Name == "PlayAction"
            && column is DataGridViewButtonColumn buttonColumn
            && buttonColumn.Text == Constants.Controls.Play)
        {
            var dataRow = (ZupTask)dgView.Rows[e.RowIndex].DataBoundItem!;
            var status = dataRow.GetTaskStatus();

            if (status == TaskStatus.Running)
            {
                p_Tasks.Stop(this, dataRow.ID);
            }
            else if (status == TaskStatus.Unclosed)
            {

            }
            else
            {
                p_Tasks.Start(this, dataRow.Task, true, !ModifierKeys.HasFlag(Keys.Shift), false, false, true, false, dataRow.ID);
            }
        }
    }

    private void dgView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgView.Columns[e.ColumnIndex].Name != "Task")
        {
            return;
        }

        e.PaintBackground(e.CellBounds, true);
        e.PaintContent(e.CellBounds);

        var dataRow = (ZupTask)dgView.Rows[e.RowIndex].DataBoundItem!;
        var tags = dataRow.Tags;

        if (tags == null || tags.Length == 0)
        {
            return;
        }

        if (tagFont == null)
        {
            tagFont = new Font(e.CellStyle!.Font!.FontFamily, 7);
        }

        var x = e.CellBounds.Right;

        for (int i = 0; i < tags.Length; i++)
        {
            var tag = tags[i];
            var textSize = e.Graphics!.MeasureString(tag, tagFont);

            x -= (int)textSize.Width + 4;

            var textRect = new Rectangle(x, e.CellBounds.Top + 3, (int)textSize.Width + 2, (int)textSize.Height + 1);
            DrawTag(tag, e.Graphics, textRect, tagFont);
        }

        e.Handled = true;
    }
    #endregion

    private void btnBrowseTimesheetFolder_Click(object sender, EventArgs e)
    {
        var result = fbdTimesheetFolder.ShowDialog();

        if (result == DialogResult.OK)
        {
            settingHelper.TimesheetsFolder = fbdTimesheetFolder.SelectedPath;
            settingHelper.Save();

            txtTimesheetFolder.Text = fbdTimesheetFolder.SelectedPath;

            SetLabelOutput();
        }
    }

    private void SetLabelOutput()
    {
        try
        {
            ttsPath.Text = GetOutputPath();
        }
        catch (Exception ex)
        {
            ttsPath.Text = ex.Message;
        }
    }

    private string GetOutputPath()
    {
        if (string.IsNullOrWhiteSpace(txtTimesheetFolder.Text))
        {
            throw new Exception("Timesheet directory is empty!");
        }

        if (!Path.Exists(txtTimesheetFolder.Text))
        {
            throw new Exception("Timesheet directory doesn't exist!");
        }

        var fileName = dtTimesheetDate.Value.ToString(dtTimesheetDate.CustomFormat);

        return Path.Combine(txtTimesheetFolder.Text, $"{fileName}{txtExtension.Text}");
    }

    private string GetContentWrapperPath(string wrapperFilename)
    {
        if (string.IsNullOrWhiteSpace(txtTimesheetFolder.Text))
        {
            throw new Exception("Timesheet directory is empty!");
        }

        if (!Path.Exists(txtTimesheetFolder.Text))
        {
            throw new Exception("Timesheet directory doesn't exist!");
        }

        return Path.Combine(txtTimesheetFolder.Text, wrapperFilename);
    }

    private void btnExportTimesheet_Click(object sender, EventArgs e)
    {
        try
        {
            var path = GetOutputPath();

            var content = GetContent(txtRowFormat.Text);
            var contentWrapper = GetContentWrapper();

            if (!string.IsNullOrWhiteSpace(contentWrapper))
            {
                content = contentWrapper.Replace("/// ~TimesheetEntries~ ///", content);
            }

            var confirm = MessageBox.Show("Exporting to: \n\n" + path + "\n\nThis will replace existing records.", "Zup", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (confirm == DialogResult.OK)
            {
                File.WriteAllText(path, content.Trim());

                if (OnExportedEvent != null)
                {
                    OnExportedEvent();
                }
            }

            settingHelper.ExportRowFormat = txtRowFormat.Text;
            settingHelper.ExportFileExtension = txtExtension.Text;

            settingHelper.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Zup");
        }
    }

    private string? GetContentWrapper()
    {
        var contentWrapperPath = GetContentWrapperPath("data-wrapper.js");

        if (!File.Exists(contentWrapperPath))
        {
            return null;
        }

        return File.ReadAllText(contentWrapperPath);
    }

    private string GetContent(string format)
    {
        var tags = TagHelper.GetTags(format);

        var content = new StringBuilder();

        dgView.SelectedRows.Cast<DataGridViewRow>()
            .Select(a => a.DataBoundItem)
            .Cast<ZupTask>()
            .Where(a => a.Duration != null && a.StartedOn != null)
            .ToList()
            .ForEach(a =>
            {
                var data = format;

                foreach (var tag in tags)
                {
                    var output = TagHelper.RunTag(tag, a);

                    data = data.Replace($"~{tag}~", output);
                }

                content.AppendLine(data);
            });

        return content.ToString();
    }

    private string ExtractComments(Guid taskID)
    {
        var notes = p_DbContext.TaskEntryNotes.Where(a => a.TaskID == taskID).ToList();

        if (!notes.Any())
        {
            return string.Empty;
        }

        var str = new List<string>();

        foreach (var note in notes)
        {
            var n = NoteSummary.CleanString(note.Notes, 100);

            if (!string.IsNullOrWhiteSpace(n))
            {
                str.Add(n);
            }
        }

        if (!str.Any())
        {
            return string.Empty;
        }

        return string.Join(';', str);
    }

    private string ExtractTag(TagHelper.TagKey tagKey, Guid taskID)
    {
        var query = from et in p_DbContext.TaskEntryTags
                    join t in p_DbContext.Tags
                     on et.TagID equals t.ID
                    where et.TaskID == taskID
                    select t;

        var tags = query.ToArray();

        return TagHelper.ExtractValue(tagKey, tags);
    }

    private string ExtractTags(Guid taskID)
    {
        var query = from et in p_DbContext.TaskEntryTags
                    join t in p_DbContext.Tags
                     on et.TagID equals t.ID
                    where et.TaskID == taskID
                    select t.Name;

        var tags = query.ToList();

        return string.Join(";", tags);
    }

    private void txtExtension_TextChanged(object sender, EventArgs e)
    {
        SetLabelOutput();
    }

    private void dtTimesheetDate_ValueChanged(object sender, EventArgs e)
    {
        SetLabelOutput();
    }

    private void txtSearch_TextChanged(object sender, EventArgs e)
    {
        tmrSearch.Enabled = false;
        tmrSearch.Enabled = true;
    }

    private void tmrSearch_Tick(object sender, EventArgs e)
    {
        var currentWeekData = LoadWeekData();

        LoadListData(currentWeekData.Start, currentWeekData.End, txtSearch.Text);
        tmrSearch.Enabled = false;
    }

    private void OpenFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = folderPath,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }
        else
        {
            MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
        }
    }

    private void ttsPath_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ttsPath.Text))
        {
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(ttsPath.Text);

            if (Directory.Exists(dir))
            {
                OpenFolder(dir);
            }
        }
        catch { }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        RefreshList();
    }

    private void lbWeek_SelectedIndexChanged(object sender, EventArgs e)
    {
        var currentWeekData = LoadWeekData();

        LoadListData(currentWeekData.Start, currentWeekData.End, txtSearch.Text);
    }

    private void frmView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
        }
    }

    private void btnRowFormatHelp_Click(object sender, EventArgs e)
    {
        var allowedTags = TagHelper.GetRegisteredTags().ToList();

        allowedTags.Add("Tag[0]");
        allowedTags.Add("Tag[0].Name");
        allowedTags.Add("Tag[0].Description");
        allowedTags.Add("Tag[Name=Bill%].Description");

        MessageBox.Show(string.Join("\n", allowedTags.Select(a => $"~{a}~")), "Available Templates");
    }

    private void DrawTag(string text, Graphics graphics, Rectangle textRect, Font font)
    {
        using (var path = GetPathRoundCorners(textRect, 2))
        using (var fillBrush = new SolidBrush(Color.Gray))
        using (var drawPen = new Pen(Brushes.DarkGray))
        using (var textBrush = new SolidBrush(Color.White))
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.FillPath(fillBrush, path);
            graphics.DrawPath(drawPen, path);

            var textLoc = new Point(textRect.X + 1, textRect.Y + 1);
            graphics.DrawString(text, font, textBrush, textLoc);
        }
    }

    private GraphicsPath GetPathRoundCorners(Rectangle rc, int radius)
    {
        var x = rc.X;
        var y = rc.Y;
        var w = rc.Width;
        var h = rc.Height;

        radius = radius << 1;

        var path = new GraphicsPath();

        if (radius > 0)
        {
            if (radius > h) radius = h;
            if (radius > w) radius = w;

            path.AddArc(x, y, radius, radius, 180, 90);
            path.AddArc(x + w - radius, y, radius, radius, 270, 90);
            path.AddArc(x + w - radius, y + h - radius, radius, radius, 0, 90);
            path.AddArc(x, y + h - radius, radius, radius, 90, 90);
            path.CloseFigure();
        }
        else
        {
            path.AddRectangle(rc);
        }

        return path;
    }

    private void btnExportAll_Click(object sender, EventArgs e)
    {
        var all = p_DbContext.TaskEntries.ToList();

        var json = System.Text.Json.JsonSerializer.Serialize(all, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        if (string.IsNullOrWhiteSpace(txtTimesheetFolder.Text))
        {
            throw new Exception("Timesheet directory is empty!");
        }

        if (!Path.Exists(txtTimesheetFolder.Text))
        {
            throw new Exception("Timesheet directory doesn't exist!");
        }

        var outputPath = Path.Combine(txtTimesheetFolder.Text, "export-all.json");

        File.WriteAllText(outputPath, json);
    }
}
