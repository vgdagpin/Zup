﻿namespace Zup;


public partial class frmNewEntry : Form
{
    private AutoCompleteStringCollection SuggestionSource = new AutoCompleteStringCollection();

    public event EventHandler<NewEntryEventArgs>? OnNewEntryEvent;

    private string[]? Suggestions = null;

    public frmNewEntry()
    {
        InitializeComponent();

        txtEntry.AutoCompleteMode = AutoCompleteMode.None;
        txtEntry.AutoCompleteSource = AutoCompleteSource.CustomSource;
        txtEntry.AutoCompleteCustomSource = SuggestionSource;
    }

    private void frmNewEntry_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;

        txtEntry.Text = "";

        Hide();
    }

    private string? GetSelectedItem(bool forceFromTextBox = false)
    {
        var temp = lbSuggestions.Items.Count > 0
                ? lbSuggestions.Items[0].ToString()
                : txtEntry.Text.Trim();

        if (lbSuggestions.SelectedIndex > 0)
        {
            temp = lbSuggestions.SelectedItem?.ToString();
        }

        if (forceFromTextBox)
        {
            temp = txtEntry.Text.Trim();
        }

        return temp;
    }

    private void txtEntry_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            lbSuggestions.Enabled = false;
        }

        if (e.KeyCode == Keys.Escape)
        {
            e.SuppressKeyPress = true;
            Close();
        }
        // add to queue
        else if (e.KeyData == (Keys.Alt | Keys.Enter)
            || e.KeyData == (Keys.Alt | Keys.Control | Keys.Enter))
        {
            e.SuppressKeyPress = true;

            var temp = GetSelectedItem(e.Control);

            Close();

            if (!string.IsNullOrWhiteSpace(temp))
            {
                if (OnNewEntryEvent != null)
                {
                    OnNewEntryEvent(this, new NewEntryEventArgs(temp)
                    {
                        GetTags = true
                    });
                }
            }
        }
        // run in parallel
        else if (e.KeyData == (Keys.Shift | Keys.Enter)
            || e.KeyData == (Keys.Shift | Keys.Control | Keys.Enter))
        {
            e.SuppressKeyPress = true;

            var temp = GetSelectedItem(e.Control);

            Close();

            if (!string.IsNullOrWhiteSpace(temp))
            {
                if (OnNewEntryEvent != null)
                {
                    OnNewEntryEvent(this, new NewEntryEventArgs(temp)
                    {
                        StartNow = true,
                        GetTags = true
                    });
                }
            }
        }
        // run this and stop others
        else if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;

            var temp = GetSelectedItem(e.Control);

            Close();

            if (!string.IsNullOrWhiteSpace(temp))
            {
                if (OnNewEntryEvent != null)
                {
                    OnNewEntryEvent(this, new NewEntryEventArgs(temp)
                    {
                        StopOtherTask = true,
                        StartNow = true,
                        GetTags = true
                    });
                }
            }
        }
        else if (e.KeyCode == Keys.Down)
        {
            if (lbSuggestions.Items.Count == 0)
            {
                return;
            }

            if (lbSuggestions.SelectedIndex < 0)
            {
                lbSuggestions.SelectedIndex = 0;
            }

            if (lbSuggestions.SelectedIndex < lbSuggestions.Items.Count - 1)
            {
                lbSuggestions.SelectedIndex++;
            }
        }
        else if (e.KeyCode == Keys.Up)
        {
            if (lbSuggestions.Items.Count == 0)
            {
                return;
            }

            if (lbSuggestions.SelectedIndex < 0)
            {
                lbSuggestions.SelectedIndex = 0;
            }

            if (lbSuggestions.SelectedIndex > 0)
            {
                lbSuggestions.SelectedIndex--;
            }
        }
    }

    private void txtEntry_KeyUp(object sender, KeyEventArgs e)
    {
        lbSuggestions.Enabled = true;
    }

    public void ShowNewEntryDialog(params string[] suggestions)
    {
        if (Visible)
        {
            return;
        }

        Suggestions = suggestions;
        tmrFocus.Enabled = true;

        ShowDialog();
    }

    private void frmNewEntry_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible)
        {
            txtEntry.Focus();

            txtEntry.AutoCompleteCustomSource.Clear();

            if (Suggestions != null && Suggestions.Length > 0)
            {
                txtEntry.AutoCompleteCustomSource.AddRange(Suggestions);

                lbSuggestions.DataSource = Suggestions;
            }
        }
    }

    private void txtEntry_TextChanged(object sender, EventArgs e)
    {
        tmrShowSuggest.Stop();
        tmrShowSuggest.Start();
    }

    private void tmrShowSuggest_Tick(object sender, EventArgs e)
    {
        tmrShowSuggest.Stop();

        var searchText = txtEntry.Text.ToLower();
        var filteredSuggestions = SuggestionSource.Cast<string>()
            .Where(item => item.ToLower().Contains(searchText))
            .ToList();

        lbSuggestions.DataSource = filteredSuggestions;
    }

    private void tmrFocus_Tick(object sender, EventArgs e)
    {
        Activate();

        tmrFocus.Enabled = false;
    }

    private void lbSuggestions_DoubleClick(object sender, EventArgs e)
    {
        var temp = GetSelectedItem();

        Close();

        if (!string.IsNullOrWhiteSpace(temp))
        {
            if (OnNewEntryEvent != null)
            {
                OnNewEntryEvent(this, new NewEntryEventArgs(temp)
                {
                    StopOtherTask = true,
                    StartNow = true,
                    GetTags = true
                });
            }
        }
    }    
}