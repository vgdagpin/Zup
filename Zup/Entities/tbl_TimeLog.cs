using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup.Entities;

public class tbl_TimeLog
{
    public int ID { get; set; }
    public string Task { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
}
