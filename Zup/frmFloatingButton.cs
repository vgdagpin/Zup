using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Zup;

public partial class frmFloatingButton : Form
{
    private Bitmap? buttonBitmap;
    private GraphicsPath? buttonPath;
    private System.Windows.Forms.Timer? updateTimer;
    private bool isRunning = true; // Timer running state
    private RectangleF? controlButtonBounds; // Bounds of the start/stop button for click detection
    private ContextMenuStrip? controlButtonMenu;
    private int elapsedSeconds = 0; // Elapsed time in seconds
    private RectangleF? taskTextBounds; // Bounds of the task text for tooltip detection
    private ToolTip? taskTooltip;
    private DateTime? startedOn = null;
    private DateTime? scheduledOn = null;

    // Constants for rendering
    private const float PADDING = 5f;
    private const int BITMAP_SCALE = 4;
    private const float BORDER_WIDTH = 4f;
    private const float BORDER_INSET_MARGIN = 2f;
    private const int MAX_TASK_TEXT_LENGTH = 20;
    private const float TASK_TEXT_LEFT_MARGIN = 12f;

    // Pre-calculated values
    private static readonly Color COLOR_WHITE_TOP = Color.FromArgb(255, 252, 252, 252);
    private static readonly Color COLOR_WHITE_BOTTOM = Color.FromArgb(255, 235, 235, 235);
    private static readonly Color COLOR_BORDER = Color.FromArgb(220, 160, 160, 160);
    private static readonly Color COLOR_TEXT = Color.FromArgb(255, 50, 50, 50);

    // Events
    public event EventHandler? OnStartEvent;
    public event EventHandler<StopEventArgs>? OnStopEvent;
    public event EventHandler? OnResetEvent;
    public event EventHandler? OnDeleteEvent;
    public event EventHandler? OnTaskTextDoubleClick;

    #region Properties
    // Override Text property to refresh UI when changed
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            buttonBitmap?.Dispose();
            CreateButtonBitmap();
            Invalidate();
        }
    }

    // Public properties
    [Category("Appearance")]
    [Description("Gets the current duration of the task.")]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan Duration
    {
        get
        {
            if (startedOn.HasValue)
            {
                return DateTime.Now - startedOn.Value;
            }
            return TimeSpan.FromSeconds(elapsedSeconds);
        }
    }

    [Category("Appearance")]
    [Description("The date and time when the task started. If set, duration is calculated from this time.")]
    [DefaultValue(null)]
    public DateTime? StartedOn
    {
        get => startedOn;
        set
        {
            startedOn = value;
            if (startedOn.HasValue)
            {
                // Reset elapsedSeconds when StartedOn is set
                elapsedSeconds = 0;
                // Start the timer if it's not running
                if (updateTimer != null && !updateTimer.Enabled)
                {
                    isRunning = true;
                    updateTimer.Start();

                    OnStartEvent?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                // Stop the timer if StartedOn is null
                if (updateTimer != null && updateTimer.Enabled)
                {
                    isRunning = false;
                    updateTimer.Stop();

                    OnStopEvent?.Invoke(this, new StopEventArgs
                    {
                        Duration = TimeSpan.FromSeconds(elapsedSeconds),
                        IsClosed = false
                    });
                }
            }
            buttonBitmap?.Dispose();
            CreateButtonBitmap();
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("The scheduled date and time for the task.")]
    [DefaultValue(null)]
    public DateTime? ScheduledOn
    {
        get => scheduledOn;
        set
        {
            scheduledOn = value;
            buttonBitmap?.Dispose();
            CreateButtonBitmap();
            Invalidate();
        }
    }
    #endregion

    public frmFloatingButton()
    {
        InitializeComponent();

        // Set form properties for transparent background and custom shape
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(120, 60);
        TopMost = true;
        AllowTransparency = true;
        DoubleBuffered = true;

        // Create the button bitmap once
        CreateButtonBitmap();

        // Initialize and start timer for updating time
        InitializeTimer();

        // Enable DWM shadow after form is loaded
        Load += FrmFloatingButton_Load;

        // Create context menu for control button
        controlButtonMenu = new ContextMenuStrip();
        controlButtonMenu.Items.Add("Reset", null, (s, e) => OnControlButtonReset());
        controlButtonMenu.Items.Add("Delete", null, (s, e) => OnControlButtonDelete());

        // Create tooltip for task text
        taskTooltip = new ToolTip();
        taskTooltip.AutoPopDelay = 5000;
        taskTooltip.InitialDelay = 500;
        taskTooltip.ReshowDelay = 100;
    }

    private void FrmFloatingButton_Load(object? sender, EventArgs e)
    {
        EnableDWMDropShadow();
    }

    private void InitializeTimer()
    {
        updateTimer = new System.Windows.Forms.Timer
        {
            Interval = 1000 // Update every second
        };
        updateTimer.Tick += UpdateTimer_Tick;

        // Only start timer if StartedOn has a value
        if (startedOn.HasValue)
        {
            updateTimer.Start();
        }
        else
        {
            isRunning = false; // Set to not running if no start time
        }
    }

    private void UpdateTimer_Tick(object? sender, EventArgs e)
    {
        // Increment elapsed time
        elapsedSeconds++;

        // Recreate bitmap with updated time
        buttonBitmap?.Dispose();
        CreateButtonBitmap();
        Invalidate(); // Trigger repaint
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassStyle |= CS_DROPSHADOW;
            // Don't use layered window - use region with smooth path instead
            return cp;
        }
    }

    private void CreateButtonBitmap()
    {
        int bitmapWidth = Width * BITMAP_SCALE;
        int bitmapHeight = Height * BITMAP_SCALE;

        // Create 4x resolution bitmap for ultra-smooth rendering with alpha channel
        buttonBitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(buttonBitmap))
        {
            // Configure graphics for maximum quality
            ConfigureGraphics(g);

            // Clear with fully transparent background
            g.Clear(Color.Transparent);

            // Pre-calculate pill bounds
            float padding2x = PADDING * 2;
            float padding4x = PADDING * 4;
            RectangleF pillBounds = new RectangleF(
                padding2x,
                padding2x,
                bitmapWidth - padding4x,
                bitmapHeight - padding4x
            );

            // Draw layers in order
            DrawPremiumPill(g, pillBounds);
            DrawBorder(g, pillBounds);
            DrawText(g, pillBounds);
        }

        SetRegionFromPath();
    }

    private static void ConfigureGraphics(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.CompositingMode = CompositingMode.SourceOver;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
    }

    private void DrawPremiumPill(Graphics g, RectangleF bounds)
    {
        using (GraphicsPath pillPath = CreatePillPathF(bounds))
        using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
            new PointF(bounds.Left, bounds.Top),
            new PointF(bounds.Left, bounds.Bottom),
            COLOR_WHITE_TOP,
            COLOR_WHITE_BOTTOM))
        {
            gradientBrush.SetSigmaBellShape(0.5f, 1f);
            g.FillPath(gradientBrush, pillPath);
        }
    }

    public void Stop()
    {
        OnControlButtonStop();
    }

    private void DrawBorder(Graphics g, RectangleF bounds)
    {
        float inset = BORDER_WIDTH + BORDER_INSET_MARGIN;
        RectangleF borderBounds = new RectangleF(
            bounds.Left + inset,
            bounds.Top + inset,
            bounds.Width - (inset * 2),
            bounds.Height - (inset * 2)
        );

        using (GraphicsPath borderPath = CreatePillPathF(borderBounds))
        using (Pen borderPen = new Pen(COLOR_BORDER, BORDER_WIDTH))
        {
            borderPen.LineJoin = LineJoin.Round;
            borderPen.EndCap = LineCap.Round;
            borderPen.StartCap = LineCap.Round;
            g.DrawPath(borderPen, borderPath);
        }
    }

    private void DrawText(Graphics g, RectangleF bounds)
    {
        // Calculate duration based on StartedOn or elapsedSeconds
        TimeSpan duration;
        if (startedOn.HasValue)
        {
            duration = DateTime.Now - startedOn.Value;
        }
        else
        {
            duration = TimeSpan.FromSeconds(elapsedSeconds);
        }

        // Format duration as HH:mm:ss
        int hours = (int)duration.TotalHours;
        int minutes = duration.Minutes;
        int seconds = duration.Seconds;
        string timeText = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        string taskText = Text;

        // Truncate task text to max length and append "..." if needed
        if (taskText.Length > MAX_TASK_TEXT_LENGTH)
        {
            taskText = taskText.Substring(0, MAX_TASK_TEXT_LENGTH) + "..";
        }

        // Create font at 4x resolution for crisp text
        float fontSize = 18f * BITMAP_SCALE; // Larger font size for time (increased from 14f)
        float smallFontSize = 11f * BITMAP_SCALE; // Smaller font for task text
        using (Font timeFont = new Font("Segoe UI", fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
        using (Font taskFont = new Font("Segoe UI", smallFontSize, FontStyle.Regular, GraphicsUnit.Pixel))
        using (SolidBrush textBrush = new SolidBrush(COLOR_TEXT))
        {
            // Measure both texts
            SizeF timeSize = g.MeasureString(timeText, timeFont);
            SizeF taskSize = g.MeasureString(taskText, taskFont);

            // Calculate spacing between lines (reduced for closer text)
            float lineSpacing = 0.5f * BITMAP_SCALE;
            float totalHeight = timeSize.Height + lineSpacing + taskSize.Height;

            // Calculate vertical center position for the text block
            float startY = bounds.Top + (bounds.Height - totalHeight) / 2f;

            // Calculate button icon size and spacing
            float buttonSize = timeSize.Height * 0.7f; // Button size relative to time text height
            float buttonSpacing = 3f * BITMAP_SCALE; // Space between button and time text

            // Calculate total width of time + spacing + button
            float totalTimeWidth = timeSize.Width + buttonSpacing + buttonSize;

            // Center the time+button group horizontally
            float timeGroupX = bounds.Left + (bounds.Width - totalTimeWidth) / 2f;

            // Draw time text
            float timeX = timeGroupX;
            g.DrawString(timeText, timeFont, textBrush, timeX, startY);

            // Draw start/stop button icon beside time
            float buttonX = timeX + timeSize.Width + buttonSpacing;
            float buttonY = startY + (timeSize.Height - buttonSize) / 2f;
            RectangleF buttonRect = new RectangleF(buttonX, buttonY, buttonSize, buttonSize);

            // Store button bounds for click detection (scaled down to 1x, accounting for padding)
            float padding2x = PADDING * 2;
            controlButtonBounds = new RectangleF(
                (padding2x + buttonX) / BITMAP_SCALE,
                (padding2x + buttonY) / BITMAP_SCALE,
                buttonSize / BITMAP_SCALE,
                buttonSize / BITMAP_SCALE
            );

            DrawControlButton(g, buttonRect);

            // Draw task text below (left aligned with small margin)
            // Center task text horizontally beneath the time/button group
            float taskX = bounds.Left + (bounds.Width - taskSize.Width) / 2f;
            float taskY = startY + timeSize.Height + lineSpacing;
            g.DrawString(taskText, taskFont, textBrush, taskX, taskY);

            // Store task text bounds for tooltip detection (scaled down to 1x, accounting for padding)
            taskTextBounds = new RectangleF(
                (padding2x + taskX) / BITMAP_SCALE,
                (padding2x + taskY) / BITMAP_SCALE,
                taskSize.Width / BITMAP_SCALE,
                taskSize.Height / BITMAP_SCALE
            );
        }
    }

    private void DrawControlButton(Graphics g, RectangleF bounds)
    {
        using (SolidBrush buttonBrush = new SolidBrush(COLOR_TEXT))
        {
            if (isRunning)
            {
                // Draw stop icon (square)
                float margin = bounds.Width * 0.25f;
                RectangleF stopRect = new RectangleF(
                    bounds.Left + margin,
                    bounds.Top + margin,
                    bounds.Width - (margin * 2),
                    bounds.Height - (margin * 2)
                );
                g.FillRectangle(buttonBrush, stopRect);
            }
            else
            {
                // Draw play icon (triangle pointing right)
                PointF[] triangle = new PointF[]
                {
                    new PointF(bounds.Left + bounds.Width * 0.2f, bounds.Top + bounds.Height * 0.15f),
                    new PointF(bounds.Left + bounds.Width * 0.2f, bounds.Top + bounds.Height * 0.85f),
                    new PointF(bounds.Left + bounds.Width * 0.85f, bounds.Top + bounds.Height * 0.5f)
                };
                g.FillPolygon(buttonBrush, triangle);
            }
        }
    }

    private void SetRegionFromPath()
    {
        if (buttonBitmap == null) return;

        // Create a smooth region using the pill shape
        // Use full form bounds for the region
        RectangleF regionBounds = new RectangleF(0, 0, Width, Height);
        buttonPath = CreatePillPathF(regionBounds);

        // Flatten the path with very high precision for ultra-smooth edges
        // Lower flatness = more points = smoother curves when rasterized
        buttonPath.Flatten(new System.Drawing.Drawing2D.Matrix(), 0.001f);

        Region = new Region(buttonPath);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (buttonBitmap != null)
        {
            ConfigureGraphics(e.Graphics);
            e.Graphics.DrawImage(buttonBitmap, 0, 0, Width, Height);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        // Recreate bitmap and region if size changes
        if (IsHandleCreated)
        {
            buttonBitmap?.Dispose();
            buttonPath?.Dispose();
            CreateButtonBitmap();
        }
    }

    private GraphicsPath CreatePillPathF(RectangleF bounds)
    {
        GraphicsPath path = new GraphicsPath();
        float radius = bounds.Height * 0.5f;
        float diameter = radius * 2f;
        float left = bounds.Left;
        float top = bounds.Top;
        float right = bounds.Right;
        float bottom = bounds.Bottom;

        // Build pill shape with arcs - using smaller arc segments for smoother curves
        // Top-left arc (90 degrees)
        path.AddArc(left, top, diameter, diameter, 180, 90);

        // Top line
        path.AddLine(left + radius, top, right - radius, top);

        // Top-right arc (90 degrees)
        path.AddArc(right - diameter, top, diameter, diameter, 270, 90);

        // Right line
        path.AddLine(right, top + radius, right, bottom - radius);

        // Bottom-right arc (90 degrees)
        path.AddArc(right - diameter, bottom - diameter, diameter, diameter, 0, 90);

        // Bottom line
        path.AddLine(right - radius, bottom, left + radius, bottom);

        // Bottom-left arc (90 degrees)
        path.AddArc(left, bottom - diameter, diameter, diameter, 90, 90);

        // Left line
        path.AddLine(left, bottom - radius, left, top + radius);

        path.CloseFigure();

        return path;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        // Show tooltip when hovering over task text
        if (taskTextBounds.HasValue && taskTextBounds.Value.Contains(e.Location))
        {
            if (taskTooltip != null && string.IsNullOrEmpty(taskTooltip.GetToolTip(this)))
            {
                taskTooltip.SetToolTip(this, Text);
            }
        }
        else
        {
            // Hide tooltip when not over task text
            if (taskTooltip != null && !string.IsNullOrEmpty(taskTooltip.GetToolTip(this)))
            {
                taskTooltip.SetToolTip(this, string.Empty);
            }
        }

        // Change cursor to pointer when hovering over the control button
        if (controlButtonBounds.HasValue && controlButtonBounds.Value.Contains(e.Location))
        {
            Cursor = Cursors.Hand;
        }
        else
        {
            Cursor = Cursors.Default;
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        // Reset cursor when mouse leaves the form
        Cursor = Cursors.Default;
        // Hide tooltip when mouse leaves the form
        taskTooltip?.SetToolTip(this, string.Empty);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButtons.Right)
        {
            // Show context menu at cursor position (anywhere on the form)
            controlButtonMenu?.Show(Cursor.Position);
            return;
        }

        // Task text double-click detection
        if (e.Button == MouseButtons.Left && e.Clicks == 2 && taskTextBounds.HasValue && taskTextBounds.Value.Contains(e.Location))
        {
            OnTaskTextDoubleClick?.Invoke(this, EventArgs.Empty);
            return; // swallow double-click so it doesn't drag window
        }

        if (controlButtonBounds.HasValue && controlButtonBounds.Value.Contains(e.Location))
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isRunning)
                {
                    OnControlButtonStop();
                }
                else
                {
                    OnControlButtonStart();
                }

                buttonBitmap?.Dispose();
                CreateButtonBitmap();
                Invalidate();
            }
        }
        else if (e.Button == MouseButtons.Left)
        {
            // Allow form to be dragged if not clicking the button
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }

    // Context menu actions
    private void OnControlButtonReset()
    {
        // Reset timer to 00:00:00 and start running
        elapsedSeconds = 0;
        startedOn = DateTime.Now; // Set StartedOn to current time
        isRunning = true;
        updateTimer?.Start();
        buttonBitmap?.Dispose();
        CreateButtonBitmap();
        Invalidate();

        // Raise OnResetEvent
        OnResetEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnControlButtonStart()
    {
        // Start button clicked - start timer
        if (!startedOn.HasValue)
        {
            startedOn = DateTime.Now;
        }
        isRunning = true;
        updateTimer?.Start();

        OnStartEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnControlButtonStop()
    {
        // Stop button clicked - stop timer and raise event
        isRunning = false;
        updateTimer?.Stop();

        // Raise OnStopEvent with current duration
        OnStopEvent?.Invoke(this, new StopEventArgs
        {
            Duration = Duration,
            IsClosed = true
        });

        // Close the form
        Close();
    }

    private void OnControlButtonDelete()
    {
        // Raise OnDeleteEvent
        OnDeleteEvent?.Invoke(this, EventArgs.Empty);

        // Close the form
        Close();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        updateTimer?.Stop();
        updateTimer?.Dispose();
        updateTimer = null;
        buttonBitmap?.Dispose();
        buttonBitmap = null;
        buttonPath?.Dispose();
        buttonPath = null;
        taskTooltip?.Dispose();
        taskTooltip = null;
        controlButtonMenu?.Dispose();
        controlButtonMenu = null;
        base.OnFormClosing(e);
    }

    // For dragging the form
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    // For drop shadow on borderless form
    private const int CS_DROPSHADOW = 0x00020000;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll")]
    private static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    // DWM API for enhanced shadow support
    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }

    private const int DWMWA_NCRENDERING_POLICY = 2;
    private const int DWMNCRP_ENABLED = 1;

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    private void EnableDWMDropShadow()
    {
        try
        {
            MARGINS margins = new MARGINS { leftWidth = 0, rightWidth = 0, topHeight = 0, bottomHeight = 1 };
            DwmExtendFrameIntoClientArea(Handle, ref margins);

            int attrValue = DWMNCRP_ENABLED;
            DwmSetWindowAttribute(Handle, DWMWA_NCRENDERING_POLICY, ref attrValue, Marshal.SizeOf(typeof(int)));
        }
        catch
        {
            // DWM API not available - fall back to CS_DROPSHADOW
        }
    }
}

public class StopEventArgs : EventArgs
{
    public TimeSpan Duration { get; set; }
    public bool IsClosed { get; set; }
}