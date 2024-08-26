namespace Zup;

partial class frmMain
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        notifIconZup = new NotifyIcon(components);
        notifCms = new ContextMenuStrip(components);
        viewToolStripMenuItem = new ToolStripMenuItem();
        tagEditorToolStripMenuItem = new ToolStripMenuItem();
        settingsToolStripMenuItem = new ToolStripMenuItem();
        openNewEntryToolStripMenuItem = new ToolStripMenuItem();
        updateCurrentRunningTaskToolStripMenuItem = new ToolStripMenuItem();
        toggleLastRunningTaskToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        exitToolStripMenuItem = new ToolStripMenuItem();
        tmrDelayShowList = new System.Windows.Forms.Timer(components);
        moveToCenterToolStripMenuItem = new ToolStripMenuItem();
        notifCms.SuspendLayout();
        SuspendLayout();
        // 
        // notifIconZup
        // 
        notifIconZup.BalloonTipIcon = ToolTipIcon.Info;
        notifIconZup.BalloonTipText = "Test";
        notifIconZup.BalloonTipTitle = "Test Title";
        notifIconZup.ContextMenuStrip = notifCms;
        notifIconZup.Text = "Zup";
        notifIconZup.Visible = true;
        notifIconZup.DoubleClick += notifIconZup_DoubleClick;
        // 
        // notifCms
        // 
        notifCms.Items.AddRange(new ToolStripItem[] { viewToolStripMenuItem, tagEditorToolStripMenuItem, settingsToolStripMenuItem, openNewEntryToolStripMenuItem, updateCurrentRunningTaskToolStripMenuItem, toggleLastRunningTaskToolStripMenuItem, moveToCenterToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
        notifCms.Name = "notifCms";
        notifCms.Size = new Size(298, 208);
        // 
        // viewToolStripMenuItem
        // 
        viewToolStripMenuItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        viewToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Shift | Keys.P;
        viewToolStripMenuItem.Size = new Size(297, 22);
        viewToolStripMenuItem.Text = "View";
        viewToolStripMenuItem.Click += viewToolStripMenuItem_Click;
        // 
        // tagEditorToolStripMenuItem
        // 
        tagEditorToolStripMenuItem.Name = "tagEditorToolStripMenuItem";
        tagEditorToolStripMenuItem.Size = new Size(297, 22);
        tagEditorToolStripMenuItem.Text = "Tag Editor";
        tagEditorToolStripMenuItem.Click += tagEditorToolStripMenuItem_Click;
        // 
        // settingsToolStripMenuItem
        // 
        settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
        settingsToolStripMenuItem.Size = new Size(297, 22);
        settingsToolStripMenuItem.Text = "Settings";
        settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
        // 
        // openNewEntryToolStripMenuItem
        // 
        openNewEntryToolStripMenuItem.Name = "openNewEntryToolStripMenuItem";
        openNewEntryToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Shift | Keys.J;
        openNewEntryToolStripMenuItem.Size = new Size(297, 22);
        openNewEntryToolStripMenuItem.Text = "Open New Entry";
        openNewEntryToolStripMenuItem.Click += openNewEntryToolStripMenuItem_Click;
        // 
        // updateCurrentRunningTaskToolStripMenuItem
        // 
        updateCurrentRunningTaskToolStripMenuItem.Name = "updateCurrentRunningTaskToolStripMenuItem";
        updateCurrentRunningTaskToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Shift | Keys.K;
        updateCurrentRunningTaskToolStripMenuItem.Size = new Size(297, 22);
        updateCurrentRunningTaskToolStripMenuItem.Text = "Update Current Running Task";
        updateCurrentRunningTaskToolStripMenuItem.Click += updateCurrentRunningTaskToolStripMenuItem_Click;
        // 
        // toggleLastRunningTaskToolStripMenuItem
        // 
        toggleLastRunningTaskToolStripMenuItem.Name = "toggleLastRunningTaskToolStripMenuItem";
        toggleLastRunningTaskToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Shift | Keys.L;
        toggleLastRunningTaskToolStripMenuItem.Size = new Size(297, 22);
        toggleLastRunningTaskToolStripMenuItem.Text = "Toggle Last Running Task";
        toggleLastRunningTaskToolStripMenuItem.Click += toggleLastRunningTaskToolStripMenuItem_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(294, 6);
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(297, 22);
        exitToolStripMenuItem.Text = "Exit";
        exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        // 
        // tmrDelayShowList
        // 
        tmrDelayShowList.Enabled = true;
        tmrDelayShowList.Interval = 300;
        tmrDelayShowList.Tick += tmrDelayShowList_Tick;
        // 
        // moveToCenterToolStripMenuItem
        // 
        moveToCenterToolStripMenuItem.Name = "moveToCenterToolStripMenuItem";
        moveToCenterToolStripMenuItem.Size = new Size(297, 22);
        moveToCenterToolStripMenuItem.Text = "Move to Center";
        moveToCenterToolStripMenuItem.Click += moveToCenterToolStripMenuItem_Click;
        // 
        // frmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(278, 85);
        FormBorderStyle = FormBorderStyle.None;
        Name = "frmMain";
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = "Zup";
        WindowState = FormWindowState.Minimized;
        Load += frmMain_Load;
        notifCms.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private NotifyIcon notifIconZup;
    private System.Windows.Forms.Timer tmrDelayShowList;
    private ContextMenuStrip notifCms;
    private ToolStripMenuItem settingsToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem openNewEntryToolStripMenuItem;
    private ToolStripMenuItem updateCurrentRunningTaskToolStripMenuItem;
    private ToolStripMenuItem toggleLastRunningTaskToolStripMenuItem;
    private ToolStripMenuItem tagEditorToolStripMenuItem;
    private ToolStripMenuItem moveToCenterToolStripMenuItem;
}
