using System;
using System.Collections.Generic;
using System.Windows;
using Facebook;
using Microsoft.Phone.Controls;



namespace Optimus.FacebookLibrary.UI
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private readonly FacebookClient _facebookClient;
        private const string AppId = "412151092171472";
        private FacebookOAuthResult _oauthResult;
        /// <summary>
        /// Extended permissions is a comma separated list of permissions to ask the user.
        /// </summary>
        /// <remarks>
        /// For extensive list of available extended permissions refer to 
        /// https://developers.facebook.com/docs/reference/api/permissions/
        /// </remarks>
        private const string ExtendedPermissions = "user_about_me,read_stream,publish_stream,read_friendlists,user_status,user_checkins,offline_access";

        public LoginPage()
        {
            InitializeComponent();
            _facebookClient = new FacebookClient();
        }
        
        private void wbLogin_Loaded(object sender, RoutedEventArgs e)
        {
            var loginUrl = GetFacebookLoginUrl(AppId, ExtendedPermissions);
            wbLogin.Navigate(loginUrl);
        }

        private void wbLogin_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
            if (!_facebookClient.TryParseOAuthCallbackUrl(e.Uri, out _oauthResult))
            {
                return;
            }

            if (_oauthResult.IsSuccess)
            {
                var accessToken = _oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                // user cancelled
                MessageBox.Show(_oauthResult.ErrorDescription);
            }
        }

        /// <summary>
        /// Function : To get the Facebook loging url through AppId and permissions
        /// </summary>
        /// <param name="appId">Facebook application Id</param>
        /// <param name="extendedPermissions">Facebook Permissions</param>
        /// <returns></returns>
        private Uri GetFacebookLoginUrl(string appId, string extendedPermissions)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = appId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";

            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = extendedPermissions;
            }

            return _facebookClient.GetLoginUrl(parameters);
        }


        /// <summary>
        /// Function : To Check whether login successful or not and redirect to landing page
        /// </summary>
        /// <param name="accessToken">Facebook AccessToken</param>
        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var id = (string)result["id"];

                var url = string.Format("/InfoPage.xaml?access_token={0}&id={1}&expire_time={2}", accessToken, id,_oauthResult.Expires);

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri(url, UriKind.Relative)));
            };

            fb.GetAsync("me?fields=id");
        }

    }
}