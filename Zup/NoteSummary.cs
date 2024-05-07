using Zup.Entities;

namespace Zup;

public class NoteSummary
{
    public const string ImageContent = "Image";

    public Guid ID { get; set; }
    public DateTime CreatedOn { get; set; }

    public string Summary { get; set; } = null!;

    public string Note { get; set; } = null!;

    public static NoteSummary Parse(tbl_TaskEntryNote newNote)
    {
        var cleanNote = CleanNotes(newNote.Notes, 30);

        return new NoteSummary
        {
            ID = newNote.ID,
            CreatedOn = newNote.CreatedOn,
            Note = newNote.Notes,
            Summary = cleanNote
        };
    }

    public static string CleanNotes(string notes, int elipsCharCount)
    {
        if (notes == null)
        {
            return string.Empty;
        }

        if (notes == ImageContent)
        {
            return string.Empty;
        }

        notes = notes.Replace('^', ' ');
        notes = notes.Replace(Environment.NewLine, " ");
        notes = notes.Replace("\n\r", " ");
        notes = notes.Replace("\n", " ");
        notes = notes.Replace("\r", " ");

        if (notes.Length > elipsCharCount)
        {
            notes = notes.Substring(0, elipsCharCount) + "..";
        }

        return notes;
    }
}