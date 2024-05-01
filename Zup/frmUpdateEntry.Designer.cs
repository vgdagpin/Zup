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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateEntry));
        rtbNote = new RichTextBox();
        lbNotes = new ListBox();
        menuStrip1 = new MenuStrip();
        editToolStripMenuItem = new ToolStripMenuItem();
        deleteEntryToolStripMenuItem = new ToolStripMenuItem();
        saveToolStripMenuItem = new ToolStripMenuItem();
        btnDeleteNote = new Button();
        btnNewNote = new Button();
        btnSaveNote = new Button();
        tmrFocus = new System.Windows.Forms.Timer(components);
        txtTask = new TextBox();
        dtFrom = new DateTimePicker();
        dtTo = new DateTimePicker();
        panel1 = new Panel();
        splitContainer1 = new SplitContainer();
        menuStrip1.SuspendLayout();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        SuspendLayout();
        // 
        // rtbNote
        // 
        rtbNote.AcceptsTab = true;
        rtbNote.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        rtbNote.BorderStyle = BorderStyle.FixedSingle;
        rtbNote.Location = new Point(0, 0);
        rtbNote.Name = "rtbNote";
        rtbNote.Size = new Size(572, 518);
        rtbNote.TabIndex = 3;
        rtbNote.Text = "";
        rtbNote.KeyDown += rtbNote_KeyDown;
        rtbNote.KeyPress += rtbNote_KeyPress;
        rtbNote.PreviewKeyDown += rtbNote_PreviewKeyDown;
        // 
        // lbNotes
        // 
        lbNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lbNotes.BorderStyle = BorderStyle.FixedSingle;
        lbNotes.DisplayMember = "Summary";
        lbNotes.DrawMode = DrawMode.OwnerDrawFixed;
        lbNotes.FormattingEnabled = true;
        lbNotes.ItemHeight = 15;
        lbNotes.Location = new Point(0, 0);
        lbNotes.Name = "lbNotes";
        lbNotes.Size = new Size(238, 422);
        lbNotes.TabIndex = 4;
        lbNotes.DrawItem += lbNotes_DrawItem;
        lbNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        lbNotes.KeyDown += lbNotes_KeyDown;
        // 
        // menuStrip1
        // 
        menuStrip1.Items.AddRange(new ToolStripItem[] { editToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(836, 24);
        menuStrip1.TabIndex = 6;
        menuStrip1.Text = "menuStrip1";
        // 
        // editToolStripMenuItem
        // 
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { deleteEntryToolStripMenuItem, saveToolStripMenuItem });
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.ShortcutKeys = Keys.Delete;
        editToolStripMenuItem.Size = new Size(39, 20);
        editToolStripMenuItem.Text = "Edit";
        // 
        // deleteEntryToolStripMenuItem
        // 
        deleteEntryToolStripMenuItem.Name = "deleteEntryToolStripMenuItem";
        deleteEntryToolStripMenuItem.ShortcutKeys = Keys.Delete;
        deleteEntryToolStripMenuItem.Size = new Size(161, 22);
        deleteEntryToolStripMenuItem.Text = "Delete Entry";
        deleteEntryToolStripMenuItem.Click += deleteEntryToolStripMenuItem_Click;
        // 
        // saveToolStripMenuItem
        // 
        saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        saveToolStripMenuItem.Size = new Size(161, 22);
        saveToolStripMenuItem.Text = "Save";
        saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
        // 
        // btnDeleteNote
        // 
        btnDeleteNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDeleteNote.Enabled = false;
        btnDeleteNote.Location = new Point(0, 466);
        btnDeleteNote.Name = "btnDeleteNote";
        btnDeleteNote.Size = new Size(128, 23);
        btnDeleteNote.TabIndex = 7;
        btnDeleteNote.Text = "Delete Note (Del)";
        btnDeleteNote.TextAlign = ContentAlignment.MiddleLeft;
        btnDeleteNote.UseVisualStyleBackColor = true;
        btnDeleteNote.Click += btnDeleteNote_Click;
        // 
        // btnNewNote
        // 
        btnNewNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnNewNote.Location = new Point(0, 442);
        btnNewNote.Name = "btnNewNote";
        btnNewNote.Size = new Size(128, 23);
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
        btnSaveNote.Location = new Point(0, 490);
        btnSaveNote.Name = "btnSaveNote";
        btnSaveNote.Size = new Size(128, 23);
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
        // txtTask
        // 
        txtTask.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtTask.Location = new Point(7, 33);
        txtTask.Name = "txtTask";
        txtTask.Size = new Size(817, 23);
        txtTask.TabIndex = 10;
        txtTask.KeyDown += txtTask_KeyDown;
        // 
        // dtFrom
        // 
        dtFrom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        dtFrom.CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
        dtFrom.Format = DateTimePickerFormat.Custom;
        dtFrom.Location = new Point(490, 62);
        dtFrom.Name = "dtFrom";
        dtFrom.Size = new Size(164, 23);
        dtFrom.TabIndex = 11;
        // 
        // dtTo
        // 
        dtTo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        dtTo.CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
        dtTo.Format = DateTimePickerFormat.Custom;
        dtTo.Location = new Point(660, 62);
        dtTo.Name = "dtTo";
        dtTo.Size = new Size(164, 23);
        dtTo.TabIndex = 12;
        dtTo.ValueChanged += dtTo_ValueChanged;
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        panel1.Controls.Add(splitContainer1);
        panel1.Location = new Point(7, 91);
        panel1.Name = "panel1";
        panel1.Size = new Size(817, 518);
        panel1.TabIndex = 13;
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.FixedPanel = FixedPanel.Panel1;
        splitContainer1.Location = new Point(0, 0);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(lbNotes);
        splitContainer1.Panel1.Controls.Add(btnNewNote);
        splitContainer1.Panel1.Controls.Add(btnDeleteNote);
        splitContainer1.Panel1.Controls.Add(btnSaveNote);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(rtbNote);
        splitContainer1.Size = new Size(817, 518);
        splitContainer1.SplitterDistance = 241;
        splitContainer1.TabIndex = 0;
        // 
        // frmUpdateEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(836, 614);
        Controls.Add(panel1);
        Controls.Add(dtTo);
        Controls.Add(dtFrom);
        Controls.Add(txtTask);
        Controls.Add(menuStrip1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = menuStrip1;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmUpdateEntry";
        Text = "Update Entry";
        FormClosing += frmUpdateEntry_FormClosing;
        Load += frmUpdateEntry_Load;
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        panel1.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private RichTextBox rtbNote;
    private ListBox lbNotes;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem editToolStripMenuItem;
    private Button btnDeleteNote;
    private Button btnNewNote;
    private Button btnSaveNote;
    private System.Windows.Forms.Timer tmrFocus;
    private TextBox txtTask;
    private DateTimePicker dtFrom;
    private DateTimePicker dtTo;
    private ToolStripMenuItem deleteEntryToolStripMenuItem;
    private ToolStripMenuItem saveToolStripMenuItem;
    private Panel panel1;
    private SplitContainer splitContainer1;
}