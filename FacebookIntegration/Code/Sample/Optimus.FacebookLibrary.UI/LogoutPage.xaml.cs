using System;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Phone.Controls;

namespace Optimus.FacebookLibrary.UI
{
    public partial class LogOutPage : PhoneApplicationPage
    {
        private string _accessToken;
        public LogOutPage()
        {
            InitializeComponent();
        }

        private void wbLogOut_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string fbLogoutDoc = wbLogOut.SaveToString();
            Regex regex = new Regex("\\<A href=\\\"(.*)\\\".*data-sigil=\\\"logout\\\"");
            MatchCollection matches = regex.Matches(fbLogoutDoc);
            if (matches.Count > 0)
            {
                string finalLogout = string.Format("http://m.facebook.com{0}", matches[0].Groups[1].ToString());
                wbLogOut.Navigate(new Uri(finalLogout));
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void wbLogOut_Loaded(object sender, RoutedEventArgs e)
        {
            wbLogOut.Navigate(new Uri("http://m.facebook.com/logout.php?confirm=1"));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _accessToken = NavigationContext.QueryString["access_token"];
        }
       
    }
}