using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zup.Entities;

namespace Zup
{
    class MigrationContextHelper : IDesignTimeDbContextFactory<ZupDbContext>
    {
        public ZupDbContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<ZupDbContext>(options =>
            {
                options.UseSqlite
                (
                    connectionString: @"Filename=C:\Working Directory\Github\vgdagpin\Zup.db",
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
}
