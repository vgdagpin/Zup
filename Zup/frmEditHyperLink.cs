namespace Zup;

public partial class frmEditHyperLink : Form
{
    public EventHandler<string>? SaveChanges;

    public frmEditHyperLink(string selectedRtf)
    {
        InitializeComponent();

        ParseRTF(selectedRtf);
    }

    private void ParseRTF(string selectedRtf)
    {
        txtLinkText.Text = selectedRtf;
        txtLinkValue.Text = selectedRtf;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveChanges?.Invoke(sender, txtLinkValue.Text);

        Close();
    }
}
