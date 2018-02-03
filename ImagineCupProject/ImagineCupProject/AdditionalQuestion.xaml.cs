using ImagineCupProject.EmergencyResponseManuals;
using System;
using System.Collections.Generic;
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

        public AdditionalQuestion()
        {
            InitializeComponent();

            this.classifiedManualGrid.Children.Add(classifiedManual);
            this.medicalManualGrid.Children.Add(medicalManual);
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

    }
}
