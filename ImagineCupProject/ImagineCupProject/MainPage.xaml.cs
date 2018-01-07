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

namespace ImagineCupProject
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        StartPage firstPage = new StartPage();
        StartPage secondPage = new StartPage();
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));

        public MainPage()
        {
            InitializeComponent();

            firstPage.Width = canvas.ActualWidth;
            firstPage.Height = canvas.ActualHeight;

            secondPage.Width = canvas.ActualWidth;
            secondPage.Height = canvas.ActualHeight;
            secondPage.Visibility = Visibility.Hidden;

            canvas.Children.Add(firstPage);
            canvas.Children.Add(secondPage);

            firstPage.previous.Click += Previous_Click;
            firstPage.next.Click += Next_Click;
            secondPage.previous.Click += Previous_Click;
            secondPage.next.Click += Next_Click;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int previousPage = 0;
            int currentPage = 1;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0.0;
            doubleAnimation.To = -this.Width;
            doubleAnimation.Duration = duration;
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Completed += delegate (object sender1, EventArgs e1)
            {
                canvas.Children[previousPage].Visibility = Visibility.Hidden;
            };
            DoubleAnimation doubleAnimationTwo = new DoubleAnimation();
            doubleAnimationTwo.From = this.Width - 800;
            doubleAnimationTwo.To = 0.0;
            doubleAnimationTwo.Duration = duration;
            doubleAnimationTwo.FillBehavior = FillBehavior.HoldEnd;
            canvas.Children[currentPage].Visibility = Visibility.Visible;
            canvas.Children[previousPage].BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            canvas.Children[currentPage].BeginAnimation(Canvas.LeftProperty, doubleAnimationTwo);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            int previousPage = 1;
            int currentPage = 0;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0.0;
            doubleAnimation.To = this.Width - 800;
            doubleAnimation.Duration = duration;
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Completed += delegate (object sender1, EventArgs e1)
            {
                canvas.Children[previousPage].Visibility = Visibility.Hidden;
            };
            DoubleAnimation doubleAnimationTwo = new DoubleAnimation();
            doubleAnimationTwo.From = -this.Width;
            doubleAnimationTwo.To = 0.0;
            doubleAnimationTwo.Duration = duration;
            doubleAnimationTwo.FillBehavior = FillBehavior.HoldEnd;
            canvas.Children[currentPage].Visibility = Visibility.Visible;
            canvas.Children[previousPage].BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            canvas.Children[currentPage].BeginAnimation(Canvas.LeftProperty, doubleAnimationTwo);
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
    }
}
