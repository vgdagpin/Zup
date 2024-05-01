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
        tmrSaveSetting = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        // 
        // flpTaskList
        // 
        flpTaskList.AutoScroll = true;
        flpTaskList.Dock = DockStyle.Fill;
        flpTaskList.FlowDirection = FlowDirection.BottomUp;
        flpTaskList.Location = new Point(0, 0);
        flpTaskList.Name = "flpTaskList";
        flpTaskList.Size = new Size(266, 86);
        flpTaskList.TabIndex = 0;
        flpTaskList.WrapContents = false;
        // 
        // tmrSaveSetting
        // 
        tmrSaveSetting.Interval = 1000;
        tmrSaveSetting.Tick += tmrSaveSetting_Tick;
        // 
        // frmEntryList
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.LawnGreen;
        ClientSize = new Size(266, 86);
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
        ResumeLayout(false);
    }

    #endregion

    private FlowLayoutPanel flpTaskList;
    private System.Windows.Forms.Timer tmrSaveSetting;
}