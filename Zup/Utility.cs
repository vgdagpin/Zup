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
                        End = i.AddDays(7).AddSeconds(-1)
                    };
                    break;
                case DayOfWeek.Monday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-1),
                        End = i.AddDays(6).AddSeconds(-1)
                    };
                    break;
                case DayOfWeek.Tuesday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-2),
                        End = i.AddDays(5).AddSeconds(-1)
                    };
                    break;
                case DayOfWeek.Wednesday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-3),
                        End = i.AddDays(4).AddSeconds(-1)
                    };
                    break;
                case DayOfWeek.Thursday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-4),
                        End = i.AddDays(3).AddSeconds(-1)
                    };
                    break;
                case DayOfWeek.Friday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-5),
                        End = i.AddDays(2).AddSeconds(-1)
                    };
                    break;
                case DayOfWeek.Saturday:
                    data[wn] = new WeekData
                    {
                        WeekNumber = wn,
                        Start = i.AddDays(-6),
                        End = i.AddDays(1).AddSeconds(-1)
                    };
                    break;
            }
        }

        return data.Select(a => a.Value)
            .ToArray();
    }

    public static DayOfWeek? GetDayOfWeek(DateTime? startedOn, TimeSpan dayStart, TimeSpan dayEnd)
    {
        if (startedOn == null)
        {
            return null;
        }

        var dt = new DateTime(startedOn.Value.Year, startedOn.Value.Month, startedOn.Value.Day);

        var ds1 = GetDayShift(dt.AddHours(-24), dayStart, dayEnd);
        var ds2 = GetDayShift(dt, dayStart, dayEnd);
        var ds3 = GetDayShift(dt.AddHours(24), dayStart, dayEnd);

        if (ds1.start <= startedOn.Value && startedOn.Value <= ds1.end)
        {
            return ds1.passedDT.DayOfWeek;
        }
        else if (ds2.start <= startedOn.Value && startedOn.Value <= ds2.end)
        {
            return ds2.passedDT.DayOfWeek;
        }
        else if (ds3.start <= startedOn.Value && startedOn.Value <= ds3.end)
        {
            return ds3.passedDT.DayOfWeek;
        }

        return null;
    }

    public static (DateTime start, DateTime end, DateTime passedDT) GetDayShift(DateTime date, TimeSpan dayStart, TimeSpan dayEnd)
    {
        var s = date;
        s = s.AddHours(dayStart.Hours - 7);
        s = s.AddMinutes(dayStart.Minutes);

        var e = date;
        e = e.AddHours(dayEnd.Hours + 8);
        e = e.AddMinutes(dayEnd.Minutes);
        e = e.AddSeconds(-1);

        e = e.AddHours(24);

        return (s, e, date);
    }
}