using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Diagnostics;

using Zup.Entities;

namespace Zup;

public partial class frmUpdateEntry : Form
{
    private readonly ZupDbContext p_DbContext;
    private Guid? selectedEntryID;
    private Guid? selectedNoteID;

    public delegate void OnDelete(Guid entryID);
    public delegate void OnSaved(tbl_TaskEntry log);

    public event OnDelete? OnDeleteEvent;
    public event OnSaved? OnSavedEvent;

    const string DateTimeCustomFormat = "MM/dd/yyyy hh:mm:ss tt";

    public frmUpdateEntry(ZupDbContext dbContext)
    {
        InitializeComponent();
        p_DbContext = dbContext;

        SetControlsEnable(false);
    }

    private void frmUpdateEntry_Load(object sender, EventArgs e)
    {

    }

    private void frmUpdateEntry_FormClosing(object sender, FormClosingEventArgs e)
    {
        SetControlsEnable(false);

        e.Cancel = true;

        Hide();
    }

    public async Task ShowUpdateEntry(Guid entryID)
    {
        Show();

        var entry = await p_DbContext.TaskEntries.FindAsync(entryID);

        selectedEntryID = null;
        selectedNoteID = null;
        lbNotes.Items.Clear();
        lbPreviousNotes.Items.Clear();
        rtbNote.Clear();

        if (entry == null)
        {
            return;
        }

        selectedEntryID = entryID;

        txtTask.Text = entry.Task;

        dtFrom.Value = entry.StartedOn != null
            ? entry.StartedOn.Value
            : dtFrom.MinDate;

        dtFrom.CustomFormat = entry.StartedOn != null
            ? DateTimeCustomFormat
            : " ";


        dtTo.Value = entry.EndedOn != null
            ? entry.EndedOn.Value
            : dtTo.MinDate;

        dtTo.CustomFormat = entry.EndedOn != null
            ? DateTimeCustomFormat
            : " ";

        _ = LoadNotes(entry);
        _ = LoadPreviousNotes(entry);

        tmrFocus.Enabled = true;

        SetControlsEnable(true);
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

        var allIDs = await p_DbContext.TaskEntries
            .Where(a => a.Task == txtTask.Text && a.ID != currentTaskEntry.ID && a.StartedOn < currentTaskEntry.StartedOn)
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

    private void rtbNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyData == (Keys.Control | Keys.S))
        {
            e.IsInputKey = true;

            btnDeleteNote.Visible = false;

            SaveChanges();
        }

        if (e.KeyData == (Keys.Control | Keys.N))
        {
            e.IsInputKey = true;

            selectedNoteID = null;
            rtbNote.Clear();
            btnDeleteNote.Visible = false;
        }
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

    void SaveChanges()
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

        if (!string.IsNullOrWhiteSpace(note.RTF))
        {
            rtbNote.Rtf = note.RTF;
        }
        else
        {
            rtbNote.Text = note.Notes;
        }

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
        e.Graphics.DrawString(item.Summary, e.Font!, textBrush, new PointF(e.Bounds.X + 50, e.Bounds.Y));

        e.DrawFocusRectangle();
    }
    #endregion

    private void btnDelete_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("Delete this entry?", "Zup", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

        if (result == DialogResult.Cancel)
        {
            return;
        }

        if (OnDeleteEvent != null)
        {
            OnDeleteEvent(selectedEntryID!.Value);
        }

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
        SaveChanges();
    }

    private void rtbNote_KeyPress(object sender, KeyPressEventArgs e)
    {
        btnSaveNote.Visible = rtbNote.Text.Trim().Length > 0;
    }

    private void tmrFocus_Tick(object sender, EventArgs e)
    {
        Activate();

        tmrFocus.Enabled = false;

        rtbNote.Focus();
    }

    private void dtTo_ValueChanged(object sender, EventArgs e)
    {
        dtTo.CustomFormat = DateTimeCustomFormat;
    }

    private void btnSaveChanges_Click(object sender, EventArgs e)
    {
        var task = p_DbContext.TaskEntries.Find(selectedEntryID);

        if (task != null)
        {
            task.Task = txtTask.Text;
            task.StartedOn = dtFrom.MinDate == dtFrom.Value ? null : dtFrom.Value;
            task.EndedOn = dtTo.MinDate == dtTo.Value ? null : dtTo.Value;

            p_DbContext.SaveChanges();

            if (OnSavedEvent != null)
            {
                OnSavedEvent(task);
            }
        }
    }

    private void rtbNote_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        Process.Start("explorer.exe", e.LinkText!);
    }

    private void frmUpdateEntry_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
        }
    }
}