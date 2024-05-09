namespace Zup.CustomControls;

public class SuggestionItem
{
    private string text;
    private object item;

    public SuggestionItem(string Text, object? Item = null)
    {
        this.Text = Text;
        this.Item = Item;

    }

    public string Text { get; set; }

    public object Item { get; set; }
}