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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateEntry));
        rtbNote = new RichTextBox();
        lbNotes = new ListBox();
        menuStrip1 = new MenuStrip();
        deleteEntryToolStripMenuItem = new ToolStripMenuItem();
        btnDeleteNote = new Button();
        btnNewNote = new Button();
        btnSaveNote = new Button();
        tmrFocus = new System.Windows.Forms.Timer(components);
        menuStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // rtbNote
        // 
        rtbNote.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        rtbNote.BorderStyle = BorderStyle.FixedSingle;
        rtbNote.Location = new Point(217, 39);
        rtbNote.Name = "rtbNote";
        rtbNote.Size = new Size(426, 327);
        rtbNote.TabIndex = 3;
        rtbNote.Text = "";
        rtbNote.KeyDown += rtbNote_KeyDown;
        rtbNote.KeyPress += rtbNote_KeyPress;
        rtbNote.PreviewKeyDown += rtbNote_PreviewKeyDown;
        // 
        // lbNotes
        // 
        lbNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lbNotes.BorderStyle = BorderStyle.FixedSingle;
        lbNotes.DisplayMember = "Summary";
        lbNotes.FormattingEnabled = true;
        lbNotes.ItemHeight = 15;
        lbNotes.Location = new Point(7, 39);
        lbNotes.Name = "lbNotes";
        lbNotes.Size = new Size(204, 242);
        lbNotes.TabIndex = 4;
        lbNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        lbNotes.KeyDown += lbNotes_KeyDown;
        // 
        // menuStrip1
        // 
        menuStrip1.Items.AddRange(new ToolStripItem[] { deleteEntryToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(655, 24);
        menuStrip1.TabIndex = 6;
        menuStrip1.Text = "menuStrip1";
        // 
        // deleteEntryToolStripMenuItem
        // 
        deleteEntryToolStripMenuItem.Name = "deleteEntryToolStripMenuItem";
        deleteEntryToolStripMenuItem.ShortcutKeys = Keys.Delete;
        deleteEntryToolStripMenuItem.Size = new Size(82, 20);
        deleteEntryToolStripMenuItem.Text = "Delete Entry";
        deleteEntryToolStripMenuItem.Click += deleteEntryToolStripMenuItem_Click;
        // 
        // btnDeleteNote
        // 
        btnDeleteNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDeleteNote.Enabled = false;
        btnDeleteNote.Location = new Point(7, 316);
        btnDeleteNote.Name = "btnDeleteNote";
        btnDeleteNote.Size = new Size(204, 23);
        btnDeleteNote.TabIndex = 7;
        btnDeleteNote.Text = "Delete Note (Del)";
        btnDeleteNote.TextAlign = ContentAlignment.MiddleLeft;
        btnDeleteNote.UseVisualStyleBackColor = true;
        btnDeleteNote.Click += btnDeleteNote_Click;
        // 
        // btnNewNote
        // 
        btnNewNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnNewNote.Location = new Point(7, 287);
        btnNewNote.Name = "btnNewNote";
        btnNewNote.Size = new Size(204, 23);
        btnNewNote.TabIndex = 8;
        btnNewNote.Text = "New Note (CTRL+N)";
        btnNewNote.TextAlign = ContentAlignment.MiddleLeft;
        btnNewNote.UseVisualStyleBackColor = true;
        btnNewNote.Click += btnNewNote_Click;
        // 
        // btnSaveNote
        // 
        btnSaveNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnSaveNote.Enabled = false;
        btnSaveNote.Location = new Point(7, 345);
        btnSaveNote.Name = "btnSaveNote";
        btnSaveNote.Size = new Size(204, 23);
        btnSaveNote.TabIndex = 9;
        btnSaveNote.Text = "Save Note (CTRL+S)";
        btnSaveNote.TextAlign = ContentAlignment.MiddleLeft;
        btnSaveNote.UseVisualStyleBackColor = true;
        btnSaveNote.Click += btnSaveNote_Click;
        // 
        // tmrFocus
        // 
        tmrFocus.Tick += tmrFocus_Tick;
        // 
        // frmUpdateEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(655, 375);
        Controls.Add(btnSaveNote);
        Controls.Add(btnNewNote);
        Controls.Add(btnDeleteNote);
        Controls.Add(lbNotes);
        Controls.Add(rtbNote);
        Controls.Add(menuStrip1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = menuStrip1;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmUpdateEntry";
        Text = "frmUpdateEntry";
        FormClosing += frmUpdateEntry_FormClosing;
        Load += frmUpdateEntry_Load;
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private RichTextBox rtbNote;
    private ListBox lbNotes;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem deleteEntryToolStripMenuItem;
    private Button btnDeleteNote;
    private Button btnNewNote;
    private Button btnSaveNote;
    private System.Windows.Forms.Timer tmrFocus;
}