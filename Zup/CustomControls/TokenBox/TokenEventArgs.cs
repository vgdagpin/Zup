namespace Zup.CustomControls;

public delegate void NotifyParentDelegate(TokenEventArgs customEventArgs);

public class TokenEventArgs : EventArgs
{
    public TokenEventArgs(string text, int positionInTokenBox, MouseButtons mouseButton)
    {
        Text = text;
        Index = positionInTokenBox;
        Button = mouseButton;
    }

    public string Text { get; set; }

    public MouseButtons Button { get; set; }

    public int Index { get; set; }
}