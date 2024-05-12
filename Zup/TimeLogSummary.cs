using System.ComponentModel.DataAnnotations;

namespace Zup;

public class TimeLogSummary
{
    public Guid ID { get; set; }

    [MaxLength(255)]
    public string Task { get; set; } = null!;

    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? DurationString { get; set; }

    public string[] Tags { get; set; } = Array.Empty<string>();
}