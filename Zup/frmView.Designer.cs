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
        dgView.Location = new Point(11, 12);
        dgView.Name = "dgView";
        dgView.ReadOnly = true;
        dgView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgView.Size = new Size(777, 403);
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
        lblSelectedTotal.Location = new Point(725, 422);
        lblSelectedTotal.Name = "lblSelectedTotal";
        lblSelectedTotal.Size = new Size(59, 23);
        lblSelectedTotal.TabIndex = 1;
        lblSelectedTotal.Text = "00:00:00";
        lblSelectedTotal.TextAlign = ContentAlignment.MiddleRight;
        // 
        // frmView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(lblSelectedTotal);
        Controls.Add(dgView);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "frmView";
        Text = "View";
        FormClosing += frmView_FormClosing;
        VisibleChanged += frmView_VisibleChanged;
        ((System.ComponentModel.ISupportInitialize)dgView).EndInit();
        ResumeLayout(false);
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
}