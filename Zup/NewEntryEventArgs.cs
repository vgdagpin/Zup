namespace Zup;

public class NewEntryEventArgs : EventArgs
{
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
    public bool BringTags { get; set; }

    public bool GetTags { get; set; }
}