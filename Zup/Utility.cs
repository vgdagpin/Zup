using System.Globalization;

namespace Zup;

public static class Utility
{
    public static int GetWeekNumber(DateTime date)
    {
        var ci = CultureInfo.CurrentCulture;

        return ci.Calendar.GetWeekOfYear(date, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
    }

    public static IEnumerable<WeekData> GetWeekData(int year)
    {
        var data = new Dictionary<int, WeekData>();
        var from = new DateTime(year, 1, 1);
        var to = new DateTime(year, 12, 31);

        for (var i = from; i <= to; i = i.AddDays(1))
        {
            var wn = GetWeekNumber(i);

            if (data.ContainsKey(wn))
            {
                continue;
            }

            switch (i.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i,
                        End = i.AddDays(6)
                    };
                    break;
                case DayOfWeek.Monday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-1),
                        End = i.AddDays(5)
                    };
                    break;
                case DayOfWeek.Tuesday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-2),
                        End = i.AddDays(4)
                    };
                    break;
                case DayOfWeek.Wednesday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-3),
                        End = i.AddDays(3)
                    };
                    break;
                case DayOfWeek.Thursday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-4),
                        End = i.AddDays(2)
                    };
                    break;
                case DayOfWeek.Friday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-5),
                        End = i.AddDays(1)
                    };
                    break;
                case DayOfWeek.Saturday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-6),
                        End = i
                    };
                    break;
            }
        }

        return data.Select(a => a.Value)
            .ToArray();
    }
}
