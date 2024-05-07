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
        btnDeleteNote = new Button();
        btnNewNote = new Button();
        btnSaveNote = new Button();
        tmrFocus = new System.Windows.Forms.Timer(components);
        txtTask = new TextBox();
        dtFrom = new DateTimePicker();
        dtTo = new DateTimePicker();
        pnlNotes = new GroupBox();
        splitContainer1 = new SplitContainer();
        label7 = new Label();
        lbPreviousNotes = new ListBox();
        btnSaveChanges = new Button();
        btnDelete = new Button();
        groupBox1 = new GroupBox();
        label3 = new Label();
        label2 = new Label();
        label1 = new Label();
        pnlNotes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        groupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // rtbNote
        // 
        rtbNote.AcceptsTab = true;
        rtbNote.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        rtbNote.Location = new Point(0, 0);
        rtbNote.Name = "rtbNote";
        rtbNote.Size = new Size(564, 377);
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
        lbNotes.DisplayMember = "Summary";
        lbNotes.DrawMode = DrawMode.OwnerDrawFixed;
        lbNotes.FormattingEnabled = true;
        lbNotes.ItemHeight = 15;
        lbNotes.Location = new Point(0, 0);
        lbNotes.Name = "lbNotes";
        lbNotes.Size = new Size(238, 169);
        lbNotes.TabIndex = 4;
        lbNotes.DrawItem += lbNotes_DrawItem;
        lbNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        lbNotes.KeyDown += lbNotes_KeyDown;
        // 
        // btnDeleteNote
        // 
        btnDeleteNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDeleteNote.Location = new Point(109, 380);
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
        btnNewNote.Location = new Point(3, 380);
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
        btnSaveNote.Location = new Point(435, 380);
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
        txtTask.Font = new Font("Segoe UI", 10F);
        txtTask.Location = new Point(106, 22);
        txtTask.Name = "txtTask";
        txtTask.Size = new Size(702, 25);
        txtTask.TabIndex = 10;
        txtTask.KeyDown += txtTask_KeyDown;
        // 
        // dtFrom
        // 
        dtFrom.CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
        dtFrom.Format = DateTimePickerFormat.Custom;
        dtFrom.Location = new Point(106, 53);
        dtFrom.Name = "dtFrom";
        dtFrom.Size = new Size(164, 23);
        dtFrom.TabIndex = 11;
        // 
        // dtTo
        // 
        dtTo.CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
        dtTo.Format = DateTimePickerFormat.Custom;
        dtTo.Location = new Point(106, 78);
        dtTo.Name = "dtTo";
        dtTo.Size = new Size(164, 23);
        dtTo.TabIndex = 12;
        dtTo.ValueChanged += dtTo_ValueChanged;
        // 
        // pnlNotes
        // 
        pnlNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlNotes.Controls.Add(splitContainer1);
        pnlNotes.Location = new Point(7, 128);
        pnlNotes.Name = "pnlNotes";
        pnlNotes.Size = new Size(815, 428);
        pnlNotes.TabIndex = 13;
        pnlNotes.TabStop = false;
        pnlNotes.Text = "Notes";
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.FixedPanel = FixedPanel.Panel1;
        splitContainer1.Location = new Point(3, 19);
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
        splitContainer1.Size = new Size(809, 406);
        splitContainer1.SplitterDistance = 241;
        splitContainer1.TabIndex = 0;
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Font = new Font("Segoe UI", 6.75F);
        label7.Location = new Point(0, 176);
        label7.Name = "label7";
        label7.Size = new Size(66, 12);
        label7.TabIndex = 11;
        label7.Text = "Previous Notes:";
        // 
        // lbPreviousNotes
        // 
        lbPreviousNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lbPreviousNotes.DisplayMember = "Summary";
        lbPreviousNotes.DrawMode = DrawMode.OwnerDrawFixed;
        lbPreviousNotes.FormattingEnabled = true;
        lbPreviousNotes.ItemHeight = 15;
        lbPreviousNotes.Location = new Point(0, 191);
        lbPreviousNotes.Name = "lbPreviousNotes";
        lbPreviousNotes.Size = new Size(238, 184);
        lbPreviousNotes.TabIndex = 10;
        lbPreviousNotes.DrawItem += lbNotes_DrawItem;
        lbPreviousNotes.SelectedIndexChanged += lbNotes_SelectedIndexChanged;
        // 
        // btnSaveChanges
        // 
        btnSaveChanges.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSaveChanges.Location = new Point(714, 80);
        btnSaveChanges.Name = "btnSaveChanges";
        btnSaveChanges.Size = new Size(94, 23);
        btnSaveChanges.TabIndex = 14;
        btnSaveChanges.Text = "Save Changes";
        btnSaveChanges.UseVisualStyleBackColor = true;
        btnSaveChanges.Click += btnSaveChanges_Click;
        // 
        // btnDelete
        // 
        btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDelete.Location = new Point(633, 80);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(75, 23);
        btnDelete.TabIndex = 15;
        btnDelete.Text = "Delete Task";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // groupBox1
        // 
        groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBox1.Controls.Add(label3);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(label1);
        groupBox1.Controls.Add(txtTask);
        groupBox1.Controls.Add(btnDelete);
        groupBox1.Controls.Add(dtFrom);
        groupBox1.Controls.Add(btnSaveChanges);
        groupBox1.Controls.Add(dtTo);
        groupBox1.Location = new Point(7, 12);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(815, 110);
        groupBox1.TabIndex = 16;
        groupBox1.TabStop = false;
        groupBox1.Text = "Task Detail";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(14, 81);
        label3.Name = "label3";
        label3.Size = new Size(59, 15);
        label3.TabIndex = 18;
        label3.Text = "Ended On";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(14, 59);
        label2.Name = "label2";
        label2.Size = new Size(63, 15);
        label2.TabIndex = 17;
        label2.Text = "Started On";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(14, 29);
        label1.Name = "label1";
        label1.Size = new Size(29, 15);
        label1.TabIndex = 16;
        label1.Text = "Task";
        // 
        // frmUpdateEntry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(834, 561);
        Controls.Add(groupBox1);
        Controls.Add(pnlNotes);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(850, 600);
        Name = "frmUpdateEntry";
        Text = "Update Entry";
        FormClosing += frmUpdateEntry_FormClosing;
        Load += frmUpdateEntry_Load;
        pnlNotes.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel1.PerformLayout();
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ResumeLayout(false);
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
    private GroupBox pnlNotes;
    private SplitContainer splitContainer1;
    private ListBox lbPreviousNotes;
    private Label label7;
    private Button btnSaveChanges;
    private Button btnDelete;
    private GroupBox groupBox1;
    private Label label3;
    private Label label2;
    private Label label1;
}