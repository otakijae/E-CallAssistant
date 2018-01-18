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
using Google.Cloud.Storage.V1;
using Google.Protobuf.Collections;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using System.IO;
using Google.Cloud.Speech.V1;
using System.Data.SqlClient;
using Aylien.TextApi;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        MicrophoneRecognitionClient _microphoneRecognitionClient;
        AzureDatabase azureDatabase;
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        Client client = new Client("3b49bfce", "d5788d26c944e091562527416046febb");
        string text = "Yes, I am a teacher at Columbine high school. There is a student here with a gun. He just shot out a window. I believe one um.   I don't know if it's. I don't know what's in my shoulder.  I am. Yes, yes! And the school is in panic and I'm in the library. I've got students down under the tables. Kids! Heads under the tables.  Um, Kids are screaming.  We need police here.  Can you please hurry? I do not know who the student is. ... I was on hall duty, I saw a gun. I said, " +
                "What's going on out there? And the kid that was following me said it was a film production, probably a joke And I said, well I don't think that's a good idea. And went walking outside to see what was going on.  He turned the gun straight at us and shot and, my god, the window went out. I am scared.I want to go home. ";
        string speechRecognitionResult;
        public MainPage()
        {
            InitializeComponent();
            mainFrame.Content = new MainQuestion();
            //AsyncRecognizeGcs("gs://emergencycall/911 pizza call - policer.wav");
            summarize();
             sentimentAnalysis();
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

        private void listViewItem1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = new TotalPage();
        }

        private void listViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = new MainQuestion();
        }

        //음성인식버튼
        private void nextButton2_Click(object sender, RoutedEventArgs e)
        {
            ConvertSpeechToText();
        }

        //  Summarize -  AYLIEN Text Analysis API 
        public void summarize()
        {
            string title = "emergency";
            var summary2 = client.Summarize(text: text, title: title, sentencesNumber: 3).Sentences;

            foreach (var sentence in summary2)
            {
                summary.Text += sentence;
            }
        }

        //  SentimentAnalyze -  AYLIEN Text Analysis API 
        public void sentimentAnalysis()
        {
            Sentiment sentiment2 = client.Sentiment(text: text);
            summary.Text += "\nsentiment : ";
            summary.Text += sentiment2.Polarity + " " + sentiment2.PolarityConfidence;
            summary.Text += "\n";
            summary.Text += sentiment2.Subjectivity + " " + sentiment2.SubjectivityConfidence;
        }

        //Azure SpeechToText
        private void ConvertSpeechToText()
        {
            var speechRecognitionMode = SpeechRecognitionMode.LongDictation;  //LongDictation 대신 ShortPhrase 선택
            string language = "en-us";
            string subscriptionKey = "5e3c0f17ea3f40b39cfb6ec28c77bf3e";
            //string subscriptionKey = ConfigurationManager.AppSettings["5e3c0f17ea3f40b39cfb6ec28c77bf3e"];
            _microphoneRecognitionClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                speechRecognitionMode,
                language,
                subscriptionKey
                );

            //_microphoneRecognitionClient.OnResponseReceived += ResponseReceived;
            _microphoneRecognitionClient.OnPartialResponseReceived += ResponseReceived;
            //_microphoneRecognitionClient.OnResponseReceived += OnMicShortPhraseResponseReceivedHandler;
            _microphoneRecognitionClient.OnResponseReceived += OnMicDictationResponseReceivedHandler;
            _microphoneRecognitionClient.StartMicAndRecognition();
        }

        //Textbox에 text입력
        private void ResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            speechRecognitionResult = e.PartialResult;
            //locationText.Text += result;
            Dispatcher.Invoke(() =>
            {
                /*
                if(e.PartialResult.Contains("am"))
                {
                    temp = e.PartialResult;
                    Responsetxt.Text = temp.Replace("am", "is"); ;
                    Responsetxt.Text += ("\n");

                }
                */
                speechRecognition.Text = (e.PartialResult);
            });
        }

        //LongDictation으로 설정했을때 receiveHandlear (문장 초기화 되기 전)
        private void OnMicDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            //if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.EndOfDictation || e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            //{
            Dispatcher.Invoke(
                (Action)(() =>
                {
                        //_microphoneRecognitionClient.EndMicAndRecognition();

                        WriteResponseResult(e);
                }));
            //}

        }

        //ShortPhrase으로 설정했을때 receiveHandlear
        private void OnMicShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                //codeText.Text += e;

                WriteResponseResult(e);
            }));
        }

        //receiveHandlear 내용 출력 메소드
        private void WriteResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                //codeText.Text += "No phrase response is available.";
            }
            else
            {
                //codeText.Text += "********* Final n-BEST Results *********";
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {
                    //아래내용 다른 textbox에 +=하면 된다. 
                    //speechRecognition.Text += e.PhraseResponse.Results[i].DisplayText; // e.PhraseResponse.Results[i].Confidence +
                }

                //codeText.Text += "\n";
            }
        }

    }
}
