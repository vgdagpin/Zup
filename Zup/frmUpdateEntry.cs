using System.Data;

using Zup.Entities;

namespace Zup;
public partial class frmUpdateEntry : Form
{
    private readonly ZupDbContext p_DbContext;
    private int? selectedEntryID;
    private int? selectedNoteID;

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
        rtbNote.Focus();


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

        Show();

        Text = entry.Task;

        foreach (var note in p_DbContext.Notes.Where(a => a.LogID == entryID).ToList())
        {
            lbNotes.Items.Add(NoteSummary.Parse(note));
        }
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

    void SaveChanges()
    {
        if (selectedNoteID != null)
        {
            var lll = lbNotes.Items.Cast<NoteSummary>().Single(a => a.ID == selectedNoteID);
            var existingNote = p_DbContext.Notes.Find(selectedNoteID);

            if (existingNote != null)
            {
                if (string.IsNullOrWhiteSpace(rtbNote.Text))
                {
                    p_DbContext.Notes.Remove(existingNote);

                    lbNotes.Items.Remove(lll);
                }
                else
                {
                    existingNote.Notes = rtbNote.Text;
                    existingNote.UpdatedOn = DateTime.Now;

                    lll.Summary = NoteSummary.Parse(existingNote).Summary;
                    lll.Note = rtbNote.Text;
                }

                p_DbContext.SaveChanges();
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(rtbNote.Text))
            {
                var newNote = new tbl_Note
                {
                    LogID = selectedEntryID!.Value,
                    Notes = rtbNote.Text,
                    CreatedOn = DateTime.Now
                };

                p_DbContext.Notes.Add(newNote);

                p_DbContext.SaveChanges();

                lbNotes.Items.Add(NoteSummary.Parse(newNote));

                rtbNote.Clear();
            }
        }
    }

    private void lbNotes_SelectedIndexChanged(object sender, EventArgs e)
    {
        var sel = lbNotes.SelectedItem as NoteSummary;

        if (sel == null)
        {
            return;
        }

        rtbNote.Text = sel.Note;
        selectedNoteID = sel.ID;
    }
}

public class NoteSummary
{
    public int ID { get; set; }
    public DateTime CreatedOn { get; set; }

    public string Summary { get; set; } = null!;

    public string Note { get; set; } = null!;

    public static NoteSummary Parse(tbl_Note newNote)
    {
        const int noteLen = 15;

        return new NoteSummary
        {
            ID = newNote.ID,
            CreatedOn = newNote.CreatedOn,
            Note = newNote.Notes,
            Summary = newNote.Notes.Length > noteLen ? newNote.Notes.Substring(0, noteLen).Trim() + ".." : newNote.Notes
        };
    }
}