namespace Zup;

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
        label1 = new Label();
        button1 = new Button();
        label2 = new Label();
        label3 = new Label();
        label4 = new Label();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(4, 4);
        label1.Name = "label1";
        label1.Size = new Size(45, 15);
        label1.TabIndex = 0;
        label1.Text = "Task #1";
        // 
        // button1
        // 
        button1.FlatStyle = FlatStyle.System;
        button1.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button1.Location = new Point(221, 2);
        button1.Name = "button1";
        button1.Size = new Size(23, 22);
        button1.TabIndex = 1;
        button1.TabStop = false;
        button1.Text = "►";
        button1.UseVisualStyleBackColor = true;
        // 
        // label2
        // 
        label2.Font = new Font("Segoe UI", 8.25F);
        label2.Location = new Point(3, 22);
        label2.Name = "label2";
        label2.Size = new Size(50, 15);
        label2.TabIndex = 2;
        label2.Text = "11:01 p";
        // 
        // label3
        // 
        label3.Font = new Font("Segoe UI", 8.25F);
        label3.Location = new Point(56, 22);
        label3.Name = "label3";
        label3.Size = new Size(55, 15);
        label3.TabIndex = 3;
        label3.Text = "12:01 p";
        // 
        // label4
        // 
        label4.Font = new Font("Segoe UI", 8.25F);
        label4.Location = new Point(46, 21);
        label4.Name = "label4";
        label4.Size = new Size(12, 15);
        label4.TabIndex = 4;
        label4.Text = "-";
        // 
        // EachEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Gainsboro;
        Controls.Add(label4);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(button1);
        Controls.Add(label1);
        Margin = new Padding(3, 1, 3, 1);
        Name = "EachEntry";
        Size = new Size(247, 42);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private Button button1;
    private Label label2;
    private Label label3;
    private Label label4;
}
