using System.ComponentModel;
using System.Drawing.Drawing2D;

using Zup.Properties;

namespace Zup.CustomControls;

public partial class Token : Control
{
    public event EventHandler<TokenEventArgs>? NotifyParentEvent;
    private Rectangle rCloseX = new Rectangle(0, 0, 0, 0);
    private Rectangle rText = new Rectangle(0, 0, 0, 0);
    private Size sizeIcon = new Size(16, 16);
    private bool isBeingHovered = false;
    #region Properties

    /// <summary>
    /// How rounded token corners will be.
    /// 0 --> Square corners, 10 --> Very round. Default 4.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Radius { get; set; } = 4;

    public override Color BackColor => base.BackColor;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int BorderWidth { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool ShowsX { get; set; } = true;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Font FontHovered { get; set; } = null!;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color ForeColorHovered { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color TokenColorHovered { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color TokenColor { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColor { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColorHovered { get; set; }

    #endregion Properties

    #region Constructors
    public Token(string textToDisplay, bool showX = true)
    {
        InitializeComponent();

        //Set default property values for the button during start up
        this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        this.BackColor = Color.Transparent;
        this.ShowsX = showX;
        this.Text = textToDisplay;
        this.Margin = new Padding(1, 0, 1, 0);

        AdjustSize();
    }

    #endregion Constructors

    private void AdjustSize()
    {
        SizeF sizeText;
        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
        {
            sizeText = g.MeasureString(base.Text, this.Font);
        }

        var initialRectangle = new Point(3, 3);

        var sizeDisplayedText = new Size((int)sizeText.Width + 3, (int)sizeText.Height + 1);
        var offsetCenterVerticalText = (sizeIcon.Height - sizeDisplayedText.Height) / 2;
        rText = new Rectangle(new Point(3, initialRectangle.Y + offsetCenterVerticalText + 2), sizeDisplayedText);

        rCloseX.Location = new Point(rText.Right + 1, initialRectangle.Y);
        if (this.ShowsX)
        {
            rCloseX.Size = sizeIcon;
        }

        this.Size = new Size(rCloseX.Right + 1, sizeIcon.Height + 6);
    }

    #region Events
    /// <summary>
    /// Default handler.Nothing to do here since we don't need to repaint the button.
    /// </summary>
    /// <param name="pe"></param>
    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);

        Rectangle rFondo = this.DisplayRectangle;

        rFondo.X += 1;
        rFondo.Y += 1;
        rFondo.Width -= 2;
        rFondo.Height -= 2;
        Color colorBgToken;
        Color colorBorder;
        Color colorText;
        Font fontText;
        if (this.isBeingHovered)
        {
            colorBgToken = TokenColorHovered;
            colorBorder = BorderColorHovered;
            colorText = ForeColorHovered;
            fontText = FontHovered;
        }
        else
        {
            colorBgToken = TokenColor;
            colorBorder = BorderColor;
            colorText = ForeColor;
            fontText = Font;
        }

        using (var bb = GetPathRoundCorners(rFondo, Radius))
        {
            //BACKGROUND
            using (var br = new SolidBrush(colorBgToken))
            {
                pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                pe.Graphics.FillPath(br, bb);
            }

            //BORDER
            using (Brush br = new SolidBrush(colorBorder))
            {

                pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                pe.Graphics.DrawPath(new Pen(br, BorderWidth), bb);
            }
        }

        //TEXT
        pe.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        pe.Graphics.DrawString(this.Text, fontText, new SolidBrush(colorText), rText);

        //CROSS
        if (this.ShowsX)
        {
            if (isBeingHovered) pe.Graphics.DrawImage(Resources.CrossRed, rCloseX);
            else pe.Graphics.DrawImage(Resources.CrossBlack, rCloseX);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);

        Cursor = Cursors.Hand;
        isBeingHovered = true;
        Refresh();
    }

    /// <summary>
    /// Event handler which call SetValuesOnFocus() method to give apecial
    /// effect to button while active
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseHover(EventArgs e)
    {
        base.OnMouseHover(e);
    }

    /// <summary>
    /// Event handler which call SetNormalValues() method to set back the button
    /// to normal state
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        Cursor = Cursors.Default;
        isBeingHovered = false;
        Refresh();
    }

    #endregion Events


    private GraphicsPath GetPathRoundCorners(Rectangle rc, int r)
    {
        int x = rc.X;
        int y = rc.Y;
        int w = rc.Width;
        int h = rc.Height;
        r = r << 1;
        GraphicsPath path = new GraphicsPath();
        if (r > 0)
        {
            if (r > h) r = h;
            if (r > w) r = w;
            path.AddArc(x, y, r, r, 180, 90);
            path.AddArc(x + w - r, y, r, r, 270, 90);
            path.AddArc(x + w - r, y + h - r, r, r, 0, 90);
            path.AddArc(x, y + h - r, r, r, 90, 90);
            path.CloseFigure();
        }
        else
        {
            path.AddRectangle(rc);
        }
        return path;
    }

    public event EventHandler? TokenBodyClicked;

    protected void OnTokenBodyClicked(object sender, MouseEventArgs e)
    {
        TokenBodyClicked?.Invoke(sender, e);
    }

    //action for when mouse click on close button
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        var indexOfThisToken = Parent!.Controls.IndexOf(this);

        if (rCloseX.Contains(e.Location) && e.Button == MouseButtons.Left)
        {
            Parent.Controls.RemoveAt(indexOfThisToken);

            if (NotifyParentEvent != null)
            {
                var tokenEventArgs = new TokenEventArgs(Text, "Remove");

                NotifyParentEvent(this, tokenEventArgs);
            }
        }
        else
        {
            if (NotifyParentEvent != null)
            {
                if (e.Clicks == 1)
                {
                    var tokenEventArgs = new TokenEventArgs(Text, "Click");

                    NotifyParentEvent(this, tokenEventArgs);
                }
            }
        }
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);

        if (NotifyParentEvent != null)
        {
            var tokenEventArgs = new TokenEventArgs(Text, "DoubleClick");

            NotifyParentEvent(this, tokenEventArgs);
        }
    }
}