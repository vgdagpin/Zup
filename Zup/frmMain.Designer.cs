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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
        notifIconZup = new NotifyIcon(components);
        notifCms = new ContextMenuStrip(components);
        settingsToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        exitToolStripMenuItem = new ToolStripMenuItem();
        tmrDelayShowList = new System.Windows.Forms.Timer(components);
        notifCms.SuspendLayout();
        SuspendLayout();
        // 
        // notifIconZup
        // 
        notifIconZup.ContextMenuStrip = notifCms;
        notifIconZup.Icon = (Icon)resources.GetObject("notifIconZup.Icon");
        notifIconZup.Text = "Zup";
        notifIconZup.Visible = true;
        // 
        // notifCms
        // 
        notifCms.Items.AddRange(new ToolStripItem[] { settingsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
        notifCms.Name = "notifCms";
        notifCms.Size = new Size(117, 54);
        // 
        // settingsToolStripMenuItem
        // 
        settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
        settingsToolStripMenuItem.Size = new Size(116, 22);
        settingsToolStripMenuItem.Text = "Settings";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(113, 6);
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(116, 22);
        exitToolStripMenuItem.Text = "Exit";
        exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        // 
        // tmrDelayShowList
        // 
        tmrDelayShowList.Enabled = true;
        tmrDelayShowList.Interval = 1000;
        tmrDelayShowList.Tick += tmrDelayShowList_Tick;
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
}
