namespace Zup;

partial class frmSetting
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetting));
        groupBox1 = new GroupBox();
        nMaxDaysDataToLoad = new NumericUpDown();
        label4 = new Label();
        lblOpacityVal = new Label();
        label2 = new Label();
        tbOpacity = new TrackBar();
        numTxtItemsToShow = new NumericUpDown();
        label1 = new Label();
        groupBox2 = new GroupBox();
        cbAutoOpenUpdateWindow = new CheckBox();
        groupBox3 = new GroupBox();
        lblKeepDaysData = new Label();
        numKeepDaysOfData = new NumericUpDown();
        btnBackupDb = new Button();
        btnDbBrowse = new Button();
        txtDbPath = new TextBox();
        label3 = new Label();
        ofdDbFile = new OpenFileDialog();
        btnTrimDb = new Button();
        groupBox4 = new GroupBox();
        label5 = new Label();
        cbDayEndNextDay = new CheckBox();
        mtDayEnd = new MaskedTextBox();
        mtDayStart = new MaskedTextBox();
        label6 = new Label();
        label7 = new Label();
        lblDayStart = new Label();
        lblDayEnd = new Label();
        groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nMaxDaysDataToLoad).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tbOpacity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numTxtItemsToShow).BeginInit();
        groupBox2.SuspendLayout();
        groupBox3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numKeepDaysOfData).BeginInit();
        groupBox4.SuspendLayout();
        SuspendLayout();
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(nMaxDaysDataToLoad);
        groupBox1.Controls.Add(label4);
        groupBox1.Controls.Add(lblOpacityVal);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(tbOpacity);
        groupBox1.Controls.Add(numTxtItemsToShow);
        groupBox1.Controls.Add(label1);
        groupBox1.Location = new Point(13, 143);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(390, 167);
        groupBox1.TabIndex = 2;
        groupBox1.TabStop = false;
        groupBox1.Text = "Entry List";
        // 
        // nMaxDaysDataToLoad
        // 
        nMaxDaysDataToLoad.Location = new Point(138, 42);
        nMaxDaysDataToLoad.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
        nMaxDaysDataToLoad.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        nMaxDaysDataToLoad.Name = "nMaxDaysDataToLoad";
        nMaxDaysDataToLoad.Size = new Size(40, 23);
        nMaxDaysDataToLoad.TabIndex = 4;
        nMaxDaysDataToLoad.TextAlign = HorizontalAlignment.Right;
        nMaxDaysDataToLoad.Value = new decimal(new int[] { 3, 0, 0, 0 });
        nMaxDaysDataToLoad.ValueChanged += nMaxDaysDataToLoad_ValueChanged;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(6, 44);
        label4.Name = "label4";
        label4.Size = new Size(126, 15);
        label4.TabIndex = 7;
        label4.Text = "Max days data to load:";
        // 
        // lblOpacityVal
        // 
        lblOpacityVal.AutoSize = true;
        lblOpacityVal.Location = new Point(349, 101);
        lblOpacityVal.Name = "lblOpacityVal";
        lblOpacityVal.Size = new Size(22, 15);
        lblOpacityVal.TabIndex = 6;
        lblOpacityVal.Text = "0.9";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(6, 101);
        label2.Name = "label2";
        label2.Size = new Size(51, 15);
        label2.TabIndex = 5;
        label2.Text = "Opacity:";
        // 
        // tbOpacity
        // 
        tbOpacity.LargeChange = 10;
        tbOpacity.Location = new Point(96, 99);
        tbOpacity.Maximum = 100;
        tbOpacity.Name = "tbOpacity";
        tbOpacity.Size = new Size(247, 45);
        tbOpacity.SmallChange = 5;
        tbOpacity.TabIndex = 6;
        tbOpacity.TickFrequency = 5;
        tbOpacity.Scroll += tbOpacity_Scroll;
        // 
        // numTxtItemsToShow
        // 
        numTxtItemsToShow.Location = new Point(96, 69);
        numTxtItemsToShow.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        numTxtItemsToShow.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        numTxtItemsToShow.Name = "numTxtItemsToShow";
        numTxtItemsToShow.Size = new Size(40, 23);
        numTxtItemsToShow.TabIndex = 5;
        numTxtItemsToShow.TextAlign = HorizontalAlignment.Right;
        numTxtItemsToShow.Value = new decimal(new int[] { 3, 0, 0, 0 });
        numTxtItemsToShow.ValueChanged += numTxtItemsToShow_ValueChanged;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(6, 71);
        label1.Name = "label1";
        label1.Size = new Size(84, 15);
        label1.TabIndex = 2;
        label1.Text = "Items to show:";
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(cbAutoOpenUpdateWindow);
        groupBox2.Location = new Point(12, 449);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new Size(390, 62);
        groupBox2.TabIndex = 3;
        groupBox2.TabStop = false;
        groupBox2.Text = "Update Entry";
        // 
        // cbAutoOpenUpdateWindow
        // 
        cbAutoOpenUpdateWindow.AutoSize = true;
        cbAutoOpenUpdateWindow.Location = new Point(9, 28);
        cbAutoOpenUpdateWindow.Name = "cbAutoOpenUpdateWindow";
        cbAutoOpenUpdateWindow.Size = new Size(239, 19);
        cbAutoOpenUpdateWindow.TabIndex = 7;
        cbAutoOpenUpdateWindow.Text = "Auto open update window on new entry";
        cbAutoOpenUpdateWindow.UseVisualStyleBackColor = true;
        cbAutoOpenUpdateWindow.CheckedChanged += cbAutoOpenUpdateWindow_CheckedChanged;
        // 
        // groupBox3
        // 
        groupBox3.Controls.Add(lblKeepDaysData);
        groupBox3.Controls.Add(numKeepDaysOfData);
        groupBox3.Controls.Add(btnBackupDb);
        groupBox3.Controls.Add(btnDbBrowse);
        groupBox3.Controls.Add(txtDbPath);
        groupBox3.Controls.Add(label3);
        groupBox3.Location = new Point(12, 9);
        groupBox3.Name = "groupBox3";
        groupBox3.Size = new Size(390, 120);
        groupBox3.TabIndex = 1;
        groupBox3.TabStop = false;
        groupBox3.Text = "Database";
        // 
        // lblKeepDaysData
        // 
        lblKeepDaysData.AutoSize = true;
        lblKeepDaysData.Location = new Point(154, 88);
        lblKeepDaysData.Name = "lblKeepDaysData";
        lblKeepDaysData.Size = new Size(115, 15);
        lblKeepDaysData.TabIndex = 5;
        lblKeepDaysData.Text = "Keep 14 days of data";
        // 
        // numKeepDaysOfData
        // 
        numKeepDaysOfData.Location = new Point(98, 84);
        numKeepDaysOfData.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
        numKeepDaysOfData.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        numKeepDaysOfData.Name = "numKeepDaysOfData";
        numKeepDaysOfData.Size = new Size(50, 23);
        numKeepDaysOfData.TabIndex = 3;
        numKeepDaysOfData.TextAlign = HorizontalAlignment.Right;
        numKeepDaysOfData.Value = new decimal(new int[] { 14, 0, 0, 0 });
        numKeepDaysOfData.ValueChanged += numKeepDaysOfData_ValueChanged;
        // 
        // btnBackupDb
        // 
        btnBackupDb.Location = new Point(9, 55);
        btnBackupDb.Name = "btnBackupDb";
        btnBackupDb.Size = new Size(75, 23);
        btnBackupDb.TabIndex = 0;
        btnBackupDb.Text = "Backup";
        btnBackupDb.TextAlign = ContentAlignment.MiddleLeft;
        btnBackupDb.UseVisualStyleBackColor = true;
        btnBackupDb.Click += btnBackupDb_Click;
        // 
        // btnDbBrowse
        // 
        btnDbBrowse.Location = new Point(350, 26);
        btnDbBrowse.Name = "btnDbBrowse";
        btnDbBrowse.Size = new Size(34, 23);
        btnDbBrowse.TabIndex = 1;
        btnDbBrowse.Text = "...";
        btnDbBrowse.UseVisualStyleBackColor = true;
        btnDbBrowse.Click += btnDbBrowse_Click;
        // 
        // txtDbPath
        // 
        txtDbPath.Location = new Point(48, 26);
        txtDbPath.Name = "txtDbPath";
        txtDbPath.ReadOnly = true;
        txtDbPath.Size = new Size(296, 23);
        txtDbPath.TabIndex = 2;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(9, 29);
        label3.Name = "label3";
        label3.Size = new Size(34, 15);
        label3.TabIndex = 0;
        label3.Text = "Path:";
        // 
        // ofdDbFile
        // 
        ofdDbFile.RestoreDirectory = true;
        // 
        // btnTrimDb
        // 
        btnTrimDb.Location = new Point(22, 93);
        btnTrimDb.Name = "btnTrimDb";
        btnTrimDb.Size = new Size(75, 23);
        btnTrimDb.TabIndex = 2;
        btnTrimDb.TabStop = false;
        btnTrimDb.Text = "Trim";
        btnTrimDb.TextAlign = ContentAlignment.MiddleLeft;
        btnTrimDb.UseVisualStyleBackColor = true;
        btnTrimDb.Click += btnTrimDb_Click;
        // 
        // groupBox4
        // 
        groupBox4.Controls.Add(lblDayEnd);
        groupBox4.Controls.Add(lblDayStart);
        groupBox4.Controls.Add(label7);
        groupBox4.Controls.Add(label6);
        groupBox4.Controls.Add(label5);
        groupBox4.Controls.Add(cbDayEndNextDay);
        groupBox4.Controls.Add(mtDayEnd);
        groupBox4.Controls.Add(mtDayStart);
        groupBox4.Location = new Point(12, 320);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new Size(391, 116);
        groupBox4.TabIndex = 4;
        groupBox4.TabStop = false;
        groupBox4.Text = "Day Start/End (HH:mm)";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(62, 34);
        label5.Name = "label5";
        label5.Size = new Size(18, 15);
        label5.TabIndex = 3;
        label5.Text = "to";
        // 
        // cbDayEndNextDay
        // 
        cbDayEndNextDay.AutoSize = true;
        cbDayEndNextDay.Location = new Point(135, 34);
        cbDayEndNextDay.Name = "cbDayEndNextDay";
        cbDayEndNextDay.Size = new Size(93, 19);
        cbDayEndNextDay.TabIndex = 2;
        cbDayEndNextDay.Text = "The next day";
        cbDayEndNextDay.UseVisualStyleBackColor = true;
        cbDayEndNextDay.CheckedChanged += cbDayEndNextDay_CheckedChanged;
        // 
        // mtDayEnd
        // 
        mtDayEnd.Location = new Point(82, 31);
        mtDayEnd.Mask = "00:00";
        mtDayEnd.Name = "mtDayEnd";
        mtDayEnd.Size = new Size(48, 23);
        mtDayEnd.TabIndex = 1;
        mtDayEnd.TextAlign = HorizontalAlignment.Center;
        mtDayEnd.ValidatingType = typeof(DateTime);
        mtDayEnd.TextChanged += mtDayEnd_TextChanged;
        // 
        // mtDayStart
        // 
        mtDayStart.Location = new Point(10, 31);
        mtDayStart.Mask = "00:00";
        mtDayStart.Name = "mtDayStart";
        mtDayStart.Size = new Size(48, 23);
        mtDayStart.TabIndex = 0;
        mtDayStart.TextAlign = HorizontalAlignment.Center;
        mtDayStart.ValidatingType = typeof(DateTime);
        mtDayStart.TextChanged += mtDayStart_TextChanged;
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(10, 69);
        label6.Name = "label6";
        label6.Size = new Size(57, 15);
        label6.TabIndex = 4;
        label6.Text = "Day Start:";
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Location = new Point(10, 87);
        label7.Name = "label7";
        label7.Size = new Size(53, 15);
        label7.TabIndex = 5;
        label7.Text = "Day End:";
        // 
        // lblDayStart
        // 
        lblDayStart.AutoSize = true;
        lblDayStart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDayStart.Location = new Point(73, 69);
        lblDayStart.Name = "lblDayStart";
        lblDayStart.Size = new Size(124, 15);
        lblDayStart.TabIndex = 6;
        lblDayStart.Text = "01/01/2024 12:00am";
        // 
        // lblDayEnd
        // 
        lblDayEnd.AutoSize = true;
        lblDayEnd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDayEnd.Location = new Point(73, 87);
        lblDayEnd.Name = "lblDayEnd";
        lblDayEnd.Size = new Size(125, 15);
        lblDayEnd.TabIndex = 7;
        lblDayEnd.Text = "01/01/2024 11:59pm";
        // 
        // frmSetting
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(415, 521);
        Controls.Add(groupBox4);
        Controls.Add(btnTrimDb);
        Controls.Add(groupBox3);
        Controls.Add(groupBox2);
        Controls.Add(groupBox1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmSetting";
        SizeGripStyle = SizeGripStyle.Hide;
        Text = "Setting";
        Load += frmSetting_Load;
        KeyDown += frmSetting_KeyDown;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nMaxDaysDataToLoad).EndInit();
        ((System.ComponentModel.ISupportInitialize)tbOpacity).EndInit();
        ((System.ComponentModel.ISupportInitialize)numTxtItemsToShow).EndInit();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        groupBox3.ResumeLayout(false);
        groupBox3.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numKeepDaysOfData).EndInit();
        groupBox4.ResumeLayout(false);
        groupBox4.PerformLayout();
        ResumeLayout(false);
    }

    #endregion
    private GroupBox groupBox1;
    private Label label1;
    private NumericUpDown numTxtItemsToShow;
    private GroupBox groupBox2;
    private CheckBox cbAutoOpenUpdateWindow;
    private TrackBar tbOpacity;
    private Label label2;
    private Label lblOpacityVal;
    private GroupBox groupBox3;
    private Label label3;
    private TextBox txtDbPath;
    private Button btnDbBrowse;
    private OpenFileDialog ofdDbFile;
    private Button btnBackupDb;
    private Button btnTrimDb;
    private Label lblKeepDaysData;
    private NumericUpDown numKeepDaysOfData;
    private NumericUpDown nMaxDaysDataToLoad;
    private Label label4;
    private GroupBox groupBox4;
    private MaskedTextBox mtDayStart;
    private Label label5;
    private CheckBox cbDayEndNextDay;
    private MaskedTextBox mtDayEnd;
    private Label label7;
    private Label label6;
    private Label lblDayStart;
    private Label lblDayEnd;
}