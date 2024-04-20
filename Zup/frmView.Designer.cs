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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmView));
        dgView = new DataGridView();
        ID = new DataGridViewTextBoxColumn();
        Task = new DataGridViewTextBoxColumn();
        StartedOn = new DataGridViewTextBoxColumn();
        EndedOn = new DataGridViewTextBoxColumn();
        Duration = new DataGridViewTextBoxColumn();
        DurationData = new DataGridViewTextBoxColumn();
        lblSelectedTotal = new Label();
        label1 = new Label();
        fbdTimesheetFolder = new FolderBrowserDialog();
        txtTimesheetFolder = new TextBox();
        btnBrowseTimesheetFolder = new Button();
        btnExportTimesheet = new Button();
        dtTimesheetDate = new DateTimePicker();
        ((System.ComponentModel.ISupportInitialize)dgView).BeginInit();
        SuspendLayout();
        // 
        // dgView
        // 
        dgView.AllowUserToAddRows = false;
        dgView.AllowUserToDeleteRows = false;
        dgView.AllowUserToResizeRows = false;
        dgView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgView.Columns.AddRange(new DataGridViewColumn[] { ID, Task, StartedOn, EndedOn, Duration, DurationData });
        dgView.Location = new Point(11, 50);
        dgView.Name = "dgView";
        dgView.ReadOnly = true;
        dgView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgView.Size = new Size(777, 417);
        dgView.TabIndex = 0;
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
        StartedOn.HeaderText = "Start";
        StartedOn.Name = "StartedOn";
        StartedOn.ReadOnly = true;
        StartedOn.Resizable = DataGridViewTriState.False;
        // 
        // EndedOn
        // 
        EndedOn.DataPropertyName = "EndedOn";
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
        Duration.Width = 60;
        // 
        // DurationData
        // 
        DurationData.DataPropertyName = "DurationData";
        DurationData.HeaderText = "DurationData";
        DurationData.Name = "DurationData";
        DurationData.ReadOnly = true;
        DurationData.Resizable = DataGridViewTriState.False;
        DurationData.Visible = false;
        // 
        // lblSelectedTotal
        // 
        lblSelectedTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblSelectedTotal.Location = new Point(725, 474);
        lblSelectedTotal.Name = "lblSelectedTotal";
        lblSelectedTotal.Size = new Size(59, 23);
        lblSelectedTotal.TabIndex = 1;
        lblSelectedTotal.Text = "00:00:00";
        lblSelectedTotal.TextAlign = ContentAlignment.MiddleRight;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(10, 14);
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
        txtTimesheetFolder.Location = new Point(121, 11);
        txtTimesheetFolder.Name = "txtTimesheetFolder";
        txtTimesheetFolder.Size = new Size(340, 23);
        txtTimesheetFolder.TabIndex = 3;
        // 
        // btnBrowseTimesheetFolder
        // 
        btnBrowseTimesheetFolder.Location = new Point(467, 11);
        btnBrowseTimesheetFolder.Name = "btnBrowseTimesheetFolder";
        btnBrowseTimesheetFolder.Size = new Size(75, 23);
        btnBrowseTimesheetFolder.TabIndex = 4;
        btnBrowseTimesheetFolder.Text = "Browse";
        btnBrowseTimesheetFolder.UseVisualStyleBackColor = true;
        btnBrowseTimesheetFolder.Click += btnBrowseTimesheetFolder_Click;
        // 
        // btnExportTimesheet
        // 
        btnExportTimesheet.Location = new Point(575, 10);
        btnExportTimesheet.Name = "btnExportTimesheet";
        btnExportTimesheet.Size = new Size(115, 23);
        btnExportTimesheet.TabIndex = 5;
        btnExportTimesheet.Text = "Export selected to";
        btnExportTimesheet.UseVisualStyleBackColor = true;
        btnExportTimesheet.Click += btnExportTimesheet_Click;
        // 
        // dtTimesheetDate
        // 
        dtTimesheetDate.Format = DateTimePickerFormat.Short;
        dtTimesheetDate.Location = new Point(692, 10);
        dtTimesheetDate.Name = "dtTimesheetDate";
        dtTimesheetDate.Size = new Size(96, 23);
        dtTimesheetDate.TabIndex = 6;
        // 
        // frmView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 502);
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
        FormClosing += frmView_FormClosing;
        Load += frmView_Load;
        VisibleChanged += frmView_VisibleChanged;
        ((System.ComponentModel.ISupportInitialize)dgView).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView dgView;
    private Label lblSelectedTotal;
    private DataGridViewTextBoxColumn ID;
    private DataGridViewTextBoxColumn Task;
    private DataGridViewTextBoxColumn StartedOn;
    private DataGridViewTextBoxColumn EndedOn;
    private DataGridViewTextBoxColumn Duration;
    private DataGridViewTextBoxColumn DurationData;
    private Label label1;
    private FolderBrowserDialog fbdTimesheetFolder;
    private TextBox txtTimesheetFolder;
    private Button btnBrowseTimesheetFolder;
    private Button btnExportTimesheet;
    private DateTimePicker dtTimesheetDate;
}