using System.Drawing.Drawing2D;

namespace Zup;

public partial class frmFloatingButton : Form
{
    private Bitmap? buttonBitmap;
    private GraphicsPath? buttonPath;

    public frmFloatingButton()
    {
        InitializeComponent();
        
        // Set form properties for transparent background and custom shape
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(140, 70);
        this.TopMost = true;
        this.AllowTransparency = true;

        //this.BackColor = Color.Red;
        //this.TransparencyKey = Color.Red;
        
        // Create the button bitmap once
        CreateButtonBitmap();
    }

    float b = 12;

    private void CreateButtonBitmap()
    {
        // Create 2x resolution bitmap for ultra-smooth rendering with alpha channel
        buttonBitmap = new Bitmap(this.Width * 2, this.Height * 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(buttonBitmap))
        {
            // Enable maximum quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.CompositingMode = CompositingMode.SourceOver;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            
            // Clear with fully transparent background
            g.Clear(Color.Transparent);
            
            // Create bounds for the pill at 2x resolution with proper padding
            RectangleF pillBounds = new RectangleF(b, b, this.Width * 2 - (b * 2), this.Height * 2 - (b * 2));
            
            // Draw drop shadow
            DrawDropShadow(g, pillBounds);
            
            // Draw the pill with premium gradient
            DrawPremiumPill(g, pillBounds);
            
            // Draw inner highlight for depth
            DrawInnerHighlight(g, pillBounds);
            
            // Draw border
            DrawBorder(g, pillBounds);
        }
        
        // Set the region to use the graphics path for smooth edges
        SetRegionFromPath();
    }

    private void DrawDropShadow(Graphics g, RectangleF bounds)
    {
        // Create shadow effect with multiple layers
        int shadowLayers = 10;
        for (int i = shadowLayers; i >= 1; i--)
        {
            float shadowAlpha = 8f * (shadowLayers - i + 1) / shadowLayers;
            float shadowOffset = i * 1.2f;
            
            RectangleF shadowBounds = new RectangleF(
                bounds.Left + shadowOffset,
                bounds.Top + shadowOffset * 1.5f,
                bounds.Width,
                bounds.Height
            );
            
            using (GraphicsPath shadowPath = CreatePillPathF(shadowBounds))
            {
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb((int)shadowAlpha, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }
        }
    }

    private void DrawPremiumPill(Graphics g, RectangleF bounds)
    {
        // Draw main body with gradient
        using (GraphicsPath pillPath = CreatePillPathF(bounds))
        {
            // Create a smooth gradient from light white to subtle gray
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                new PointF(bounds.Left, bounds.Top),
                new PointF(bounds.Left, bounds.Bottom),
                Color.FromArgb(255, 252, 252, 252),
                Color.FromArgb(255, 235, 235, 235)
            );
            gradientBrush.SetSigmaBellShape(0.5f, 1f);
            
            g.FillPath(gradientBrush, pillPath);
            gradientBrush.Dispose();
        }
    }

    private void DrawInnerHighlight(Graphics g, RectangleF bounds)
    {
        // Create a subtle inner highlight on top for depth
        float highlightInset = 5f;
        RectangleF highlightBounds = new RectangleF(
            bounds.Left + highlightInset,
            bounds.Top + highlightInset,
            bounds.Width - (highlightInset * 2),
            bounds.Height * 0.3f
        );
        
        using (GraphicsPath highlightPath = CreatePillPathF(highlightBounds))
        {
            using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
            {
                g.FillPath(highlightBrush, highlightPath);
            }
        }
    }

    private void DrawBorder(Graphics g, RectangleF bounds)
    {
        // Draw a refined border
        using (GraphicsPath borderPath = CreatePillPathF(bounds))
        {
            using (Pen borderPen = new Pen(Color.FromArgb(220, 160, 160, 160), 2f))
            {
                borderPen.LineJoin = LineJoin.Round;
                borderPen.EndCap = LineCap.Round;
                borderPen.StartCap = LineCap.Round;
                g.DrawPath(borderPen, borderPath);
            }
        }
    }

    private void SetRegionFromPath()
    {
        if (buttonBitmap == null) return;
        
        // Create region from a graphics path for smooth edges
        Rectangle pillBounds = new Rectangle(0, 0, this.Width, this.Height);
        buttonPath = CreatePillPath(pillBounds);
        this.Region = new Region(buttonPath);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        if (buttonBitmap != null)
        {
            // Draw the pre-rendered button bitmap at full quality
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            
            // Scale down the 2x bitmap to fit the form
            e.Graphics.DrawImage(buttonBitmap, 0, 0, this.Width, this.Height);
        }
    }

    private GraphicsPath CreatePillPath(Rectangle bounds)
    {
        return CreatePillPathF(new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height));
    }

    private GraphicsPath CreatePillPathF(RectangleF bounds)
    {
        GraphicsPath path = new GraphicsPath();
        
        // For a pill shape, the radius should be half the height
        float radius = bounds.Height / 2f;
        
        // Top-left arc
        path.AddArc(bounds.Left, bounds.Top, radius * 2, radius * 2, 180, 90);
        
        // Top line
        path.AddLine(bounds.Left + radius, bounds.Top, bounds.Right - radius, bounds.Top);
        
        // Top-right arc
        path.AddArc(bounds.Right - radius * 2, bounds.Top, radius * 2, radius * 2, 270, 90);
        
        // Right line
        path.AddLine(bounds.Right, bounds.Top + radius, bounds.Right, bounds.Bottom - radius);
        
        // Bottom-right arc
        path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        
        // Bottom line
        path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.Left + radius, bounds.Bottom);
        
        // Bottom-left arc
        path.AddArc(bounds.Left, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        
        // Left line
        path.AddLine(bounds.Left, bounds.Bottom - radius, bounds.Left, bounds.Top + radius);
        
        path.CloseFigure();
        
        return path;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        
        // Allow form to be dragged
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        buttonBitmap?.Dispose();
        buttonBitmap = null;
        buttonPath?.Dispose();
        buttonPath = null;
        base.OnFormClosing(e);
    }

    // For dragging the form
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();
}
