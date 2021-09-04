using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace VTM
{
    public static class Support
    {
        public static RichTextBox tbLog;
        public static void Write(string content)
        {
            tbLog.Document.Blocks.Add(new Paragraph(new Run(content)));
            tbLog.ScrollToEnd();
        }
        public static void WriteLine(string content)
        {
            tbLog.Document.Blocks.Add(new Paragraph(new Run(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss -  ") + content + Environment.NewLine)));
            tbLog.ScrollToEnd();
        }
        public static void AppentLine(string content)
        {
            tbLog.Document.Blocks.Add(new Paragraph(new Run(content + Environment.NewLine)));
            tbLog.ScrollToEnd();
        }
    }
}
