﻿namespace Zup;

public partial class frmSetting : Form
{
    public delegate void OnSettingUpdated(string name, object value);
    public delegate void OnDbTrim(int daysToKeep);
    public delegate void OnDbBackup();

    public event OnSettingUpdated? OnSettingUpdatedEvent;
    public event OnDbTrim? OnDbTrimEvent;
    public event OnDbBackup? OnDbBackupEvent;

    public frmSetting()
    {
        InitializeComponent();
    }

    private void frmSetting_Load(object sender, EventArgs e)
    {
        var myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var path = Path.Combine(myDoc, "Zup");

        ofdDbFile.InitialDirectory = path;
        ofdDbFile.Filter = "Database Files (*.db)|*.db";

        numTxtItemsToShow.Value = Properties.Settings.Default.ItemsToShow;
        cbAutoOpenUpdateWindow.Checked = Properties.Settings.Default.AutoOpenUpdateWindow;
        lblOpacityVal.Text = $"{Properties.Settings.Default.EntryListOpacity * 100}%";
        tbOpacity.Value = Convert.ToInt32(Properties.Settings.Default.EntryListOpacity * 100);
        txtDbPath.Text = Properties.Settings.Default.DbPath;
        numKeepDaysOfData.Value = Properties.Settings.Default.TrimDaysToKeep;
        nMaxDaysDataToLoad.Value = Properties.Settings.Default.NumDaysOfDataToLoad;


        mtDayStart.Text = $"{Properties.Settings.Default.DayStart.Hours:00}:{Properties.Settings.Default.DayStart.Minutes:00}";
        mtDayEnd.Text = $"{Properties.Settings.Default.DayEnd.Hours:00}:{Properties.Settings.Default.DayEnd.Minutes:00}";
        cbDayEndNextDay.Checked = Properties.Settings.Default.DayEndNextDay;
    }

    private void nMaxDaysDataToLoad_ValueChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.NumDaysOfDataToLoad = Convert.ToInt32(nMaxDaysDataToLoad.Value);
        Properties.Settings.Default.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent(nameof(Properties.Settings.Default.NumDaysOfDataToLoad), Convert.ToInt32(nMaxDaysDataToLoad.Value));
        }
    }

    private void numTxtItemsToShow_ValueChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.ItemsToShow = Convert.ToInt32(numTxtItemsToShow.Value);
        Properties.Settings.Default.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent("ItemsToShow", Convert.ToInt32(numTxtItemsToShow.Value));
        }
    }

    private void cbAutoOpenUpdateWindow_CheckedChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.AutoOpenUpdateWindow = cbAutoOpenUpdateWindow.Checked;
        Properties.Settings.Default.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent("AutoOpenUpdateWindow", cbAutoOpenUpdateWindow.Checked);
        }
    }

    private void tbOpacity_Scroll(object sender, EventArgs e)
    {
        lblOpacityVal.Text = $"{tbOpacity.Value}%";

        var val = Convert.ToDouble(tbOpacity.Value / 100.0);

        Properties.Settings.Default.EntryListOpacity = val;
        Properties.Settings.Default.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent("EntryListOpacity", val);
        }
    }

    private void btnDbBrowse_Click(object sender, EventArgs e)
    {
        var result = ofdDbFile.ShowDialog();

        if (result == DialogResult.OK)
        {
            txtDbPath.Text = ofdDbFile.FileName;
            Properties.Settings.Default.DbPath = ofdDbFile.FileName;
            Properties.Settings.Default.Save();

            if (OnSettingUpdatedEvent != null)
            {
                OnSettingUpdatedEvent("UpdateDbPath", ofdDbFile.FileName);
            }
        }
    }

    private void btnBackupDb_Click(object sender, EventArgs e)
    {
        if (OnDbBackupEvent != null)
        {
            OnDbBackupEvent();
        }
    }

    private void numKeepDaysOfData_ValueChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.TrimDaysToKeep = Convert.ToInt32(numKeepDaysOfData.Value);
        Properties.Settings.Default.Save();

        lblKeepDaysData.Text = $"Keep {numKeepDaysOfData.Value} days of data";
    }

    private void btnTrimDb_Click(object sender, EventArgs e)
    {
        if (OnDbTrimEvent != null)
        {
            OnDbTrimEvent(Convert.ToInt32(numKeepDaysOfData.Value));
        }
    }

    private void frmSetting_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
        }
    }

    private void cbDayEndNextDay_CheckedChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.DayEndNextDay = cbDayEndNextDay.Checked;
        Properties.Settings.Default.Save();

        RecalcDayStartAndEnd();
    }

    private void mtDayStart_TextChanged(object sender, EventArgs e)
    {
        if (TimeSpan.TryParse(mtDayStart.Text, out var dayStart))
        {
            Properties.Settings.Default.DayStart = dayStart;
            Properties.Settings.Default.Save();

            RecalcDayStartAndEnd();
        }
    }

    private void mtDayEnd_TextChanged(object sender, EventArgs e)
    {
        if (TimeSpan.TryParse(mtDayEnd.Text, out var dayEnd))
        {
            Properties.Settings.Default.DayEnd = dayEnd;
            Properties.Settings.Default.Save();

            RecalcDayStartAndEnd();
        }
    }

    private void RecalcDayStartAndEnd()
    {
        var dayShift = Utility.GetDayShift(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

        // 01/01/2024 12:00am
        lblDayStart.Text = $"{dayShift.start:MM/dd/yyyy hh:mmtt}";
        lblDayEnd.Text = $"{dayShift.end:MM/dd/yyyy hh:mmtt}";
    }
}