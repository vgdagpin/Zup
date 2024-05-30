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
        tmrSaveSetting = new System.Windows.Forms.Timer(components);
        showClosedTasksToolStripMenuItem = new ToolStripMenuItem();
        cmsList.SuspendLayout();
        SuspendLayout();
        // 
        // flpTaskList
        // 
        flpTaskList.AutoScroll = true;
        flpTaskList.ContextMenuStrip = cmsList;
        flpTaskList.Dock = DockStyle.Fill;
        flpTaskList.FlowDirection = FlowDirection.TopDown;
        flpTaskList.Location = new Point(0, 0);
        flpTaskList.Name = "flpTaskList";
        flpTaskList.Size = new Size(271, 86);
        flpTaskList.TabIndex = 0;
        flpTaskList.WrapContents = false;
        // 
        // cmsList
        // 
        cmsList.Items.AddRange(new ToolStripItem[] { reorderToolStripMenuItem, showQueuedTasksToolStripMenuItem, showRankedTasksToolStripMenuItem, showClosedTasksToolStripMenuItem });
        cmsList.Name = "contextMenuStrip1";
        cmsList.Size = new Size(181, 114);
        // 
        // reorderToolStripMenuItem
        // 
        reorderToolStripMenuItem.Name = "reorderToolStripMenuItem";
        reorderToolStripMenuItem.Size = new Size(180, 22);
        reorderToolStripMenuItem.Text = "Reorder";
        reorderToolStripMenuItem.Click += reorderToolStripMenuItem_Click;
        // 
        // showQueuedTasksToolStripMenuItem
        // 
        showQueuedTasksToolStripMenuItem.Checked = true;
        showQueuedTasksToolStripMenuItem.CheckState = CheckState.Checked;
        showQueuedTasksToolStripMenuItem.Name = "showQueuedTasksToolStripMenuItem";
        showQueuedTasksToolStripMenuItem.Size = new Size(180, 22);
        showQueuedTasksToolStripMenuItem.Text = "Show Queued Tasks";
        showQueuedTasksToolStripMenuItem.Click += showQueuedTasksToolStripMenuItem_Click;
        // 
        // showRankedTasksToolStripMenuItem
        // 
        showRankedTasksToolStripMenuItem.Checked = true;
        showRankedTasksToolStripMenuItem.CheckState = CheckState.Checked;
        showRankedTasksToolStripMenuItem.Name = "showRankedTasksToolStripMenuItem";
        showRankedTasksToolStripMenuItem.Size = new Size(180, 22);
        showRankedTasksToolStripMenuItem.Text = "Show Ranked Tasks";
        showRankedTasksToolStripMenuItem.Click += showRankedTasksToolStripMenuItem_Click;
        // 
        // tmrSaveSetting
        // 
        tmrSaveSetting.Interval = 1000;
        tmrSaveSetting.Tick += tmrSaveSetting_Tick;
        // 
        // showClosedTasksToolStripMenuItem
        // 
        showClosedTasksToolStripMenuItem.Checked = true;
        showClosedTasksToolStripMenuItem.CheckState = CheckState.Checked;
        showClosedTasksToolStripMenuItem.Name = "showClosedTasksToolStripMenuItem";
        showClosedTasksToolStripMenuItem.Size = new Size(180, 22);
        showClosedTasksToolStripMenuItem.Text = "Show Closed Tasks";
        showClosedTasksToolStripMenuItem.Click += showClosedTasksToolStripMenuItem_Click;
        // 
        // frmEntryList
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.LawnGreen;
        ClientSize = new Size(271, 86);
        Controls.Add(flpTaskList);
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
}