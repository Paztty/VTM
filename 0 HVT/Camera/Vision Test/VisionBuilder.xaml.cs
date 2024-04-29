using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Camera
{
    /// <summary>
    /// Interaction logic for VisionBuilder.xaml
    /// </summary>
    public partial class VisionBuilder : UserControl
    {

        private VisionModel _Models = new VisionModel();
        public VisionModel Models
        {
            get { return _Models; }
            set
            {
                if (value != null && value != _Models)
                {
                    _Models = value;
                    functionCanvas.Children.Clear();
                    foreach (var item in Models.FNDs)
                    {
                        item.SetParentCanvas(functionCanvas);
                    }
                    foreach (var item in Models.LCDs)
                    {
                        item.SetParentCanvas(functionCanvas);
                    }
                    foreach (var item in Models.GLED)
                    {
                        item.SetParentCanvas(functionCanvas);
                    }
                    foreach (var item in Models.LED)
                    {
                       item.SetParentCanvas(functionCanvas);
                    }
                }
                
            }
        }

        bool RenderFinnish = false;

        public VisionBuilder()
        {
            InitializeComponent();
            functionCanvas.Children.Clear();
        }


        private void DrawingCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void DrawingCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.Handled == false)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var focusElement = Keyboard.FocusedElement;
                    if (focusElement != null && focusElement.GetType() == typeof(Label) && (e.Source as FrameworkElement) == functionCanvas)
                    {
                        focusElement.RaiseEvent(e);
                        //Canvas.SetTop((Label)focusElement, e.GetPosition(DrawingCanvas).Y);
                        //Canvas.SetRight((Label)focusElement, e.GetPosition(DrawingCanvas).X);
                    }
                }
            }

        }

        private void DrawingCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                Keyboard.ClearFocus();
                FocusManager.SetFocusedElement(functionCanvas, null);
            }
        }

        private void functionCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderFinnish = true;
            foreach (var item in Models.FNDs)
            {
                item.SetParentCanvas(functionCanvas);
            }
            foreach (var item in Models.LCDs)
            {
                item.SetParentCanvas(functionCanvas);
            }
            foreach (var item in Models.GLED)
            {
                item.SetParentCanvas(functionCanvas);
            }
            foreach (var item in Models.LED)
            {
                item.SetParentCanvas(functionCanvas);
            }
        }
    }
}
