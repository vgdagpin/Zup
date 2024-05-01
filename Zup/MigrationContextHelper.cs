using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Zup;

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
