using HVT.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Camera
{
    /// <summary>
    /// Interaction logic for FND_SegementBuilder.xaml
    /// </summary>
    public partial class FND_SegementBuilder : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private SegementCharacter _SelectedCharacter = new SegementCharacter();
        public SegementCharacter SelectedCharacter
        {
            get { return _SelectedCharacter; }
            set
            {
                if (value != null && value != _SelectedCharacter)
                {
                    _SelectedCharacter = value;
                    NotifyPropertyChanged("SelectedCharacter");
                }
            }
        }
        public FND_SegementBuilder()
        {
            InitializeComponent();
            this.DataContext = this;
            dgrSEGLOOKUP.ItemsSource = FND.SEG_LOOKUP;
        }

        private void ToggleSegement_mouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lb = (sender as Label);
            if (lb == null || dgrSEGLOOKUP.SelectedItem == null)
            {
                return;
            }

            SegementCharacter character = (dgrSEGLOOKUP.SelectedItem as SegementCharacter);
            if (character == null)
            {
                return;
            }
            switch (lb.Content)
            {
                case "a":
                    if (character.digit[0] == 0)
                    {
                        character.digit[0] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[0] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                case "b":
                    if (character.digit[1] == 0)
                    {
                        character.digit[1] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[1] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                case "c":
                    if (character.digit[2] == 0)
                    {
                        character.digit[2] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[2] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                case "d":
                    if (character.digit[3] == 0)
                    {
                        character.digit[3] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[3] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                case "e":
                    if (character.digit[4] == 0)
                    {
                        character.digit[4] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[4] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                case "f":
                    if (character.digit[5] == 0)
                    {
                        character.digit[5] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[5] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                case "g":
                    if (character.digit[6] == 0)
                    {
                        character.digit[6] = 1;
                        lb.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        character.digit[6] = 0;
                        lb.Background = new SolidColorBrush(Colors.Black);
                    }
                    break;
                default:
                    break;
            }
            character.DigitChange();
        }

        private void dgrSEGLOOKUP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCharacter = (dgrSEGLOOKUP.SelectedItem as SegementCharacter);
            SegementCharacter character = (dgrSEGLOOKUP.SelectedItem as SegementCharacter);
            if (character == null)
            {
                return;
            }
            foreach (var item in gridSegement.Children)
            {
                Label lb = (item as Label);
                if (lb == null)
                {
                    continue;
                }
                switch (lb.Content)
                {
                    case "a":
                        if (character.digit[0] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    case "b":
                        if (character.digit[1] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    case "c":
                        if (character.digit[2] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    case "d":
                        if (character.digit[3] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    case "e":
                        if (character.digit[4] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    case "f":
                        if (character.digit[5] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    case "g":
                        if (character.digit[6] == 1)
                        {
                            lb.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            lb.Background = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void btOpenModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".FND",
                Title = "Open Fnd define file",
            };
            openFile.Filter = "Fnd define files (*.FND)|*.FND";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog())
            {
                dgrSEGLOOKUP.ItemsSource = null;
                FND.SEG_LOOKUP = Extensions.OpenFromFile<ObservableCollection<SegementCharacter>>(openFile.FileName);
                dgrSEGLOOKUP.ItemsSource = FND.SEG_LOOKUP;

            }
        }

        private void btSaveAsModel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "Fnd define files (*.FND)|*.FND";
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.Title = "Save Fnd define file";
            if ((bool)saveDlg.ShowDialog())
            {
                Extensions.SaveToFile(FND.SEG_LOOKUP, saveDlg.FileName);
            }
        }

        public void Update(ObservableCollection<SegementCharacter> characters)
        {
            dgrSEGLOOKUP.ItemsSource = null;
            FND.SEG_LOOKUP = characters.Clone();
            dgrSEGLOOKUP.ItemsSource = FND.SEG_LOOKUP;
        }
    }
}
