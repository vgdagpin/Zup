using System.Text;

namespace Zup;

public partial class frmEditHyperLink : Form
{
    public EventHandler<string>? SaveChanges;

    public frmEditHyperLink(string text, string rtf)
    {
        InitializeComponent();

        txtLinkText.Text = text;
        txtLinkValue.Text = rtf;

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

    protected string GetResult()
    {
        var sb = new StringBuilder();

        sb.Append(@"{\rtf1\ansi{\field{\*\fldinst{HYPERLINK ");
        sb.Append("http://www.google.com");
        sb.Append(@"}}{\fldrslt{");
        sb.Append(txtLinkText.Text);
        sb.Append(@"}}}}");

        return sb.ToString();
    }
}
