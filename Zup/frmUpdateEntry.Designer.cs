namespace Zup;

partial class frmUpdateEntry
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateEntry));
        label2 = new Label();
        rtbNote = new RichTextBox();
        lbNotes = new ListBox();
        SuspendLayout();
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(12, 9);
        label2.Name = "label2";
        label2.Size = new Size(38, 15);
        label2.TabIndex = 2;
        label2.Text = "Notes";
        // 
        // rtbNote
        // 
        rtbNote.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        rtbNote.BorderStyle = BorderStyle.FixedSingle;
        rtbNote.Location = new Point(217, 10);
        rtbNote.Name = "rtbNote";
        rtbNote.Size = new Size(426, 346);
        rtbNote.TabIndex = 3;
        rtbNote.Text = "";
        rtbNote.PreviewKeyDown += rtbNote_PreviewKeyDown;
        // 
        // lbNotes
        // 
        lbNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lbNotes.BorderStyle = BorderStyle.FixedSingle;
        lbNotes.DisplayMember = "Summary";
        lbNotes.FormattingEnabled = true;
        lbNotes.ItemHeight = 15;
        lbNotes.Location = new Point(67, 9);
        lbNotes.Name = "lbNotes";
        lbNotes.Size = new Size(144, 347);
        lbNotes.TabIndex = 4;
        lbNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        // 
        // frmUpdateEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(655, 365);
        Controls.Add(lbNotes);
        Controls.Add(rtbNote);
        Controls.Add(label2);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmUpdateEntry";
        Text = "frmUpdateEntry";
        FormClosing += frmUpdateEntry_FormClosing;
        Load += frmUpdateEntry_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Label label2;
    private RichTextBox rtbNote;
    private ListBox lbNotes;
}