namespace Zup;

partial class frmNewEntry
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewEntry));
        txtEntry = new TextBox();
        tmrShowSuggest = new System.Windows.Forms.Timer(components);
        lbSuggestions = new ListBox();
        tmrFocus = new System.Windows.Forms.Timer(components);
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        label4 = new Label();
        label5 = new Label();
        label6 = new Label();
        label7 = new Label();
        SuspendLayout();
        // 
        // txtEntry
        // 
        txtEntry.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        txtEntry.Location = new Point(12, 12);
        txtEntry.Name = "txtEntry";
        txtEntry.Size = new Size(513, 25);
        txtEntry.TabIndex = 0;
        txtEntry.TextChanged += txtEntry_TextChanged;
        txtEntry.KeyDown += txtEntry_KeyDown;
        txtEntry.KeyUp += txtEntry_KeyUp;
        // 
        // tmrShowSuggest
        // 
        tmrShowSuggest.Tick += tmrShowSuggest_Tick;
        // 
        // lbSuggestions
        // 
        lbSuggestions.FormattingEnabled = true;
        lbSuggestions.ItemHeight = 15;
        lbSuggestions.Location = new Point(12, 115);
        lbSuggestions.Name = "lbSuggestions";
        lbSuggestions.Size = new Size(511, 109);
        lbSuggestions.TabIndex = 1;
        lbSuggestions.DoubleClick += lbSuggestions_DoubleClick;
        // 
        // tmrFocus
        // 
        tmrFocus.Tick += tmrFocus_Tick;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Segoe UI", 6.75F, FontStyle.Bold);
        label1.Location = new Point(11, 41);
        label1.Name = "label1";
        label1.Size = new Size(30, 12);
        label1.TabIndex = 2;
        label1.Text = "Enter:";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Segoe UI", 6.75F);
        label2.Location = new Point(41, 41);
        label2.Name = "label2";
        label2.Size = new Size(138, 12);
        label2.TabIndex = 3;
        label2.Text = "Stop other task and start new task";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Font = new Font("Segoe UI", 6.75F);
        label3.Location = new Point(66, 54);
        label3.Name = "label3";
        label3.Size = new Size(82, 12);
        label3.TabIndex = 5;
        label3.Text = "Start task in parallel";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Font = new Font("Segoe UI", 6.75F, FontStyle.Bold);
        label4.Location = new Point(11, 54);
        label4.Name = "label4";
        label4.Size = new Size(56, 12);
        label4.TabIndex = 4;
        label4.Text = "Shift+Enter:";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Font = new Font("Segoe UI", 6.75F);
        label5.Location = new Point(60, 68);
        label5.Name = "label5";
        label5.Size = new Size(80, 12);
        label5.TabIndex = 7;
        label5.Text = "Queue as next task";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Font = new Font("Segoe UI", 6.75F, FontStyle.Bold);
        label6.Location = new Point(11, 68);
        label6.Name = "label6";
        label6.Size = new Size(49, 12);
        label6.TabIndex = 6;
        label6.Text = "Alt+Enter:";
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Font = new Font("Segoe UI", 6.75F);
        label7.Location = new Point(11, 100);
        label7.Name = "label7";
        label7.Size = new Size(32, 12);
        label7.TabIndex = 8;
        label7.Text = "History";
        // 
        // frmNewEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(537, 237);
        Controls.Add(label7);
        Controls.Add(label5);
        Controls.Add(label6);
        Controls.Add(label3);
        Controls.Add(label4);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(lbSuggestions);
        Controls.Add(txtEntry);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmNewEntry";
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Task";
        TopMost = true;
        FormClosing += frmNewEntry_FormClosing;
        VisibleChanged += frmNewEntry_VisibleChanged;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox txtEntry;
    private System.Windows.Forms.Timer tmrShowSuggest;
    private ListBox lbSuggestions;
    private System.Windows.Forms.Timer tmrFocus;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private Label label5;
    private Label label6;
    private Label label7;
}