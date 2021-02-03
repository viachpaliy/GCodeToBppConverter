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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace GCodeToBppConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string gcodeFile = "";
        double Lpx = 800;
        double Lpy = 500;
        double Lpz = 18;
        string OriginList = "5,8";
        string ToolName = "";
        double ShiftX = 0;
        double ShiftY = 0;
        string bppCodeFile = "";
        string[] gcodeLines;
        List<string> BppCode = new List<string>();
        List<List<string>> BppPrg = new List<List<string>>();
        int numberLines = 100000;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                gcodeFile = openFileDialog.FileName;
                Title = gcodeFile;
                string gCode = "";
                try
                {
                    // Open the text file using a stream reader.
                    using (var sr = new StreamReader(gcodeFile))
                    {
                        // Read the stream as a string, and write the string to the console.
                        gCode = sr.ReadToEnd();
                    }
                }
                catch (IOException e1)
                {
                    MessageBox.Show("The file could not be read:\n" + e1.Message);

                }
                //GCode.Text = gCode;
                BppCodeTabItem item = new BppCodeTabItem(gCode);
                Vkladki.Items.Add(item);
                //BppCodeTabItem item = (BppCodeTabItem)Vkladki.SelectedContent;
                item.fileName = gcodeFile;
                item.Header = System.IO.Path.GetFileName(gcodeFile);
                PieceDialogCommand();
                ToolButtonCommand();
                ShiftButtonCommand();
            }
            else
            {
                MessageBox.Show("You need choise a Gcode file!!!");
            }
        }

        private void PieceDialogCommand()
        {
            PieceDialog pieceDialog = new PieceDialog(Lpx.ToString(), Lpy.ToString(), Lpz.ToString(), OriginList);
            if (pieceDialog.ShowDialog() == true)
            {
                Lpx = double.Parse(pieceDialog.Lpx.Replace(',', '.'), CultureInfo.InvariantCulture);
                Lpy = double.Parse(pieceDialog.Lpy.Replace(',', '.'), CultureInfo.InvariantCulture);
                Lpz = double.Parse(pieceDialog.Lpz.Replace(',', '.'), CultureInfo.InvariantCulture);
                OriginList = pieceDialog.OriginList;
            }
        }

        private void ToolButtonCommand()
        {
            ToolNameDialog toolNameDialog = new ToolNameDialog(ToolName);
            if (toolNameDialog.ShowDialog() == true)
            {
                ToolName = toolNameDialog.ToolName;
            }
        }

        private void ShiftButtonCommand()
        {
            ShiftDialog shiftDialog = new ShiftDialog(ShiftX.ToString(), ShiftY.ToString());
            if (shiftDialog.ShowDialog() == true)
            {
                ShiftX = double.Parse(shiftDialog.X.Replace(',', '.'), CultureInfo.InvariantCulture);
                ShiftY = double.Parse(shiftDialog.Y.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
        }

        private void SaveButtonCommand()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "bpp";
            string initDir = @"C:\WNC\home\d_xnc\p_p\prog\";
            if (Directory.Exists(initDir))
            {
                saveFileDialog.InitialDirectory = initDir;
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                bppCodeFile = saveFileDialog.FileName;
            }
            using (StreamWriter outputFile = new StreamWriter(bppCodeFile))
            {
               BppCodeTabItem item = (BppCodeTabItem) Vkladki.SelectedItem;
                outputFile.WriteLine(item.bppCode.Text);
                item.fileName = saveFileDialog.FileName;
                item.Header = System.IO.Path.GetFileName(item.fileName);
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveButtonCommand();
        }

        private void ShiftButtonClick(object sender, RoutedEventArgs e)
        {
            ShiftButtonCommand();
        }
        private void ToolButtonClick(object sender, RoutedEventArgs e)
        {
            ToolButtonCommand();
        }
        private void PieceButtonClick(object sender, RoutedEventArgs e)
        {
            PieceDialogCommand();
        }

        private void NcButtonClick(object sender, RoutedEventArgs e)
        {
            BsXncSocketServer.Document doc = new BsXncSocketServer.Document();
            BppCodeTabItem item = (BppCodeTabItem)Vkladki.SelectedItem;
            if (item.fileName != "")
            {
                doc.ProgramSelect(item.fileName, "");
            }
        }

        private void RunButtonClick(object sender, RoutedEventArgs e)
        {
            
            BppCode.Clear();
            BppCode.Add(GetBppHeader());
            BppCode.Add(GetBppVariables());
            BppCode.Add("\n[PROGRAM]\n");
            GetShiftOp();
            GetRoutOp();
            GetBppCodeLines();
            GetEndFile();
            StringBuilder sb1 = new StringBuilder();

            foreach (var str in BppCode)
            {
                sb1.Append(str);
            }

            Vkladki.Items.Add(new BppCodeTabItem(sb1.ToString()));

            //TextBox tb1 = new TextBox();
            //tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            //tb1.Text = sb1.ToString();
            //Vkladki.Items.Add(new TabItem
            //{
             //   Content = tb1
            //});
        }

        private void GetEndFile()
        {
            string code = @"  @ ENDPATH, """", """", 95894508, """", 0 :

[VBSCRIPT]

[MACRODATA]

[TDCODES]

[PCF]

[TOOLING]

[SUBPROGS]

";
            BppCode.Add(code + '\n');

        }
        private void GetShiftOp()
        {
            if (!(ShiftX==0 && ShiftY == 0))
            {
                string shiftCode = String.Format(@"@ SHIFT, """", """", 374974684, """", 0 : {0}, {1}", Convert.ToString(ShiftX).Replace(',', '.'), Convert.ToString(ShiftY).Replace(',', '.'));
                BppCode.Add(shiftCode + '\n');
            }
        }
        private void GetRoutOp()
        {
            string code = String.Format(@"@ ROUT, """", """", 95893420, """", 0 : ""P1001"", 0, ""1"", 0, 0, """", 1, 1.6, -1, 0, 0, 32, 32, 50, 0, 45, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, -1, 0, 0, 0, 0, 0, ""{0}"", 100, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, """", 0, 0, 0, 0, 0, 0, 0, 0, 0, """", 5, 0, 20, 80, 60, 0, """", """", ""ROUT"", 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.1, 0, 0, 0, 99, 0
  @ START_POINT, """", """", 95893036, """", 0 : 0, 0, 0", ToolName);
            BppCode.Add(code + '\n');
        }

        private void GetRoutOp(double x, double y, double z)
        {
            string code = String.Format(@"@ ROUT, """", """", 95893420, """", 0 : ""P1001"", 0, ""1"", 0, 0, """", 1, 1.6, -1, 0, 0, 32, 32, 50, 0, 45, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, -1, 0, 0, 0, 0, 0, ""{0}"", 100, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, """", 0, 0, 0, 0, 0, 0, 0, 0, 0, """", 5, 0, 20, 80, 60, 0, """", """", ""ROUT"", 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.1, 0, 0, 0, 99, 0
  @ START_POINT, """", """", 95893036, """", 0 : {1}, {2}, 0", ToolName, Convert.ToString(x).Replace(',', '.'), Convert.ToString(y).Replace(',', '.'));
            BppCode.Add(code + '\n');
            string codeLine = String.Format(@"  @ LINE_EP, """", """", 1, """", 0 : {0}, {1}, 0, {2}, 0, 0, 0, 0, 0", Convert.ToString(x).Replace(',', '.'), Convert.ToString(y).Replace(',', '.'), Convert.ToString(-z).Replace(',', '.'));
            BppCode.Add(codeLine + '\n');
        }
        private void GetBppCodeLines()
        {
            if (gcodeFile == "")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    gcodeFile = openFileDialog.FileName;
                    Title = gcodeFile;
                }
                else
                {
                    MessageBox.Show("You need choise a Gcode file!!!");
                }
            }
            string gCode="";
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new StreamReader(gcodeFile))
                {
                    // Read the stream as a string, and write the string to the console.
                    gCode = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                MessageBox.Show("The file could not be read:\n" + e.Message);
                
            }
            gcodeLines = gCode.Split('\n');
            double Xprev = 0, Yprev = 0, Zprev = 0;

            int startLine = GetStartLine();
            int endLine = GetEndLine(startLine);
            //int endLine = gcodeLines.Length-1;
            int numLine = 0;
            //MessageBox.Show("Start");
            for(int i = startLine; i < endLine; i++)
            {
                double Xi = Xprev;
                double Yi = Yprev;
                double Zi = Zprev;
                Match xMatch = Regex.Match(gcodeLines[i], @".*X(?<x>(\+|-)?\d{1,3}(\.\d{1,3})?).*");
                if (xMatch.Success)
                {
                    Group grp = xMatch.Groups["x"];
                    Xi = double.Parse(grp.Value.Replace(',', '.'), CultureInfo.InvariantCulture); 
                }

                Match yMatch = Regex.Match(gcodeLines[i], @".*Y(?<y>(\+|-)?\d{1,3}(\.\d{1,3})?).*");
                if (yMatch.Success)
                {
                    Group grp = yMatch.Groups["y"];
                    Yi = double.Parse(grp.Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                }

                Match zMatch = Regex.Match(gcodeLines[i], @".*Z(?<z>(\+|-)?\d{1,3}(\.\d{1,3})?).*");
                if (zMatch.Success)
                {
                    Group grp = zMatch.Groups["z"];
                    Zi = double.Parse(grp.Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                }

                double dZ = Zprev - Zi;
                string strId = i.ToString();
                string strX = Convert.ToString(Xi).Replace(',', '.');
                string strY = Convert.ToString(-1*Yi).Replace(',', '.');
                string strZ = Convert.ToString(dZ).Replace(',', '.');
                string codeLine = String.Format(@"  @ LINE_EP, """", """", {0}, """", 0 : {1}, {2}, 0, {3}, 0, 0, 0, 0, 0", strId, strX, strY, strZ);
                BppCode.Add(codeLine + '\n');
                //MessageBox.Show(codeLine);
               Zprev = Zi;
                Xprev = Xi;
                Yprev = Yi;
                numLine += 1;
                if (numLine > numberLines)
                {
                    numLine = 0;
                    GetEndFile();
                    StringBuilder sb = new StringBuilder();

                    foreach (var str in BppCode)
                    {
                        sb.Append(str);
                    }

                    // TextBox tb = new TextBox();
                    // tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    // tb.Text = sb.ToString();
                    // Vkladki.Items.Add(new TabItem
                    // {
                    //     Content = tb
                    // });
                    Vkladki.Items.Add(new BppCodeTabItem(sb.ToString()));
                    BppCode.Clear();
                    BppCode.Add(GetBppHeader());
                    BppCode.Add(GetBppVariables());
                    BppCode.Add("\n[PROGRAM]\n");
                    GetShiftOp();
                    GetRoutOp(Xprev,Yprev,Zprev);
                    
                    
                }
            }
           

        }

        private int GetEndLine(int startLine)
        {
            int endLine = gcodeLines.Length;
            for (int i = startLine; i < gcodeLines.Length; i++)
            {
                if (Regex.Match(gcodeLines[i], @"M02").Success)
                {
                    endLine = i;
                }
            }
            return endLine;
        }

        private int GetStartLine()
        {
            int startLine = 0;
            for (int i = 0; i < gcodeLines.Length; i++)
            {

                if (Regex.Match(gcodeLines[i], @"$T\d").Success)
                {
                    startLine = i + 1;
                }
            }
            return startLine;
        }

        private string GetBppHeader()
        {
            return @"[HEADER]
TYPE=BPP
VER=150

[DESCRIPTION]

";
        }

        private string GetBppVariables()
        {
            return String.Format(@"[VARIABLES]
PAN=LPX|{0}||4|
PAN=LPY|{1}||4|
PAN=LPZ|{2}||4|
PAN=ORLST|""{3}""||3|
PAN=SIMMETRY|1||1|
PAN=TLCHK|0||1|
PAN=TOOLING|""""||3|
PAN=CUSTSTR|$B$KBsExportToNcRvA.XncExtraPanelData$V""""||3|
PAN=FCN|1.000000||2|
PAN=XCUT|0||4|
PAN=YCUT|0||4|
PAN=JIGTH|0||4|
PAN=CKOP|0||1|
PAN=UNIQUE|0||1|
PAN=MATERIAL|""wood""||3|
PAN=PUTLST|""""||3|
PAN=OPPWKRS|0||1|
PAN=UNICLAMP|0||1|
PAN=CHKCOLL|0||1|
PAN=WTPIANI|0||1|
PAN=COLLTOOL|0||1|
PAN=CALCEDTH|0||1|
PAN=ENABLELABEL|0||1|
PAN=LOCKWASTE|0||1|
PAN=LOADEDGEOPT|0||1|
PAN=ITLTYPE|0||1|
PAN=RUNPAV|0||1|
PAN=FLIPEND|0||1|
PAN=ENABLEMACHLINKS|0||1|
PAN=ENABLEPURSUITS|0||1|
PAN=ENABLEFASTVERTBORINGS|0||1|
PAN=FASTVERTBORINGSVALUE|0||4|

", Convert.ToString(Lpx).Replace(',', '.'), Convert.ToString(Lpy).Replace(',', '.'), Convert.ToString(Lpz).Replace(',', '.'), OriginList);
        }





    }
}
