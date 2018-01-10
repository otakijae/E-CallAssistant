using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        StartPage firstPage = new StartPage();
        StartPage secondPage = new StartPage();
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));

        public MainPage()
        {
            InitializeComponent();
        }

        private void word2vec_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //다른 파이썬으로 실행
                string python = @"C:\Python36\python.exe";
                //string python = Environment.CurrentDirectory + @"\Python36\python.exe";
                string myPythonApp = "WordClassification.py";

                //string으로 핵심단어들 받아서, List에 넣고, List에서 단어 하나씩 뽑아서 하나의 string으로 만듬
                //string wordOne, wordTwo, keyWords = "";
                //List<string> exampleList = new List<string>();
                //exampleList.Add("ONE");
                //exampleList.Add("TWO");
                //exampleList.Add("THREE");
                //foreach (string i in exampleList)
                //{
                //    keyWords = keyWords + i + ",";
                //}
                //keyWords = keyWords.Remove(keyWords.Length - 1);

                string keyWords = problemText.Text.Remove(problemText.Text.Length - 1);

                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcessStartInfo.Arguments = myPythonApp + " " + keyWords;

                Process myProcess = new Process();
                myProcess.StartInfo = myProcessStartInfo;
                myProcess.Start();
                StreamReader myStreamReader = myProcess.StandardOutput;
                string codeResult = myStreamReader.ReadToEnd();
                myProcess.WaitForExit();
                myProcess.Close();

                codeText.Text = codeResult;
            }
            catch (Exception ex)
            {
                codeText.Text = ex.Message;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }
    }
}
