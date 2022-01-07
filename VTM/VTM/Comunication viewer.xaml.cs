using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HVT.VTM.Program;

namespace VTM
{
    /// <summary>
    /// Interaction logic for Comunication_viewer.xaml
    /// </summary>
    public partial class Comunication_viewer : Window
    {
        Program Program;
        public FlowDocument document { get; set; } = new FlowDocument();

        public Comunication_viewer( Program program)
        {
            this.Program = program;
            InitializeComponent();

            rtbUUT1.Content = this.Program.UUTs[0].LogBox;
            rtbUUT2.Content = this.Program.UUTs[1].LogBox;
            rtbUUT3.Content = this.Program.UUTs[2].LogBox;
            rtbUUT4.Content = this.Program.UUTs[3].LogBox;

            rtbMUX1.Content = program.MUX_CARD.Port1.LogBox;
            rtbMUX2.Content = program.MUX_CARD.Port2.LogBox;

            rtbDMM1.Document = document;
        }
    }
}
