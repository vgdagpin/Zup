using System.ComponentModel;

namespace Zup.CustomControls;

public partial class TokenBox : FlowLayoutPanel
{
    private Color defaultTokenBorderColor = Color.DarkGray;
    private Color defaultTokenBorderColorHovered = Color.DarkGray;
    private Color defaultTokenTextColor = Color.Black;
    private Color defaultTokenForeColorHovered = Color.Blue;
    private Color defaultTokenBackgroundColor = Color.LightGray;
    private Color defaultTokenBackgroundColorHovered = Color.GhostWhite;

    private Font defaultTokenFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular);
    private Font defaultTokenFontHovered = new Font("Microsoft Sans Serif", 8F, FontStyle.Underline);

    public event EventHandler<TokenEventArgs>? TokenClicked;
    public event EventHandler<TokenEventArgs>? TokenDoubleClicked;
    public event EventHandler? TokenChanged;

    bool triggerChangeEvent = true;

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
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public List<string> AutoCompleteList { get; set; } = new List<string>();


    /// <summary>
    /// If set to true, user can write in TokenBox and Tab or Enter to add a new Token.
    /// </summary>
    public bool CanAddTokenByText { get; set; } = true;

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
    public IEnumerable<string> Tokens
    {
        get
        {
            return Controls.OfType<Token>().Select(a => a.Text).ToArray();
        }
        set
        {
            triggerChangeEvent = false;

            foreach (var item in value)
            {
                AddToken(item);
            }

            triggerChangeEvent = true;
        }
    }

    /// <summary>
    /// If set to True, a list of suggested texts will be shown to the user when writing the name of a new TokenBox. Needs to have CanAddTokenByText property set to True.
    /// </summary>
    public bool ShowAutoComplete { get; set; } = true;

   


    public bool CanWriteInTokenBox { get; set; } = true;

    /// <summary>
    /// If set to False, the user will not be able to delete Tokens using Backspace when cursor is in TokenBox. Needs CanAddTokenByText set to True.
    /// </summary>
    public bool CanDeleteTokensWithBackspace { get; set; } = true;

    /// <summary>
    /// If set to True, a red cross will be shown in the rightmost part of the Token. Clicking on this cross will delete Token.
    /// </summary>
    public bool ShowDeleteCross { get; set; } = true;

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
        if (Controls.OfType<Token>().Any(a => a.Text == text))
        {
            return;
        }

        var newToken = new Token(text, ShowDeleteCross);

        newToken.TokenColor = DefaultTokenBackgroundColor;
        newToken.TokenColorHovered = DefaultTokenBackgroundColorHovered;
        newToken.ForeColor = DefaultTokenForeColor;
        newToken.ForeColorHovered = DefaultTokenForeColorHovered;
        newToken.Font = DefaultTokenFont;
        newToken.FontHovered = DefaultTokenFontHovered;
        newToken.BorderColor = DefaultTokenBorderColor;
        newToken.BorderColorHovered = defaultTokenBorderColorHovered;
        newToken.NotifyParentEvent += NewToken_NotifyParentEvent; ;

        Controls.Add(newToken);

        Controls.SetChildIndex(AutoCompleteTextBox, Controls.Count - 1);

        if (TokenChanged != null && triggerChangeEvent)
        {
            TokenChanged(this, new EventArgs());
        }
    }

    private void NewToken_NotifyParentEvent(object? sender, TokenEventArgs e)
    {
        if (e.EventType == "Click")
        {
            TokenClicked?.Invoke(null, e);
        }
        if (e.EventType == "DoubleClick")
        {
            TokenDoubleClicked?.Invoke(null, e);
        }
        else if (e.EventType == "Remove")
        {
            if (TokenChanged != null && triggerChangeEvent)
            {
                TokenChanged(this, new EventArgs());
            }
        }
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
            Controls.RemoveAt(Controls.Count - 2);

            if (TokenChanged != null)
            {
                TokenChanged(sender, e);
            }
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
