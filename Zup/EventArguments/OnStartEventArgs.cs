namespace Zup.EventArguments;

public class OnStartEventArgs : EventArgs
{
    public TaskStatus PreviousStatus { get; set; }
}
