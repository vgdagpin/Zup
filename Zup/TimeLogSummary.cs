using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup;

public class TimeLogSummary
{
    public int ID { get; set; }

    [MaxLength(255)]
    public string Task { get; set; } = null!;


    public string StartedOn { get; set; } = null!;
    public string? EndedOn { get; set; }
    public string? Duration { get; set; }

    public DateTime StartedOnData { get; set; }
    public DateTime? EndedOnData { get; set; }
    public TimeSpan? DurationData { get; set; }
}