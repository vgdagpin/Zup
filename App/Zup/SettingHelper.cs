using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Zup.Entities;

namespace Zup;

public class SettingHelper
{
    private readonly IServiceProvider services;
    private ZupDbContext pDbContext = null!;

    protected ZupDbContext dbContext
    {
        get
        {
            if (pDbContext == null)
            {
                pDbContext = services.GetRequiredService<ZupDbContext>();
            }

            return pDbContext;
        }
    }

    T GetSetting<T>(string name, T defautValue)
    {
        var setting = dbContext.Settings.FirstOrDefault(s => s.Name == name);

        if (setting != null)
        {
            return Convert<T>(setting.Value);
        }

        return defautValue;
    }

    void SetSetting<T>(string name, T value)
    {
        var setting = dbContext.Settings.FirstOrDefault(s => s.Name == name);

        if (setting != null)
        {
            setting.Value = value.ToString();
        }
        else
        {
            dbContext.Settings.Add(new tbl_Setting
            {
                Name = name,
                DataType = typeof(T).Name,
                Value = value.ToString()
            });
        }
    }

    T Convert<T>(string input)
    {
        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                // Cast ConvertFromString(string text) : object to (T)
                return (T)converter.ConvertFromString(input);
            }

            return default;
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }

    public bool ShowQueuedTasks
    {
        get => GetSetting(nameof(ShowQueuedTasks), Properties.Settings.Default.ShowQueuedTasks);
        set => SetSetting(nameof(ShowQueuedTasks), value);
    }

    public bool ShowRankedTasks
    {
        get => GetSetting(nameof(ShowRankedTasks), Properties.Settings.Default.ShowRankedTasks);
        set => SetSetting(nameof(ShowRankedTasks), value);
    }

    public bool ShowClosedTasks
    {
        get => GetSetting(nameof(ShowClosedTasks), Properties.Settings.Default.ShowClosedTasks);
        set => SetSetting(nameof(ShowClosedTasks), value);
    }

    public double EntryListOpacity
    {
        get => GetSetting(nameof(EntryListOpacity), Properties.Settings.Default.EntryListOpacity);
        set => SetSetting(nameof(EntryListOpacity), value);
    }

    public int NumDaysOfDataToLoad
    {
        get => GetSetting(nameof(NumDaysOfDataToLoad), Properties.Settings.Default.NumDaysOfDataToLoad);
        set => SetSetting(nameof(NumDaysOfDataToLoad), value);
    }

    public int ItemsToShow
    {
        get => GetSetting(nameof(ItemsToShow), Properties.Settings.Default.ItemsToShow);
        set => SetSetting(nameof(ItemsToShow), value);
    }

    public bool AutoOpenUpdateWindow
    {
        get => GetSetting(nameof(AutoOpenUpdateWindow), Properties.Settings.Default.AutoOpenUpdateWindow);
        set => SetSetting(nameof(AutoOpenUpdateWindow), value);
    }

    public bool UsePillTimer
    {
        get => GetSetting(nameof(UsePillTimer), Properties.Settings.Default.UsePillTimer);
        set => SetSetting(nameof(UsePillTimer), value);
    }

    public TimeSpan DayStart
    {
        get => GetSetting(nameof(DayStart), Properties.Settings.Default.DayStart);
        set => SetSetting(nameof(DayStart), value);
    }

    public TimeSpan DayEnd
    {
        get => GetSetting(nameof(DayEnd), Properties.Settings.Default.DayEnd);
        set => SetSetting(nameof(DayEnd), value);
    }

    public bool DayEndNextDay
    {
        get => GetSetting(nameof(DayEndNextDay), Properties.Settings.Default.DayEndNextDay);
        set => SetSetting(nameof(DayEndNextDay), value);
    }

    public string TimesheetsFolder
    {
        get => GetSetting(nameof(TimesheetsFolder), Properties.Settings.Default.TimesheetsFolder);
        set => SetSetting(nameof(TimesheetsFolder), value);
    }

    public string ExportRowFormat
    {
        get => GetSetting(nameof(ExportRowFormat), Properties.Settings.Default.ExportRowFormat);
        set => SetSetting(nameof(ExportRowFormat), value);
    }

    public string ExportFileExtension
    {
        get => GetSetting(nameof(ExportFileExtension), Properties.Settings.Default.ExportFileExtension);
        set => SetSetting(nameof(ExportFileExtension), value);
    }

    public int TrimDaysToKeep
    {
        get => GetSetting(nameof(TrimDaysToKeep), Properties.Settings.Default.TrimDaysToKeep);
        set => SetSetting(nameof(TrimDaysToKeep), value);
    }

    public int FormLocationX
    {
        get => GetSetting(nameof(FormLocationX), Properties.Settings.Default.FormLocationX);
        set => SetSetting(nameof(FormLocationX), value);
    }

    public int FormLocationY
    {
        get => GetSetting(nameof(FormLocationY), Properties.Settings.Default.FormLocationY);
        set => SetSetting(nameof(FormLocationY), value);
    }

    public string DbPath
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
        set
        {
            Properties.Settings.Default.DbPath = value;
        }
    }       

    public SettingHelper(IServiceProvider services)
    {
        this.services = services;
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }
}
