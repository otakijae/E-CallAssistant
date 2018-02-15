using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Microsoft.CognitiveServices.SpeechRecognition;
using System.Collections;
using Google.Cloud.Language.V1;
using Google.Protobuf.Collections;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using ImagineCupProject.EmergencyResponseManuals;
using Aylien.TextApi;
using System.Windows.Input;

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        MicrophoneRecognitionClient microphoneRecognitionClient;
        AzureDatabase azureDatabase;
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        AutoResetEvent finalResponceEvent;

        SimpleManual simpleManual = new SimpleManual();
        StandardManual standardManual = new StandardManual();
        ClassifiedManual classifiedManual = new ClassifiedManual();
        MedicalManual medicalManual = new MedicalManual();

        Client client = new Client("3579c5de", "e6d591839df8a0788c75564f3ae0e6fd");
        string text = "I am the passenger and I see the Starbucks building at New York subway station is on fire. I think 911 need to check this out quickly" +
                "At least 37 people have been killed and dozens injured in a fire at a hospital and nursing home in New York, in the country's deadliest blaze for a decade";
        string speechRecognitionResult;
        ArrayList textArrayList = new ArrayList();
        ArrayList textShapeArrayList = new ArrayList();
        AdditionalQuestion additionalQuestion;
        MainQuestion mainQuestion;
        private readonly ToastViewModel toastViewModel;

        EventVO currentEvent = new EventVO();
        List<EventVO> savedEventList = new List<EventVO>();

        public MainPage()
        {
            InitializeComponent();
            azureDatabase = new AzureDatabase();
            DataContext = toastViewModel = new ToastViewModel();
            additionalQuestion = new AdditionalQuestion(toastViewModel, loadingProcess, currentEvent);
            mainQuestion = new MainQuestion(additionalQuestion, toastViewModel, loadingProcess, currentEvent);
            mainFrame.Content = mainQuestion;
            //AsyncRecognizeGcs("gs://emergencycall/911 pizza call - policer.wav");
            Summarize();
            SentimentAnalysis();
            phoneLoadingProcess.loadingText.Visibility = Visibility.Hidden;
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
                SaveCurrentEventVO();

                if (currentEvent.EventLOCATION == "")
                {
                    toastViewModel.ShowError("Location data is missing. Ask where is the accident scene.");
                    return;
                }
                else if (currentEvent.EventPROBLEM == "")
                {
                    toastViewModel.ShowError("Problem data is missing. Ask what is the problem.");
                    return;
                }

                additionalQuestion.location.Text = currentEvent.EventLOCATION;

                //MainPage, MainQuestion, AdditionalQuestion CurrentEvent VO 동기화 작업
                mainQuestion.CurrentEventVO = currentEvent;
                additionalQuestion.CurrentEventVO = currentEvent;

                PrintCurrentEvent(currentEvent);//VO 객체 값 할당된 거 확인하는 용도, 나중에 지울 것

                mainFrame.Content = additionalQuestion;
                nextButton.Content = "Previous";
            }
            else
            {
                SynchronizeEventData();

                //MainPage, MainQuestion, AdditionalQuestion CurrentEvent VO 동기화 작업
                currentEvent = additionalQuestion.CurrentEventVO;
                mainQuestion.CurrentEventVO = currentEvent;

                //카테고리가 나오기 전에 다음 화면으로 넘어갔을 경우 현재사건VO 객체에 코드 정보가 저장이 안 되어있기 때문에,
                //다음 화면(AdditionalQuestion 화면)에서 카테고리 결과가 출력되면 VO 객체에 값을 넣어줌
                currentEvent.EventCODE = mainQuestion.classifiedResult;
                mainQuestion.codeText.Text = currentEvent.EventCODE;

                PrintCurrentEvent(currentEvent);//VO 객체 값 할당된 거 확인하는 용도, 나중에 지울 것

                mainFrame.Content = mainQuestion;
                nextButton.Content = "Next";
            }
        }

        private void listViewItem1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = new TotalPage();
        }

        private void listViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Content = mainQuestion;
        }

        //  Summarize -  AYLIEN Text Analysis API 
        public void Summarize()
        {
            string title = "emergency";
            var summary2 = client.Summarize(text: text, title: title, sentencesNumber: 3).Sentences;

            foreach (var sentence in summary2)
            {
                summary.Text += sentence;
            }
        }

        //  SentimentAnalyze - AYLIEN Text Analysis API 
        public void SentimentAnalysis()
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
            string subscriptionKey = "39f4a264949c435fba61ff86acc47043";
            //string subscriptionKey = ConfigurationManager.AppSettings["5e3c0f17ea3f40b39cfb6ec28c77bf3e"];
            microphoneRecognitionClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                speechRecognitionMode,
                language,
                subscriptionKey
                );

            //_microphoneRecognitionClient.OnResponseReceived += ResponseReceived;
            microphoneRecognitionClient.OnPartialResponseReceived += ResponseReceived;
            //_microphoneRecognitionClient.OnResponseReceived += OnMicShortPhraseResponseReceivedHandler;
            microphoneRecognitionClient.OnResponseReceived += OnMicDictationResponseReceivedHandler;
            microphoneRecognitionClient.StartMicAndRecognition();
        }

        //Textbox에 text입력
        private void ResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            speechRecognitionResult = e.PartialResult;
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
                additionalQuestion.testBox.Text = speechRecognition.Text;/////////////////////////////////////////////////////////
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
                    //callerStatement.Text += e.PhraseResponse.Results[i].DisplayText; // e.PhraseResponse.Results[i].Confidence +
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
                callerStatement.Text += textArrayList[i];
                mainQuestion.problemText.Text += textArrayList[i];
            }

            //음성인식 기능은 전화받으면 계속 실행됨
            //mainFrame의 Content가 mainQuestion이면 problem 분석란에 음성인식 텍스트 추가
            //mainFrame의 Content가 additionalQuestion이면 답변 분석하기 위한 란에 음성인식 텍스트 추가
            //additionalQuestion.testBox.Text = speechRecognition.Text;//////////////////////////////////////////////////////////////////////

            //280, 200, 250자 정도에서 주기적으로 분석
            if (callerStatement.Text.Length > 200)
            {
                mainQuestion.textClassify.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                callerStatement.Text = "";
            }
            //mainQuestion.analyze();
            //MessageBox.Show(mainQuestion.responseText.Text); 
            textArrayList.Clear();
        }

        private void btnSendTo112_Click(object sender, RoutedEventArgs e)
        {
            //mainQuestion.sendTo112();
            toastViewModel.ShowSuccess("Send To 112 Success");
            //_vm.ShowWarning(String.Format("{0} Warning", _count++));
            //_vm.ShowError(String.Format("{0} Error", _count++));
        }

        private void btnSendTo110_Click(object sender, RoutedEventArgs e)
        {
            //mainQuestion.sendTo110();
            toastViewModel.ShowSuccess("Send To 110 Success");
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
            toastViewModel.ShowSuccess("Dispatch completed");
        }

        //통화 수신 및 시작 버튼
        private void PhoneReceiveButton_Click(object sender, RoutedEventArgs e)
        {
            toastViewModel.ShowInformation("Answering the call.");
            mainQuestion.timeText.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
            ConvertSpeechToText();
            this.phoneAnswerGrid.Visibility = Visibility.Collapsed;
        }

        //통화 왔을 때 알림
        private void PackIcon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (microphoneRecognitionClient == null)
            {
                ResetEvent();
                phoneAnswerGrid.Visibility = Visibility.Visible;
            }
        }

        //통화 종료 버튼
        private void btnQuitRecord_Click(object sender, RoutedEventArgs e)
        {
            if (microphoneRecognitionClient != null)
            {
                currentEvent.EventENDTIME = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
                savedEventList.Add(currentEvent);
                microphoneRecognitionClient.EndMicAndRecognition();
                microphoneRecognitionClient.Dispose();
                microphoneRecognitionClient = null;
                toastViewModel.ShowInformation("Hang up the call.");
                PrintCurrentEvent(currentEvent);
                InsertCurrentEvent(currentEvent);
                //현재 처리 중이었던 사건 저장 및 UI 초기화
                ResetEvent();
            }
        }

        //UI 초기화
        private void ResetEvent()
        {
            currentEvent = new EventVO();
            additionalQuestion = new AdditionalQuestion(toastViewModel, loadingProcess, currentEvent);
            mainQuestion = new MainQuestion(additionalQuestion, toastViewModel, loadingProcess, currentEvent);
            nextButton.Content = "Next";
            speechRecognition.Text = "";
            callerStatement.Text = "";
            mainFrame.Content = mainQuestion;
        }

        //MessageBox로 currentEvent 값 확인
        private void PrintCurrentEvent(EventVO currentEvent)
        {
            //VO 객체 값 할당된 거 확인하는 용도, 나중에 지울 것
            MessageBox.Show(currentEvent.EventCODE + "\n" + currentEvent.EventOPERATOR + "\n" + currentEvent.EventSTARTTIME + "\n" +
                currentEvent.EventENDTIME + "\n" + currentEvent.EventLOCATION + "\n" + currentEvent.EventPHONENUMBER + "\n" +
                currentEvent.EventCALLERNAME + "\n" + currentEvent.EventPROBLEM + "\n" + currentEvent.EventCODE + "\n" +
                currentEvent.EventFirstANSWER + "\n" + currentEvent.EventSecondANSWER + "\n" + currentEvent.EventThirdANSWER + "\n" +
                currentEvent.EventFourthANSWER + "\n" + currentEvent.EventFifthANSWER + "\n" + currentEvent.EventSixthANSWER + "\n" +
                currentEvent.EventSeventhANSWER + "\n" + currentEvent.EventEighthANSWER);
        }

        //MessageBox로 currentEvent 값 확인
        private void InsertCurrentEvent(EventVO currentEvent)
        {
            azureDatabase.InsertData(currentEvent);
        }
        private void SaveCurrentEventVO()
        {
            //EventNUMBER는 AUTO INCREMENT로 설정, PRIMARY KEY로 설정
            //currentEvent.EventNUMBER = null;
            currentEvent.EventOPERATOR = mainQuestion.operatorText.Text;
            currentEvent.EventSTARTTIME = mainQuestion.timeText.Text;
            //currentEvent.EventENDTIME = null;
            currentEvent.EventLOCATION = mainQuestion.locationText.Text;
            currentEvent.EventPHONENUMBER = mainQuestion.phoneNumberText.Text;
            currentEvent.EventCALLERNAME = mainQuestion.callerNameText.Text;
            currentEvent.EventPROBLEM = mainQuestion.problemText.Text;
            currentEvent.EventCODE = mainQuestion.codeText.Text;
        }

        private void SynchronizeEventData()
        {
            mainQuestion.operatorText.Text = currentEvent.EventOPERATOR;
            mainQuestion.timeText.Text = currentEvent.EventSTARTTIME;
            mainQuestion.locationText.Text = currentEvent.EventLOCATION;
            mainQuestion.phoneNumberText.Text = currentEvent.EventPHONENUMBER;
            mainQuestion.callerNameText.Text = currentEvent.EventCALLERNAME;
            mainQuestion.problemText.Text = currentEvent.EventPROBLEM;
            mainQuestion.codeText.Text = currentEvent.EventCODE;
        }
    }
}