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
        notipIconZup = new NotifyIcon(components);
        SuspendLayout();
        // 
        // notipIconZup
        // 
        notipIconZup.Icon = (Icon)resources.GetObject("notipIconZup.Icon");
        notipIconZup.Text = "Zup";
        notipIconZup.Visible = true;
        // 
        // frmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Name = "frmMain";
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = "Form1";
        WindowState = FormWindowState.Minimized;
        Load += frmMain_Load;
        ResumeLayout(false);
    }

    #endregion

    private NotifyIcon notipIconZup;
}
