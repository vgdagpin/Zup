﻿namespace Zup.CustomControls;

public class SuggestionItem
{
    private string text;
    private object item;

    public SuggestionItem(string Text, object? Item = null)
    {
        this.Text = Text;
        this.Item = Item;

    }

    public string Text
    {
        get
        {
            return text;
        }

        set
        {
            text = value;
        }
    }

    public object Item
    {
        get
        {
            return item;
        }

        set
        {
            item = value;
        }
    }
}