using ImagineCupProject.EmergencyResponseManuals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImagineCupProject
{
    /// <summary>
    /// AdditionalQuestion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AdditionalQuestion : Page
    {
        ClassifiedManual classifiedManual = new ClassifiedManual();
        MedicalManual medicalManual = new MedicalManual();

        MainQuestion mainQuestion;
        private readonly ToastViewModel toastViewModel;
        LoadingAnimation loadingAnimation;
        EventVO currentEvent;
        string classifiedResult;

        public AdditionalQuestion(MainQuestion mainQuestion, ToastViewModel toastViewModel, LoadingAnimation loadingAnimation, EventVO currentEvent)
        {
            InitializeComponent();

            this.classifiedManualGrid.Children.Add(classifiedManual);
            this.medicalManualGrid.Children.Add(medicalManual);

            this.mainQuestion = mainQuestion;
            this.toastViewModel = toastViewModel;
            this.loadingAnimation = loadingAnimation;
            this.currentEvent = currentEvent;

            firstQuestion.Text = currentEvent.EventFirstQUESTION;
            secondQuestion.Text = currentEvent.EventSecondQUESTION;
            thirdQuestion.Text = currentEvent.EventThirdQUESTION;
            fourthQuestion.Text = currentEvent.EventFourthUESTION;
            fifthQuestion.Text = currentEvent.EventFifthQUESTION;
            sixthQuestion.Text = currentEvent.EventSixthQUESTION;
            seventhQuestion.Text = currentEvent.EventSeventhQUESTION;
            eighthQuestion.Text = currentEvent.EventEighthQUESTION;
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

        private void textPositiveNegativeClassify_Click(object sender, RoutedEventArgs e)
        {
            Run(testBox.Text);
            loadingAnimation.Visibility = Visibility.Visible;
        }

        private async void Run(string keyWords)
        {
            this.testBox.Text = await TextIsPositiveClassificationAsync(keyWords);
            loadingAnimation.Visibility = Visibility.Hidden;

            //분류된 카테고리에 대한 매뉴얼 출력후 Toast알림 띄우기
            toastViewModel.ShowWarning("Text Classification : " + classifiedResult);
        }

        private async Task<string> TextIsPositiveClassificationAsync(string keyWords)
        {
            try
            {
                string python = @"C:\Python36\python.exe";
                string myPythonApp = "eval_posneg.py";

                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcessStartInfo.Arguments = myPythonApp + " " + "--eval_train" + " " + "--checkpoint_dir=\"./runs/1518189792/checkpoints/\"" + " \"" + keyWords + "\"";

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
    }
}
