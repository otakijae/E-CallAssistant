using System;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using System.Configuration;
using Microsoft.CognitiveServices.SpeechRecognition;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Net.Http.Headers;

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AutoResetEvent _FinalResponceEvent;
        MicrophoneRecognitionClient _microphoneRecognitionClient;

        public MainWindow()
        {
            InitializeComponent();
            SpeakBtn.Content = "Start Recording";
            _FinalResponceEvent = new AutoResetEvent(false);
            Responsetxt.Background = Brushes.White;
            Responsetxt.Foreground = Brushes.Black;
        }

        private void ConvertSpeechToText()
        {
            var speechRecognitionMode = SpeechRecognitionMode.ShortPhrase;  //LongDictation 대신 ShortPhrase 선택
            string language = "en-us";
            string subscriptionKey = ConfigurationManager.AppSettings["MicrosoftSpeechApiKey"].ToString();
            _microphoneRecognitionClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                speechRecognitionMode,
                language,
                subscriptionKey
                );
        
        
            _microphoneRecognitionClient.OnPartialResponseReceived += ResponseReceived;
            _microphoneRecognitionClient.StartMicAndRecognition();
        }

        //Textbox에 text입력
        private void ResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            string result = e.PartialResult;
            Dispatcher.Invoke(() =>
            {
                Responsetxt.Text = (e.PartialResult);
                Responsetxt.Text += ("\n");
            });
        }

        //START버튼 누른후 음성인식 시작
        private void SpeakBtn_Click(object sender, RoutedEventArgs e)   
        {
            SpeakBtn.Content = "Listening ...";
            SpeakBtn.IsEnabled = false;
            Responsetxt.Background = Brushes.Green;
            Responsetxt.Foreground = Brushes.White;
            ConvertSpeechToText();

        }

        //STOP버튼과 함께 음성인식 종료
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                _FinalResponceEvent.Set();
                _microphoneRecognitionClient.EndMicAndRecognition();
                _microphoneRecognitionClient.Dispose();
                _microphoneRecognitionClient = null;
                SpeakBtn.Content = "Start Recording";
                SpeakBtn.IsEnabled = true;
                Responsetxt.Background = Brushes.White;
                Responsetxt.Foreground = Brushes.Black;
            }));
        }

        //Text Analytics API 사용 감정상태 분석
        private async void keyPhrasesRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var keyPhrasesUri = "https://eastasia.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases?" + queryString;   //핵심어구
            byte[] byteData = Encoding.UTF8.GetBytes("{'documents': [{ 'id': 'inputText', 'text': '" + Responsetxt.Text + "'}]}");
            var content = new ByteArrayContent(byteData);
            HttpResponseMessage keyPhrasesResponse = null;

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2ee0076fd06543029e0668de66971b68");
            queryString["numberOfLanguagesToDetect"] = "1";
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            keyPhrasesResponse = await client.PostAsync(keyPhrasesUri, content);
            keyPhrasesTxt.Text = keyPhrasesResponse.Content.ReadAsStringAsync().Result;

         
            content.Dispose();
        }

        private async void sentimentRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var sentimentUri = "https://eastasia.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + queryString;      //감정상태
             byte[] byteData = Encoding.UTF8.GetBytes("{'documents': [{ 'id': 'inputText', 'text': '" + Responsetxt.Text + "'}]}");
             var content = new ByteArrayContent(byteData);
            HttpResponseMessage sentimentUriResponse = null;

            
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2ee0076fd06543029e0668de66971b68");
            queryString["numberOfLanguagesToDetect"] = "1";
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            sentimentUriResponse = await client.PostAsync(sentimentUri, content);
            sentimentTxt.Text = sentimentUriResponse.Content.ReadAsStringAsync().Result;

            content.Dispose();
            //keyPhrasesTxt.Text = keyPhrasesResponse.Content.ReadAsStringAsync().Result;
            //sentimentTxt.Text = keyPhrasesResponse.Content.ReadAsStringAsync().Result;

            //MessageBox.Show(response.Content.ReadAsStringAsync().Result);
        }

        private void AnalyzeStart_Click(object sender, RoutedEventArgs e)
        {
            keyPhrasesRequest();
            sentimentRequest();
        }
    }
}
