using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VTM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        SplashScreen splashScreen = new SplashScreen();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //initialize the splash screen and set it as the application main window
            this.MainWindow = splashScreen;
            splashScreen.Show();
            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread

            var mainWindow = new MainWindow();
            mainWindow.Loaded += MainWindow_Loaded;
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            splashScreen.Close();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.Message.ToString());
            Console.WriteLine(e.Exception.StackTrace.ToString());
        }
    }
}
