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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Configuration;
using Microsoft.CognitiveServices.SpeechRecognition;
using System.Collections;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Google.Cloud.Storage.V1;
using Google.Protobuf.Collections;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using System.IO;
using Google.Cloud.Speech.V1;
using System.Data.SqlClient;

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        AzureDatabase azureDatabase;
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        MicrophoneRecognitionClient _microphoneRecognitionClient;
        int a=0;
        public MainPage()
        {
            InitializeComponent();
            mainFrame.Content = new MainQuestion();
            //AsyncRecognizeGcs("gs://emergencycall/911 pizza call - policer.wav");

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

        

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if(nextButton.Content.Equals("Next"))
            { 
                mainFrame.Content = new AdditionalQuestion();
                nextButton.Content = "Before";
            }
            else
            {
                mainFrame.Content = new MainQuestion();
                nextButton.Content = "Next";
            }

        }
        
    }
}
