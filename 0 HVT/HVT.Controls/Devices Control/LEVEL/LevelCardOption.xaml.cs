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
    /// Interaction logic for LevelCardOption.xaml
    /// </summary>
    public partial class LevelCardOption : UserControl
    {
        private LevelCard _Card = new LevelCard();
        public LevelCard Card
        {
            get { return _Card; }
            set
            {
                if (value != null || value != _Card)
                {
                    _Card = value;
                    if (_contentLoaded)
                    {
                        _Card.PCB_COUNT_CHANGE += _Card_PCB_COUNT_CHANGE;
                        UpdateCardSellect(_Card.PCB_Count);
                        this.DataContext = _Card;
                    }
                }
            }
        }

        private void _Card_PCB_COUNT_CHANGE(object sender, EventArgs e)
        {
            UpdateCardSellect(Card.PCB_Count);
        }

        public LevelCardOption()
        {
            InitializeComponent();
            this.DataContext = _Card;
            UpdateCardSellect(_Card.PCB_Count);
            _Card.PCB_COUNT_CHANGE += _Card_PCB_COUNT_CHANGE;
        }

        public void UpdateCardSellect(int boardCount)
        {
            LevelChannelsPanelDC1.Children.Clear();
            LevelChannelsPanelAC1.Children.Clear();
            LevelChannelsPanelDC2.Children.Clear();
            LevelChannelsPanelAC2.Children.Clear();
            LevelChannelsPanelDC2.Visibility = Visibility.Visible;
            LevelChannelsPanelAC2.Visibility = Visibility.Visible;

            foreach (var item in _Card.Chanels)
            {
                item.CbUse.Visibility = Visibility.Visible;
            }

            switch (boardCount)
            {
                case 1:
                    foreach (var item in _Card.Chanels)
                    {
                        if (item.Channel < 17)
                        {
                            LevelChannelsPanelDC1.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 9 ? Visibility.Visible : Visibility.Hidden;
                        }
                        else if (item.Channel < 33)
                        {
                            LevelChannelsPanelAC1.Children.Add(item.CbUse);
                        }
                        else if (item.Channel < 49)
                        {
                            LevelChannelsPanelDC2.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 41 ? Visibility.Visible : Visibility.Hidden;
                        }
                        else
                        {
                            LevelChannelsPanelAC2.Children.Add(item.CbUse);
                        }
                    }

                    break;
                case 2:
                    foreach (var item in _Card.Chanels)
                    {
                        if (item.Channel < 17)
                        {
                            LevelChannelsPanelDC1.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 9 ? Visibility.Visible : Visibility.Hidden;
                        }
                        else if (item.Channel < 33)
                        {
                            LevelChannelsPanelAC1.Children.Add(item.CbUse);
                        }
                    }

                    break;
                case 3:
                    foreach (var item in _Card.Chanels)
                    {
                        if (item.Channel < 17)
                        {
                            LevelChannelsPanelDC1.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 5 ? Visibility.Visible : Visibility.Hidden;
                        }
                        else if (item.Channel < 33)
                        {
                            LevelChannelsPanelAC1.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 25 ? Visibility.Visible : Visibility.Hidden;
                        }
                    }
                    break;
                case 4:
                    foreach (var item in _Card.Chanels)
                    {
                        if (item.Channel < 17)
                        {
                            LevelChannelsPanelDC1.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 5 ? Visibility.Visible : Visibility.Hidden;
                        }
                        else if (item.Channel < 33)
                        {
                            LevelChannelsPanelAC1.Children.Add(item.CbUse);
                            item.CbUse.Visibility = item.Channel < 25 ? Visibility.Visible : Visibility.Hidden;
                        }
                    }
                    break;
                default:
                    break;
            }
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
