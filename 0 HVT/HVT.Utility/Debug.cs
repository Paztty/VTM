using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using DataFormats = System.Windows.DataFormats;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace HVT.Utility
{
    public static class Debug
    {
        public enum ContentType
        {
            Error = 0,
            Notify = 1,
            Log = 2,
            Warning = 3,
        }

        public static RichTextBox LogBox = new RichTextBox()
        {
            Background = new SolidColorBrush(Colors.Black)
        };
        public static Dispatcher dispatcher;

        public static void Write(string content, ContentType type, int Size = 0)
        {
            LogBox.Dispatcher.BeginInvoke(new Action(delegate
            {
                var paragraph = new Paragraph();
                var run1 = new Run(DateTime.Now.ToString("HH:mm:ss -> "));
                run1.Foreground = new SolidColorBrush(Colors.Cyan);
                var run2 = new Run(content);
                if (Size > 0)
                {
                    run2.FontSize = Size;
                    run2.FontWeight = FontWeights.DemiBold;

                }
                switch (type)
                {
                    case ContentType.Error:
                        run2.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case ContentType.Notify:
                        run2.Foreground = new SolidColorBrush(Colors.LightGreen);
                        break;
                    case ContentType.Log:
                        run2.Foreground = new SolidColorBrush(Colors.White);
                        break;
                    case ContentType.Warning:
                        run2.Foreground = new SolidColorBrush(Colors.Yellow);
                        break;
                    default:
                        break;
                }

                paragraph.Inlines.Add(run1);
                paragraph.Inlines.Add(run2);

                if (LogBox.Document.Blocks.Count > 100)
                {
                    LogBox.Document.Blocks.Clear();
                }
                //File.AppendAllText("VTMLog.txt", DateTime.Now.ToString("HH:mm:ss -> ") + content + Environment.NewLine);

                LogBox.Document.Blocks.Add(paragraph);
                LogBox.ScrollToEnd();
            }), DispatcherPriority.Normal);
        }
        public static void Appent(string content, ContentType type)
        {
            LogBox.Dispatcher.BeginInvoke(new Action(delegate
            {
                var paragraph = new Paragraph();
                //File.AppendAllText("VTMLog.txt", content + Environment.NewLine);
                paragraph.Inlines.Add(new Run("\t" + content));
                switch (type)
                {
                    case ContentType.Error:
                        paragraph.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case ContentType.Notify:
                        paragraph.Foreground = new SolidColorBrush(Colors.LightGreen);
                        break;
                    case ContentType.Log:
                        paragraph.Foreground = new SolidColorBrush(Colors.White);
                        break;
                    case ContentType.Warning:
                        paragraph.Foreground = new SolidColorBrush(Colors.Yellow);
                        break;
                    default:
                        break;
                }
                if (LogBox.Document.Blocks.Count > 100)
                {
                    LogBox.Document.Blocks.Clear();
                }
                LogBox.Document.Blocks.Add(paragraph);
                LogBox.ScrollToEnd();
            }), DispatcherPriority.Normal);
        }

        public static void ClearLog()
        {

            if (dispatcher != null)
            {
                dispatcher.Invoke(new Action(delegate
                {
                    LogBox.Document.Blocks.Clear();
                }), DispatcherPriority.Normal);
            }
        }
    }
}
