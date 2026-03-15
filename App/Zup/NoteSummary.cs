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
        var cleanNote = CleanString(newNote.Notes, 30, true);

        return new NoteSummary
        {
            ID = newNote.ID,
            CreatedOn = newNote.CreatedOn,
            Note = newNote.Notes,
            Summary = cleanNote
        };
    }

    public static string CleanString(string notes, int elipsCharCount, bool includeImageTag = true)
    {
        if (notes == null)
        {
            return string.Empty;
        }

        if (notes == ImageContent && !includeImageTag)
        {
            return string.Empty;
        }

        notes = notes.Replace('^', ' ');
        notes = notes.Replace(Environment.NewLine, " ");
        notes = notes.Replace("\n\r", " ");
        notes = notes.Replace("\n", " ");
        notes = notes.Replace("\r", " ");
        notes = notes.Replace("'", "\\'");

        if (notes.Length > elipsCharCount)
        {
            notes = notes.Substring(0, elipsCharCount) + "..";
        }

        return notes;
    }
}