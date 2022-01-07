using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

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

        public static RichTextBox LogBox;
        public static Dispatcher dispatcher;

        public static void Write(string content, ContentType type)
        {
            if (LogBox != null)
            {
                var paragraph = new Paragraph();

                paragraph.Inlines.Add(new Run(DateTime.Now.ToString("HH:mm:ss ") + content));
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

                if (LogBox.Dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(new Action(delegate
                    {
                        LogBox.Document.Blocks.Add(paragraph);
                        LogBox.ScrollToEnd();
                    }), DispatcherPriority.Normal);
                }
            }
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
