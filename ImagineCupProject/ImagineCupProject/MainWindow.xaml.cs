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
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Google.Cloud.Storage.V1;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using System.IO;

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
            //byte[] byteData = Encoding.UTF8.GetBytes("{'documents': [{ 'id': 'inputText', 'text': '" + Responsetxt.Text + "'}]}");
            var content = new ByteArrayContent(byteData);
            HttpResponseMessage keyPhrasesResponse = null;

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2ee0076fd06543029e0668de66971b68");
            queryString["numberOfLanguagesToDetect"] = "1";
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            keyPhrasesResponse = await client.PostAsync(keyPhrasesUri, content);
            keyPhrasesTxt.Text = keyPhrasesResponse.Content.ReadAsStringAsync().Result.Substring(15).Split(',')[0];

         
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
            sentimentTxt.Text = sentimentUriResponse.Content.ReadAsStringAsync().Result.Substring(15).Split(',')[0];

            content.Dispose();
            //keyPhrasesTxt.Text = keyPhrasesResponse.Content.ReadAsStringAsync().Result;
            //sentimentTxt.Text = keyPhrasesResponse.Content.ReadAsStringAsync().Result;

            //MessageBox.Show(response.Content.ReadAsStringAsync().Result);
        }
        
        private void AzureAnalyzeStart_Click(object sender, RoutedEventArgs e)
        {
            keyPhrasesRequest();
            sentimentRequest();
            string text = Responsetxt.Text;
            var client = LanguageServiceClient.Create();
            var response = client.AnalyzeSentiment(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            WriteSentiment(response.DocumentSentiment, response.Sentences);
            
            var response2 = client.AnalyzeEntities(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            WriteEntities(response2.Entities);

            var response3 = client.AnnotateText(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            new Features() { ExtractSyntax = true });
            WriteSentences(response3.Sentences, response3.Tokens);

            var response4 = client.AnalyzeEntitySentiment(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            WriteEntitySentiment(response4.Entities);
        }

        /*
        private async void GoogleNLPStart_Click(object sender, RoutedEventArgs e)
        {
            string text = "my favorite country is south korea because there are many beautiful citys.";
            var client = LanguageServiceClient.Create();
            var response = client.AnalyzeSentiment(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            WriteSentiment(response.DocumentSentiment, response.Sentences);
        }*/

        //sentiment
        private async void WriteSentiment(Sentiment sentiment, RepeatedField<Sentence> sentences)
        {
            googleAnalyzeSentiment.Text += "Overall document sentiment(주어진 텍스트를 검사하고 텍스트 내에서 우세한 감정적인 의견을 확인)";
            googleAnalyzeSentiment.Text += $"\nScore: {sentiment.Score}";
            googleAnalyzeSentiment.Text += $"\tMagnitude: {sentiment.Magnitude}\n";
            googleAnalyzeSentiment.Text += "Sentence level sentiment:";
            foreach (var sentence in sentences)
            {
                googleAnalyzeSentiment.Text += $" ({sentence.Sentiment.Score})";   //"\t{sentence.Text.Content}: "+ $
                
            }
        }
        //entities
        private async void WriteEntities(IEnumerable<Entity> entities)
        {
            googleAnalyzeEntities.Text = "Entities(텍스트를 검사하고 해당 엔티티에 대한 정보를 반환)";
            foreach (var entity in entities)
            {
                googleAnalyzeEntities.Text += $"\nName: {entity.Name}";
                googleAnalyzeEntities.Text += $"\tType: {entity.Type}";
                googleAnalyzeEntities.Text += $"\tSalience: {entity.Salience}";
                /*
                googleAnalyzeEntities.Text += "\tMentions:";
                foreach (var mention in entity.Mentions)
                { 
                    googleAnalyzeEntities.Text += $"\t\t{mention.Text.BeginOffset}: {mention.Text.Content}";
                }
                googleAnalyzeEntities.Text += "\tMetadata:";
                foreach (var keyval in entity.Metadata)
                {
                    googleAnalyzeEntities.Text += $"\t\t{keyval.Key}: {keyval.Value}";
                }
                */
            }
        }
        //syntax
        private async void WriteSentences(IEnumerable<Sentence> sentences, RepeatedField<Token> tokens)
        {
            /*
            googleAnnotateText.Text += "Sentences:";
            foreach (var sentence in sentences)
            {
                googleAnnotateText.Text += $"\t{sentence.Text.BeginOffset}: {sentence.Text.Content}"; 
            }
            */
            //googleAnnotateText.Text += "Tokens:";
            foreach (var token in tokens)
            {
                googleAnnotateText.Text += $"{token.PartOfSpeech.Tag} "+ $"{token.Text.Content}\n";
            }
        }
        //entity sentiment
        private async void WriteEntitySentiment(IEnumerable<Entity> entities)
        {
            googleAnalyzeEntitySentiment.Text += "Entity Sentiment(엔티티 분석 및 감정 분석을 결합하고 텍스트 내의 엔티티에 대해 표현 된 정서를 결정)\n";
            foreach (var entity in entities)
            {
                googleAnalyzeEntitySentiment.Text += $"{entity.Name} "+ $"({(int)(entity.Salience * 100)}%)";
                googleAnalyzeEntitySentiment.Text += $"\tScore: {entity.Sentiment.Score}";
                googleAnalyzeEntitySentiment.Text += $"\tMagnitude { entity.Sentiment.Magnitude}\n";

            }
        }
    }
}
