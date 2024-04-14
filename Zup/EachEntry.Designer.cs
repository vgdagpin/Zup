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
        button2 = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(5, 5);
        label1.Name = "label1";
        label1.Size = new Size(38, 15);
        label1.TabIndex = 0;
        label1.Text = "label1";
        // 
        // button1
        // 
        button1.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button1.Location = new Point(202, 6);
        button1.Name = "button1";
        button1.Size = new Size(25, 23);
        button1.TabIndex = 1;
        button1.Text = "►";
        button1.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        button2.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button2.Location = new Point(202, 28);
        button2.Name = "button2";
        button2.Size = new Size(25, 23);
        button2.TabIndex = 2;
        button2.Text = "●";
        button2.UseVisualStyleBackColor = true;
        // 
        // EachEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Gainsboro;
        Controls.Add(button2);
        Controls.Add(button1);
        Controls.Add(label1);
        Name = "EachEntry";
        Size = new Size(247, 54);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private Button button1;
    private Button button2;
}
