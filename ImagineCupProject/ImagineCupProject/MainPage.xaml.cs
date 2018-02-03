using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Microsoft.CognitiveServices.SpeechRecognition;
using System.Collections;
using Google.Cloud.Language.V1;
using Google.Protobuf.Collections;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using System.Threading.Tasks;
using ImagineCupProject.EmergencyResponseManuals;
using System.Windows.Controls.Primitives;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Speech.V1;
using System.Data.SqlClient;
using Aylien.TextApi;
using System.Windows.Input;

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

        AutoResetEvent _FinalResponceEvent;
        String temp;
        string time = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");

        SimpleManual simpleManual = new SimpleManual();
        StandardManual standardManual = new StandardManual();
        ClassifiedManual classifiedManual = new ClassifiedManual();
        MedicalManual medicalManual = new MedicalManual();

        Client client = new Client("3b49bfce", "d5788d26c944e091562527416046febb");
        string text = "I am the passenger and I see the Starbucks building at New York subway station is on fire. I think 911 need to check this out quickly" +
                "At least 37 people have been killed and dozens injured in a fire at a hospital and nursing home in New York, in the country's deadliest blaze for a decade";
        string speechRecognitionResult;
        ArrayList textArrayList;
        ArrayList textShapeArrayList;
        MainQuestion mainQuestion;
        TotalPage totalPage;
        AdditionalQuestion additionalQuestion;
        private readonly ToastViewModel _vm;

        EventVO CurrentEvent = new EventVO();

        public MainPage()
        {
            InitializeComponent();
            textArrayList = new ArrayList();
            textShapeArrayList = new ArrayList();
            DataContext = _vm = new ToastViewModel();
            additionalQuestion = new AdditionalQuestion();
            totalPage = new TotalPage();
            mainQuestion = new MainQuestion(additionalQuestion);
            mainFrame.Content = mainQuestion;
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
            if (nextButton.Content.Equals("Next"))
            {
                CurrentEvent.EventNUMBER = null;
                CurrentEvent.EventOPERATOR = mainQuestion.operatorText.Text;
                CurrentEvent.EventSTARTTIME = mainQuestion.timeText.Text;
                CurrentEvent.EventENDTIME = null;
                CurrentEvent.EventLOCATION = mainQuestion.locationText.Text;
                CurrentEvent.EventPHONENUMBER = mainQuestion.phoneNumberText.Text;
                CurrentEvent.EventCALLERNAME = mainQuestion.callerNameText.Text;
                CurrentEvent.EventPROBLEM = mainQuestion.problemText.Text;
                CurrentEvent.EventCODE = mainQuestion.codeText.Text;
                
                mainFrame.Content = additionalQuestion;
                nextButton.Content = "Previous";
            }
            else
            {
                mainQuestion.operatorText.Text = CurrentEvent.EventOPERATOR;
                mainQuestion.timeText.Text = CurrentEvent.EventSTARTTIME;
                mainQuestion.locationText.Text = CurrentEvent.EventLOCATION;
                mainQuestion.phoneNumberText.Text = CurrentEvent.EventPHONENUMBER;
                mainQuestion.callerNameText.Text = CurrentEvent.EventCALLERNAME;
                mainQuestion.problemText.Text = CurrentEvent.EventPROBLEM;

                //카테고리가 나오기 전에 다음 화면으로 넘어갔을 경우 현재사건VO 객체에 코드 정보가 저장이 안 되어있기 때문에 다음 화면에서 카테고리 결과가 출력되면 VO 객체에 값을 넣어줌
                CurrentEvent.EventCODE = mainQuestion.classifiedResult;
                mainQuestion.codeText.Text = CurrentEvent.EventCODE;

                mainFrame.Content = mainQuestion;
                nextButton.Content = "Next";
            }

        }

        private void listViewItem1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = totalPage;
        }

        private void listViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = mainQuestion;
        }
        /*
            //Manual xaml 매뉴얼
            this.simpleManualGrid.Children.Add(simpleManual);
            this.standardManualGrid.Children.Add(standardManual);
            this.classifiedManualGrid.Children.Add(classifiedManual);
            this.medicalManualGrid.Children.Add(medicalManual);
         */
        //음성인식버튼
        private void btnStartRecord_Click(object sender, RoutedEventArgs e)
        {
            _vm.ShowInformation("Ring the Call.");
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
            Aylien.TextApi.Sentiment sentiment2 = client.Sentiment(text: text);
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
                //mainQuestion.
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

                    //mainQuestion.locationText.Text += "HI";
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
                    //speechResult.Text += e.PhraseResponse.Results[i].DisplayText; // e.PhraseResponse.Results[i].Confidence +
                    string text = e.PhraseResponse.Results[i].DisplayText;
                    var client = LanguageServiceClient.Create();
                    var response = client.AnnotateText(new Document()
                    {
                        Content = text,
                        Type = Document.Types.Type.PlainText
                    },
                    new Features() { ExtractSyntax = true });
                    CorrectSentences(response.Sentences, response.Tokens);
                }
                //codeText.Text += "\n";
            }
        }

        //음성 끊길때마다 문장을 . 표시로 구별해주기
        private async void CorrectSentences(IEnumerable<Google.Cloud.Language.V1.Sentence> sentences, RepeatedField<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.PartOfSpeech.Tag.ToString().Equals("Verb"))
                {
                    if (textShapeArrayList[textShapeArrayList.Count - 1].ToString().Equals("Det") | textShapeArrayList[textShapeArrayList.Count - 1].ToString().Equals("Noun") | textShapeArrayList[textShapeArrayList.Count - 1].ToString().Equals("Pron"))
                    {
                        if (!(textArrayList.Count.ToString().Equals("1") | textArrayList.Count.ToString().Equals("2")))
                        {
                            string temp = textArrayList[textArrayList.Count - 2].ToString().Remove(textArrayList[textArrayList.Count - 2].ToString().Length - 1) + ". ";
                            textArrayList.RemoveAt(textArrayList.Count - 2);
                            textArrayList.Insert(textArrayList.Count - 1, temp);
                        }
                    }
                }
                textArrayList.Add(token.Text.Content + " ");
                textShapeArrayList.Add(token.PartOfSpeech.Tag);
            }
            for (int i = 0; i < textArrayList.Count; i++)
            {
                speechResult.Text += textArrayList[i];
                //mainQuestion.responseText.Text += textArrayList[i];

            }
            //mainQuestion.analyze();
            //MessageBox.Show(mainQuestion.responseText.Text); 
            textArrayList.Clear();

        }
        

        private void btnSendTo112_Click(object sender, RoutedEventArgs e)
        {
            //mainQuestion.sendTo112();
            _vm.ShowSuccess("SendTo112 Success");
            //_vm.ShowWarning(String.Format("{0} Warning", _count++));
            //_vm.ShowError(String.Format("{0} Error", _count++));
        }

        private void btnSendTo110_Click(object sender, RoutedEventArgs e)
        {
            //mainQuestion.sendTo110();
            _vm.ShowSuccess("SendTo110 Success");
            //_vm.ShowInformation(String.Format("{0} Information", _count++));
            //_vm.ShowSuccess(String.Format("{0} Success", _count++));
        }

        private void listViewItem2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = new _112DataPage();
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            //mainQuestion.analyze();
            _vm.ShowSuccess("Transfer complete");
        }
     
    }
}
