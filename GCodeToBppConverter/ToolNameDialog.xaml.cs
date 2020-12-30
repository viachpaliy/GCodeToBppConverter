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

namespace GCodeToBppConverter
{
    /// <summary>
    /// Interaction logic for ToolNameDialog.xaml
    /// </summary>
    public partial class ToolNameDialog : Window
    {
        public string ToolName { get; set; }
        public ToolNameDialog(string toolName)
        {
            InitializeComponent();
            ToolName = toolName;
            ToolNameBox.Text = ToolName;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            ToolName = ToolNameBox.Text;
        }
    }
}
