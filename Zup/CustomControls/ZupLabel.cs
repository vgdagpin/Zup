namespace Zup.CustomControls;

public class ZupLabel : Label
{
    private const int WM_LBUTTONDBLCLK = 0x203;

    // need to override Label because double clicking label will copy label (Text) content to clipboard due to some microsoft devs messed up some coding

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_LBUTTONDBLCLK)
        {
            var clipTxt = Clipboard.GetText();
            var clipImg = Clipboard.GetImage();

            base.WndProc(ref m);

            if (clipImg != null)
            {
                Clipboard.SetImage(clipImg);
                return;
            }

            if (!string.IsNullOrEmpty(clipTxt))
            {
                Clipboard.SetText(clipTxt);
            }
        }
        else
        {
            base.WndProc(ref m);
        }
    }
}
