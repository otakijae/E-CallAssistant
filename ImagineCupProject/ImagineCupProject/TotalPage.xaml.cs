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
        int totalNumber=0;
        int code1 = 0;
        int code2 = 0;
        public TotalPage()
        {
            InitializeComponent();
            this.DataContext = new ChartViewModel();
        }

        
    }
}
