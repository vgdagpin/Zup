using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

using Zup.Entities;

namespace Zup;

public class ZupDbContext : DbContext
{
    private readonly SettingHelper settingHelper;

    public DbSet<tbl_Setting> Settings { get; set; }
    public DbSet<tbl_Tag> Tags { get; set; }
    public DbSet<tbl_TaskEntryTag> TaskEntryTags { get; set; }
    public DbSet<tbl_TaskEntry> TaskEntries { get; set; }
    public DbSet<tbl_TaskEntryNote> TaskEntryNotes { get; set; }

    public ZupDbContext(DbContextOptions<ZupDbContext> dbContextOptions, SettingHelper settingHelper) : base(dbContextOptions)
    {
        this.settingHelper = settingHelper;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZupDbContext).Assembly);

    public string? BackupDb()
    {
        if (string.IsNullOrWhiteSpace(settingHelper.DbPath))
        {
            return null;
        }

        if (!File.Exists(settingHelper.DbPath))
        {
            return null;
        }

        var dir = Path.Combine(Path.GetDirectoryName(settingHelper.DbPath)!, "Backup");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var newFileName = Path.GetFileNameWithoutExtension(settingHelper.DbPath) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-bak.db";

        var backupPath = Path.Combine(dir, newFileName);

        if (!File.Exists(backupPath))
        {
            File.Copy(settingHelper.DbPath, backupPath);
        }

        return dir;
    }
}

class MigrationContextHelper : IDesignTimeDbContextFactory<ZupDbContext>
{
    public ZupDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<SettingHelper>();

        services.AddDbContext<ZupDbContext>((sp, options) =>
        {
            var settingsHelper = sp.GetRequiredService<SettingHelper>();

            options.UseSqlite
            (
                connectionString: $"Filename={settingsHelper.DbPath}",
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