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
using System.Data.SqlClient;

namespace ImagineCupProject
{
    /// <summary>
    /// TotalPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TotalPage : Page
    {
        AzureDatabase azureDatabase;
        string maxName;
        string maxCode;
        int maxTime;
        string codeName;
        public TotalPage()
        {
            InitializeComponent();
            this.DataContext = new ChartViewModel();
            azureDatabase = new AzureDatabase();
            maxName = azureDatabase.PrintData();
            totalCallNumber();
            maxTime = Convert.ToInt32(maxName.Split(' ')[0]) * 2 - 2;
            maxCode = maxName.Split(' ')[1];
            if (maxCode == "0")
            {
                codeName = "Terror & Gunshot";
            }
            else if (maxCode == "1")
            {
                codeName = "Disaster";
            }
            else if (maxCode == "2")
            {
                codeName = "Fire";
            }
            else if (maxCode == "3")
            {
                codeName = "Violence";
            }
            else if (maxCode == "4")
            {
                codeName = "Motor Vehicle Accidents";
            }
            else if (maxCode == "5")
            {
                codeName = "Not emergency";
            }
            codeInformation.Text = codeName + "-related calls were the most frequent.";
            timeInformation.Text = "The most frequent call time is " + maxTime + ".";
        }

        public void totalCallNumber()
        {
            int total = azureDatabase.PrintTotalNumber();
            operatorReceiveNumText.Text = "    operator JW received a total of " + total + " calls this month.";
        }



    }
}
