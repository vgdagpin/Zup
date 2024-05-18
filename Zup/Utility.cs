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