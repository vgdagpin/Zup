using Zup.Entities;

namespace Zup.EventArguments;

public class SaveEventArgs : EventArgs
{
    public SaveEventArgs(tbl_TaskEntry task)
    {
        Task = task;
    }

    public tbl_TaskEntry Task { get; }
}
