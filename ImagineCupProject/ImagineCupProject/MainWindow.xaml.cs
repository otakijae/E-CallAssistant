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
using Google.Cloud.Speech.V1;

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AutoResetEvent _FinalResponceEvent;
        MicrophoneRecognitionClient _microphoneRecognitionClient;
        String temp;
        public MainWindow()
        {
            InitializeComponent();
            speakBtn.Content = "Start Recording";
            _FinalResponceEvent = new AutoResetEvent(false);
            responsetxt.Background = Brushes.White;
            responsetxt.Foreground = Brushes.Black;
            
        }

        //Azure SpeechToText
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
                /*
                if(e.PartialResult.Contains("am"))
                {
                    temp = e.PartialResult;
                    Responsetxt.Text = temp.Replace("am", "is"); ;
                    Responsetxt.Text += ("\n");

                }
                */
                responsetxt.Text = (e.PartialResult);
                responsetxt.Text += ("\n");
            });
        }

        //START버튼 누른후 음성인식 시작
        private void SpeakBtn_Click(object sender, RoutedEventArgs e)   
        {
            speakBtn.Content = "Listening ...";
            speakBtn.IsEnabled = false;
            responsetxt.Background = Brushes.Green;
            responsetxt.Foreground = Brushes.White;
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
                speakBtn.Content = "Start Recording";
                speakBtn.IsEnabled = true;
                responsetxt.Background = Brushes.White;
                responsetxt.Foreground = Brushes.Black;
            }));
        }

        //Text Analytics API 사용 핵심어구 분석
        private async void keyPhrasesRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var keyPhrasesUri = "https://eastasia.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases?" + queryString;   //핵심어구
            byte[] byteData = Encoding.UTF8.GetBytes("{'documents': [{ 'id': 'inputText', 'text': '" + responsetxt.Text + "'}]}");
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

        //Text Analytics API 사용 감정상태 분석
        private async void sentimentRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var sentimentUri = "https://eastasia.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + queryString;      //감정상태
             byte[] byteData = Encoding.UTF8.GetBytes("{'documents': [{ 'id': 'inputText', 'text': '" + responsetxt.Text + "'}]}");
             var content = new ByteArrayContent(byteData);
            HttpResponseMessage sentimentUriResponse = null;

            
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2ee0076fd06543029e0668de66971b68");
            queryString["numberOfLanguagesToDetect"] = "1";
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            sentimentUriResponse = await client.PostAsync(sentimentUri, content);
            sentimentTxt.Text = sentimentUriResponse.Content.ReadAsStringAsync().Result.Substring(15).Split(',')[0];

            content.Dispose();
        }
        
        //텍스트 분석 클릭버튼
        private void AzureAnalyzeStart_Click(object sender, RoutedEventArgs e)
        {
            if(keyPhrasesTxt.Text!="")
            { 
                keyPhrasesTxt.Text = null;
                sentimentTxt.Text = null;
                googleAnalyzeSentiment.Text = null;
                googleAnalyzeEntities.Text = null;
                googleAnnotateText.Text = null;
                googleAnalyzeEntitySentiment.Text = null;
            }
            keyPhrasesRequest();
            sentimentRequest();
            string text = responsetxt.Text;
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

        //sentiment
        private async void WriteSentiment(Sentiment sentiment, RepeatedField<Sentence> sentences)
        {
            googleAnalyzeSentiment.Text += "Overall document sentiment(핵심 단어에 대한 종류를 파악하고 중요도 추출)";
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
            }
        }
        //syntax
        private async void WriteSentences(IEnumerable<Sentence> sentences, RepeatedField<Token> tokens)
        {
            //googleAnnotateText.Text += "Tokens:";
            googleAnnotateText.Text += "형태소분석\n";
            foreach (var token in tokens)
            {
                googleAnnotateText.Text += $"{token.PartOfSpeech.Tag} "+ $"{token.Text.Content}\n";
            }
        }
        //entity sentiment
        private async void WriteEntitySentiment(IEnumerable<Entity> entities)
        {
            googleAnalyzeEntitySentiment.Text += "Entity Sentiment(핵심 단어 추출 및 감정 분석)\n";
            foreach (var entity in entities)
            {
                googleAnalyzeEntitySentiment.Text += $"{entity.Name} "+ $"({(int)(entity.Salience * 100)}%)";
                googleAnalyzeEntitySentiment.Text += $"\tScore: {entity.Sentiment.Score}";
                googleAnalyzeEntitySentiment.Text += $"\tMagnitude { entity.Sentiment.Magnitude}\n";

            }
        }

        //Google Storage에서 FlacFile Load
        private void loadFlacFile_Click(object sender, RoutedEventArgs e)
        {
            AsyncRecognizeGcs("gs://emergencycall/55.flac");
        }

        //Google Cloud Storage에있는 (1분이내) 오디오 파일에서 동기식 음성 인식을 직접 수행
        static object SyncRecognizeGcs(string storageUri)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                SampleRateHertz = 16000,
                LanguageCode = "en",
            }, RecognitionAudio.FromStorageUri(storageUri));
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
            return 0;
        }
        // Google Cloud Storage에 저장된 (1분 이상의)오디오를 인식
        public object AsyncRecognizeGcs(string storageUri)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                SampleRateHertz = 44100,
                LanguageCode = "en",
            }, RecognitionAudio.FromStorageUri(storageUri));
            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    responsetxt.Text = (alternative.Transcript);
                }
            }
            return 0;
        }
    }
}
