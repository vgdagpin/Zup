using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskStatus = Zup.CustomControls.TaskStatus;

namespace Zup.EventArguments
{
    public class OnStartEventArgs : EventArgs
    {
        public TaskStatus PreviousStatus { get; set; }
    }
}
