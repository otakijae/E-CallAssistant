using Google.Cloud.Language.V1;
using ImagineCupProject.EmergencyResponseManuals;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static Google.Cloud.Language.V1.AnnotateTextRequest.Types;
using System.Windows.Media;

namespace ImagineCupProject
{
    /// <summary>
    /// AdditionalQuestion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AdditionalQuestion : Page
    {
        ClassifiedManual classifiedManual = new ClassifiedManual();
        MedicalManual medicalManual = new MedicalManual();

        private readonly ToastViewModel toastViewModel;
        LoadingAnimation loadingAnimation;
        EventVO currentEvent;
        string classifiedResult;
        string changeSentence;
        string keyWords;

        SolidColorBrush mainColorSolidColorBrush = new SolidColorBrush();
        SolidColorBrush pointColorSolidColorBrush = new SolidColorBrush();

        public AdditionalQuestion(ToastViewModel toastViewModel, LoadingAnimation loadingAnimation, EventVO currentEvent)
        {
            InitializeComponent();

            this.classifiedManualGrid.Children.Add(classifiedManual);
            this.medicalManualGrid.Children.Add(medicalManual);

            this.toastViewModel = toastViewModel;
            this.loadingAnimation = loadingAnimation;
            this.currentEvent = currentEvent;

            firstQuestion.Content = currentEvent.EventFirstQUESTION;
            secondQuestion.Content = currentEvent.EventSecondQUESTION;
            thirdQuestion.Content = currentEvent.EventThirdQUESTION;
            fourthQuestion.Content = currentEvent.EventFourthUESTION;
            fifthQuestion.Content = currentEvent.EventFifthQUESTION;
            sixthQuestion.Content = currentEvent.EventSixthQUESTION;
            seventhQuestion.Content = currentEvent.EventSeventhQUESTION;
            eighthQuestion.Content = currentEvent.EventEighthQUESTION;

            mainColorSolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3580BF"));
            pointColorSolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF5E00"));
        }

        public void ShowClassifiedManuals(string category)
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
                    classifiedManual.motorVehicleAccidents.Visibility = Visibility.Collapsed;
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
                    classifiedManual.motorVehicleAccidents.Visibility = Visibility.Collapsed;
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
                    classifiedManual.motorVehicleAccidents.Visibility = Visibility.Collapsed;
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
                    classifiedManual.motorVehicleAccidents.Visibility = Visibility.Collapsed;
                    break;
                case "Motor vehicle accidents\r\n":
                    classifiedManual.earthquake.Visibility = Visibility.Collapsed;
                    classifiedManual.flood.Visibility = Visibility.Collapsed;
                    classifiedManual.severeWeather.Visibility = Visibility.Collapsed;
                    classifiedManual.terrorAndGunshot.Visibility = Visibility.Collapsed;
                    classifiedManual.fire.Visibility = Visibility.Collapsed;
                    classifiedManual.womenViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.teenageViolence.Visibility = Visibility.Collapsed;
                    classifiedManual.elderlyCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.childCruelTreatment.Visibility = Visibility.Collapsed;
                    classifiedManual.suicide.Visibility = Visibility.Collapsed;
                    classifiedManual.motorVehicleAccidents.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void MedicalResponse_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
                medicalManual.medicalManualGrid.Visibility = Visibility.Visible;
            else
                medicalManual.medicalManualGrid.Visibility = Visibility.Collapsed;
        }

        private async Task<string> TextIsPositiveClassificationAsync(string keyWords)
        {
            try
            {
                string python = @"C:\Python36\python.exe";
                string myPythonApp = "eval_posneg.py";

                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                myProcessStartInfo.CreateNoWindow = true;
                myProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                //신재혁 컴퓨터에서 실행 시
                //myProcessStartInfo.Arguments = myPythonApp + " " + "--eval_train" + " " + "--checkpoint_dir=\"./runs/1518189792/checkpoints/\"" + " \"" + keyWords + "\"";
                //손장원 컴퓨터에서 실행 시
                myProcessStartInfo.Arguments = myPythonApp + " " + "--eval_train" + " " + "--checkpoint_dir=\"./runs/1518617407/checkpoints/\"" + " \"" + keyWords + "\"";


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

        //문장 분석
        public void AnalyzeText()
        {
            changeSentence = testBox.Text;

            var client = LanguageServiceClient.Create();

            //형태소 분석 ( 명사, 형용사, 동사 추출)
            var response = client.AnnotateText(new Document()
            {
                Content = testBox.Text,
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

            //파이썬 연동
            RunSentenceModify(keyWords);
            loadingAnimation.Visibility = Visibility.Visible;
        }

        //문장 수정
        public async void RunSentenceModify(string keyWords)
        {
            string changeWords = await SentenceModify(keyWords);
            changeSentence = changeSentence.Replace(changeWords.ToString().Split(' ')[0], changeWords.ToString().Split(' ')[1].Replace("\r\n", ""));
            testBox.Text = changeSentence;
            //this.textClassify.IsEnabled = true;
            loadingAnimation.Visibility = Visibility.Hidden;

            //분류된 카테고리에 대한 매뉴얼 출력후 Toast알림 띄우기, 현재 EventVO에 분류 결과 저장
            ShowClassifiedManuals(classifiedResult);
            toastViewModel.ShowWarning("Text Classification : " + classifiedResult);
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

        private void FirstQuestion_Click(object sender, RoutedEventArgs e)
        {
            // 1. Speech 인지 시작
            // 2. 실시간 음성 인지 텍스트 나타내기
            // 3. 버튼 누르면 다른 버튼 disable
            // 4. 결과값 나오면 인지 종료
            AnalyzeText();

            if (this.testBox.Text.ToUpper().Trim() == "YES" || this.testBox.Text.ToUpper().Trim() == "YEAH")
            {
                classifiedResult = "1.0\r\n";
                ChangeAnswerButtonState(firstAnswer, firstToggle, true);
                currentEvent.EventFirstANSWER = 1;
            }
            else if (this.testBox.Text.ToUpper().Trim() == "NO")
            {
                classifiedResult = "0.0\r\n";
                ChangeAnswerButtonState(firstAnswer, firstToggle, false);
                currentEvent.EventFirstANSWER = 0;
            }
            else
            {
                FirstQuestionRun(testBox.Text);
                loadingAnimation.Visibility = Visibility.Visible;
            }
        }
        private async void FirstQuestionRun(string keyWords)
        {
            await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            currentEvent.EventFirstANSWER = CheckClassifiedResultTrueOrFalse(firstAnswer, firstToggle);
        }



        private void SecondQuestion_Click(object sender, RoutedEventArgs e)
        {
            //음성 인식한 것 그대로 항목에 채우기
            secondAnswer.Text += testBox.Text;
            currentEvent.EventSecondANSWER = secondAnswer.Text;
        }



        private void ThirdQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (this.testBox.Text.ToUpper().Trim() == "YES" || this.testBox.Text.ToUpper().Trim() == "YEAH")
            {
                classifiedResult = "1.0\r\n";
                ChangeAnswerButtonState(thirdAnswer, thirdToggle, true);
                currentEvent.EventThirdANSWER = 1;
            }
            else if (this.testBox.Text.ToUpper().Trim() == "NO")
            {
                classifiedResult = "0.0\r\n";
                ChangeAnswerButtonState(thirdAnswer, thirdToggle, false);
                currentEvent.EventThirdANSWER = 0;
            }
            else
            {
                ThirdQuestionRun(testBox.Text);
                loadingAnimation.Visibility = Visibility.Visible;
            }
        }
        private async void ThirdQuestionRun(string keyWords)
        {
            await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            currentEvent.EventThirdANSWER = CheckClassifiedResultTrueOrFalse(thirdAnswer, thirdToggle);
        }



        private void FourthQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (this.testBox.Text.ToUpper().Trim() == "YES" || this.testBox.Text.ToUpper().Trim() == "YEAH")
            {
                classifiedResult = "1.0\r\n";
                ChangeAnswerButtonState(fourthAnswer, fourthToggle, true);
                currentEvent.EventFourthANSWER = 1;
            }
            else if (this.testBox.Text.ToUpper().Trim() == "NO")
            {
                classifiedResult = "0.0\r\n";
                ChangeAnswerButtonState(fourthAnswer, fourthToggle, false);
                currentEvent.EventFourthANSWER = 0;
            }
            else
            {
                FourthQuestionRun(testBox.Text);
                loadingAnimation.Visibility = Visibility.Visible;
            }
        }
        private async void FourthQuestionRun(string keyWords)
        {
            await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            currentEvent.EventFourthANSWER = CheckClassifiedResultTrueOrFalse(fourthAnswer, fourthToggle);
        }



        private void FifthQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (this.testBox.Text.ToUpper().Trim() == "YES" || this.testBox.Text.ToUpper().Trim() == "YEAH")
            {
                classifiedResult = "1.0\r\n";
                ChangeAnswerButtonState(fifthAnswer, fifthToggle, true);
                currentEvent.EventFifthANSWER = 1;
            }
            else if (this.testBox.Text.ToUpper().Trim() == "NO")
            {
                classifiedResult = "0.0\r\n";
                ChangeAnswerButtonState(fifthAnswer, fifthToggle, false);
                currentEvent.EventFifthANSWER = 0;
            }
            else
            {
                FifthQuestionRun(testBox.Text);
                loadingAnimation.Visibility = Visibility.Visible;
            }
        }
        private async void FifthQuestionRun(string keyWords)
        {
            await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            currentEvent.EventFifthANSWER = CheckClassifiedResultTrueOrFalse(fifthAnswer, fifthToggle);
        }



        private void SixthQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (this.testBox.Text.ToUpper().Trim() == "YES" || this.testBox.Text.ToUpper().Trim() == "YEAH")
            {
                classifiedResult = "1.0\r\n";
                ChangeAnswerButtonState(sixthAnswer, sixthToggle, true);
                currentEvent.EventSixthANSWER = 1;
            }
            else if (this.testBox.Text.ToUpper().Trim() == "NO")
            {
                classifiedResult = "0.0\r\n";
                ChangeAnswerButtonState(sixthAnswer, sixthToggle, false);
                currentEvent.EventSixthANSWER = 0;
            }
            else
            {
                SixthQuestionRun(testBox.Text);
                loadingAnimation.Visibility = Visibility.Visible;
            }
        }
        private async void SixthQuestionRun(string keyWords)
        {
            await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            currentEvent.EventSixthANSWER = CheckClassifiedResultTrueOrFalse(sixthAnswer, sixthToggle);
        }



        private void SeventhQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (this.testBox.Text.ToUpper().Trim() == "YES" || this.testBox.Text.ToUpper().Trim() == "YEAH")
            {
                classifiedResult = "1.0\r\n";
                ChangeAnswerButtonState(seventhAnswer, seventhToggle, true);
                currentEvent.EventSeventhANSWER = 1;
            }
            else if (this.testBox.Text.ToUpper().Trim() == "NO")
            {
                classifiedResult = "0.0\r\n";
                ChangeAnswerButtonState(seventhAnswer, seventhToggle, false);
                currentEvent.EventSeventhANSWER = 0;
            }
            else
            {
                SeventhQuestionRun(testBox.Text);
                loadingAnimation.Visibility = Visibility.Visible;
            }
        }
        private async void SeventhQuestionRun(string keyWords)
        {
            await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            currentEvent.EventSeventhANSWER = CheckClassifiedResultTrueOrFalse(seventhAnswer, seventhToggle);
        }



        private void EighthQuestion_Click(object sender, RoutedEventArgs e)
        {
            //음성 인식한 것 그대로 항목에 채우기
            eighthAnswer.Text = testBox.Text;
            currentEvent.EventEighthANSWER += eighthAnswer.Text;
        }



        private int CheckClassifiedResultTrueOrFalse(TextBlock answerTextBlock, ToggleButton toggleButton)
        {
            if (classifiedResult == "0.0\r\n")
            {
                ChangeAnswerButtonState(answerTextBlock, toggleButton, false);
                return 0;
            }
            else
            {
                ChangeAnswerButtonState(answerTextBlock, toggleButton, true);
                return 1;
            }
        }



        private void ChangeAnswerButtonState(TextBlock answerTextBlock, ToggleButton toggleButton, bool isChecked)
        {
            if (isChecked)
            {
                toggleButton.IsChecked = true;
                answerTextBlock.Text = "YES";
                answerTextBlock.Foreground = mainColorSolidColorBrush;
            }
            else
            {
                toggleButton.IsChecked = false;
                answerTextBlock.Text = "NO";
                answerTextBlock.Foreground = pointColorSolidColorBrush;
            }
        }



        private void FirstAnswer_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                firstAnswer.Text = "YES";
                firstAnswer.Foreground = mainColorSolidColorBrush;
                currentEvent.EventFirstANSWER = 1;
            }
            else
            {
                firstAnswer.Text = "NO";
                firstAnswer.Foreground = pointColorSolidColorBrush;
                currentEvent.EventFirstANSWER = 0;
            }
        }

        private void ThirdAnswer_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                thirdAnswer.Text = "YES";
                thirdAnswer.Foreground = mainColorSolidColorBrush;
                currentEvent.EventThirdANSWER = 1;
            }
            else
            {
                thirdAnswer.Text = "NO";
                thirdAnswer.Foreground = pointColorSolidColorBrush;
                currentEvent.EventThirdANSWER = 0;
            }
        }

        private void FourthAnswer_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                fourthAnswer.Text = "YES";
                fourthAnswer.Foreground = mainColorSolidColorBrush;
                currentEvent.EventFourthANSWER = 1;
            }
            else
            {
                fourthAnswer.Text = "NO";
                fourthAnswer.Foreground = pointColorSolidColorBrush;
                currentEvent.EventFourthANSWER = 0;
            }
        }

        private void FifthAnswer_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                fifthAnswer.Text = "YES";
                fifthAnswer.Foreground = mainColorSolidColorBrush;
                currentEvent.EventFifthANSWER = 1;
            }
            else
            {
                fifthAnswer.Text = "NO";
                fifthAnswer.Foreground = pointColorSolidColorBrush;
                currentEvent.EventFifthANSWER = 0;
            }
        }

        private void SixthAnswer_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                sixthAnswer.Text = "YES";
                sixthAnswer.Foreground = mainColorSolidColorBrush;
                currentEvent.EventSixthANSWER = 1;
            }
            else
            {
                sixthAnswer.Text = "NO";
                sixthAnswer.Foreground = pointColorSolidColorBrush;
                currentEvent.EventSixthANSWER = 0;
            }
        }

        private void SeventhAnswer_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                seventhAnswer.Text = "YES";
                seventhAnswer.Foreground = mainColorSolidColorBrush;
                currentEvent.EventSeventhANSWER = 1;
            }
            else
            {
                seventhAnswer.Text = "NO";
                seventhAnswer.Foreground = pointColorSolidColorBrush;
                currentEvent.EventSeventhANSWER = 0;
            }
        }

        public EventVO CurrentEventVO
        {
            get { return currentEvent; }
            set { currentEvent = value; }
        }
    }
}
