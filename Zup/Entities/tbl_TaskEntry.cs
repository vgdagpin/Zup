﻿using System.ComponentModel.DataAnnotations;

namespace Zup.Entities;

public class tbl_TaskEntry
{
    public Guid ID { get; set; }

    [MaxLength(255)]
    public string Task { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }

    public byte? Rank { get; set; }
}
