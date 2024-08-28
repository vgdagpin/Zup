using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup
{
    public class LoadedListControlDetail
    {
        public int OngoingTasksCount { get; set; }
        public int RankedTasksCount { get; set; }
        public int QueuedTasksCount { get; set; }

        public bool HasItems => OngoingTasksCount > 0 || RankedTasksCount > 0 || QueuedTasksCount > 0;
    }
}
