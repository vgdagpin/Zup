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
        btnDeleteNote = new Button();
        btnNewNote = new Button();
        btnSaveNote = new Button();
        tmrFocus = new System.Windows.Forms.Timer(components);
        txtTask = new TextBox();
        dtFrom = new DateTimePicker();
        dtTo = new DateTimePicker();
        panel1 = new Panel();
        splitContainer1 = new SplitContainer();
        label7 = new Label();
        lbPreviousNotes = new ListBox();
        btnSaveChanges = new Button();
        btnDelete = new Button();
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
        rtbNote.Size = new Size(572, 489);
        rtbNote.TabIndex = 3;
        rtbNote.Text = "";
        rtbNote.LinkClicked += rtbNote_LinkClicked;
        rtbNote.KeyDown += rtbNote_KeyDown;
        rtbNote.KeyPress += rtbNote_KeyPress;
        rtbNote.PreviewKeyDown += rtbNote_PreviewKeyDown;
        // 
        // lbNotes
        // 
        lbNotes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lbNotes.BorderStyle = BorderStyle.FixedSingle;
        lbNotes.DisplayMember = "Summary";
        lbNotes.DrawMode = DrawMode.OwnerDrawFixed;
        lbNotes.FormattingEnabled = true;
        lbNotes.ItemHeight = 15;
        lbNotes.Location = new Point(0, 0);
        lbNotes.Name = "lbNotes";
        lbNotes.Size = new Size(238, 152);
        lbNotes.TabIndex = 4;
        lbNotes.DrawItem += lbNotes_DrawItem;
        lbNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        lbNotes.KeyDown += lbNotes_KeyDown;
        // 
        // btnDeleteNote
        // 
        btnDeleteNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDeleteNote.Location = new Point(109, 492);
        btnDeleteNote.Name = "btnDeleteNote";
        btnDeleteNote.Size = new Size(105, 23);
        btnDeleteNote.TabIndex = 7;
        btnDeleteNote.Text = "Delete Note (Del)";
        btnDeleteNote.UseVisualStyleBackColor = true;
        btnDeleteNote.Visible = false;
        btnDeleteNote.Click += btnDeleteNote_Click;
        // 
        // btnNewNote
        // 
        btnNewNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnNewNote.Location = new Point(3, 492);
        btnNewNote.Name = "btnNewNote";
        btnNewNote.Size = new Size(102, 23);
        btnNewNote.TabIndex = 8;
        btnNewNote.Text = "Clear (CTRL+N)";
        btnNewNote.UseVisualStyleBackColor = true;
        btnNewNote.Click += btnNewNote_Click;
        // 
        // btnSaveNote
        // 
        btnSaveNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSaveNote.Location = new Point(443, 492);
        btnSaveNote.Name = "btnSaveNote";
        btnSaveNote.Size = new Size(126, 23);
        btnSaveNote.TabIndex = 9;
        btnSaveNote.Text = "Save Note (CTRL+S)";
        btnSaveNote.UseVisualStyleBackColor = true;
        btnSaveNote.Visible = false;
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
        splitContainer1.Panel1.Controls.Add(label7);
        splitContainer1.Panel1.Controls.Add(lbPreviousNotes);
        splitContainer1.Panel1.Controls.Add(lbNotes);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(rtbNote);
        splitContainer1.Panel2.Controls.Add(btnNewNote);
        splitContainer1.Panel2.Controls.Add(btnSaveNote);
        splitContainer1.Panel2.Controls.Add(btnDeleteNote);
        splitContainer1.Size = new Size(817, 518);
        splitContainer1.SplitterDistance = 241;
        splitContainer1.TabIndex = 0;
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Font = new Font("Segoe UI", 6.75F);
        label7.Location = new Point(0, 171);
        label7.Name = "label7";
        label7.Size = new Size(66, 12);
        label7.TabIndex = 11;
        label7.Text = "Previous Notes:";
        // 
        // lbPreviousNotes
        // 
        lbPreviousNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lbPreviousNotes.BorderStyle = BorderStyle.FixedSingle;
        lbPreviousNotes.DisplayMember = "Summary";
        lbPreviousNotes.DrawMode = DrawMode.OwnerDrawFixed;
        lbPreviousNotes.FormattingEnabled = true;
        lbPreviousNotes.ItemHeight = 15;
        lbPreviousNotes.Location = new Point(0, 186);
        lbPreviousNotes.Name = "lbPreviousNotes";
        lbPreviousNotes.Size = new Size(238, 302);
        lbPreviousNotes.TabIndex = 10;
        lbPreviousNotes.DrawItem += lbNotes_DrawItem;
        lbPreviousNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        // 
        // btnSaveChanges
        // 
        btnSaveChanges.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSaveChanges.Location = new Point(695, 4);
        btnSaveChanges.Name = "btnSaveChanges";
        btnSaveChanges.Size = new Size(129, 23);
        btnSaveChanges.TabIndex = 14;
        btnSaveChanges.Text = "Save Changes";
        btnSaveChanges.UseVisualStyleBackColor = true;
        btnSaveChanges.Click += btnSaveChanges_Click;
        // 
        // btnDelete
        // 
        btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDelete.Location = new Point(614, 4);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(75, 23);
        btnDelete.TabIndex = 15;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // frmUpdateEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(836, 614);
        Controls.Add(btnDelete);
        Controls.Add(btnSaveChanges);
        Controls.Add(panel1);
        Controls.Add(dtTo);
        Controls.Add(dtFrom);
        Controls.Add(txtTask);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmUpdateEntry";
        Text = "Update Entry";
        FormClosing += frmUpdateEntry_FormClosing;
        Load += frmUpdateEntry_Load;
        panel1.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel1.PerformLayout();
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private RichTextBox rtbNote;
    private ListBox lbNotes;
    private Button btnDeleteNote;
    private Button btnNewNote;
    private Button btnSaveNote;
    private System.Windows.Forms.Timer tmrFocus;
    private TextBox txtTask;
    private DateTimePicker dtFrom;
    private DateTimePicker dtTo;
    private Panel panel1;
    private SplitContainer splitContainer1;
    private ListBox lbPreviousNotes;
    private Label label7;
    private Button btnSaveChanges;
    private Button btnDelete;
}