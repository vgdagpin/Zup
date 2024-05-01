using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

namespace Zup;

internal static class Program
{
    internal static string DbPath
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
                        connectionString: $"Filename={DbPath}",
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