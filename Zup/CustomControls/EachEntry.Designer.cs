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
        lblText = new Label();
        btnToggleStartStop = new Button();
        lblStart = new Label();
        tmr = new System.Windows.Forms.Timer(components);
        lblDuration = new Label();
        btnUpdate = new Button();
        toolTip = new ToolTip(components);
        SuspendLayout();
        // 
        // lblText
        // 
        lblText.AutoEllipsis = true;
        lblText.Font = new Font("Segoe UI", 7F);
        lblText.Location = new Point(4, 4);
        lblText.Name = "lblText";
        lblText.Size = new Size(194, 13);
        lblText.TabIndex = 0;
        lblText.Text = "Task #1";
        toolTip.SetToolTip(lblText, "Test");
        // 
        // btnToggleStartStop
        // 
        btnToggleStartStop.FlatAppearance.BorderColor = Color.Gray;
        btnToggleStartStop.FlatStyle = FlatStyle.Flat;
        btnToggleStartStop.Font = new Font("Arial Narrow", 7F, FontStyle.Regular, GraphicsUnit.Pixel, 0);
        btnToggleStartStop.ForeColor = Color.Black;
        btnToggleStartStop.Location = new Point(225, 2);
        btnToggleStartStop.Name = "btnToggleStartStop";
        btnToggleStartStop.Size = new Size(19, 19);
        btnToggleStartStop.TabIndex = 1;
        btnToggleStartStop.TabStop = false;
        btnToggleStartStop.Text = "►";
        btnToggleStartStop.UseVisualStyleBackColor = true;
        btnToggleStartStop.Click += btnToggleStartStop_Click;
        // 
        // lblStart
        // 
        lblStart.Font = new Font("Segoe UI Light", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblStart.Location = new Point(5, 18);
        lblStart.Name = "lblStart";
        lblStart.Size = new Size(94, 12);
        lblStart.TabIndex = 2;
        lblStart.Text = "00:00AM - 00:00AM";
        // 
        // tmr
        // 
        tmr.Interval = 1000;
        tmr.Tick += timer1_Tick;
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
        // 
        // btnUpdate
        // 
        btnUpdate.FlatAppearance.BorderColor = Color.Gray;
        btnUpdate.FlatStyle = FlatStyle.Flat;
        btnUpdate.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnUpdate.ForeColor = Color.Black;
        btnUpdate.Location = new Point(204, 2);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(19, 19);
        btnUpdate.TabIndex = 6;
        btnUpdate.TabStop = false;
        btnUpdate.Text = "₰";
        toolTip.SetToolTip(btnUpdate, "Update Entry");
        btnUpdate.UseVisualStyleBackColor = true;
        btnUpdate.Click += btnUpdate_Click;
        // 
        // EachEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Gainsboro;
        Controls.Add(btnUpdate);
        Controls.Add(lblDuration);
        Controls.Add(lblStart);
        Controls.Add(btnToggleStartStop);
        Controls.Add(lblText);
        Margin = new Padding(3, 1, 3, 1);
        Name = "EachEntry";
        Size = new Size(247, 35);
        ResumeLayout(false);
    }

    #endregion

    private Label lblText;
    private Button btnToggleStartStop;
    private Label lblStart;
    private System.Windows.Forms.Timer tmr;
    private Label lblDuration;
    private Button btnUpdate;
    private ToolTip toolTip;
}
