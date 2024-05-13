using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HVT.Controls.CustomControls
{
    /// <summary>
    /// Interaction logic for IntegerUpDown.xaml
    /// </summary>
    public partial class IntegerUpDown : UserControl
    {

        public event EventHandler ValueChanged;
        public IntegerUpDown()
        {
            InitializeComponent();
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }


        // Dependency Property
        public static readonly DependencyProperty ValueProperty =
             DependencyProperty.Register("Value", typeof(int),
             typeof(IntegerUpDown), new FrameworkPropertyMetadata(0));

        private int _Value = 0;

        public int Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                _Value =_Value < Minimum ? Minimum : _Value;
                _Value =_Value > Maximum ? Minimum : _Value;
                txtNum.Text = _Value.ToString();
                ValueChanged?.Invoke(this, null);
            }
        }
        private int _Minimum;
        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                if (_Minimum != value)
                {
                    _Minimum = value;
                    Value = Value < value ? value : Value;
                }
            }
        }
        private int _Maximum;
        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (_Maximum != value)
                {
                    _Maximum = value;
                    Value = Value > value ? value : Value;
                }
            }
        }
        public bool ClipValueToMinMax { get; set; }
        public string ParsingNumberStyle { get; set; } = "HexNumber";

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _Value))
                txtNum.Text = _Value.ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
