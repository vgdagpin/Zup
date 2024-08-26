namespace Zup;

partial class frmEntryList
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
        flpTaskList = new FlowLayoutPanel();
        cmsList = new ContextMenuStrip(components);
        reorderToolStripMenuItem = new ToolStripMenuItem();
        showQueuedTasksToolStripMenuItem = new ToolStripMenuItem();
        showRankedTasksToolStripMenuItem = new ToolStripMenuItem();
        showClosedTasksToolStripMenuItem = new ToolStripMenuItem();
        tmrSaveSetting = new System.Windows.Forms.Timer(components);
        flpQueuedTaskList = new FlowLayoutPanel();
        flpRankedTasks = new FlowLayoutPanel();
        tblLayoutPanel = new TableLayoutPanel();
        cmsList.SuspendLayout();
        tblLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // flpTaskList
        // 
        flpTaskList.AutoScroll = true;
        flpTaskList.ContextMenuStrip = cmsList;
        flpTaskList.Dock = DockStyle.Fill;
        flpTaskList.FlowDirection = FlowDirection.TopDown;
        flpTaskList.Location = new Point(3, 3);
        flpTaskList.Name = "flpTaskList";
        flpTaskList.Size = new Size(268, 186);
        flpTaskList.TabIndex = 0;
        flpTaskList.WrapContents = false;
        // 
        // cmsList
        // 
        cmsList.Items.AddRange(new ToolStripItem[] { reorderToolStripMenuItem, showQueuedTasksToolStripMenuItem, showRankedTasksToolStripMenuItem, showClosedTasksToolStripMenuItem });
        cmsList.Name = "contextMenuStrip1";
        cmsList.Size = new Size(179, 92);
        // 
        // reorderToolStripMenuItem
        // 
        reorderToolStripMenuItem.Name = "reorderToolStripMenuItem";
        reorderToolStripMenuItem.Size = new Size(178, 22);
        reorderToolStripMenuItem.Text = "Reorder";
        reorderToolStripMenuItem.Click += reorderToolStripMenuItem_Click;
        // 
        // showQueuedTasksToolStripMenuItem
        // 
        showQueuedTasksToolStripMenuItem.Checked = true;
        showQueuedTasksToolStripMenuItem.CheckState = CheckState.Checked;
        showQueuedTasksToolStripMenuItem.Name = "showQueuedTasksToolStripMenuItem";
        showQueuedTasksToolStripMenuItem.Size = new Size(178, 22);
        showQueuedTasksToolStripMenuItem.Text = "Show Queued Tasks";
        showQueuedTasksToolStripMenuItem.Click += showQueuedTasksToolStripMenuItem_Click;
        // 
        // showRankedTasksToolStripMenuItem
        // 
        showRankedTasksToolStripMenuItem.Checked = true;
        showRankedTasksToolStripMenuItem.CheckState = CheckState.Checked;
        showRankedTasksToolStripMenuItem.Name = "showRankedTasksToolStripMenuItem";
        showRankedTasksToolStripMenuItem.Size = new Size(178, 22);
        showRankedTasksToolStripMenuItem.Text = "Show Ranked Tasks";
        showRankedTasksToolStripMenuItem.Click += showRankedTasksToolStripMenuItem_Click;
        // 
        // showClosedTasksToolStripMenuItem
        // 
        showClosedTasksToolStripMenuItem.Checked = true;
        showClosedTasksToolStripMenuItem.CheckState = CheckState.Checked;
        showClosedTasksToolStripMenuItem.Name = "showClosedTasksToolStripMenuItem";
        showClosedTasksToolStripMenuItem.Size = new Size(178, 22);
        showClosedTasksToolStripMenuItem.Text = "Show Closed Tasks";
        showClosedTasksToolStripMenuItem.Click += showClosedTasksToolStripMenuItem_Click;
        // 
        // tmrSaveSetting
        // 
        tmrSaveSetting.Interval = 1000;
        tmrSaveSetting.Tick += tmrSaveSetting_Tick;
        // 
        // flpQueuedTaskList
        // 
        flpQueuedTaskList.AutoScroll = true;
        flpQueuedTaskList.ContextMenuStrip = cmsList;
        flpQueuedTaskList.Dock = DockStyle.Fill;
        flpQueuedTaskList.FlowDirection = FlowDirection.TopDown;
        flpQueuedTaskList.Location = new Point(3, 195);
        flpQueuedTaskList.Name = "flpQueuedTaskList";
        flpQueuedTaskList.Size = new Size(268, 90);
        flpQueuedTaskList.TabIndex = 1;
        flpQueuedTaskList.WrapContents = false;
        // 
        // flpRankedTasks
        // 
        flpRankedTasks.AutoScroll = true;
        flpRankedTasks.ContextMenuStrip = cmsList;
        flpRankedTasks.Dock = DockStyle.Fill;
        flpRankedTasks.FlowDirection = FlowDirection.TopDown;
        flpRankedTasks.Location = new Point(3, 291);
        flpRankedTasks.Name = "flpRankedTasks";
        flpRankedTasks.Size = new Size(268, 90);
        flpRankedTasks.TabIndex = 2;
        flpRankedTasks.WrapContents = false;
        // 
        // tblLayoutPanel
        // 
        tblLayoutPanel.ColumnCount = 1;
        tblLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblLayoutPanel.Controls.Add(flpTaskList, 0, 0);
        tblLayoutPanel.Controls.Add(flpQueuedTaskList, 0, 1);
        tblLayoutPanel.Controls.Add(flpRankedTasks, 0, 2);
        tblLayoutPanel.Dock = DockStyle.Fill;
        tblLayoutPanel.Location = new Point(0, 0);
        tblLayoutPanel.Name = "tblLayoutPanel";
        tblLayoutPanel.RowCount = 3;
        tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tblLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tblLayoutPanel.Size = new Size(274, 384);
        tblLayoutPanel.TabIndex = 3;
        // 
        // frmEntryList
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.LawnGreen;
        ClientSize = new Size(274, 384);
        Controls.Add(tblLayoutPanel);
        FormBorderStyle = FormBorderStyle.None;
        Name = "frmEntryList";
        Opacity = 0.9D;
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "frmEntryList";
        TopMost = true;
        TransparencyKey = Color.LawnGreen;
        FormClosing += frmEntryList_FormClosing;
        Load += frmEntryList_Load;
        Move += frmEntryList_Move;
        cmsList.ResumeLayout(false);
        tblLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private FlowLayoutPanel flpTaskList;
    private System.Windows.Forms.Timer tmrSaveSetting;
    private ContextMenuStrip cmsList;
    private ToolStripMenuItem reorderToolStripMenuItem;
    private ToolStripMenuItem showQueuedTasksToolStripMenuItem;
    private ToolStripMenuItem showRankedTasksToolStripMenuItem;
    private ToolStripMenuItem showClosedTasksToolStripMenuItem;
    private FlowLayoutPanel flpQueuedTaskList;
    private FlowLayoutPanel flpRankedTasks;
    private TableLayoutPanel tblLayoutPanel;
}