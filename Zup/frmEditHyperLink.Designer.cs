namespace Zup;

partial class frmEditHyperLink
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
        btnSave = new Button();
        btnCancel = new Button();
        label1 = new Label();
        txtLinkText = new TextBox();
        txtLinkValue = new TextBox();
        label2 = new Label();
        SuspendLayout();
        // 
        // btnSave
        // 
        btnSave.Location = new Point(256, 199);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 23);
        btnSave.TabIndex = 2;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(337, 199);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 23);
        btnCancel.TabIndex = 3;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(8, 12);
        label1.Name = "label1";
        label1.Size = new Size(28, 15);
        label1.TabIndex = 2;
        label1.Text = "Text";
        // 
        // txtLinkText
        // 
        txtLinkText.Location = new Point(60, 9);
        txtLinkText.Name = "txtLinkText";
        txtLinkText.Size = new Size(352, 23);
        txtLinkText.TabIndex = 0;
        // 
        // txtLinkValue
        // 
        txtLinkValue.Location = new Point(60, 48);
        txtLinkValue.Multiline = true;
        txtLinkValue.Name = "txtLinkValue";
        txtLinkValue.Size = new Size(352, 121);
        txtLinkValue.TabIndex = 1;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(8, 53);
        label2.Name = "label2";
        label2.Size = new Size(22, 15);
        label2.TabIndex = 4;
        label2.Text = "Url";
        // 
        // frmEditHyperLink
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(425, 234);
        Controls.Add(txtLinkValue);
        Controls.Add(label2);
        Controls.Add(txtLinkText);
        Controls.Add(label1);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmEditHyperLink";
        Text = "Edit HyperLink";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button btnSave;
    private Button btnCancel;
    private Label label1;
    private Label label2;
    private TextBox txtLinkText;
    private TextBox txtLinkValue;
}