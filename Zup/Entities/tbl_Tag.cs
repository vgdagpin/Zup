using System.ComponentModel.DataAnnotations;

namespace Zup.Entities;

public class tbl_Tag
{
    public Guid ID { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = null!;
}