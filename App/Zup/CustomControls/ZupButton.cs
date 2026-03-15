namespace Zup.CustomControls;

public class ZupButton : Button
{
    protected override bool ShowFocusCues
    {
        get { return false; }
    }

    public ZupButton()
    {
        SetStyle(ControlStyles.Selectable, false);
    }
}