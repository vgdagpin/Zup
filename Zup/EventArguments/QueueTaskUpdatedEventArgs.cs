namespace Zup.EventArguments;

public class QueueTaskUpdatedEventArgs : EventArgs
{
    public QueueTaskUpdatedEventArgs(int queueCount)
    {
        QueueCount = queueCount;
    }

    public int QueueCount { get; }
}
