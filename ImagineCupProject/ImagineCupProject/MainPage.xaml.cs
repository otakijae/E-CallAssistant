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

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        AzureDatabase azureDatabase;
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        AutoResetEvent _FinalResponceEvent;
        MicrophoneRecognitionClient _microphoneRecognitionClient;
        String temp;
        ArrayList textArrayList = new ArrayList(); 
        ArrayList textShapeArrayList = new ArrayList();
        string time = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");

        SimpleManual simpleManual = new SimpleManual();
        StandardManual standardManual = new StandardManual();
        ClassifiedManual classifiedManual = new ClassifiedManual();
        MedicalManual medicalManual = new MedicalManual();

        string classifiedResult;

        public MainPage()
        {
            InitializeComponent();
            _FinalResponceEvent = new AutoResetEvent(false);
            timeText.Text = time;
            azureDatabase = new AzureDatabase();

            //Manual xaml 매뉴얼
            this.simpleManualGrid.Children.Add(simpleManual);
            this.standardManualGrid.Children.Add(standardManual);
            this.classifiedManualGrid.Children.Add(classifiedManual);
            this.medicalManualGrid.Children.Add(medicalManual);
        }

        //Azure SpeechToText
        private void ConvertSpeechToText()
        {
            var speechRecognitionMode = SpeechRecognitionMode.ShortPhrase;  //LongDictation 대신 ShortPhrase 선택
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
            _microphoneRecognitionClient.StartMicAndRecognition();
        }

        //Textbox에 text입력
        private void ResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            string result = e.PartialResult;
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
                responseText.Text = (e.PartialResult);
                responseText.Text += ("\n");
            });
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

        //START버튼 누른후 음성인식 시작
        private void speakBtn_Click(object sender, RoutedEventArgs e)
        {
            speakBtn.IsEnabled = false;
            ConvertSpeechToText();
        }

        //STOP버튼과 함께 음성인식 종료
        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                _FinalResponceEvent.Set();
                _microphoneRecognitionClient.EndMicAndRecognition();
                _microphoneRecognitionClient.Dispose();
                _microphoneRecognitionClient = null;
                speakBtn.Content = "Start Recording";
                speakBtn.IsEnabled = true;
            }));

            string text = responseText.Text;
            var client = LanguageServiceClient.Create();
            var response4 = client.AnnotateText(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            new Features() { ExtractSyntax = true });
            CorrectSentences(response4.Sentences, response4.Tokens);
        }

        //텍스트 분석 클릭버튼
        private void analyzeBtn_Click(object sender, RoutedEventArgs e)
        {

            if (entityRecognition.Text != "")
            {
                entityRecognition.Text = null;
                sentimentRecognition.Text = null;
                syntaxRecognition.Text = null;
                problemText.Text = null;
                locationText.Text = null;
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
            if (summary.Text.Contains("kill"))
            {
                entityRecognition.Text += $"Name: kill";
                entityRecognition.Text += $" /Event\n";
                codeText.Text += "kill";
            }
            if (summary.Text.Contains("shot"))
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

        // Google Cloud Storage에 저장된 (1분 이상의)오디오를 인식
        /*
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
        }*/

        /*
        private void Correct_Click(object sender, RoutedEventArgs e)
        {
            //AsyncRecognizeGcs("gs://emergencycall/test2.flac");
            string text = responseText.Text;
            var client = LanguageServiceClient.Create();
            var response4 = client.AnnotateText(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            new Features() { ExtractSyntax = true });
            CorrectSentences(response4.Sentences, response4.Tokens);
        }*/

        //음성 stop버튼 누르면 문장을 . 표시로 구별해주기
        private async void CorrectSentences(IEnumerable<Sentence> sentences, RepeatedField<Token> tokens)
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
            responseText.Text = null;
            for (int i = 0; i < textArrayList.Count; i++)
            {
                responseText.Text += textArrayList[i];
            }
        }
        
        private void TextClassify_Click(object sender, RoutedEventArgs e)
        {
            Run(problemText.Text);
            this.loadingProcess.Visibility = Visibility.Visible;
        }

        private async void Run(string keyWords)
        {
            this.codeText.Text = await TextClassificationAsync(keyWords);
            this.textClassify.IsEnabled = true;
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
