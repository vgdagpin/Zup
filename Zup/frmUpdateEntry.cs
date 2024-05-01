using System.Data;
using System.Windows.Forms;

using Zup.Entities;

namespace Zup;

public partial class frmUpdateEntry : Form
{
    private readonly ZupDbContext p_DbContext;
    private int? selectedEntryID;
    private int? selectedNoteID;

    public delegate void OnDelete(int entryID);
    public delegate void OnSaved(tbl_TimeLog log);

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

    public void ShowUpdateEntry(int entryID)
    {
        var entry = p_DbContext.TimeLogs.Find(entryID);

        selectedEntryID = null;
        selectedNoteID = null;
        lbNotes.Items.Clear();
        rtbNote.Clear();

        if (entry == null)
        {
            return;
        }

        selectedEntryID = entryID;

        txtTask.Text = entry.Task;
        dtFrom.Value = entry.StartedOn;


        if (entry.EndedOn != null)
        {
            dtTo.Value = entry.EndedOn.Value;
            dtTo.CustomFormat = DateTimeCustomFormat;
        }
        else
        {
            dtTo.CustomFormat = " ";
        }

        foreach (var note in p_DbContext.Notes.Where(a => a.LogID == entryID).ToList())
        {
            lbNotes.Items.Add(NoteSummary.Parse(note));
        }

        tmrFocus.Enabled = true;

        Show();
    }

    private void rtbNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyData == (Keys.Control | Keys.S))
        {
            e.IsInputKey = true;

            SaveChanges();
        }

        if (e.KeyData == (Keys.Control | Keys.N))
        {
            e.IsInputKey = true;

            selectedNoteID = null;
            rtbNote.Clear();
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
        var existingNote = p_DbContext.Notes.Find(selectedNoteID);

        if (existingNote != null)
        {
            p_DbContext.Notes.Remove(existingNote);

            lbNotes.Items.Remove(lll);

            p_DbContext.SaveChanges();
        }

        btnDeleteNote.Enabled = false;
        selectedNoteID = null;
        rtbNote.Clear();
    }

    string GetNoteData()
    {
        if (string.IsNullOrWhiteSpace(rtbNote.Text) 
            && !string.IsNullOrWhiteSpace(rtbNote.Rtf) 
            && rtbNote.Rtf.Length > 500)
        {
            return "Image";
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
            var existingNote = p_DbContext.Notes.Find(selectedNoteID);

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
                var newNote = new tbl_Note
                {
                    LogID = selectedEntryID!.Value,
                    Notes = noteData,
                    RTF = rtfNoteData!,
                    CreatedOn = DateTime.Now
                };

                p_DbContext.Notes.Add(newNote);

                p_DbContext.SaveChanges();

                lbNotes.Items.Add(NoteSummary.Parse(newNote));                
            }
        }

        rtbNote.Clear();

        selectedNoteID = null;
        btnDeleteNote.Enabled = false;
    }

    private void lbNotes_SelectedIndexChanged(object sender, EventArgs e)
    {
        var sel = lbNotes.SelectedItem as NoteSummary;

        if (sel == null)
        {
            return;
        }

        var note = p_DbContext.Notes.Find(sel.ID);

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

        selectedNoteID = note.ID;

        btnDeleteNote.Enabled = true;
    }

    private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        DeleteEntry();
    }

    private void DeleteEntry()
    {
        var result = MessageBox.Show(Text, "Delete Entry", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

        if (result == DialogResult.Cancel)
        {
            return;
        }

        var entry = p_DbContext.TimeLogs.Find(selectedEntryID);

        if (entry != null)
        {
            p_DbContext.TimeLogs.Remove(entry);
            p_DbContext.SaveChanges();
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

    private void lbNotes_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.Delete)
        {
            DeleteNote();
        }
    }

    private void btnNewNote_Click(object sender, EventArgs e)
    {
        selectedNoteID = null;
        rtbNote.Clear();
    }

    private void btnSaveNote_Click(object sender, EventArgs e)
    {
        SaveChanges();
    }

    private void rtbNote_KeyPress(object sender, KeyPressEventArgs e)
    {
        btnSaveNote.Enabled = rtbNote.Text.Trim().Length > 0;
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
        else if (e.KeyCode == Keys.Delete)
        {
            DeleteEntry();
        }
    }

    private void dtTo_ValueChanged(object sender, EventArgs e)
    {
        dtTo.CustomFormat = DateTimeCustomFormat;
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var task = p_DbContext.TimeLogs.Find(selectedEntryID);

        if (task != null)
        {
            task.Task = txtTask.Text;
            task.StartedOn = dtFrom.Value;
            task.EndedOn = dtTo.Value;

            p_DbContext.SaveChanges();

            if (OnSavedEvent != null)
            {
                OnSavedEvent(task);
            }
        }
    }

    private void lbNotes_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index == -1)
        {
            return;
        }

        e.DrawBackground();

        var item = (NoteSummary)lbNotes.Items[e.Index];

        var bold = new Font(e.Font!.FontFamily, e.Font.Size, FontStyle.Bold);

        e.Graphics.DrawString(item.CreatedOn.ToString("hh:mm:ss"), bold, Brushes.Black, e.Bounds);
        e.Graphics.DrawString(item.Summary, e.Font, Brushes.Black, new PointF(e.Bounds.X + 55, e.Bounds.Y));

        e.DrawFocusRectangle();
    }

    private void txtTask_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            e.SuppressKeyPress = true;
            Close();
        }
    }
}