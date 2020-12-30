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
    /// Interaction logic for ShiftDialog.xaml
    /// </summary>
    public partial class ShiftDialog : Window
    {
        public string X { get; set; }
        public string Y { get; set; }
        public ShiftDialog(string x, string y)
        {
            InitializeComponent();
            X = x;
            Y = y;
            ShiftXBox.Text = X;
            ShiftYBox.Text = Y;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            X = ShiftXBox.Text;
            Y = ShiftYBox.Text;
        }
    }
}
