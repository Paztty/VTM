using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for CircleGraph.xaml
    /// </summary>
    public partial class CircleGraph : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CancellationTokenSource _shutDown = new CancellationTokenSource();
        private double pass = 1;
        private double fail = 1;
        public double Pass
        {
            get { return pass; }
            set
            {
                pass = value;
                NotifyPropertyChanged(nameof(Pass));
                NotifyPropertyChanged(nameof(Total));
                NotifyPropertyChanged(nameof(Percentage));
                Draw();
            }
        }
        public double Fail
        {
            get { return fail; }
            set
            {
                fail = value;
                NotifyPropertyChanged(nameof(Fail));
                NotifyPropertyChanged(nameof(Total));
                NotifyPropertyChanged(nameof(Percentage));
                Draw();
            }
        }
        public double Total
        {
            get { return Pass + Fail; }
        }



        public double Percentage
        {
            get { return Pass / Total; }
        }

        public CircleGraph()
        {
            InitializeComponent();
            this.DataContext = this;
            Timer.Enabled = true;
            Timer.Elapsed += Timer_Elapsed;
        }



        private GeometryDrawing CreatePathGeometry(Brush brush, Point startPoint, Point arcPoint, bool isLargeArc)
        {
            var midPoint = new Point(_pieChartImage.Width / 2, _pieChartImage.Height / 2);

            var drawing = new GeometryDrawing { Brush = brush };
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure { StartPoint = midPoint };

            var ls1 = new LineSegment(startPoint, false);
            var arc = new ArcSegment
            {
                SweepDirection = SweepDirection.Clockwise,
                Size = new Size(_pieChartImage.Width / 2, _pieChartImage.Height / 2),
                Point = arcPoint,
                IsLargeArc = isLargeArc
            };
            var ls2 = new LineSegment(midPoint, false);

            drawing.Geometry = pathGeometry;
            pathGeometry.Figures.Add(pathFigure);

            pathFigure.Segments.Add(ls1);
            pathFigure.Segments.Add(arc);
            pathFigure.Segments.Add(ls2);

            return drawing;
        }
        Timer Timer = new Timer()
        {
            Interval = 50,
        };

        double Angle = 1;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Angle += ((360 - Angle) / 10 > 1 ? (360 - Angle) / 10 : 1) ;
            if (Angle >= 360)
            {
                Angle = 359;
                Timer.Stop();
            }
            if (!this._shutDown.IsCancellationRequested)
            {
                this.Dispatcher.Invoke(new Action(() =>
            {
                var di = new DrawingImage();
                _pieChartImage.Source = di;

                var dg = new DrawingGroup();
                di.Drawing = dg;
                lbPercent.Content = Math.Round(Percentage * 100, 2);
                var angle = Angle * Percentage;
                var radians = (Math.PI / 180) * angle;
                var endPointX = Math.Sin(radians) * _pieChartImage.Height / 2 + _pieChartImage.Height / 2;
                var endPointY = _pieChartImage.Width / 2 - Math.Cos(radians) * _pieChartImage.Width / 2;
                var endPoint = new Point(endPointX, endPointY);

                dg.Children.Add(CreatePathGeometry(new SolidColorBrush(Color.FromArgb(255, 0, 113, 0)), new Point(_pieChartImage.Width / 2, 0), endPoint, radians >= (Math.PI)));
                dg.Children.Add(CreatePathGeometry(new SolidColorBrush(Color.FromArgb(255, 170, 0, 0)), endPoint, new Point(_pieChartImage.Width / 2, 0), radians < (Math.PI)));
            }));
            }
            if (Angle >= 360)
            {
                Angle = 0;
            }
        }
        public void Draw()
        {
            Angle = 0;
            Timer.Start();
        }
    }
}
