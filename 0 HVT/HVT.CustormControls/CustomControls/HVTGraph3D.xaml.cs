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
using System.Windows.Threading;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for HVTGraph3D.xaml
    /// </summary>
    public partial class HVTGraph3D : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Point> A_Points { get; set; } = new ObservableCollection<Point>();
        public ObservableCollection<Point> B_Points { get; set; } = new ObservableCollection<Point>();

        private Point BaseA = new Point(12, 25);
        private Point BaseB = new Point(24, 45);

        private string graphName = "";
        public string GraphName
        {
            get { return graphName; }
            set
            {
                if (graphName != value)
                {
                    graphName = value;
                    NotifyPropertyChanged(nameof(GraphName));
                }
            }
        }


        public HVTGraph3D()
        {
            InitializeComponent();
            for (int i = 0; i < 200; i++)
            {
                Line line1 = new Line()
                {
                    X1 = 0,
                    Y1 = i * 50,
                    X2 = 30,
                    Y2 = i * 50 + 50,
                    StrokeDashArray = new DoubleCollection(2) { 5, 5 },
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(118, 118, 118))
                };
                graphCanvasBacground.Children.Add(line1);
                Line line2 = new Line()
                {
                    X1 = 30,
                    Y1 = (i + 1) * 50,
                    X2 = 1000,
                    Y2 = (i + 1) * 50,
                    StrokeDashArray = new DoubleCollection(2) { 5, 5 },
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(118, 118, 118))

                };
                graphCanvasBacground.Children.Add(line2);

                Line line3 = new Line()
                {
                    X1 = i * 50,
                    Y1 = 0,
                    X2 = i * 50 + 30,
                    Y2 = 50,
                    StrokeDashArray = new DoubleCollection(2) { 5, 5 },
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(118, 118, 118))
                };
                graphCanvasBacground.Children.Add(line3);
                Line line4 = new Line()
                {
                    X1 = i * 50 + 30,
                    Y1 = 50,
                    X2 = i * 50 + 30,
                    Y2 = 1000,
                    StrokeDashArray = new DoubleCollection(2) { 5, 5 },
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(118, 118, 118))

                };
                graphCanvasBacground.Children.Add(line4);

                Label label = new Label()
                {
                    Content = i * 250,
                    Margin = new Thickness(0, 985 - (i * 50), 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                };
                xAxis.Children.Add(label);
                Label label2 = new Label()
                {
                    Content = i * 10,
                    Margin = new Thickness(i * 50 - 5, 5, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                };
                yAxis.Children.Add(label2);
            }

            mainScroll.ScrollToLeftEnd();
            mainScroll.ScrollToBottom();
            A_graph.Points.Add(BaseA);
            B_graph.Points.Add(BaseB);
            A_graph.Points.Add(new Point(12, 20));
            B_graph.Points.Add(new Point(24, 40));
            //ClearChart();
            this.DataContext = this;
        }


        private void GraphCanvasMouse_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.MouseDevice.GetPosition(graphCanvasMouse);

            MouseVertical.X1 = point.X;
            MouseVertical.Y1 = 25;
            MouseVertical.X2 = point.X;
            MouseVertical.Y2 = 1000;

            MouseHorizontal.X1 = 15;
            MouseHorizontal.Y1 = point.Y;
            MouseHorizontal.X2 = 1000;
            MouseHorizontal.Y2 = point.Y;

            MouseZ.X1 = point.X - 15;
            MouseZ.Y1 = point.Y - 25;
            MouseZ.X2 = point.X + 15;
            MouseZ.Y2 = point.Y + 25;

            graphGrid.PreviewMouseDown -= GraphCanvasMouse_MouseMove;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            xScroll.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
            yScroll.ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset);
            Console.WriteLine((sender as ScrollViewer).VerticalOffset);
        }

        public void ClearChart()
        {
            A_graph.Points.Clear();
            B_graph.Points.Clear();
            A_graph.Points.Add(new Point(12, 20));
            B_graph.Points.Add(new Point(24, 40));
        }

        public void AddToA(Point point)
        {
            A_graph.Points.Remove(BaseA);
            A_graph.Points.Add(new Point(point.X + 12, point.Y * 2 + 20));
            BaseA = new Point(point.X + 12, 20);
            A_graph.Points.Add(BaseA);
        }

        public void AddToB(Point point)
        {
            B_graph.Points.Remove(BaseB);
            B_graph.Points.Add(new Point(point.X + 24, point.Y * 2 + 40));
            BaseB = new Point(point.X + 24, 20);
            B_graph.Points.Add(BaseB);
        }

        public void AddToA(Point[] points)
        {
            foreach (var item in points)
            {
                A_graph.Points.Add(item);
            }
        }

        public void AddToB(Point[] points)
        {
            foreach (var item in points)
            {
                B_graph.Points.Add(item);
            }
        }
    }
}
