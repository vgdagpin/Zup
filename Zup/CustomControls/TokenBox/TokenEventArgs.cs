namespace Zup.CustomControls;

public delegate void NotifyParentDelegate(TokenEventArgs customEventArgs);

public class TokenEventArgs : EventArgs
{
    private string _name;
    private MouseButtons _mb;
    private int _index;
    public TokenEventArgs(string Name, int PositionInTokenBox, MouseButtons Mb)
    {
        _index = PositionInTokenBox;
        _name = Name;
        _mb = Mb;
    }

    public string Text
    {
        get { return _name; }
        set { _name = value; }
    }

    public MouseButtons Button
    {
        get
        {
            return _mb;
        }

        set
        {
            _mb = value;
        }
    }

    public int Index
    {
        get
        {
            return _index;
        }

        set
        {
            _index = value;
        }
    }
}