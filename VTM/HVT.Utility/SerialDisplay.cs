using System.Timers;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HVT.Utility
{
    public class SerialDisplay
    {
        public Rectangle IsOpenRect;
        public Rectangle TX;
        public Rectangle RX;

        Timer timer = new Timer()
        {
            Interval = 150,
        };

        public SerialDisplay()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TX?.Dispatcher.Invoke(new System.Action(delegate
            {
                TX.Fill = new SolidColorBrush(Colors.DarkRed);
            }));
            RX?.Dispatcher.Invoke(new System.Action(delegate
            {
                RX.Fill = new SolidColorBrush(Colors.DarkGreen);
            }));
            timer.Stop();
        }

        public void BlinkTX()
        {
            TX?.Dispatcher.Invoke(new System.Action(delegate
            {
                TX.Fill =  new SolidColorBrush(Colors.Red);
            }), System.Windows.Threading.DispatcherPriority.Send);
            timer.Start();
        }

        public void BlinkRX()
        {
            RX?.Dispatcher.Invoke(new System.Action(delegate
            {
                RX.Fill = new SolidColorBrush(Colors.LightGreen);
            }), System.Windows.Threading.DispatcherPriority.Send);
            timer.Start();
        }

        public void ShowCOMStatus(bool IsOpen)
        {
            if (IsOpenRect != null)
            {
                IsOpenRect.Dispatcher.BeginInvoke(new System.Action(delegate
                 {
                     IsOpenRect.Fill = IsOpen ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Gray);
                 }), System.Windows.Threading.DispatcherPriority.Send);
            }
        }
    }
}
