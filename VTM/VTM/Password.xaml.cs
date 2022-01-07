using HVT.StandantLocalUsers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VTM
{
    /// <summary>
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class Password : Window
    {
        Users.Permissions permissions = Users.Permissions.None;
        public Password(Users.Permissions permissions)
        {
            InitializeComponent();
            this.permissions = permissions;
            LoginStatus.Text = permissions.ToString() + " password request.";
            LoginStatus.Foreground = new SolidColorBrush(Colors.LightBlue);
            AdminPassword.Focus();
            AdminPassword.KeyDown += AdminPassword_PreviewKeyDown;

            // for fast debug
            
        }

        private void AdminPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (!MainWindow.Program.CheckPassWord(AdminPassword.Password, this.permissions))
                {
                    LoginStatus.Text = "Wrong " + permissions.ToString() + " password.";
                    LoginStatus.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        private void ClosePassword_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.Program.CheckPassWord(AdminPassword.Password, this.permissions))
            {
                LoginStatus.Text = "Wrong " + permissions.ToString() + " password.";
                LoginStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
