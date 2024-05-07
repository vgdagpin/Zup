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
    }

    private void frmUpdateEntry_Load(object sender, EventArgs e)
    {

    }

    private void frmUpdateEntry_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        Hide();
    }

    public void ShowUpdateEntry(Guid entryID)
    {
        var entry = p_DbContext.TaskEntries.Find(entryID);

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


        foreach (var note in p_DbContext.TaskEntryNotes.Where(a => a.TaskID == entryID).ToList())
        {
            lbNotes.Items.Add(NoteSummary.Parse(note));
        }

        LoadPreviousNotes();

        tmrFocus.Enabled = true;

        Show();
    }

    private void LoadPreviousNotes()
    {
        var allIDs = p_DbContext.TaskEntries
            .Where(a => a.Task == txtTask.Text && a.ID != selectedEntryID)
            .OrderByDescending(a => a.StartedOn)
            .Select(a => a.ID)
            .ToArray();

        foreach (var note in p_DbContext.TaskEntryNotes
            .Where(a => allIDs.Contains(a.TaskID))
            .OrderByDescending(a => a.CreatedOn)
            .Take(50)
            .AsNoTracking()
            .ToList())
        {
            lbPreviousNotes.Items.Add(NoteSummary.Parse(note));
        }
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

        e.Graphics.DrawString(createdOnStr, e.Font!, Brushes.Gray, e.Bounds);
        e.Graphics.DrawString(item.Summary, e.Font!, Brushes.Black, new PointF(e.Bounds.X + 50, e.Bounds.Y));

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

    private void rtbNote_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            e.SuppressKeyPress = true;
            Close();
        }
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


    private void txtTask_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            e.SuppressKeyPress = true;
            Close();
        }
    }

    private void rtbNote_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        Process.Start("explorer.exe", e.LinkText!);
    }
}