using System.Data;

using Zup.Entities;

namespace Zup;
public partial class frmTagEditor : Form
{
    private readonly ZupDbContext p_DbContext;

    bool IsListLoaded = false;
    Guid? SelectedTagID = null;

    public frmTagEditor(ZupDbContext dbContext)
    {
        InitializeComponent();
        p_DbContext = dbContext;
    }

    private void frmTagEditor_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible)
        {
            LoadAllTags();
        }
    }

    private void LoadAllTags()
    {
        IsListLoaded = false;
        SelectedTagID = null;

        lbTags.DataSource = p_DbContext.Tags.OrderBy(a => a.Name).ToList();

        lbTags.ClearSelected();

        IsListLoaded = true;
    }

    private void lbTags_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedTagID = null;
        txtName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        btnSaveChanges.Enabled = false;
        btnDelete.Enabled = false;
        txtName.Enabled = false;
        txtDescription.Enabled = false;

        if (!IsListLoaded)
        {
            return;
        }

        var item = lbTags.SelectedItem as tbl_Tag;

        if (item != null)
        {
            btnSaveChanges.Enabled = true;
            btnDelete.Enabled = true;
            txtName.Enabled = true;
            txtDescription.Enabled = true;

            SelectedTagID = item.ID;
            txtName.Text = item.Name;
            txtDescription.Text = item.Description;
        }
    }

    private void btnSaveChanges_Click(object sender, EventArgs e)
    {
        var curTag = p_DbContext.Tags.Find(SelectedTagID);

        if (curTag == null)
        {
            return;
        }

        curTag.Name = txtName.Text;
        curTag.Description = txtDescription.Text;

        p_DbContext.SaveChanges();

        MessageBox.Show("Tag updated", "Tag");
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        var curTag = p_DbContext.Tags.Find(SelectedTagID);

        if (curTag == null)
        {
            return;
        }

        if (MessageBox.Show("Delete Tag", "Tag", MessageBoxButtons.OKCancel) != DialogResult.OK)
        {
            return;
        }

        if (p_DbContext.TaskEntryTags.Any(a => a.TagID == curTag.ID))
        {
            MessageBox.Show("Tag being used.", "Tag");

            return;
        }

        p_DbContext.Tags.Remove(curTag);

        p_DbContext.SaveChanges();

        LoadAllTags();

        MessageBox.Show("Tag deleted", "Tag");

        SelectedTagID = null;
        txtName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        btnSaveChanges.Enabled = false;
        btnDelete.Enabled = false;
        txtName.Enabled = false;
        txtDescription.Enabled = false;
    }

    public void SelectTag(string tagName)
    {
        var data = lbTags.Items.Cast<tbl_Tag>().FirstOrDefault(a => a.Name == tagName);

        if (data != null)
        {
            lbTags.SelectedIndex = lbTags.Items.IndexOf(data);
        }
    }
}
