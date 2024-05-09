using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup.CustomControls;

/// <summary>
/// This TokenProject borrows code from:
/// http://autocompletetexboxcs.codeplex.com/
/// Thank you to Peter Holpar for sharing his work.
/// Under MS-Public License (below)
/// </summary>
public class AutoCompleteTextBox : TextBox
{
    private ListBox _listBox;
    private bool _isAdded;
    private String[] _values;
    private String _formerValue = String.Empty;
    private int _MouseIndex = -1;
    private bool _showAutoComplete;

    #region Properties

    public bool ShowAutoComplete
    {
        get
        {
            return _showAutoComplete;
        }

        set
        {
            _showAutoComplete = value;
        }
    }

    public string[] Values
    {
        get
        {
            return _values;
        }
        set
        {
            _values = value;
        }
    }

    public List<string> SelectedValues
    {
        get
        {
            var result = Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(result);
        }
    }
    #endregion //Properties

    #region Constructors
    private void InitializeComponent()
    {
        _listBox = new ListBox();
        //Events
        _listBox.MouseClick += _listBox_MouseClick;
        _listBox.MouseMove += _listBox_MouseMove;
        KeyDown += this_KeyDown;
        KeyUp += this_KeyUp;

    }


    public AutoCompleteTextBox() : this(true)
    {

    }

    public AutoCompleteTextBox(bool showAutoComplete)
    {
        ShowAutoComplete = showAutoComplete;
        InitializeComponent();
        ResetListBox();

    }

    #endregion Constructors

    #region Events

    private void _listBox_MouseMove(object sender, MouseEventArgs e)
    {
        //throw new NotImplementedException();
        int index = _listBox.IndexFromPoint(e.Location);

        if (index != -1 && index != _MouseIndex)
        {
            if (_MouseIndex != -1)
            {
                _listBox.SetSelected(_MouseIndex, false);
            }
            _MouseIndex = index;
            _listBox.SetSelected(_MouseIndex, true);
            _listBox.Invalidate();

        }
    }

    private void _listBox_MouseClick(object sender, MouseEventArgs e)
    {
        string seleccionado = ((ListBox)sender).SelectedItem.ToString();
        //MessageBox.Show(((ListBox)sender).SelectedItem.ToString());
        introduceToken(seleccionado, true);
        this.Focus();
    }

    public void tokenBox_BackColorChanged(object sender, EventArgs e)
    {//The textbox needs to have the same background color as the parent so it is 
     //not noticed.
        this.BackColor = ((TokenBox)sender).BackColor;
    }

    private void this_KeyUp(object sender, KeyEventArgs e)//todo: y esto??
    {
        if (ShowAutoComplete)
        {
            UpdateListBoxWithLocalMatches();
        }
    }

    private void this_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Enter:
            case Keys.Tab:
                {
                    AcceptInput();
                    break;
                }
            case Keys.Down:
                {
                    if ((_listBox.Visible) && (_listBox.SelectedIndex < _listBox.Items.Count - 1))
                        _listBox.SelectedIndex++;
                    break;
                }
            case Keys.Up:
                {
                    if ((_listBox.Visible) && (_listBox.SelectedIndex > 0))
                        _listBox.SelectedIndex--;
                    break;
                }
        }
    }


    protected override void OnLeave(EventArgs e)
    {
        base.OnLeave(e);
        AcceptInput();
    }
    private void introduceToken(String textToken, bool hasX)
    {
        ((TokenBox)Parent).AddToken(textToken);
        this.Text = String.Empty;
        ResetListBox();
    }
    #endregion Events

    #region Methods
    public void ShowExternalSuggestionList(string[] sil)
    {
        SizeF sizeText;
        int widthListBox = _listBox.Width;
        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
        {
            foreach (var si in sil)
            {
                sizeText = g.MeasureString(si, this._listBox.Font);
                if (sizeText.Width > widthListBox) widthListBox = (int)sizeText.Width;
                //I might as well just add it here...
                _listBox.Items.Add(si);
            }
        }
        _listBox.Width = widthListBox;
        _listBox.ItemHeight = 15;
        //_listBox.Items.AddRange(ListSuggestions.Select(x => x).ToArray());
        if (!_isAdded) //TODO: NO ENTIENDO PORQUE TIENE QUE AÑADIR ESTO
        {
            Parent.Parent.Controls.Add(_listBox); //CLARO, SE TIENE QUE IR A LA VENTANA PARA QUE EL DESPLEGABLE ESTÉ POR ENCIMA DE TODO.

            _isAdded = true;
        }//esto de arriba?

        _listBox.Top = this.Bottom + Parent.Top;
        _listBox.Left = this.Left + Parent.Left;
        if (_listBox.Right + Parent.Left > Parent.Parent.Width)
        {
            _listBox.Left -= _listBox.Right - Parent.Parent.Width;
        }
        _listBox.Visible = true;
        _listBox.BringToFront();

    }

    private void ShowListBox()
    {
        if (ShowAutoComplete)
        {
            if (!_isAdded) //TODO: NO ENTIENDO PORQUE TIENE QUE AÑADIR ESTO
            {
                Parent.Parent.Controls.Add(_listBox); //CLARO, SE TIENE QUE IR A LA VENTANA PARA QUE EL DESPLEGABLE ESTÉ POR ENCIMA DE TODO.

                _isAdded = true;
            }//esto de arriba?

            _listBox.Top = this.Bottom + Parent.Top;
            _listBox.Left = this.Left + Parent.Left;
            _listBox.Visible = true;
            _listBox.BringToFront();
        }
    }

    private void ResetListBox()
    {
        _listBox.Visible = false;
        _MouseIndex = -1;
    }



    private void AcceptInput()
    {
        if (_listBox.Visible)
        {
            var seleccionado = (string)_listBox.SelectedItem!;
            introduceToken(seleccionado, true);
            _formerValue = Text;
            this.Focus();
        }
        else
        {
            string entrada = this.Text;
            entrada = entrada.Trim();
            if (entrada.Length > 0)
            {
                ((TokenBox)Parent).AddToken(entrada);
                this.Clear();
                this.Focus();
            }
        }
    }

    private void UpdateListBoxWithLocalMatches()
    {
        if (Text == _formerValue) return;
        _formerValue = Text;
        var word = GetWord();

        if (_values != null && word.Length > 0)
        {
            var matches = Array.FindAll(_values,
             x => (x.StartsWith(word, StringComparison.OrdinalIgnoreCase) && !SelectedValues.Contains(x)));
            if (matches.Length > 0)
            {
                ShowListBox();
                _listBox.Items.Clear();
                Array.ForEach(matches, x => _listBox.Items.Add(x));
                //_listBox.SelectedIndex = 0;
                _listBox.Height = 0;
                _listBox.Width = 0;
                Focus();
                using (Graphics graphics = _listBox.CreateGraphics())
                {
                    for (int i = 0; i < _listBox.Items.Count; i++)
                    {
                        _listBox.Height += _listBox.GetItemHeight(i);
                        // it item width is larger than the current one
                        // set it to the new max item width
                        // GetItemRectangle does not work for me
                        // we add a little extra space by using '_'
                        int itemWidth = (int)graphics.MeasureString(((String)_listBox.Items[i]) + "_", _listBox.Font).Width;
                        _listBox.Width = (_listBox.Width < itemWidth) ? itemWidth : _listBox.Width;
                    }
                }
            }
            else
            {
                ResetListBox();
            }
        }
        else
        {
            ResetListBox();
        }
    }

    private string GetWord()
    {
        int pos = SelectionStart;

        int posStart = Text.LastIndexOf(' ', (pos < 1) ? 0 : pos - 1);
        posStart = (posStart == -1) ? 0 : posStart + 1;
        int posEnd = Text.IndexOf(' ', pos);
        posEnd = (posEnd == -1) ? Text.Length : posEnd;

        int length = ((posEnd - posStart) < 0) ? 0 : posEnd - posStart;

        return Text.Substring(posStart, length);
    }
    #endregion Methods
}