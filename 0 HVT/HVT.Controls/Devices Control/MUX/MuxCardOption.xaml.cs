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

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for MuxCardOption.xaml
    /// </summary>
    public partial class MuxCardOption : UserControl
    {
        private MuxCard _Card = new MuxCard();
        public MuxCard Card
        {
            get { return _Card; }
            set
            {
                if (value != null || value != _Card)
                {
                    _Card = value;
                    if (_contentLoaded)
                    {
                        MuxChannelsPanel.Children.Clear();
                        foreach (var item in _Card.Chanels)
                        {
                            MuxChannelsPanel.Children.Add(item.CbUse);
                        }
                        _Card.UpdateCardSelect(MuxChannelsPanel);
                        _Card.PCB_COUNT_CHANGE += _Card_PCB_COUNT_CHANGE;
                        this.DataContext = _Card;
                    }
                }
            }
        }

        public MuxCardOption()
        {
            InitializeComponent();
            MuxChannelsPanel.Children.Clear();
            foreach (var item in _Card.Chanels)
            {
                MuxChannelsPanel.Children.Add(item.CbUse);
            }
            this.DataContext = _Card;
            _Card.PCB_COUNT_CHANGE += _Card_PCB_COUNT_CHANGE;

        }

        private void _Card_PCB_COUNT_CHANGE(object sender, EventArgs e)
        {
            _Card.UpdateCardSelect(MuxChannelsPanel);
        }

        private void SellectAll_Click(object sender, RoutedEventArgs e)
        {
            _Card.SelectAll();
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            _Card.ClearAll();
        }
    }
}
