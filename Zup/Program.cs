using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

namespace Zup;

internal static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => 
            {
                services.AddTransient<frmMain>();
                services.AddTransient<frmEntryList>();
                services.AddTransient<frmSetting>();
                services.AddTransient<frmView>();
                services.AddTransient<frmNewEntry>();
                services.AddTransient<frmUpdateEntry>();

                services.AddDbContext<ZupDbContext>(optionsAction =>
                {
                    optionsAction.UseSqlite
                    (
                        connectionString: $"Filename={Utility.DbPath}",
                        sqliteOptionsAction: opt =>
                        {
                            opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                            opt.MigrationsHistoryTable("tbl_MigrationHistory");
                        }
                    );
                });
            });
    }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var host = CreateHostBuilder().Build();
        ServiceProvider = host.Services;

        Application.Run(ServiceProvider.GetRequiredService<frmMain>());
    }
}