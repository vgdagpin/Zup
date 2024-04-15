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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateEntry));
        rtbNote = new RichTextBox();
        lbNotes = new ListBox();
        btnDelete = new Button();
        SuspendLayout();
        // 
        // rtbNote
        // 
        rtbNote.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        rtbNote.BorderStyle = BorderStyle.FixedSingle;
        rtbNote.Location = new Point(217, 33);
        rtbNote.Name = "rtbNote";
        rtbNote.Size = new Size(426, 323);
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
        lbNotes.Location = new Point(7, 9);
        lbNotes.Name = "lbNotes";
        lbNotes.Size = new Size(204, 347);
        lbNotes.TabIndex = 4;
        lbNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(568, 4);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(75, 23);
        btnDelete.TabIndex = 5;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // frmUpdateEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(655, 365);
        Controls.Add(btnDelete);
        Controls.Add(lbNotes);
        Controls.Add(rtbNote);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmUpdateEntry";
        Text = "frmUpdateEntry";
        FormClosing += frmUpdateEntry_FormClosing;
        Load += frmUpdateEntry_Load;
        ResumeLayout(false);
    }

    #endregion
    private RichTextBox rtbNote;
    private ListBox lbNotes;
    private Button btnDelete;
}