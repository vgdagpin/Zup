using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using System.Globalization;
using System.Reflection;

using Zup.Entities;

namespace Zup;

public class ZupDbContext : DbContext
{
    public int WeekNumber { get; private set; }

    public DbSet<tbl_TimeLog> TimeLogs { get; set; }
    public DbSet<tbl_Note> Notes { get; set; }

    public ZupDbContext(DbContextOptions<ZupDbContext> dbContextOptions) : base(dbContextOptions)
    {
        WeekNumber = GetWeekNumber(DateTime.Now);
    }

    public static int GetWeekNumber(DateTime date)
    {
        CultureInfo ci = CultureInfo.CurrentCulture;
        Calendar cal = ci.Calendar;
        CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
        DayOfWeek dow = ci.DateTimeFormat.FirstDayOfWeek;

        return cal.GetWeekOfYear(date, cwr, dow);
    }

    public bool IsThisWeekDb() => GetWeekNumber(DateTime.Now) == WeekNumber;

    public string? BackupDb()
    {
        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DbPath))
        {
            return null;
        }

        if (!File.Exists(Properties.Settings.Default.DbPath))
        {
            return null;
        }

        var dir = Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.DbPath)!, "Backup");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var newFileName = Path.GetFileNameWithoutExtension(Properties.Settings.Default.DbPath) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-bak.db";

        var backupPath = Path.Combine(dir, newFileName);

        File.Copy(Properties.Settings.Default.DbPath, backupPath);

        return dir;
    }
}

class MigrationContextHelper : IDesignTimeDbContextFactory<ZupDbContext>
{
    public ZupDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();

        services.AddDbContext<ZupDbContext>(options =>
        {
            options.UseSqlite
            (
                connectionString: $"Filename={Program.DbPath}",
                sqliteOptionsAction: opt =>
                {
                    opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    opt.MigrationsHistoryTable("tbl_MigrationHistory");
                }
            );
        });

        return services.BuildServiceProvider().GetRequiredService<ZupDbContext>();
    }
}