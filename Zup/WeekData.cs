using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup;

public class WeekData
{
    public int WeekNumber { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public override string ToString()
    {
        return $"{WeekNumber:00}: {Start:MM/dd/yyyy} - {End:MM/dd/yyyy}";
    }
}
