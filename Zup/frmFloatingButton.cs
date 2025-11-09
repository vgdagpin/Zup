using System.Drawing.Drawing2D;

namespace Zup;

public partial class frmFloatingButton : Form
{
    private Bitmap? buttonBitmap;

    public frmFloatingButton()
    {
        InitializeComponent();
        
        // Set form properties for transparent background and custom shape
        this.FormBorderStyle = FormBorderStyle.None;
        //this.BackColor = Color.FromArgb(255, 1, 1, 1); // Use near-black instead of magenta
        //this.TransparencyKey = Color.FromArgb(255, 1, 1, 1);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(120, 60); // Pill shape - wider than tall
        this.TopMost = true; // Always on top
        
        // Create the button bitmap once
        CreateButtonBitmap();
    }

    private void CreateButtonBitmap()
    {
        buttonBitmap = new Bitmap(this.Width, this.Height);
        using (Graphics g = Graphics.FromImage(buttonBitmap))
        {
            // Enable high-quality rendering
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            
            // Clear with transparent background - use the same near-black color
           // g.Clear(Color.FromArgb(255, 1, 1, 1));
            
            // Create bounds for the pill
            Rectangle pillBounds = new Rectangle(5, 5, this.Width - 10, this.Height - 10);
            
            // Draw the pill with fading transparency
            DrawPillWithFadingEdge(g, pillBounds);
            
            // Draw gray border on top
            DrawGrayBorder(g, pillBounds);
        }
        
        // Set the region to include everything visible
        SetRegionFromBitmap();
    }

    private void DrawGrayBorder(Graphics g, Rectangle bounds)
    {
        // Create the pill path for the border
        using (GraphicsPath borderPath = CreatePillPath(bounds))
        {
            // Draw the border with a gray pen
            using (Pen borderPen = new Pen(Color.FromArgb(200, 128, 128, 128), 2f))
            {
                g.DrawPath(borderPen, borderPath);
            }
        }
    }

    private void SetRegionFromBitmap()
    {
        if (buttonBitmap == null) return;
        
        // Create region from non-transparent pixels in the bitmap
        Region region = new Region();
        region.MakeEmpty();
        
        Color transparentKey = Color.FromArgb(255, 1, 1, 1);
        
        // Scan the bitmap and add rectangles for non-transparent pixels
        for (int y = 0; y < buttonBitmap.Height; y++)
        {
            int startX = -1;
            for (int x = 0; x < buttonBitmap.Width; x++)
            {
                Color pixel = buttonBitmap.GetPixel(x, y);
                // Only consider pixels that are significantly different from the transparency key
                bool isVisible = pixel.A > 10 && 
                    (Math.Abs(pixel.R - transparentKey.R) > 5 || 
                     Math.Abs(pixel.G - transparentKey.G) > 5 || 
                     Math.Abs(pixel.B - transparentKey.B) > 5);
                
                if (isVisible && startX == -1)
                {
                    startX = x;
                }
                else if (!isVisible && startX != -1)
                {
                    region.Union(new Rectangle(startX, y, x - startX, 1));
                    startX = -1;
                }
            }
            
            if (startX != -1)
            {
                region.Union(new Rectangle(startX, y, buttonBitmap.Width - startX, 1));
            }
        }
        
        this.Region = region;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        if (buttonBitmap != null)
        {
            // Draw the pre-rendered button bitmap
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            
            e.Graphics.DrawImage(buttonBitmap, 0, 0);
        }
    }

    private GraphicsPath CreatePillPath(Rectangle bounds)
    {
        GraphicsPath path = new GraphicsPath();
        
        // For a pill shape, the radius should be half the height
        int radius = bounds.Height;
        
        // Top-left arc
        path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
        
        // Top line
        path.AddLine(bounds.Left + radius / 2, bounds.Top, bounds.Right - radius / 2, bounds.Top);
        
        // Top-right arc
        path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
        
        // Right line
        path.AddLine(bounds.Right, bounds.Top + radius / 2, bounds.Right, bounds.Bottom - radius / 2);
        
        // Bottom-right arc
        path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
        
        // Bottom line
        path.AddLine(bounds.Right - radius / 2, bounds.Bottom, bounds.Left + radius / 2, bounds.Bottom);
        
        // Bottom-left arc
        path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
        
        // Left line
        path.AddLine(bounds.Left, bounds.Bottom - radius / 2, bounds.Left, bounds.Top + radius / 2);
        
        path.CloseFigure();
        
        return path;
    }

    private void DrawPillWithFadingEdge(Graphics g, Rectangle bounds)
    {
        // Create multiple layers with decreasing size for fading effect
        for (int i = 15; i >= 0; i--)
        {
            int alpha = (int)(255 * (1.0f - i * 0.065f));
            
            if (alpha < 0) alpha = 0;
            if (alpha > 255) alpha = 255;
            
            // Calculate inset for each layer
            int inset = (int)(i * 0.8f);
            Rectangle layerBounds = new Rectangle(
                bounds.Left + inset,
                bounds.Top + inset,
                bounds.Width - (inset * 2),
                bounds.Height - (inset * 2)
            );
            
            if (layerBounds.Width > 0 && layerBounds.Height > 0)
            {
                using (GraphicsPath layerPath = CreatePillPath(layerBounds))
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 211, 211, 211)))
                    {
                        g.FillPath(brush, layerPath);
                    }
                }
            }
        }
        
        // Draw the solid center
        int centerInset = 12;
        Rectangle centerBounds = new Rectangle(
            bounds.Left + centerInset,
            bounds.Top + centerInset,
            bounds.Width - (centerInset * 2),
            bounds.Height - (centerInset * 2)
        );
        
        if (centerBounds.Width > 0 && centerBounds.Height > 0)
        {
            using (GraphicsPath centerPath = CreatePillPath(centerBounds))
            {
                using (SolidBrush centerBrush = new SolidBrush(Color.FromArgb(255, 211, 211, 211)))
                {
                    g.FillPath(centerBrush, centerPath);
                }
            }
        }
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
