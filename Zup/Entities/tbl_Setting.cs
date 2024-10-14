using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup.Entities;

public class tbl_Setting
{
    [Key]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string DataType { get; set; } = null!;

    public string Value { get; set; } = null!;
}
