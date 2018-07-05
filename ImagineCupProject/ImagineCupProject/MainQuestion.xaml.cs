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
using System.Threading;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http;
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
        List<string> addressList = new List<string>();

        SimpleManual simpleManual = new SimpleManual();
        StandardManual standardManual = new StandardManual();
        AdditionalQuestion additionalQuestion;

        private readonly ToastViewModel toastViewModel;
        LoadingAnimation loadingAnimation;
        EventVO currentEvent;
        string keyWords;

        SolidColorBrush mainColorSolidColorBrush = new SolidColorBrush();
        SolidColorBrush pointColorSolidColorBrush = new SolidColorBrush();

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

            mainColorSolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3580BF"));
            pointColorSolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF5E00"));
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
            string location = "";
            foreach (var entity in entities)
            {
                if (entity.Type.ToString().Equals("Location") | entity.Type.ToString().Equals("Organization"))
                {
                    location += entity.Name;
                    location += " ";

                }
            }
            try
            {
                locationText.Text = location;
                GooglePlacesAPI();
            }
            catch(Exception e)
            {
                
            }
                
        }

        //장소 검색
        public void GooglePlacesAPI()
        {
            addressList.Clear();
            string url = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + locationText.Text + "&language=en&key=AIzaSyB55GQJ3tv_L2aALoWxIa4vkfJRdtunMtU";

            string json = "";
            using (WebClient webClient = new WebClient())
            {
                json = webClient.DownloadString(url);

                RootObject test = JsonConvert.DeserializeObject<RootObject>(json);
                foreach (var singleResult in test.predictions)
                {
                    var location = singleResult.description;
                    addressList.Add(location);
                }
            }
            locationText.Text += "\n" + addressList[0];

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
            //azureDatabase.SendDataTo112(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
        }

        public void SendTo110()
        {
            //azureDatabase.SendDataTo110(operatorText.Text, timeText.Text, locationText.Text, phoneNumberText.Text, callerNameText.Text, problemText.Text, codeText.Text);
        }

        private void TextClassify_Click(object sender, RoutedEventArgs e)
        {
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
            toastViewModel.ShowWarning("Event Classification : " + classifiedResult);

            //코드 분석 완료 텍스트 박스 배경색 변경
            codeText.Background = pointColorSolidColorBrush;

            currentEvent.EventCODE = classifiedResult;
        }

        private async Task<string> TextClassificationAsync(string keyWords)
        {
            try
            {
                //string python = @"C:\Python36\python.exe";
                //string myPythonApp = "predict.py";

                //ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                //myProcessStartInfo.CreateNoWindow = true;
                //myProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //myProcessStartInfo.UseShellExecute = false;
                //myProcessStartInfo.RedirectStandardOutput = true;
                //myProcessStartInfo.Arguments = myPythonApp + " " + "./trained_model_1516629873/" + " \"" + keyWords + "\"";

                //Process myProcess = new Process();
                //myProcess.StartInfo = myProcessStartInfo;
                //myProcess.Start();
                //StreamReader myStreamReader = myProcess.StandardOutput;
                //classifiedResult = await myStreamReader.ReadToEndAsync();
                //myProcess.WaitForExit();
                //myProcess.Close();

                //### URL request ###
                string url = "http://localhost:4000/predict?query=" + keyWords;
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    string result = await content.ReadAsStringAsync();
                    JObject jo = JObject.Parse(result);
                    classifiedResult = (string)jo["result"];
                }

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
