namespace Zup.CustomControls;

partial class EachEntry
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblText = new ZupLabel();
        btnToggleStartStop = new Button();
        lblTimeInOut = new ZupLabel();
        tmr = new System.Windows.Forms.Timer(components);
        lblDuration = new ZupLabel();
        toolTip = new ToolTip(components);
        SuspendLayout();
        // 
        // lblText
        // 
        lblText.AutoEllipsis = true;
        lblText.Font = new Font("Segoe UI", 7F);
        lblText.Location = new Point(4, 4);
        lblText.Name = "lblText";
        lblText.Size = new Size(215, 13);
        lblText.TabIndex = 0;
        lblText.Text = "Task #1";
        toolTip.SetToolTip(lblText, "Test");
        lblText.MouseDown += lblText_MouseDown;
        // 
        // btnToggleStartStop
        // 
        btnToggleStartStop.FlatAppearance.BorderColor = Color.Gray;
        btnToggleStartStop.FlatStyle = FlatStyle.Flat;
        btnToggleStartStop.Font = new Font("Arial Narrow", 7F, FontStyle.Regular, GraphicsUnit.Pixel, 0);
        btnToggleStartStop.ForeColor = Color.Black;
        btnToggleStartStop.Location = new Point(225, 2);
        btnToggleStartStop.Name = "btnToggleStartStop";
        btnToggleStartStop.Size = new Size(18, 18);
        btnToggleStartStop.TabIndex = 1;
        btnToggleStartStop.TabStop = false;
        btnToggleStartStop.Text = "►";
        btnToggleStartStop.UseVisualStyleBackColor = true;
        btnToggleStartStop.Click += btnToggleStartStop_Click;
        // 
        // lblTimeInOut
        // 
        lblTimeInOut.Font = new Font("Segoe UI Light", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblTimeInOut.Location = new Point(5, 18);
        lblTimeInOut.Name = "lblTimeInOut";
        lblTimeInOut.Size = new Size(94, 12);
        lblTimeInOut.TabIndex = 2;
        lblTimeInOut.Text = "00:00AM - 00:00AM";
        lblTimeInOut.MouseDown += lblTimeInOut_MouseDown;
        // 
        // tmr
        // 
        tmr.Interval = 1000;
        tmr.Tick += tmrDuration_Tick;
        // 
        // lblDuration
        // 
        lblDuration.Font = new Font("Segoe UI Light", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblDuration.Location = new Point(192, 21);
        lblDuration.Name = "lblDuration";
        lblDuration.RightToLeft = RightToLeft.Yes;
        lblDuration.Size = new Size(55, 12);
        lblDuration.TabIndex = 5;
        lblDuration.Text = "00:00:00";
        lblDuration.MouseDown += lblDuration_MouseDown;
        // 
        // EachEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Gainsboro;
        Controls.Add(lblDuration);
        Controls.Add(lblTimeInOut);
        Controls.Add(btnToggleStartStop);
        Controls.Add(lblText);
        Margin = new Padding(1, 0, 1, 1);
        Name = "EachEntry";
        Size = new Size(247, 35);
        MouseDown += EachEntry_MouseDown;
        ResumeLayout(false);
    }

    #endregion

    private ZupLabel lblText;
    private Button btnToggleStartStop;
    private ZupLabel lblTimeInOut;
    private System.Windows.Forms.Timer tmr;
    private ZupLabel lblDuration;
    private ToolTip toolTip;
}
