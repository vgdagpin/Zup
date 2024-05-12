namespace Zup.CustomControls;

public class TokenEventArgs : EventArgs
{
    public TokenEventArgs(string text, string eventType)
    {
        Text = text;
        EventType = eventType;
    }

    public string Text { get; set; }
    public string EventType { get; set; }
}

