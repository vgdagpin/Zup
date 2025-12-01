using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.EntityFrameworkCore;

using Zup.CustomControls;
using Zup.Entities;
using Zup.EventArguments;

namespace Zup;

public partial class frmUpdateEntry : Form
{
    private frmMain m_FormMain = null!;

    private readonly ZupDbContext p_DbContext;
    private Guid? selectedEntryID;
    private Guid? selectedNoteID;


    public event EventHandler<SaveEventArgs>? OnSavedEvent;
    public event EventHandler<TokenEventArgs>? OnTokenDoubleClicked;
    public event EventHandler<NewEntryEventArgs>? OnReRunEvent;

    const string DateTimeCustomFormat = "MM/dd/yyyy hh:mm:ss tt";

    private tbl_TaskEntry? selectedEntry;

    private bool startTracking = false;

    private bool isEntryModified = false;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool IsEntryModified
    {
        get
        {
            return isEntryModified;
        }
        set
        {
            if (!startTracking)
            {
                return;
            }

            if (isEntryModified == value)
            {
                return;
            }

            isEntryModified = value;

            if (isEntryModified)
            {
                Text = "Update Entry ●";
            }
            else
            {
                Text = "Update Entry";
            }
        }
    }

    private bool isNotesModified = false;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool IsNotesModified
    {
        get
        {
            return isNotesModified;
        }
        set
        {
            if (!startTracking)
            {
                return;
            }

            if (isNotesModified == value)
            {
                return;
            }

            isNotesModified = value;

            if (isNotesModified)
            {
                Text = "Update Entry ●";
            }
            else
            {
                Text = "Update Entry";
            }
        }
    }

    public frmUpdateEntry(ZupDbContext dbContext)
    {
        InitializeComponent();
        p_DbContext = dbContext;

        SetControlsEnable(false);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public DateTime? StartedOn
    {
        get
        {
            return dtFrom.MinDate == dtFrom.Value ? null : dtFrom.Value;
        }
        set
        {
            if (value == null)
            {
                dtFrom.Value = dtFrom.MinDate;
                dtFrom.CustomFormat = " ";
            }
            else
            {
                dtFrom.Value = value.Value;
                dtFrom.CustomFormat = DateTimeCustomFormat;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public DateTime? EndedOn
    {
        get
        {
            return dtTo.MinDate == dtTo.Value ? null : dtTo.Value;
        }
        set
        {
            if (value == null)
            {
                dtTo.Value = dtTo.MinDate;
                dtTo.CustomFormat = " ";
            }
            else
            {
                dtTo.Value = value.Value;
                dtTo.CustomFormat = DateTimeCustomFormat;
            }
        }
    }

    #region Save
    private void SaveEntry()
    {
        var task = p_DbContext.TaskEntries.Find(selectedEntryID);

        if (task != null)
        {
            task.Task = txtTask.Text;
            task.StartedOn = StartedOn;
            task.EndedOn = EndedOn;

            if (numRank.Value <= 0)
            {
                task.Rank = null;
            }
            else
            {
                task.Rank = (byte)numRank.Value;
            }

            SaveTags(task.ID, tokenBoxTags.Tokens.ToArray());

            p_DbContext.SaveChanges();

            IsEntryModified = false;

            if (OnSavedEvent != null)
            {
                OnSavedEvent(this, new SaveEventArgs(task));
            }
        }
    }

    private void SaveTags(Guid taskID, string[] tags)
    {
        if (tags == null)
        {
            return;
        }

        var allTagsNameIDDictionary = p_DbContext.Tags.Where(a => tags.Contains(a.Name))
            .ToList()
            .ToDictionary(a => a.Name, a => a.ID);

        var query = from tet in p_DbContext.TaskEntryTags
                    join t in p_DbContext.Tags
                        on tet.TagID equals t.ID
                    where tet.TaskID == taskID
                    orderby tet.CreatedOn
                    select new
                    {
                        t.ID,
                        t.Name
                    };

        var existing = query.ToArray();


        #region Tags to remove
        var tagIDsToRemove = new List<Guid>();

        foreach (var item in existing)
        {
            if (!tags.Contains(item.Name))
            {
                tagIDsToRemove.Add(item.ID);
            }
        }

        if (tagIDsToRemove.Any())
        {
            var tagEToRem = p_DbContext.TaskEntryTags.Where(a => a.TaskID == taskID && tagIDsToRemove.Contains(a.TagID))
            .ToList();

            p_DbContext.TaskEntryTags.RemoveRange(tagEToRem);
        }
        #endregion

        #region Tags to add
        var tagNamesToAdd = new List<string>();

        foreach (var newTag in tags)
        {
            if (!existing.Any(a => a.Name == newTag))
            {
                tagNamesToAdd.Add(newTag);
            }
        }


        foreach (var tag in tagNamesToAdd.Distinct())
        {
            if (allTagsNameIDDictionary.ContainsKey(tag))
            {
                p_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                {
                    TagID = allTagsNameIDDictionary[tag],
                    TaskID = taskID,
                    CreatedOn = DateTime.Now
                });
            }
            else
            {
                var newTag = new tbl_Tag
                {
                    ID = Guid.NewGuid(),
                    Name = tag
                };

                p_DbContext.Tags.Add(newTag);

                p_DbContext.TaskEntryTags.Add(new tbl_TaskEntryTag
                {
                    TagID = newTag.ID,
                    TaskID = taskID,
                    CreatedOn = DateTime.Now
                });
            }
        }
        #endregion
    }

    private void SaveNotes()
    {
        var noteData = GetNoteData();
        var rtfNoteData = GetRichTextNote();

        // note entry selected?
        // see if saving note as empty; if empty then delete note
        if (selectedNoteID != null)
        {
            var lll = lbNotes.Items.Cast<NoteSummary>().Single(a => a.ID == selectedNoteID);
            var existingNote = p_DbContext.TaskEntryNotes.Find(selectedNoteID);

            if (existingNote != null)
            {
                if (string.IsNullOrWhiteSpace(noteData))
                {
                    DeleteNote();
                }
                else
                {
                    existingNote.Notes = noteData;
                    existingNote.UpdatedOn = DateTime.Now;
                    existingNote.RTF = rtfNoteData!;

                    lll.Summary = NoteSummary.Parse(existingNote).Summary;
                    lll.Note = noteData;

                    p_DbContext.SaveChanges();

                    lbNotes.Refresh();
                }
            }
        }
        // else if notes not empty then create new one
        else
        {
            if (!string.IsNullOrWhiteSpace(noteData))
            {
                var newNote = new tbl_TaskEntryNote
                {
                    ID = Guid.NewGuid(),
                    TaskID = selectedEntryID!.Value,
                    Notes = noteData,
                    RTF = rtfNoteData!,
                    CreatedOn = DateTime.Now
                };

                p_DbContext.TaskEntryNotes.Add(newNote);

                p_DbContext.SaveChanges();

                lbNotes.Items.Add(NoteSummary.Parse(newNote));
            }
        }

        rtbNote.Clear();

        selectedNoteID = null;
        btnDeleteNote.Visible = false;
        IsNotesModified = false;
    }
    #endregion

    private void frmUpdateEntry_Load(object sender, EventArgs e)
    {

    }

    private void frmUpdateEntry_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
        }
        else if (e.KeyCode == Keys.T && e.Alt)
        {
            e.SuppressKeyPress = true;
            tokenBoxTags.Focus();
        }
        else if (e.KeyCode == Keys.N && e.Alt)
        {
            e.SuppressKeyPress = true;
            rtbNote.Focus();
        }
        else if (e.KeyCode == Keys.S && e.Alt)
        {
            e.SuppressKeyPress = true;
            dtFrom.Focus();

        }
        else if (e.KeyCode == Keys.E && e.Alt)
        {
            e.SuppressKeyPress = true;
            dtTo.Focus();

        }
        else if (e.KeyCode == Keys.A && e.Alt)
        {
            e.SuppressKeyPress = true;
            txtTask.Focus();
        }

        if (e.KeyData == (Keys.Control | Keys.S))
        {
            e.SuppressKeyPress = true;

            if (ActiveControl == splitContainer1)
            {
                btnDeleteNote.Visible = false;

                SaveNotes();
            }
            else
            {
                SaveEntry();
            }
        }

        if (e.KeyData == (Keys.Control | Keys.N))
        {
            e.SuppressKeyPress = true;

            selectedNoteID = null;
            rtbNote.Clear();
            btnDeleteNote.Visible = false;
            rtbNote.Focus();

            IsNotesModified = false;
        }
    }

    private void frmUpdateEntry_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        if (IsEntryModified || IsNotesModified)
        {
            var saveChanges = MessageBox.Show("Data was modified, save changes?", "Save Changes", MessageBoxButtons.YesNoCancel);

            if (saveChanges == DialogResult.Yes)
            {
                SaveEntry();
                SaveNotes();
            }
            else if (saveChanges == DialogResult.No)
            {
                SetControlsEnable(false);

                Hide();
            }
        }
        else
        {
            SetControlsEnable(false);

            Hide();
        }
    }

    public void ShowUpdateEntry(ITask eachEntry, bool canReRun = false)
    {
        Text = "Update Entry";
        startTracking = false;
        isEntryModified = false;
        isNotesModified = false;

        btnRerun.Visible = canReRun;
        btnRemind.Visible = eachEntry.TaskStatus == TaskStatus.Queued || eachEntry.TaskStatus == TaskStatus.Ranked;

        Show();

        selectedEntry = p_DbContext.TaskEntries.Find(eachEntry.EntryID);

        selectedEntryID = null;
        selectedNoteID = null;
        lbNotes.Items.Clear();
        lbPreviousNotes.Items.Clear();
        rtbNote.Clear();

        if (selectedEntry == null)
        {
            return;
        }

        selectedEntryID = eachEntry.EntryID;

        txtTask.Text = selectedEntry.Task;

        StartedOn = selectedEntry.StartedOn;
        EndedOn = selectedEntry.EndedOn;

        numRank.Value = selectedEntry.Rank ?? 0;

        _ = LoadTags(selectedEntry);
        _ = LoadNotes(selectedEntry);
        _ = LoadPreviousNotes(selectedEntry);

        /// <see cref="tmrFocus_Tick"/>
        tmrFocus.Enabled = true;

        SetControlsEnable(true);

        startTracking = true;
    }

    private void SetControlsEnable(bool value)
    {
        txtTask.Enabled = value;
        dtFrom.Enabled = value;
        dtTo.Enabled = value;
        btnDelete.Enabled = value;
        btnSaveChanges.Enabled = value;
        rtbNote.Enabled = value;

        if (!value)
        {
            lbNotes.BackColor = Color.LightGray;
            lbPreviousNotes.BackColor = Color.LightGray;
        }
    }

    private async Task LoadTags(tbl_TaskEntry currentTaskEntry)
    {
        tokenBoxTags.RemoveAllTokens();
        tokenBoxTags.AutoCompleteList.Clear();

        var query = from tet in p_DbContext.TaskEntryTags
                    join t in p_DbContext.Tags
                        on tet.TagID equals t.ID
                    where tet.TaskID == currentTaskEntry.ID
                    orderby tet.CreatedOn
                    select t.Name;

        tokenBoxTags.Tokens = await query.ToArrayAsync();

        tokenBoxTags.AutoCompleteList.AddRange(p_DbContext.Tags.Select(a => a.Name));
    }

    private async Task LoadNotes(tbl_TaskEntry currentTaskEntry)
    {
        foreach (var note in await p_DbContext.TaskEntryNotes.Where(a => a.TaskID == currentTaskEntry.ID).ToListAsync())
        {
            lbNotes.Items.Add(NoteSummary.Parse(note));
        }

        lbNotes.BackColor = Color.White;
    }

    private async Task LoadPreviousNotes(tbl_TaskEntry currentTaskEntry)
    {
        if (currentTaskEntry == null)
        {
            return;
        }

        var maxDate = currentTaskEntry.StartedOn ?? DateTime.MaxValue;

        var allIDs = await p_DbContext.TaskEntries
            .Where(a => a.Task == txtTask.Text && a.ID != currentTaskEntry.ID && a.StartedOn < maxDate)
            .OrderByDescending(a => a.StartedOn)
            .Select(a => a.ID)
            .ToArrayAsync();

        foreach (var note in await p_DbContext.TaskEntryNotes
            .Where(a => allIDs.Contains(a.TaskID))
            .OrderByDescending(a => a.CreatedOn)
            .Take(50)
            .AsNoTracking()
            .ToListAsync())
        {
            lbPreviousNotes.Items.Add(NoteSummary.Parse(note));
        }

        lbPreviousNotes.BackColor = Color.White;
    }

    void DeleteNote()
    {
        var confirm = MessageBox.Show("Continue to delete this note?", "Delete Note", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

        if (confirm == DialogResult.Cancel)
        {
            return;
        }

        var lll = lbNotes.Items.Cast<NoteSummary>().Single(a => a.ID == selectedNoteID);
        var existingNote = p_DbContext.TaskEntryNotes.Find(selectedNoteID);

        if (existingNote != null)
        {
            p_DbContext.TaskEntryNotes.Remove(existingNote);

            lbNotes.Items.Remove(lll);

            p_DbContext.SaveChanges();
        }

        btnDeleteNote.Visible = false;
        selectedNoteID = null;
        rtbNote.Clear();
    }

    string GetNoteData()
    {
        if (string.IsNullOrWhiteSpace(rtbNote.Text)
            && !string.IsNullOrWhiteSpace(rtbNote.Rtf)
            && rtbNote.Rtf.Length > 500)
        {
            return NoteSummary.ImageContent;
        }

        return rtbNote.Text;
    }

    string? GetRichTextNote()
    {
        return rtbNote.Rtf;
    }

    #region lbNotes and lvPreviousNotes
    private void lbNotes_SelectedIndexChanged(object sender, EventArgs e)
    {
        var control = (ListBox)sender;

        selectedNoteID = null;

        if (control.SelectedIndex == -1)
        {
            return;
        }

        if (control.Name == lbNotes.Name && lbPreviousNotes.SelectedIndex > -1)
        {
            lbPreviousNotes.ClearSelected();
        }
        else if (control.Name == lbPreviousNotes.Name && lbNotes.SelectedIndex > -1)
        {
            lbNotes.ClearSelected();
        }

        var sel = control.SelectedItem as NoteSummary;

        if (sel == null)
        {
            return;
        }

        var note = p_DbContext.TaskEntryNotes.Find(sel.ID);

        if (note == null)
        {
            return;
        }

        startTracking = false;
        if (!string.IsNullOrWhiteSpace(note.RTF))
        {
            rtbNote.Rtf = note.RTF;
        }
        else
        {
            rtbNote.Text = note.Notes;
        }
        startTracking = true;

        if (control.Name == lbNotes.Name)
        {
            selectedNoteID = note.ID;

            btnDeleteNote.Visible = true;
        }
        else
        {
            btnDeleteNote.Visible = false;
        }
    }

    private void lbNotes_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.Delete)
        {
            DeleteNote();
        }
    }

    private void lbNotes_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index == -1)
        {
            return;
        }

        var control = (ListBox)sender;

        e.DrawBackground();

        var item = (NoteSummary)control.Items[e.Index];

        var createdOnStr = item.CreatedOn.ToString("hh:mm:ss");

        if (control.Name == lbPreviousNotes.Name)
        {
            createdOnStr = item.CreatedOn.ToString("MM/dd/yy");
        }

        var dateBrush = Brushes.Gray;
        var textBrush = Brushes.Black;

        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
        {
            dateBrush = Brushes.LightGray;
            textBrush = Brushes.White;
        }

        e.Graphics.DrawString(createdOnStr, e.Font!, dateBrush, e.Bounds);

        if (control.Name == lbPreviousNotes.Name)
        {
            e.Graphics.DrawString(item.Summary, e.Font!, textBrush, new PointF(e.Bounds.X + 60, e.Bounds.Y));
        }
        else
        {
            e.Graphics.DrawString(item.Summary, e.Font!, textBrush, new PointF(e.Bounds.X + 50, e.Bounds.Y));
        }

        e.DrawFocusRectangle();
    }
    #endregion

    public void SetFormMain(frmMain frmMain)
    {
        m_FormMain = frmMain;
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("Delete this entry?", "Zup", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

        if (result == DialogResult.Cancel)
        {
            return;
        }

        m_FormMain.DeleteEntry(selectedEntryID!.Value);

        Close();
    }

    private void btnDeleteNote_Click(object sender, EventArgs e)
    {
        DeleteNote();
    }

    private void btnNewNote_Click(object sender, EventArgs e)
    {
        selectedNoteID = null;
        rtbNote.Clear();
        lbNotes.SelectedIndex = -1;
        lbPreviousNotes.SelectedIndex = -1;
    }

    private void btnSaveNote_Click(object sender, EventArgs e)
    {
        SaveNotes();
    }

    #region rtbNote Events
    private void rtbNote_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (rtbNote.Text.Trim().Length > 0)
        {
            btnSaveNote.Visible = true;
            IsNotesModified = true;
        }
        else
        {
            btnSaveNote.Visible = false;
            IsNotesModified = false;
        }
    }

    private void rtbNote_KeyDown(object sender, KeyEventArgs e)
    {
        var rtb = (RichTextBox)sender;

        if (e.Control)
        {
            if (e.KeyCode == Keys.B)
            {
                if (rtb.SelectionFont != null)
                {
                    var currentFont = rtb.SelectionFont;

                    var newFontStyle = rtb.SelectionFont.Bold
                        ? currentFont.Style & ~FontStyle.Bold
                        : currentFont.Style | FontStyle.Bold;

                    rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.K)
            {
                var editLink = new frmEditHyperLink(rtbNote.SelectedText, rtbNote.SelectedRtf);

                editLink.SaveChanges += (s, e) =>
                {
                    try
                    {
                        rtbNote.SelectedRtf = e;
                    }
                    catch { }
                };

                editLink.Show();

                e.SuppressKeyPress = true;
            }
        }
    }

    private void rtbNote_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        OpenUrl(e.LinkText!);
    }
    #endregion

    private void tmrFocus_Tick(object sender, EventArgs e)
    {
        Activate();

        tmrFocus.Enabled = false;

        rtbNote.Focus();
    }

    private void dtTo_ValueChanged(object sender, EventArgs e)
    {
        dtTo.CustomFormat = DateTimeCustomFormat;

        IsEntryModified = true;
    }

    private void btnSaveChanges_Click(object sender, EventArgs e)
    {
        SaveEntry();
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    private void tokenBoxTags_TokenChanged(object sender, EventArgs e)
    {
        IsEntryModified = true;
    }

    private void txtTask_TextChanged(object sender, EventArgs e)
    {
        IsEntryModified = true;
    }

    private void dtFrom_ValueChanged(object sender, EventArgs e)
    {
        IsEntryModified = true;
    }

    private void tokenBoxTags_TokenDoubleClicked(object sender, TokenEventArgs e)
    {
        OnTokenDoubleClicked?.Invoke(sender, e);
    }

    private void numRank_ValueChanged(object sender, EventArgs e)
    {
        IsEntryModified = true;
    }

    private void btnRerun_Click(object sender, EventArgs e)
    {
        if (OnReRunEvent != null)
        {
            OnReRunEvent(this, new NewEntryEventArgs(txtTask.Text)
            {
                StopOtherTask = !ModifierKeys.HasFlag(Keys.Shift),
                StartNow = true,
                ParentEntryID = selectedEntryID,
                BringTags = true
            });
        }
    }

    private void btnRemind_Click(object sender, EventArgs e)
    {
        m_FormMain.Notify("Reminder! lol");
    }
}