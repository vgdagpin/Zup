namespace Zup;

public partial class frmSetting : Form
{
    private readonly SettingHelper settingHelper;

    private frmMain m_FormMain = null!;

    public delegate void OnSettingUpdated(string name, object value);
    public delegate void OnDbBackup();

    public event OnSettingUpdated? OnSettingUpdatedEvent;
    public event OnDbBackup? OnDbBackupEvent;

    public frmSetting(SettingHelper settingHelper)
    {
        InitializeComponent();
        this.settingHelper = settingHelper;
    }

    public void SetFormMain(frmMain frmMain)
    {
        m_FormMain = frmMain;
    }

    private void frmSetting_Load(object sender, EventArgs e)
    {
        var myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var path = Path.Combine(myDoc, "Zup");

        ofdDbFile.InitialDirectory = path;
        ofdDbFile.Filter = "Database Files (*.db)|*.db";

        numTxtItemsToShow.Value = settingHelper.ItemsToShow;
        cbAutoOpenUpdateWindow.Checked = settingHelper.AutoOpenUpdateWindow;
        lblOpacityVal.Text = $"{settingHelper.EntryListOpacity * 100}%";
        tbOpacity.Value = Convert.ToInt32(settingHelper.EntryListOpacity * 100);
        txtDbPath.Text = settingHelper.DbPath;
        numKeepDaysOfData.Value = settingHelper.TrimDaysToKeep;
        nMaxDaysDataToLoad.Value = settingHelper.NumDaysOfDataToLoad;


        mtDayStart.Text = $"{settingHelper.DayStart.Hours:00}:{settingHelper.DayStart.Minutes:00}";
        mtDayEnd.Text = $"{settingHelper.DayEnd.Hours:00}:{settingHelper.DayEnd.Minutes:00}";
        cbDayEndNextDay.Checked = settingHelper.DayEndNextDay;
    }

    private void nMaxDaysDataToLoad_ValueChanged(object sender, EventArgs e)
    {
        settingHelper.NumDaysOfDataToLoad = Convert.ToInt32(nMaxDaysDataToLoad.Value);
        settingHelper.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent(nameof(settingHelper.NumDaysOfDataToLoad), Convert.ToInt32(nMaxDaysDataToLoad.Value));
        }
    }

    private void numTxtItemsToShow_ValueChanged(object sender, EventArgs e)
    {
        settingHelper.ItemsToShow = Convert.ToInt32(numTxtItemsToShow.Value);
        settingHelper.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent("ItemsToShow", Convert.ToInt32(numTxtItemsToShow.Value));
        }
    }

    private void cbAutoOpenUpdateWindow_CheckedChanged(object sender, EventArgs e)
    {
        settingHelper.AutoOpenUpdateWindow = cbAutoOpenUpdateWindow.Checked;
        settingHelper.Save();

        if (OnSettingUpdatedEvent != null)
        {
            OnSettingUpdatedEvent("AutoOpenUpdateWindow", cbAutoOpenUpdateWindow.Checked);
        }
    }

    private void tbOpacity_Scroll(object sender, EventArgs e)
    {
        lblOpacityVal.Text = $"{tbOpacity.Value}%";

        var val = Convert.ToDouble(tbOpacity.Value / 100.0);

        settingHelper.EntryListOpacity = val;
        settingHelper.Save();

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
            settingHelper.DbPath = ofdDbFile.FileName;
            settingHelper.Save();

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
        settingHelper.TrimDaysToKeep = Convert.ToInt32(numKeepDaysOfData.Value);
        settingHelper.Save();

        lblKeepDaysData.Text = $"Keep {numKeepDaysOfData.Value} days of data";
    }

    private void btnTrimDb_Click(object sender, EventArgs e)
    {
        m_FormMain.TrimDb(Convert.ToInt32(numKeepDaysOfData.Value));
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
        settingHelper.DayEndNextDay = cbDayEndNextDay.Checked;
        settingHelper.Save();

        RecalcDayStartAndEnd();
    }

    private void mtDayStart_TextChanged(object sender, EventArgs e)
    {
        if (TimeSpan.TryParse(mtDayStart.Text, out var dayStart))
        {
            settingHelper.DayStart = dayStart;
            settingHelper.Save();

            RecalcDayStartAndEnd();
        }
    }

    private void mtDayEnd_TextChanged(object sender, EventArgs e)
    {
        if (TimeSpan.TryParse(mtDayEnd.Text, out var dayEnd))
        {
            settingHelper.DayEnd = dayEnd;
            settingHelper.Save();

            RecalcDayStartAndEnd();
        }
    }

    private void RecalcDayStartAndEnd()
    {
        var dayShift = Utility.GetDayShift(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), settingHelper.DayStart, settingHelper.DayEnd);

        // 01/01/2024 12:00am
        lblDayStart.Text = $"{dayShift.start:MM/dd/yyyy hh:mmtt}";
        lblDayEnd.Text = $"{dayShift.end:MM/dd/yyyy hh:mmtt}";
    }
}