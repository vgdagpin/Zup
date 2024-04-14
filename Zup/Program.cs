using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Zup.Entities;

namespace Zup;

internal static class Program
{
    internal static string DbPath
    {
        get
        {
            var myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(myDoc, "Zup");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, "Zup.db");
        }
    }

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

                services.AddDbContext<ZupDbContext>(optionsAction =>
                {
                    optionsAction.UseSqlite
                    (
                        connectionString: $"Filename={DbPath}",
                        sqliteOptionsAction: opt =>
                        {
                            opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                            opt.MigrationsHistoryTable("tbl_MigrationHistory");
                        }
                    );
                }, ServiceLifetime.Singleton, ServiceLifetime.Singleton);
            });
    }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var host = CreateHostBuilder().Build();
        ServiceProvider = host.Services;

        Application.Run(ServiceProvider.GetRequiredService<frmMain>());
    }
}