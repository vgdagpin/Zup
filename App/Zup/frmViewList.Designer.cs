namespace Zup;

partial class frmViewList
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
        components = new System.ComponentModel.Container();
        var dataGridViewCellStyle1 = new DataGridViewCellStyle();
        var dataGridViewCellStyle2 = new DataGridViewCellStyle();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewList));
        dgView = new DataGridView();
        lblSelectedTotal = new Label();
        label1 = new Label();
        fbdTimesheetFolder = new FolderBrowserDialog();
        txtTimesheetFolder = new TextBox();
        btnBrowseTimesheetFolder = new Button();
        btnExportTimesheet = new Button();
        dtTimesheetDate = new DateTimePicker();
        txtExtension = new TextBox();
        label2 = new Label();
        txtSearch = new TextBox();
        tmrSearch = new System.Windows.Forms.Timer(components);
        statusStrip1 = new StatusStrip();
        ttsPath = new ToolStripStatusLabel();
        txtRowFormat = new TextBox();
        label4 = new Label();
        groupBox1 = new GroupBox();
        btnRowFormatHelp = new Button();
        btnRefresh = new Button();
        lbWeek = new ListBox();
        btnExportAll = new Button();
        ID = new DataGridViewTextBoxColumn();
        Task = new DataGridViewTextBoxColumn();
        StartedOn = new DataGridViewTextBoxColumn();
        EndedOn = new DataGridViewTextBoxColumn();
        Duration = new DataGridViewTextBoxColumn();
        DurationString = new DataGridViewTextBoxColumn();
        DayOfWeek = new DataGridViewTextBoxColumn();
        PlayAction = new DataGridViewButtonColumn();
        CreatedOn = new DataGridViewTextBoxColumn();
        Reminder = new DataGridViewTextBoxColumn();
        Rank = new DataGridViewTextBoxColumn();
        IsRunning = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)dgView).BeginInit();
        statusStrip1.SuspendLayout();
        groupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // dgView
        // 
        dgView.AllowUserToAddRows = false;
        dgView.AllowUserToDeleteRows = false;
        dgView.AllowUserToResizeRows = false;
        dgView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgView.Columns.AddRange(new DataGridViewColumn[] { ID, Task, StartedOn, EndedOn, Duration, DurationString, DayOfWeek, PlayAction, CreatedOn, Reminder, Rank, IsRunning });
        dgView.Location = new Point(158, 41);
        dgView.Name = "dgView";
        dgView.ReadOnly = true;
        dgView.RowHeadersVisible = false;
        dgView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgView.Size = new Size(914, 406);
        dgView.TabIndex = 1;
        dgView.CellContentClick += dgView_CellContentClick;
        dgView.CellFormatting += dgView_CellFormatting;
        dgView.CellPainting += dgView_CellPainting;
        dgView.SelectionChanged += dgView_SelectionChanged;
        dgView.DoubleClick += dgView_DoubleClick;
        // 
        // lblSelectedTotal
        // 
        lblSelectedTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblSelectedTotal.Location = new Point(997, 450);
        lblSelectedTotal.Name = "lblSelectedTotal";
        lblSelectedTotal.Size = new Size(75, 23);
        lblSelectedTotal.TabIndex = 1;
        lblSelectedTotal.Text = "00:00:00";
        lblSelectedTotal.TextAlign = ContentAlignment.MiddleRight;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(7, 57);
        label1.Name = "label1";
        label1.Size = new Size(106, 15);
        label1.TabIndex = 2;
        label1.Text = "Timesheets Folder:";
        // 
        // fbdTimesheetFolder
        // 
        fbdTimesheetFolder.RootFolder = Environment.SpecialFolder.MyDocuments;
        // 
        // txtTimesheetFolder
        // 
        txtTimesheetFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtTimesheetFolder.Location = new Point(122, 54);
        txtTimesheetFolder.Name = "txtTimesheetFolder";
        txtTimesheetFolder.Size = new Size(626, 23);
        txtTimesheetFolder.TabIndex = 3;
        // 
        // btnBrowseTimesheetFolder
        // 
        btnBrowseTimesheetFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowseTimesheetFolder.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
        btnBrowseTimesheetFolder.FlatStyle = FlatStyle.Flat;
        btnBrowseTimesheetFolder.Location = new Point(749, 54);
        btnBrowseTimesheetFolder.Name = "btnBrowseTimesheetFolder";
        btnBrowseTimesheetFolder.Size = new Size(28, 23);
        btnBrowseTimesheetFolder.TabIndex = 4;
        btnBrowseTimesheetFolder.Text = "...";
        btnBrowseTimesheetFolder.UseVisualStyleBackColor = true;
        btnBrowseTimesheetFolder.Click += btnBrowseTimesheetFolder_Click;
        // 
        // btnExportTimesheet
        // 
        btnExportTimesheet.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExportTimesheet.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportTimesheet.Location = new Point(950, 54);
        btnExportTimesheet.Name = "btnExportTimesheet";
        btnExportTimesheet.Size = new Size(103, 23);
        btnExportTimesheet.TabIndex = 5;
        btnExportTimesheet.Text = "Export selected";
        btnExportTimesheet.UseVisualStyleBackColor = true;
        btnExportTimesheet.Click += btnExportTimesheet_Click;
        // 
        // dtTimesheetDate
        // 
        dtTimesheetDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        dtTimesheetDate.CustomFormat = "MM-dd-yyyy";
        dtTimesheetDate.Format = DateTimePickerFormat.Custom;
        dtTimesheetDate.Location = new Point(803, 54);
        dtTimesheetDate.Name = "dtTimesheetDate";
        dtTimesheetDate.Size = new Size(96, 23);
        dtTimesheetDate.TabIndex = 6;
        dtTimesheetDate.ValueChanged += dtTimesheetDate_ValueChanged;
        // 
        // txtExtension
        // 
        txtExtension.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        txtExtension.Location = new Point(903, 54);
        txtExtension.Name = "txtExtension";
        txtExtension.Size = new Size(41, 23);
        txtExtension.TabIndex = 7;
        txtExtension.Text = ".fd";
        txtExtension.TextAlign = HorizontalAlignment.Center;
        txtExtension.TextChanged += txtExtension_TextChanged;
        // 
        // label2
        // 
        label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        label2.AutoSize = true;
        label2.Location = new Point(783, 58);
        label2.Name = "label2";
        label2.Size = new Size(12, 15);
        label2.TabIndex = 8;
        label2.Text = "/";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(158, 10);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(533, 23);
        txtSearch.TabIndex = 0;
        txtSearch.TextChanged += txtSearch_TextChanged;
        // 
        // tmrSearch
        // 
        tmrSearch.Interval = 500;
        tmrSearch.Tick += tmrSearch_Tick;
        // 
        // statusStrip1
        // 
        statusStrip1.Items.AddRange(new ToolStripItem[] { ttsPath });
        statusStrip1.Location = new Point(0, 589);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(1084, 22);
        statusStrip1.TabIndex = 11;
        statusStrip1.Text = "statusStrip1";
        // 
        // ttsPath
        // 
        ttsPath.DoubleClickEnabled = true;
        ttsPath.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
        ttsPath.IsLink = true;
        ttsPath.LinkColor = Color.FromArgb(64, 64, 64);
        ttsPath.Name = "ttsPath";
        ttsPath.Size = new Size(31, 17);
        ttsPath.Text = "Path";
        ttsPath.Click += ttsPath_Click;
        // 
        // txtRowFormat
        // 
        txtRowFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtRowFormat.Location = new Point(122, 22);
        txtRowFormat.Name = "txtRowFormat";
        txtRowFormat.Size = new Size(903, 23);
        txtRowFormat.TabIndex = 15;
        txtRowFormat.Text = "~StartedOnTicks~^~Task~^~Comments~^~Tag[Name=Bill%].Description~^~Duration~^False^False";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(6, 25);
        label4.Name = "label4";
        label4.Size = new Size(110, 15);
        label4.TabIndex = 14;
        label4.Text = "Export Row Format:";
        // 
        // groupBox1
        // 
        groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        groupBox1.Controls.Add(btnRowFormatHelp);
        groupBox1.Controls.Add(txtRowFormat);
        groupBox1.Controls.Add(label1);
        groupBox1.Controls.Add(label4);
        groupBox1.Controls.Add(txtTimesheetFolder);
        groupBox1.Controls.Add(btnBrowseTimesheetFolder);
        groupBox1.Controls.Add(btnExportTimesheet);
        groupBox1.Controls.Add(dtTimesheetDate);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(txtExtension);
        groupBox1.Location = new Point(11, 491);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(1061, 86);
        groupBox1.TabIndex = 16;
        groupBox1.TabStop = false;
        groupBox1.Text = "Export Settings";
        // 
        // btnRowFormatHelp
        // 
        btnRowFormatHelp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRowFormatHelp.Location = new Point(1029, 22);
        btnRowFormatHelp.Name = "btnRowFormatHelp";
        btnRowFormatHelp.Size = new Size(24, 23);
        btnRowFormatHelp.TabIndex = 17;
        btnRowFormatHelp.Text = "?";
        btnRowFormatHelp.UseVisualStyleBackColor = true;
        btnRowFormatHelp.Click += btnRowFormatHelp_Click;
        // 
        // btnRefresh
        // 
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.Location = new Point(997, 10);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(75, 23);
        btnRefresh.TabIndex = 17;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // lbWeek
        // 
        lbWeek.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lbWeek.FormattingEnabled = true;
        lbWeek.Location = new Point(8, 41);
        lbWeek.Name = "lbWeek";
        lbWeek.SelectionMode = SelectionMode.MultiExtended;
        lbWeek.Size = new Size(144, 409);
        lbWeek.TabIndex = 18;
        lbWeek.SelectedIndexChanged += lbWeek_SelectedIndexChanged;
        // 
        // btnExportAll
        // 
        btnExportAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExportAll.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportAll.Location = new Point(8, 456);
        btnExportAll.Name = "btnExportAll";
        btnExportAll.Size = new Size(144, 23);
        btnExportAll.TabIndex = 19;
        btnExportAll.Text = "Export All";
        btnExportAll.UseVisualStyleBackColor = true;
        btnExportAll.Click += btnExportAll_Click;
        // 
        // ID
        // 
        ID.DataPropertyName = "ID";
        ID.HeaderText = "ID";
        ID.Name = "ID";
        ID.ReadOnly = true;
        ID.Visible = false;
        // 
        // Task
        // 
        Task.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        Task.DataPropertyName = "Task";
        Task.HeaderText = "Task";
        Task.Name = "Task";
        Task.ReadOnly = true;
        // 
        // StartedOn
        // 
        StartedOn.DataPropertyName = "StartedOn";
        dataGridViewCellStyle1.Format = "MM/dd hh:mm tt";
        StartedOn.DefaultCellStyle = dataGridViewCellStyle1;
        StartedOn.HeaderText = "Start";
        StartedOn.Name = "StartedOn";
        StartedOn.ReadOnly = true;
        StartedOn.Resizable = DataGridViewTriState.False;
        // 
        // EndedOn
        // 
        EndedOn.DataPropertyName = "EndedOn";
        dataGridViewCellStyle2.Format = "MM/dd hh:mm tt";
        EndedOn.DefaultCellStyle = dataGridViewCellStyle2;
        EndedOn.HeaderText = "End";
        EndedOn.Name = "EndedOn";
        EndedOn.ReadOnly = true;
        EndedOn.Resizable = DataGridViewTriState.False;
        // 
        // Duration
        // 
        Duration.DataPropertyName = "Duration";
        Duration.HeaderText = "Duration";
        Duration.Name = "Duration";
        Duration.ReadOnly = true;
        Duration.Resizable = DataGridViewTriState.False;
        Duration.Visible = false;
        Duration.Width = 80;
        // 
        // DurationString
        // 
        DurationString.DataPropertyName = "DurationString";
        DurationString.HeaderText = "Duration";
        DurationString.Name = "DurationString";
        DurationString.ReadOnly = true;
        DurationString.Resizable = DataGridViewTriState.False;
        DurationString.Width = 80;
        // 
        // DayOfWeek
        // 
        DayOfWeek.DataPropertyName = "DayOfWeek";
        DayOfWeek.HeaderText = "Day of Week";
        DayOfWeek.Name = "DayOfWeek";
        DayOfWeek.ReadOnly = true;
        // 
        // PlayAction
        // 
        PlayAction.HeaderText = "";
        PlayAction.Name = "PlayAction";
        PlayAction.ReadOnly = true;
        PlayAction.Resizable = DataGridViewTriState.False;
        PlayAction.Text = "▶";
        PlayAction.UseColumnTextForButtonValue = true;
        PlayAction.Width = 40;
        // 
        // CreatedOn
        // 
        CreatedOn.DataPropertyName = "CreatedOn";
        CreatedOn.HeaderText = "CreatedOn";
        CreatedOn.Name = "CreatedOn";
        CreatedOn.ReadOnly = true;
        CreatedOn.Visible = false;
        // 
        // Reminder
        // 
        Reminder.DataPropertyName = "Reminder";
        Reminder.HeaderText = "Reminder";
        Reminder.Name = "Reminder";
        Reminder.ReadOnly = true;
        Reminder.Visible = false;
        // 
        // Rank
        // 
        Rank.DataPropertyName = "Rank";
        Rank.HeaderText = "Rank";
        Rank.Name = "Rank";
        Rank.ReadOnly = true;
        Rank.Visible = false;
        // 
        // IsRunning
        // 
        IsRunning.DataPropertyName = "IsRunning";
        IsRunning.HeaderText = "IsRunning";
        IsRunning.Name = "IsRunning";
        IsRunning.ReadOnly = true;
        IsRunning.Visible = false;
        // 
        // frmViewList
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1084, 611);
        Controls.Add(btnExportAll);
        Controls.Add(lbWeek);
        Controls.Add(btnRefresh);
        Controls.Add(groupBox1);
        Controls.Add(statusStrip1);
        Controls.Add(txtSearch);
        Controls.Add(lblSelectedTotal);
        Controls.Add(dgView);
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        MinimumSize = new Size(1100, 650);
        Name = "frmViewList";
        Text = "View";
        Load += frmView_Load;
        VisibleChanged += frmView_VisibleChanged;
        KeyDown += frmView_KeyDown;
        ((System.ComponentModel.ISupportInitialize)dgView).EndInit();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView dgView;
    private Label lblSelectedTotal;
    private Label label1;
    private FolderBrowserDialog fbdTimesheetFolder;
    private TextBox txtTimesheetFolder;
    private Button btnBrowseTimesheetFolder;
    private Button btnExportTimesheet;
    private DateTimePicker dtTimesheetDate;
    private TextBox txtExtension;
    private Label label2;
    private TextBox txtSearch;
    private System.Windows.Forms.Timer tmrSearch;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel ttsPath;
    private TextBox txtRowFormat;
    private Label label4;
    private GroupBox groupBox1;
    private Button btnRowFormatHelp;
    private Button btnRefresh;
    private ListBox lbWeek;
    private Button btnExportAll;
    private DataGridViewTextBoxColumn ID;
    private DataGridViewTextBoxColumn Task;
    private DataGridViewTextBoxColumn StartedOn;
    private DataGridViewTextBoxColumn EndedOn;
    private DataGridViewTextBoxColumn Duration;
    private DataGridViewTextBoxColumn DurationString;
    private DataGridViewTextBoxColumn DayOfWeek;
    private DataGridViewButtonColumn PlayAction;
    private DataGridViewTextBoxColumn CreatedOn;
    private DataGridViewTextBoxColumn Reminder;
    private DataGridViewTextBoxColumn Rank;
    private DataGridViewTextBoxColumn IsRunning;
}