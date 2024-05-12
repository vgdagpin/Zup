using System.ComponentModel;

namespace Zup.CustomControls;

public partial class TokenBox : FlowLayoutPanel
{
    bool showAutoComplete = true;
    private bool canAddTokenByText = true;
    private bool canDeleteTokensWithBackspace = true;
    private bool canWriteInTokenBox = true;
    private bool showDeleteCross = true;

    private Color defaultTokenBorderColor = Color.DarkGray;
    private Color defaultTokenBorderColorHovered = Color.DarkGray;
    private Color defaultTokenTextColor = Color.Black;
    private Color defaultTokenForeColorHovered = Color.Blue;
    private Color defaultTokenBackgroundColor = Color.LightGray;
    private Color defaultTokenBackgroundColorHovered = Color.GhostWhite;

    private Font defaultTokenFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular);
    private Font defaultTokenFontHovered = new Font("Microsoft Sans Serif", 8F, FontStyle.Underline);

    public event EventHandler<TokenEventArgs> TokenClicked;

    #region Properties
    private AutoCompleteTextBox? acTxtBox;
    private AutoCompleteTextBox AutoCompleteTextBox
    {
        get
        {
            if (acTxtBox == null)
            {
                acTxtBox = new AutoCompleteTextBox();

                Controls.Add(acTxtBox);

                acTxtBox.Margin = new Padding(4, 7, 2, 4);

                acTxtBox.KeyDown += tb_KeyDown;
                acTxtBox.TextChanged += tb_TextChanged;
                acTxtBox.BackColor = BackColor;
                acTxtBox.Width = 15;
                acTxtBox.MinimumSize = acTxtBox.Size;
                acTxtBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                acTxtBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                acTxtBox.BorderStyle = BorderStyle.None;
                acTxtBox.Values = AutoCompleteList;

                BackColorChanged += acTxtBox.tokenBox_BackColorChanged;
                acTxtBox.PreviewKeyDown += TokenBox_PreviewKeyDown;

                acTxtBox.InitializeComponent();
            }

            return acTxtBox;
        }
    }

    private void TokenBox_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
    {
        OnPreviewKeyDown(e);
    }

    /// <summary>
    /// List of the suggested values to be shown if ShowAutoComplete is set to True.
    /// </summary>
    public List<string> AutoCompleteList { get; set; } = new List<string>();


    /// <summary>
    /// If set to true, user can write in TokenBox and Tab or Enter to add a new Token.
    /// </summary>
    public bool CanAddTokenByText
    {
        set
        {
            //AutoCompleteTextBox.ReadOnly = !value;
            //AutoCompleteTextBox.Text = String.Empty;
            canAddTokenByText = value;
        }
        get
        {
            return canAddTokenByText;
        }
    }

    /// <summary>
    /// Returns True if there are Tokens added in the TokenBox.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasTokens
    {
        get
        {
            return this.Controls.ContainsKey("Token");
        }
    }

    /// <summary>
    /// Returns a List of Tokens in the TokenBox.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<Token> Tokens
    {
        get
        {
            return Controls.OfType<Token>();
        }

    }

    /// <summary>
    /// If set to True, a list of suggested texts will be shown to the user when writing the name of a new TokenBox. Needs to have CanAddTokenByText property set to True.
    /// </summary>
    public bool ShowAutoComplete
    {
        get
        {
            return showAutoComplete;
        }

        set
        {
            showAutoComplete = value;
            //if (AutoCompleteTextBox.ShowAutoComplete != value)
            //{
            //    AutoCompleteTextBox.ShowAutoComplete = value;
            //}
        }
    }

   


    public bool CanWriteInTokenBox
    {
        get
        {
            return canWriteInTokenBox;
        }
        set
        {
            canWriteInTokenBox = value;

            //if (value)
            //{
            //    if (!Controls.Contains(AutoCompleteTextBox))
            //    { 
            //        Controls.Add(AutoCompleteTextBox); 
            //    }
            //}
            //else
            //{
            //    if (Controls.Contains(AutoCompleteTextBox))
            //    { 
            //        Controls.Remove(AutoCompleteTextBox); 
            //    }
            //}
        }
    }

    /// <summary>
    /// If set to False, the user will not be able to delete Tokens using Backspace when cursor is in TokenBox. Needs CanAddTokenByText set to True.
    /// </summary>
    public bool CanDeleteTokensWithBackspace
    {
        get
        {
            return canDeleteTokensWithBackspace;
        }

        set
        {
            canDeleteTokensWithBackspace = value;
        }
    }

    /// <summary>
    /// If set to True, a red cross will be shown in the rightmost part of the Token. Clicking on this cross will delete Token.
    /// </summary>
    public bool ShowDeleteCross
    {
        get
        {
            return showDeleteCross;
        }

        set
        {
            showDeleteCross = value;
        }
    }

    public Color DefaultTokenBackgroundColor
    {
        get
        {
            return defaultTokenBackgroundColor;
        }

        set
        {
            defaultTokenBackgroundColor = value;
        }
    }


    public Color DefaultTokenBorderColor
    {
        get
        {
            return defaultTokenBorderColor;
        }

        set
        {
            defaultTokenBorderColor = value;
        }
    }

    public Font DefaultTokenFont
    {
        get
        {
            return defaultTokenFont;
        }

        set
        {
            defaultTokenFont = value;
        }
    }

    public Color DefaultTokenForeColor
    {
        get
        {
            return defaultTokenTextColor;
        }

        set
        {
            defaultTokenTextColor = value;
        }
    }

    public Color DefaultTokenBackgroundColorHovered
    {
        get
        {
            return defaultTokenBackgroundColorHovered;
        }

        set
        {
            defaultTokenBackgroundColorHovered = value;
        }
    }


    public Color DefaultTokenBorderColorHovered
    {
        get
        {
            return defaultTokenBorderColorHovered;
        }

        set
        {
            defaultTokenBorderColorHovered = value;
        }
    }

    public Font DefaultTokenFontHovered
    {
        get
        {
            return defaultTokenFontHovered;
        }

        set
        {
            defaultTokenFontHovered = value;
        }
    }

    public Color DefaultTokenForeColorHovered
    {
        get
        {
            return defaultTokenForeColorHovered;
        }

        set
        {
            defaultTokenForeColorHovered = value;
        }
    }


    #endregion

    #region Constructors
    public TokenBox()
    {
        this.SuspendLayout();
        // 
        // TokenBox
        // 
        this.Name = "TokenBox";
        this.Size = new System.Drawing.Size(200, 25);
        this.ResumeLayout(false);


        
        this.MouseClick += mouseClick;
        this.ControlAdded += controlAdded;
        this.BackColor = Color.FromKnownColor(KnownColor.Window);
        this.Cursor = Cursors.IBeam;
        this.AutoScroll = true;
        this.AutoSize = false;
        this.Padding = new Padding(0, 0, 10, 0);
        this.WrapContents = true;
        this.BorderStyle = BorderStyle.FixedSingle;
        
        this.MouseEnter += OnMouseEnter;
    }

    #endregion Constructors

    #region Methods
    /// <summary>
    /// Adds a Token to the TokenBox.
    /// </summary>
    /// <param name="text">Text that will be shown in the Token.</param>
    public void AddToken(string text)
    {
        var newToken = new Token(text, ShowDeleteCross);

        newToken.TokenColor = DefaultTokenBackgroundColor;
        newToken.TokenColorHovered = DefaultTokenBackgroundColorHovered;
        newToken.ForeColor = DefaultTokenForeColor;
        newToken.ForeColorHovered = DefaultTokenForeColorHovered;
        newToken.Font = DefaultTokenFont;
        newToken.FontHovered = DefaultTokenFontHovered;
        newToken.BorderColor = DefaultTokenBorderColor;
        newToken.BorderColorHovered = defaultTokenBorderColorHovered;
        newToken.NotifyParentEvent += OnTokenClicked;

        Controls.Add(newToken);

        Controls.SetChildIndex(AutoCompleteTextBox, Controls.Count - 1);
    }

    public void RemoveToken(int Position)
    {
        if (Position > 0 && Position < Controls.Count - 1)
        {
            Controls.RemoveAt(Position);
        }
    }

    public void RemoveAllTokens()
    {
        for (int i = Controls.Count - 1; i >= 0; i--)
        {
            if (Controls[i] != AutoCompleteTextBox) 
            { 
                Controls.RemoveAt(i); 
            }
        }
    }

    public void ShowSuggestionList(string[] suggestions)
    {
        AutoCompleteTextBox.ShowExternalSuggestionList(suggestions);
    }
    #endregion Methods

    #region Events
    public void OnMouseEnter(object? sender, EventArgs e)
    {
        Cursor = CanWriteInTokenBox ? Cursors.IBeam : Cursors.Default;
    }

    public void OnTokenClicked(TokenEventArgs tokenEventArgs)
    {
        TokenClicked?.Invoke(null, tokenEventArgs);
    }

    private void mouseClick(object? sender, MouseEventArgs e)
    {
        AutoCompleteTextBox.Focus();
    }

    private void tb_KeyDown(object ?sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Back 
            && AutoCompleteTextBox.SelectionStart == 0 
            && AutoCompleteTextBox.SelectionLength == 0 
            && CanDeleteTokensWithBackspace 
            && Controls.Count - 1 > 0)
        {
            Controls.RemoveAt(this.Controls.Count - 2);
        }
    }

    private void tb_TextChanged(object? sender, EventArgs e)
    {
        var size = TextRenderer.MeasureText(AutoCompleteTextBox.Text, AutoCompleteTextBox.Font);

        AutoCompleteTextBox.Width = size.Width + 16;
    }

    private void controlAdded(object? sender, ControlEventArgs e)
    {
        var outermostControlLoc = Controls.OfType<Control>().Max(x => x.Location.Y + x.Height) + 9;

        if (Height < outermostControlLoc && AutoScroll)
        {
            if (MaximumSize.Height != 0 && MaximumSize.Height < outermostControlLoc)
            {
                Height = MaximumSize.Height;
            }
            else
            {
                Height = outermostControlLoc;
            }
        }
    }
    #endregion Events

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        AutoCompleteTextBox.Focus();
    }
}
