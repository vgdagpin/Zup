namespace Zup.EventArguments;

public class NewEntryEventArgs : EventArgs
{
    public ITask Task { get; set; } = null!;

    public NewEntryEventArgs(string entry)
    {
        Entry = entry;
    }

    public string Entry { get; set; }
    public bool StopOtherTask { get; set; }
    public bool StartNow { get; set; }
    public Guid? ParentEntryID { get; set; }
    public bool HideParent { get; set; }
    public bool BringNotes { get; set; }


    /// <summary>
    /// Bring tags from parent, <see cref="ParentEntryID"/> should not be null
    /// </summary>
    public bool BringTags { get; set; }

    /// <summary>
    /// Get all tags from previous entries
    /// </summary>
    public bool GetTags { get; set; }

}