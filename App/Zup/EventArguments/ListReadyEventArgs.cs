namespace Zup.EventArguments;

public class ListReadyEventArgs : EventArgs
{
    public bool HasItem { get; }

    public ListReadyEventArgs(bool hasItem)
    {
        HasItem = hasItem;
    }
}
