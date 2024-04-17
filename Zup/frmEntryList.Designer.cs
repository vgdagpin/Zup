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
        flpTaskList = new FlowLayoutPanel();
        SuspendLayout();
        // 
        // flowLayoutPanel1
        // 
        flpTaskList.AutoScroll = true;
        flpTaskList.Dock = DockStyle.Fill;
        flpTaskList.FlowDirection = FlowDirection.BottomUp;
        flpTaskList.Location = new Point(0, 0);
        flpTaskList.Name = "flowLayoutPanel1";
        flpTaskList.Size = new Size(266, 71);
        flpTaskList.TabIndex = 0;
        flpTaskList.WrapContents = false;
        // 
        // frmEntryList
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(192, 192, 255);
        ClientSize = new Size(266, 71);
        Controls.Add(flpTaskList);
        FormBorderStyle = FormBorderStyle.None;
        Name = "frmEntryList";
        Opacity = 0.9D;
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "frmEntryList";
        TopMost = true;
        TransparencyKey = Color.FromArgb(192, 192, 255);
        FormClosing += frmEntryList_FormClosing;
        Load += frmEntryList_Load;
        ResumeLayout(false);
    }

    #endregion

    private FlowLayoutPanel flpTaskList;
}