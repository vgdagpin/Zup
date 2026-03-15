namespace Zup;

partial class frmTagEditor
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTagEditor));
        lbTags = new ListBox();
        groupBox1 = new GroupBox();
        btnDelete = new Button();
        btnSaveChanges = new Button();
        txtDescription = new TextBox();
        label2 = new Label();
        txtName = new TextBox();
        label1 = new Label();
        groupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // lbTags
        // 
        lbTags.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lbTags.DisplayMember = "Name";
        lbTags.FormattingEnabled = true;
        lbTags.ItemHeight = 15;
        lbTags.Location = new Point(12, 21);
        lbTags.Name = "lbTags";
        lbTags.Size = new Size(184, 529);
        lbTags.TabIndex = 0;
        lbTags.SelectedIndexChanged += lbTags_SelectedIndexChanged;
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(btnDelete);
        groupBox1.Controls.Add(btnSaveChanges);
        groupBox1.Controls.Add(txtDescription);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(txtName);
        groupBox1.Controls.Add(label1);
        groupBox1.Location = new Point(202, 12);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(444, 538);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        groupBox1.Text = "Tag Detail";
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(305, 497);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(123, 23);
        btnDelete.TabIndex = 5;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnSaveChanges
        // 
        btnSaveChanges.Location = new Point(305, 109);
        btnSaveChanges.Name = "btnSaveChanges";
        btnSaveChanges.Size = new Size(123, 23);
        btnSaveChanges.TabIndex = 4;
        btnSaveChanges.Text = "Save Changes";
        btnSaveChanges.UseVisualStyleBackColor = true;
        btnSaveChanges.Click += btnSaveChanges_Click;
        // 
        // txtDescription
        // 
        txtDescription.Location = new Point(84, 66);
        txtDescription.Name = "txtDescription";
        txtDescription.Size = new Size(344, 23);
        txtDescription.TabIndex = 3;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(11, 69);
        label2.Name = "label2";
        label2.Size = new Size(67, 15);
        label2.TabIndex = 2;
        label2.Text = "Description";
        // 
        // txtName
        // 
        txtName.Location = new Point(84, 37);
        txtName.Name = "txtName";
        txtName.Size = new Size(344, 23);
        txtName.TabIndex = 1;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(11, 40);
        label1.Name = "label1";
        label1.Size = new Size(39, 15);
        label1.TabIndex = 0;
        label1.Text = "Name";
        // 
        // frmTagEditor
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(662, 572);
        Controls.Add(groupBox1);
        Controls.Add(lbTags);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmTagEditor";
        SizeGripStyle = SizeGripStyle.Hide;
        Text = "Tag Editor";
        VisibleChanged += frmTagEditor_VisibleChanged;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private ListBox lbTags;
    private GroupBox groupBox1;
    private TextBox txtName;
    private Label label1;
    private TextBox txtDescription;
    private Label label2;
    private Button btnDelete;
    private Button btnSaveChanges;
}