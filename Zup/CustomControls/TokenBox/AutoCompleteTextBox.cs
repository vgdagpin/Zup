namespace Zup.CustomControls;

/// <summary>
/// This TokenProject borrows code from:
/// http://autocompletetexboxcs.codeplex.com/
/// Thank you to Peter Holpar for sharing his work.
/// Under MS-Public License (below)
/// </summary>
public class AutoCompleteTextBox : TextBox
{
    private TokenBox ParentTokenBox = null!;
    private Form ParentForm = null!;


    private ListBox lbSuggestions = null!;
    private bool lbSuggestAddedToControl;
    private string _formerValue = string.Empty;
    private int _MouseIndex = -1;

    #region Properties

    public bool ShowAutoComplete { get; set; } = true;

    public List<string> Values { get; set; } = new List<string>();

    public string[] SelectedValues
    {
        get
        {
            return Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
    #endregion //Properties

    public void InitializeComponent()
    {
        lbSuggestions = new ListBox();
        //Events
        lbSuggestions.MouseClick += _listBox_MouseClick;
        lbSuggestions.MouseMove += _listBox_MouseMove;
        KeyDown += this_KeyDown;
        KeyUp += this_KeyUp;

        ParentTokenBox = (TokenBox)Parent!;
        ParentForm = GetParentForm(this);
    }

    #region Events

    private void _listBox_MouseMove(object? sender, MouseEventArgs e)
    {
        var index = lbSuggestions.IndexFromPoint(e.Location);

        if (index != -1 && index != _MouseIndex)
        {
            if (_MouseIndex != -1)
            {
                lbSuggestions.SetSelected(_MouseIndex, false);
            }
            _MouseIndex = index;
            lbSuggestions.SetSelected(_MouseIndex, true);
            lbSuggestions.Invalidate();

        }
    }

    private void _listBox_MouseClick(object? sender, MouseEventArgs e)
    {
        var selectedToken = ((ListBox)sender!).SelectedItem!.ToString()!;

        IntroduceToken(selectedToken);

        Focus();
    }

    public void tokenBox_BackColorChanged(object? sender, EventArgs e)
    {
        BackColor = ((TokenBox)sender!).BackColor;
    }

    private void this_KeyUp(object? sender, KeyEventArgs e)
    {
        if (ShowAutoComplete)
        {
            UpdateListBoxWithLocalMatches();
        }
    }

    private void this_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.S && e.Control)
        {
            e.SuppressKeyPress = true;
        }

        switch (e.KeyCode)
        {
            case Keys.Enter:
            case Keys.Tab:
            case Keys.Space:
                e.SuppressKeyPress = true;
                AcceptInput();
                break;
            case Keys.Down:
                if (lbSuggestions.Visible)
                {
                    e.SuppressKeyPress = true;

                    if (lbSuggestions.SelectedIndex < lbSuggestions.Items.Count - 1)
                    {
                        lbSuggestions.SelectedIndex++;
                    }
                }
                break;
            case Keys.Up:
                if (lbSuggestions.Visible)
                {
                    e.SuppressKeyPress = true;

                    if (lbSuggestions.SelectedIndex > 0)
                    {
                        lbSuggestions.SelectedIndex--;
                    }
                }
                break;
            default:
                break;
        }
    }


    protected override void OnLeave(EventArgs e)
    {
        base.OnLeave(e);
        
        AcceptInput();
    }
    #endregion Events

    #region Methods
    private void IntroduceToken(string textToken)
    {
        if (string.IsNullOrWhiteSpace(textToken))
        {
            return;
        }

        ParentTokenBox.AddToken(textToken);

        Text = string.Empty;        
    }

    public void ShowExternalSuggestionList(string[] suggestionList)
    {
        var widthListBox = lbSuggestions.Width;

        using (var g = Graphics.FromHwnd(IntPtr.Zero))
        {
            foreach (var suggestion in suggestionList)
            {
                var sizeText = g.MeasureString(suggestion, this.lbSuggestions.Font);

                if (sizeText.Width > widthListBox)
                {
                    widthListBox = (int)sizeText.Width;
                }

                if (!lbSuggestions.Items.Contains(suggestion))
                {
                    lbSuggestions.Items.Add(suggestion);
                }
            }
        }

        lbSuggestions.Width = widthListBox;
        lbSuggestions.ItemHeight = 15;

        if (!lbSuggestAddedToControl)
        {
            ParentForm.Controls.Add(lbSuggestions);

            lbSuggestAddedToControl = true;
        }

        lbSuggestions.Top = Bottom + ParentTokenBox.Top;
        lbSuggestions.Left = Left + ParentTokenBox.Left;

        if (lbSuggestions.Right + Parent!.Left > ParentForm.Width)
        {
            lbSuggestions.Left -= lbSuggestions.Right - ParentForm.Width;
        }

        lbSuggestions.Visible = true;
        lbSuggestions.BringToFront();
    }

    private Form GetParentForm(Control? control)
    {
        if (control == null)
        {
            throw new ArgumentNullException("Control");
        }

        var t = control.GetType();

        if (control.Parent is Form parent)
        {
            return parent;
        }

        return GetParentForm(control.Parent);
    }

    private void ShowListBox()
    {
        if (ShowAutoComplete)
        {
            if (!lbSuggestAddedToControl)
            {
                ParentForm.Controls.Add(lbSuggestions);

                lbSuggestAddedToControl = true;
            }

            lbSuggestions.Top = Bottom + ParentTokenBox.Top;
            lbSuggestions.Left = Left + ParentTokenBox.Left;
            lbSuggestions.Visible = true;

            lbSuggestions.BringToFront();
        }
    }

    private void ResetListBox()
    {
        lbSuggestions.Visible = false;
        _MouseIndex = -1;
    }



    private void AcceptInput()
    {
        if (lbSuggestions.Visible)
        {
            if (lbSuggestions.SelectedIndex == -1)
            {
                if (lbSuggestions.Items.Count > 0)
                {
                    lbSuggestions.SelectedIndex = 0;

                    Focus();
                }                

                return;
            }

            var suggestSelection = (string)lbSuggestions.SelectedItem!;

            if (!string.IsNullOrWhiteSpace(suggestSelection))
            {
                IntroduceToken(suggestSelection);
                _formerValue = Text;
                ResetListBox();
                Focus();
            }            
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                ParentTokenBox.AddToken(Text.Trim());

                Clear();
                Focus();
            }
        }
    }

    private void UpdateListBoxWithLocalMatches()
    {
        if (Text == _formerValue) return;

        _formerValue = Text;

        var word = GetWord();

        if (Values != null && word.Length > 0)
        {
            var matches = Values
                .Where(a => a.Contains(word, StringComparison.OrdinalIgnoreCase) && !ParentTokenBox.Tokens.Contains(a))
                .ToArray();

            if (matches.Length > 0)
            {
                ShowListBox();
                lbSuggestions.Items.Clear();
                Array.ForEach(matches, x => lbSuggestions.Items.Add(x));
                //_listBox.SelectedIndex = 0;
                lbSuggestions.Height = 0;
                lbSuggestions.Width = 0;
                Focus();
                using (var graphics = lbSuggestions.CreateGraphics())
                {
                    for (int i = 0; i < lbSuggestions.Items.Count; i++)
                    {
                        lbSuggestions.Height += lbSuggestions.GetItemHeight(i);
                        // it item width is larger than the current one
                        // set it to the new max item width
                        // GetItemRectangle does not work for me
                        // we add a little extra space by using '_'
                        int itemWidth = (int)graphics.MeasureString(((string)lbSuggestions.Items[i]) + "_", lbSuggestions.Font).Width;
                        
                        lbSuggestions.Width = (lbSuggestions.Width < itemWidth) ? itemWidth : lbSuggestions.Width;
                    }
                }
            }
            else
            {
                ResetListBox();
            }
        }
        else
        {
            ResetListBox();
        }
    }

    private string GetWord()
    {
        int pos = SelectionStart;

        int posStart = Text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
        posStart = (posStart == -1) ? 0 : posStart + 1;
        int posEnd = Text.IndexOf(' ', pos);
        posEnd = (posEnd == -1) ? Text.Length : posEnd;

        int length = ((posEnd - posStart) < 0) ? 0 : posEnd - posStart;

        return Text.Substring(posStart, length);
    }
    #endregion Methods
}