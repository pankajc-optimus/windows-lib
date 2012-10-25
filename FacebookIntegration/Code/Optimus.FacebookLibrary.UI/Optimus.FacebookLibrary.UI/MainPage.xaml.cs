using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace Optimus.FacebookLibrary.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnFbLogin_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to LoginPage
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}