namespace Zup;

partial class frmView
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmView));
        dgView = new DataGridView();
        ID = new DataGridViewTextBoxColumn();
        Task = new DataGridViewTextBoxColumn();
        StartedOn = new DataGridViewTextBoxColumn();
        EndedOn = new DataGridViewTextBoxColumn();
        Duration = new DataGridViewTextBoxColumn();
        DurationString = new DataGridViewTextBoxColumn();
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
        label3 = new Label();
        statusStrip1 = new StatusStrip();
        ttsPath = new ToolStripStatusLabel();
        ((System.ComponentModel.ISupportInitialize)dgView).BeginInit();
        statusStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // dgView
        // 
        dgView.AllowUserToAddRows = false;
        dgView.AllowUserToDeleteRows = false;
        dgView.AllowUserToResizeRows = false;
        dgView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgView.Columns.AddRange(new DataGridViewColumn[] { ID, Task, StartedOn, EndedOn, Duration, DurationString });
        dgView.Location = new Point(11, 41);
        dgView.Name = "dgView";
        dgView.ReadOnly = true;
        dgView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgView.Size = new Size(777, 384);
        dgView.TabIndex = 1;
        dgView.SelectionChanged += dgView_SelectionChanged;
        dgView.DoubleClick += dgView_DoubleClick;
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
        DurationString.Width = 60;
        // 
        // lblSelectedTotal
        // 
        lblSelectedTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblSelectedTotal.Location = new Point(729, 427);
        lblSelectedTotal.Name = "lblSelectedTotal";
        lblSelectedTotal.Size = new Size(59, 23);
        lblSelectedTotal.TabIndex = 1;
        lblSelectedTotal.Text = "00:00:00";
        lblSelectedTotal.TextAlign = ContentAlignment.MiddleRight;
        // 
        // label1
        // 
        label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        label1.AutoSize = true;
        label1.Location = new Point(8, 466);
        label1.Name = "label1";
        label1.Size = new Size(105, 15);
        label1.TabIndex = 2;
        label1.Text = "Timesheets Folder:";
        // 
        // fbdTimesheetFolder
        // 
        fbdTimesheetFolder.RootFolder = Environment.SpecialFolder.MyDocuments;
        // 
        // txtTimesheetFolder
        // 
        txtTimesheetFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtTimesheetFolder.Location = new Point(119, 463);
        txtTimesheetFolder.Name = "txtTimesheetFolder";
        txtTimesheetFolder.Size = new Size(389, 23);
        txtTimesheetFolder.TabIndex = 3;
        // 
        // btnBrowseTimesheetFolder
        // 
        btnBrowseTimesheetFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnBrowseTimesheetFolder.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
        btnBrowseTimesheetFolder.FlatStyle = FlatStyle.Flat;
        btnBrowseTimesheetFolder.Location = new Point(507, 463);
        btnBrowseTimesheetFolder.Name = "btnBrowseTimesheetFolder";
        btnBrowseTimesheetFolder.Size = new Size(24, 23);
        btnBrowseTimesheetFolder.TabIndex = 4;
        btnBrowseTimesheetFolder.Text = "...";
        btnBrowseTimesheetFolder.UseVisualStyleBackColor = true;
        btnBrowseTimesheetFolder.Click += btnBrowseTimesheetFolder_Click;
        // 
        // btnExportTimesheet
        // 
        btnExportTimesheet.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnExportTimesheet.Location = new Point(692, 463);
        btnExportTimesheet.Name = "btnExportTimesheet";
        btnExportTimesheet.Size = new Size(96, 23);
        btnExportTimesheet.TabIndex = 5;
        btnExportTimesheet.Text = "Export selected";
        btnExportTimesheet.UseVisualStyleBackColor = true;
        btnExportTimesheet.Click += btnExportTimesheet_Click;
        // 
        // dtTimesheetDate
        // 
        dtTimesheetDate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        dtTimesheetDate.CustomFormat = "MM-dd-yyyy";
        dtTimesheetDate.Format = DateTimePickerFormat.Custom;
        dtTimesheetDate.Location = new Point(553, 463);
        dtTimesheetDate.Name = "dtTimesheetDate";
        dtTimesheetDate.Size = new Size(96, 23);
        dtTimesheetDate.TabIndex = 6;
        dtTimesheetDate.ValueChanged += dtTimesheetDate_ValueChanged;
        // 
        // txtExtension
        // 
        txtExtension.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        txtExtension.Location = new Point(657, 463);
        txtExtension.Name = "txtExtension";
        txtExtension.Size = new Size(30, 23);
        txtExtension.TabIndex = 7;
        txtExtension.Text = ".fd";
        txtExtension.TextChanged += txtExtension_TextChanged;
        // 
        // label2
        // 
        label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        label2.AutoSize = true;
        label2.Location = new Point(537, 467);
        label2.Name = "label2";
        label2.Size = new Size(12, 15);
        label2.TabIndex = 8;
        label2.Text = "/";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(60, 9);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(277, 23);
        txtSearch.TabIndex = 0;
        txtSearch.TextChanged += txtSearch_TextChanged;
        // 
        // tmrSearch
        // 
        tmrSearch.Interval = 500;
        tmrSearch.Tick += tmrSearch_Tick;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(11, 15);
        label3.Name = "label3";
        label3.Size = new Size(45, 15);
        label3.TabIndex = 10;
        label3.Text = "Search:";
        // 
        // statusStrip1
        // 
        statusStrip1.Items.AddRange(new ToolStripItem[] { ttsPath });
        statusStrip1.Location = new Point(0, 508);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(800, 22);
        statusStrip1.TabIndex = 11;
        statusStrip1.Text = "statusStrip1";
        // 
        // ttsPath
        // 
        ttsPath.Name = "ttsPath";
        ttsPath.Size = new Size(31, 17);
        ttsPath.Text = "Path";
        // 
        // frmView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 530);
        Controls.Add(statusStrip1);
        Controls.Add(label3);
        Controls.Add(txtSearch);
        Controls.Add(label2);
        Controls.Add(txtExtension);
        Controls.Add(dtTimesheetDate);
        Controls.Add(btnExportTimesheet);
        Controls.Add(btnBrowseTimesheetFolder);
        Controls.Add(txtTimesheetFolder);
        Controls.Add(label1);
        Controls.Add(lblSelectedTotal);
        Controls.Add(dgView);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "frmView";
        Text = "View";
        Load += frmView_Load;
        VisibleChanged += frmView_VisibleChanged;
        ((System.ComponentModel.ISupportInitialize)dgView).EndInit();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
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
    private DataGridViewTextBoxColumn ID;
    private DataGridViewTextBoxColumn Task;
    private DataGridViewTextBoxColumn StartedOn;
    private DataGridViewTextBoxColumn EndedOn;
    private DataGridViewTextBoxColumn Duration;
    private DataGridViewTextBoxColumn DurationString;
    private TextBox txtExtension;
    private Label label2;
    private TextBox txtSearch;
    private System.Windows.Forms.Timer tmrSearch;
    private Label label3;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel ttsPath;
}