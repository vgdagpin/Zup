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
        // 
        // tmrShowSuggest
        // 
        tmrShowSuggest.Tick += tmrShowSuggest_Tick;
        // 
        // listBox1
        // 
        lbSuggestions.FormattingEnabled = true;
        lbSuggestions.ItemHeight = 15;
        lbSuggestions.Location = new Point(12, 43);
        lbSuggestions.Name = "listBox1";
        lbSuggestions.Size = new Size(511, 64);
        lbSuggestions.TabIndex = 1;
        lbSuggestions.DoubleClick += lbSuggestions_DoubleClick;
        // 
        // tmrFocus
        // 
        tmrFocus.Tick += tmrFocus_Tick;
        // 
        // frmNewEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(537, 120);
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
}