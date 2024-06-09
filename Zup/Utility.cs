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

    public static DayOfWeek? GetDayOfWeek(DateTime? startedOn)
    {
        if (startedOn == null)
        {
            return null;
        }

        var dt = new DateTime(startedOn.Value.Year, startedOn.Value.Month, startedOn.Value.Day);

        var ds1 = GetDayShift(dt.AddHours(-24));
        var ds2 = GetDayShift(dt);
        var ds3 = GetDayShift(dt.AddHours(24));

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

    public static (DateTime start, DateTime end, DateTime passedDT) GetDayShift(DateTime date)
    {
        TimeSpan tsStart = Properties.Settings.Default.DayStart;
        TimeSpan tsEnd = Properties.Settings.Default.DayEnd;

        var s = date;
        s = s.AddHours(tsStart.Hours - 7);
        s = s.AddMinutes(tsStart.Minutes);

        var e = date;
        e = e.AddHours(tsEnd.Hours + 8);
        e = e.AddMinutes(tsEnd.Minutes);
        e = e.AddSeconds(-1);

        if (Properties.Settings.Default.DayEndNextDay)
        {
            e = e.AddHours(24);
        }

        return (s, e, date);
    }

    public static string DbPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DbPath))
            {
                var myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = Path.Combine(myDoc, "Zup");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Properties.Settings.Default.DbPath = Path.Combine(path, $"Zup.db");

                Properties.Settings.Default.Save();
            }

            var dir = Path.GetDirectoryName(Properties.Settings.Default.DbPath)!;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return Properties.Settings.Default.DbPath;
        }
    }
}