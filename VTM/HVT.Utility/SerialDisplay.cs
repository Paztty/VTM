using System.Windows.Media;
using System.Windows.Shapes;

namespace HVT.Utility
{
    public class SerialDisplay
    {
        public Rectangle IsOpenRect;
        public Rectangle TX;
        public Rectangle RX;

        private bool blinkRX = true;
        private bool blinkTX = true;

        public void BlinkTX()
        {
            TX?.Dispatcher.Invoke(new System.Action(delegate
            {
                TX.Fill = blinkTX == true ? new SolidColorBrush(Colors.DarkRed) : new SolidColorBrush(Colors.Red);
            }));
            blinkTX = !blinkTX;
        }

        public void BlinkRX()
        {
            RX?.Dispatcher.Invoke(new System.Action(delegate
            {
                RX.Fill = blinkRX == true ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.LightGreen);
            }));
            blinkRX = !blinkRX;
        }

        public void ShowCOMStatus(bool IsOpen)
        {
            if (IsOpenRect != null)
            {
                IsOpenRect.Dispatcher.BeginInvoke(new System.Action(delegate
                 {
                     IsOpenRect.Fill = IsOpen ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Gray);
                 }));
            }
        }
    }
}
