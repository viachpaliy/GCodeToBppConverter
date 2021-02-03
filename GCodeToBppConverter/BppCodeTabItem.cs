using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GCodeToBppConverter 
{
    class BppCodeTabItem : TabItem
    {
        public TextBox bppCode;
        public string fileName = "";
        public BppCodeTabItem(string code)
        {
            bppCode = new TextBox();
            bppCode.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            bppCode.Text = code;
            Content = bppCode;
            Header = "";
        }
    }
}
