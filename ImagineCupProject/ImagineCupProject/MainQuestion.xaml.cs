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
using System.Net;
using Newtonsoft.Json;
using ImagineCupProject.EmergencyResponseManuals;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
//using Aylien.TextApi;

namespace ImagineCupProject
{
    /// <summary>
    /// 전화가 시작된 후 우선적으로 장소, 사건 등 기본 정보를 듣는 화면
    /// </summary>
    public partial class MainQuestion : Page
    {
        
        AzureDatabase azureDatabase;
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        String temp;
        ArrayList textArrayList = new ArrayList();
        ArrayList textShapeArrayList = new ArrayList();
        string time = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
        string result;
        //MainPage mainPage;
        string classifiedResult;
         
        StandardManual standardManual = new StandardManual();
        ClassifiedManual classifiedManual = new ClassifiedManual();
        MedicalManual medicalManual = new MedicalManual();

        public MainQuestion()
        {
            //mainPage = new MainPage();
            InitializeComponent();
            timeText.Text = time;
            //azureDatabase = new AzureDatabase();

            //Manual xaml 매뉴얼 
            this.standardManualGrid.Children.Add(standardManual);
            this.classifiedManualGrid.Children.Add(classifiedManual);
            this.medicalManualGrid.Children.Add(medicalManual);
        }

        public string TextBoxText
        {
            get { return responseText.Text; }
            set
            { 
                responseText.Text += value;
                responseText.Text += "test";
            }
        }

        //텍스트 분석 클릭버튼            
        public void analyze()
        {
            
            if (entityRecognition.Text != "")
            {
                entityRecognition.Text = null;
                sentimentRecognition.Text = null;
                syntaxRecognition.Text = null;
                problemText.Text = null;
                locationText.Text = null;
                codeText.Text = null;
            }
            
            string text = responseText.Text;
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

            azureDatabase.insertData(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
            googlePlacesAPI();
        }



        //긍정 부정 분석 google api
        private async void WriteSentiment(Sentiment sentiment, RepeatedField<Sentence> sentences)
        {
            sentimentRecognition.Text += $"Score: {sentiment.Score}";
            sentimentRecognition.Text += $"\tMagnitude: {sentiment.Magnitude}\n";
            //stt.Text += "Sentence level sentiment:";
            foreach (var sentence in sentences)
            {
                sentimentRecognition.Text += $"{sentence.Text.Content}:" + $" ({sentence.Sentiment.Score * 100}%)\n";   //"\t{sentence.Text.Content}: "+ $

            }
        }

        //entity분석 google api
        private async void WriteEntities(IEnumerable<Entity> entities)
        {
            if (responseText.Text.Contains("kill"))
            {
                entityRecognition.Text += $"Name: kill";
                entityRecognition.Text += $" /Event\n";
                codeText.Text += "kill";
            }
            if (responseText.Text.Contains("shot"))
            {
                entityRecognition.Text += $"Name: shot";
                entityRecognition.Text += $" /Event\n";
                codeText.Text += "shot";
            }
            foreach (var entity in entities)
            {
                if (entity.Type.ToString().Equals("Location") | entity.Type.ToString().Equals("Organization"))
                {
                    locationText.Text += entity.Name;
                    locationText.Text += " ";
                }
                if (entity.Type.ToString().Equals("Event"))
                {
                    codeText.Text += entity.Name;
                }
                entityRecognition.Text += $"Name: {entity.Name}";
                entityRecognition.Text += $" /{entity.Type}\n";
            }
        }

        //형태소 분석 google api
        private async void WriteSentences(IEnumerable<Sentence> sentences, RepeatedField<Token> tokens)
        {
            syntaxRecognition.Text += "\n";
            foreach (var token in tokens)
            {
                syntaxRecognition.Text += $"{token.PartOfSpeech.Tag} " + $"{token.Text.Content}\n";
            }
        }


    /*
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
                responseText.Text = (alternative.Transcript);
            }
        }
        return 0;
    }
    */
    
    public void googlePlacesAPI()
        {
            //string url = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + locationText.Text + "&language=en&key=AIzaSyB55GQJ3tv_L2aALoWxIa4vkfJRdtunMtU";
            string json = "";
            using (WebClient wc = new WebClient())
            {
                //json = wc.DownloadString(url);

                RootObject test = JsonConvert.DeserializeObject<RootObject>(json);
                foreach (var singleResult in test.predictions)
                {
                    var location = singleResult.description;
                    MessageBox.Show(location);

                }

            }
        }

        public class MatchedSubstring
        {
            public int length { get; set; }
            public int offset { get; set; }
        }

        public class Term
        {
            public int offset { get; set; }
            public string value { get; set; }
        }

        public class Prediction
        {
            public string description { get; set; }
            public string id { get; set; }
            public List<MatchedSubstring> matched_substrings { get; set; }
            public string reference { get; set; }
            public List<Term> terms { get; set; }
            public List<string> types { get; set; }
        }

        public class RootObject
        {
            public List<Prediction> predictions { get; set; }
            public string status { get; set; }
        }
        
        public void sendTo112()
        {
            azureDatabase.sendDataTo112(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
        }

        public void sendTo110()
        {
            azureDatabase.sendDataTo110(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
        }

        private void TextClassify_Click(object sender, RoutedEventArgs e)
        {
            Run(problemText.Text);
            this.loadingProcess.Visibility = Visibility.Visible;
        }

        private async void Run(string keyWords)
        {
            this.codeText.Text = await TextClassificationAsync(keyWords);
            //this.textClassify.IsEnabled = true;
            this.loadingProcess.Visibility = Visibility.Hidden;

            //분류된 카테고리에 대한 매뉴얼 출력, 나중에 완료하면 Toast알림 띄우기
            ShowClassifiedManuals(classifiedResult);
        }

        private void ShowClassifiedManuals(string category)
        {
            switch (category)
            {
                case "Disaster\r\n":
                    classifiedManual.earthquake.Visibility = Visibility.Visible;
                    classifiedManual.flood.Visibility = Visibility.Visible;
                    classifiedManual.severeWeather.Visibility = Visibility.Visible;
                    classifiedManual.terrorAndGunshot.Visibility = Visibility.Collapsed;
                    classifiedManual.fire.Visibility = Visibility.Collapsed;
                    classifiedManual.womenViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.teenageViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.elderlyCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.childCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.suicide.Visibility = Visibility.Collapsed;
                    break;
                case "Terror\r\n":
                    classifiedManual.earthquake.Visibility = Visibility.Collapsed;
                    classifiedManual.flood.Visibility = Visibility.Collapsed;
                    classifiedManual.severeWeather.Visibility = Visibility.Collapsed;
                    classifiedManual.terrorAndGunshot.Visibility = Visibility.Visible;
                    classifiedManual.fire.Visibility = Visibility.Collapsed;
                    classifiedManual.womenViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.teenageViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.elderlyCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.childCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.suicide.Visibility = Visibility.Collapsed;
                    break;
                case "Fire\r\n":
                    classifiedManual.earthquake.Visibility = Visibility.Collapsed;
                    classifiedManual.flood.Visibility = Visibility.Collapsed;
                    classifiedManual.severeWeather.Visibility = Visibility.Collapsed;
                    classifiedManual.terrorAndGunshot.Visibility = Visibility.Collapsed;
                    classifiedManual.fire.Visibility = Visibility.Visible;
                    classifiedManual.womenViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.teenageViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.elderlyCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.childCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.suicide.Visibility = Visibility.Collapsed;
                    break;
                case "Violence\r\n":
                    classifiedManual.earthquake.Visibility = Visibility.Collapsed;
                    classifiedManual.flood.Visibility = Visibility.Collapsed;
                    classifiedManual.severeWeather.Visibility = Visibility.Collapsed;
                    classifiedManual.terrorAndGunshot.Visibility = Visibility.Collapsed;
                    classifiedManual.fire.Visibility = Visibility.Collapsed;
                    classifiedManual.womenViolence.Visibility = Visibility.Visible;
                    classifiedManual.teenageViolence.Visibility = Visibility.Visible;
                    classifiedManual.elderlyCruelTreatment.Visibility = Visibility.Visible;
                    classifiedManual.childCruelTreatment.Visibility = Visibility.Visible;
                    classifiedManual.suicide.Visibility = Visibility.Visible;
                    break;
                case "Motor vehicle accidents\r\n":
                    break;
                default:
                    break;
            }
        }

        private async Task<string> TextClassificationAsync(string keyWords)
        {
            try
            {
                string python = @"C:\Python36\python.exe";
                string myPythonApp = "predict.py";

                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcessStartInfo.Arguments = myPythonApp + " " + "./trained_model_1516629873/" + " " + keyWords;

                Process myProcess = new Process();
                myProcess.StartInfo = myProcessStartInfo;
                myProcess.Start();
                StreamReader myStreamReader = myProcess.StandardOutput;
                classifiedResult = await myStreamReader.ReadToEndAsync();
                myProcess.WaitForExit();
                myProcess.Close();

                return classifiedResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void StandardResponse_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
                standardManual.standardManualGrid.Visibility = Visibility.Visible;
            else
                standardManual.standardManualGrid.Visibility = Visibility.Collapsed;
        }

        private void MedicalResponse_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
                medicalManual.medicalManualGrid.Visibility = Visibility.Visible;
            else
                medicalManual.medicalManualGrid.Visibility = Visibility.Collapsed;
        }
    }
 
}
