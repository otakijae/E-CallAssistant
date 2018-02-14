using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using Google.Cloud.Language.V1;
using Google.Protobuf.Collections;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using System.IO;
using System.Net;
using Newtonsoft.Json;
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
        ArrayList textArrayList = new ArrayList();
        ArrayList textShapeArrayList = new ArrayList();
        public string classifiedResult;

        SimpleManual simpleManual = new SimpleManual();
        StandardManual standardManual = new StandardManual();
        AdditionalQuestion additionalQuestion;

        private readonly ToastViewModel toastViewModel;
        LoadingAnimation loadingAnimation;
        EventVO currentEvent;
        string keyWords;
        string changeSentence;

        public MainQuestion(AdditionalQuestion additionalQuestion, ToastViewModel toastViewModel, LoadingAnimation loadingAnimation, EventVO currentEvent)
        {
            InitializeComponent();
            //azureDatabase = new AzureDatabase();

            //Manual xaml 매뉴얼 
            this.simpleManualGrid.Children.Add(simpleManual);
            this.standardManualGrid.Children.Add(standardManual);
            this.additionalQuestion = additionalQuestion;
            this.toastViewModel = toastViewModel;
            this.loadingAnimation = loadingAnimation;
            this.currentEvent = currentEvent;
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
        
        //entity분석 google api
        private async void WriteEntities(IEnumerable<Entity> entities)
        {
            string location="";
            foreach (var entity in entities)
            {
                if (entity.Type.ToString().Equals("Location") | entity.Type.ToString().Equals("Organization"))
                {
                    location += entity.Name;
                    location += " ";
                }
            }
            locationText.Text = location;
        }
        
        //장소 검색
        public void GooglePlacesAPI()
        {
            //string url = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + locationText.Text + "&language=en&key=AIzaSyB55GQJ3tv_L2aALoWxIa4vkfJRdtunMtU";

            string json = "";
            using (WebClient webClient = new WebClient())
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
        //구글 places api
        public class MatchedSubstring
        {
            public int length { get; set; }
            public int offset { get; set; }
        }
        //구글 places api
        public class Term
        {
            public int offset { get; set; }
            public string value { get; set; }
        }
        //구글 places api
        public class Prediction
        {
            public string description { get; set; }
            public string id { get; set; }
            public List<MatchedSubstring> matched_substrings { get; set; }
            public string reference { get; set; }
            public List<Term> terms { get; set; }
            public List<string> types { get; set; }
        }
        //구글 places api
        public class RootObject
        {
            public List<Prediction> predictions { get; set; }
            public string status { get; set; }
        }
        
        public void SendTo112()
        {
            azureDatabase.SendDataTo112(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
        }

        public void SendTo110()
        {
            azureDatabase.SendDataTo110(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
        }

        //문장 분석
        public void AnalyzeText()
        {
            changeSentence = problemText.Text;

            var client = LanguageServiceClient.Create();
            //형태소 분석 ( 명사, 형용사, 동사 추출)
            var response = client.AnnotateText(new Document()
            {
                Content = problemText.Text,
                Type = Document.Types.Type.PlainText
            },
            new Features() { ExtractSyntax = true });
            foreach (var token in response.Tokens)
            {
                if (token.PartOfSpeech.Tag.ToString().Equals("Noun") || token.PartOfSpeech.Tag.ToString().Equals("Verb") || token.PartOfSpeech.Tag.ToString().Equals("Adj"))
                {
                    keyWords += token.Text.Content.ToString() + " ";
                }
            }
            RunSentenceModify(keyWords);
            loadingAnimation.Visibility = Visibility.Visible;
        }

        //문장 수정
        public async void RunSentenceModify(string keyWords)
        {
            string changeWords = await SentenceModify(keyWords);
            MessageBox.Show(changeWords);
            changeSentence = changeSentence.Replace(changeWords.ToString().Split(' ')[0],changeWords.ToString().Split(' ')[1].Replace("\r\n","")); //.Substring(0, changeWords.ToString().Split(' ')[1].Length - 1)
            codeText.Text = changeSentence;
            MessageBox.Show(changeSentence);
            //this.textClassify.IsEnabled = true;
            loadingAnimation.Visibility = Visibility.Hidden;

            //분류된 카테고리에 대한 매뉴얼 출력후 Toast알림 띄우기, 현재 EventVO에 분류 결과 저장
            additionalQuestion.ShowClassifiedManuals(classifiedResult);
            toastViewModel.ShowWarning("Text Classification : " + classifiedResult);
            currentEvent.EventCODE = classifiedResult;
        }

        //파이썬 연동
        public async Task<string> SentenceModify(string keyWords)
        {
            try
            {
                string python = @"C:\Python36\python.exe";
                string myPythonApp = "sentenceModify.py";

                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcessStartInfo.Arguments = myPythonApp + " " + keyWords;
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

        private void TextClassify_Click(object sender, RoutedEventArgs e)
        {
            //AnalyzeText();
            Run(problemText.Text);
            loadingAnimation.Visibility = Visibility.Visible;

            var client = LanguageServiceClient.Create();
            //장소 추출
            var responseEntites = client.AnalyzeEntities(new Document()
            {
                Content = problemText.Text,
                Type = Document.Types.Type.PlainText
            });
            WriteEntities(responseEntites.Entities);
        }

        private async void Run(string keyWords)
        {
            this.codeText.Text = await TextClassificationAsync(keyWords);
            //this.textClassify.IsEnabled = true;
            loadingAnimation.Visibility = Visibility.Hidden;

            //분류된 카테고리에 대한 매뉴얼 출력후 Toast알림 띄우기, 현재 EventVO에 분류 결과 저장
            additionalQuestion.ShowClassifiedManuals(classifiedResult);
            toastViewModel.ShowWarning("Text Classification : " + classifiedResult);
            currentEvent.EventCODE = classifiedResult;
        }
        
        private async Task<string> TextClassificationAsync(string keyWords)
        {
            try
            {
                string python = @"C:\Python36\python.exe";
                string myPythonApp = "predict.py";

                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                myProcessStartInfo.CreateNoWindow = true;
                myProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcessStartInfo.Arguments = myPythonApp + " " + "./trained_model_1516629873/" + " \"" + keyWords + "\"";

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

        public EventVO CurrentEventVO
        {
            get { return currentEvent; }
            set { currentEvent = value; }
        }
    }
}
