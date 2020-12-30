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
    /// Interaction logic for PieceDialog.xaml
    /// </summary>
    public partial class PieceDialog : Window
    {
        public string Lpx { get; set; }
        public string Lpy { get; set; }

        public string Lpz { get; set; }

        public string OriginList { get; set; }
        
        public PieceDialog(string lpx, string lpy, string lpz, string origins)
        {
            InitializeComponent();
            Lpx = lpx;
            Lpy = lpy;
            Lpz = lpz;
            OriginList = origins;
            LpxTxtBox.Text = Lpx;
            LpyTxtBox.Text = Lpy;
            LpzTxtBox.Text = Lpz;
            OrgTxtBox.Text = OriginList;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Lpx = this.LpxTxtBox.Text;
            this.Lpy = this.LpyTxtBox.Text;
            this.Lpz = this.LpzTxtBox.Text;
            OriginList = OrgTxtBox.Text;
        }
    }
}
