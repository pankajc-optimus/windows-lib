/*
 Class InfoPage
 * Code behind class for InfoPage.xaml 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Facebook;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Optimus.FacebookLibrary.UI
{
    public partial class InfoPage : PhoneApplicationPage
    {
        private string _accessToken;
        private string _userId;
        private DateTime _expireTime;
        private string _lastMessageId;
        private Stream _imgStream;

        /// <summary>
        /// Constructor
        /// </summary>
        public InfoPage()
        {
            InitializeComponent();
            btnDeletePost.IsEnabled = false;
        }

        /// <summary>
        /// Overrided function OnNavigatedTo
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _accessToken = NavigationContext.QueryString["access_token"];
            _userId = NavigationContext.QueryString["id"];
            _expireTime = Convert.ToDateTime(NavigationContext.QueryString["expire_time"]);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserInformation();
        }

        private void btnAccessToken_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtInput.Text = _accessToken;
        }

        private void btnExpireTime_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtInput.Text = _expireTime.ToString();
        }

        private void btnWallPost_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                MessageBox.Show("Enter message.");
                return;
            }
            WallPost();
        }

        private void btnDeletePost_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btnDeletePost.IsEnabled = false;

            DeletePost();
        }


        private void btnChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooser = new PhotoChooserTask();
            photoChooser.Completed += (o, args) =>
                                          {
                                              _imgStream = args.ChosenPhoto;
                                          };
            photoChooser.Show();
        }

        private void btnPostPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (_imgStream == null)
            {
                MessageBox.Show("Pick a Photo to upload");
                return;
            }
            PostPhoto();

        }

        private void btnFriendList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GetFriendList();
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchFriend();
        }

        private void btnGraphApi_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GraphApi();
        }

        private void btnFql_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FqlSample();
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            CheckIn();
        }

        private void btnlogout_Click(object sender, RoutedEventArgs e)
        {
            var url = string.Format("/FacebokLogoutPage.xaml?access_token={0}", _accessToken);

            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        /// <summary>
        /// Load Facebook information of the user
        /// </summary>
        public void LoadUserInformation()
        {
            // available picture types: square (50x50), small (50xvariable height), large (about 200x variable height) (all size in pixels)
            string profilePictureUrl = string.Format(
                "https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", _userId, "square", _accessToken);
            // load profile picture
            picProfile.Source = new BitmapImage(new Uri(profilePictureUrl));

            var fb = new FacebookClient(_accessToken);
            fb.GetCompleted += (o, e) =>
                                   {
                                       if (e.Error != null)
                                       {
                                           Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                                           return;
                                       }

                                       var result = (IDictionary<string, object>) e.GetResultData();
                                       Dispatcher.BeginInvoke(() =>
                                                                  {
                                                                      ProfileName.Text = "Hi " + (string) result["name"];
                                                                      FirstName.Text = "First Name: " +
                                                                                       (string) result["first_name"];
                                                                      FirstName.Text = "Last Name: " +
                                                                                       (string) result["last_name"];
                                                                  });
                                   };

            // get user information
            fb.GetAsync("me");
        }
        
        /// <summary>
        /// Post on wall of Facebook user
        /// </summary>
        public void WallPost()
        {
            var fb = new FacebookClient(_accessToken);
            fb.PostCompleted += (o, args) =>
                                    {
                                        if (args.Error != null)
                                        {
                                            Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                            return;
                                        }

                                        var result = (IDictionary<string, object>) args.GetResultData();
                                        _lastMessageId = (string) result["id"];

                                        Dispatcher.BeginInvoke(() =>
                                                                   {
                                                                       MessageBox.Show("Message Posted successfully");

                                                                       txtAnswer.Text = string.Empty;
                                                                       btnDeletePost.IsEnabled = true;
                                                                   });
                                    };

            var parameters = new Dictionary<string, object>();
            parameters["message"] = txtInput.Text;

            fb.PostAsync("me/feed", parameters); // post message on wall of user
        }

        /// <summary>
        /// Delete post from wall of facebook user
        /// </summary>
        public void DeletePost()
        {
            var fb = new FacebookClient(_accessToken);

            fb.DeleteCompleted += (o, args) =>
                                      {
                                          if (args.Error != null)
                                          {
                                              Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                              return;
                                          }

                                          Dispatcher.BeginInvoke(() =>
                                                                     {
                                                                         MessageBox.Show("Message deleted successfully");
                                                                         btnDeletePost.IsEnabled = false;
                                                                     });
                                      };

            fb.DeleteAsync(_lastMessageId);  // delete message from wall
        }

        /// <summary>
        /// Post photo on wall of facebook user
        /// </summary>
        public void PostPhoto()
        {
            var facebookClient = new FacebookClient(_accessToken);
            FacebookMediaStream media = new FacebookMediaStream {ContentType = "image/jpeg", FileName = "test Image"};
            media.SetValue(_imgStream);

            facebookClient.PostCompleted += (o, args) =>
                                                {
                                                    if (args.Error != null)
                                                    {
                                                        Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                                        return;
                                                    }
                                                    var result = (IDictionary<string, object>) args.GetResultData();
                                                    _lastMessageId = (string) result["id"];

                                                    Dispatcher.BeginInvoke(() =>
                                                                               {
                                                                                   MessageBox.Show(
                                                                                       "Message Posted successfully");
                                                                                   btnDeletePost.IsEnabled = true;
                                                                               });
                                                };


            IDictionary<string, object> parameters = new Dictionary<string, object>();

            //// to upload photo with link
            //parameters["name"] = "Check this out";
            //parameters["link"] = "www.xyz.com";
            //parameters["caption"] = "xyz dot com";
            //parameters["description"] = "Test url hello how are you, Black Code , Media Test";
            //parameters["picture"] = media;
            //parameters["message"] = "Check this out";
            //parameters["actions"] = "";
            //facebookClient.PostAsync("me/feed", parameters);  



            // to upload only photo with message
            parameters["message"] = "Photo upload test " + DateTime.Now.ToString();
            parameters["file"] = media;

            facebookClient.PostAsync("me/Photos", parameters);  // post photo on wall
        }

        /// <summary>
        /// Get friends list of facebook user
        /// </summary>
        public void GetFriendList()
        {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, args) =>
                                   {
                                       if (args.Error != null)
                                       {
                                           Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                           return;
                                       }

                                       var result = (IDictionary<string, object>) args.GetResultData();
                                       var data = (IList<object>) result["data"];

                                       var count = data.Count;

                                       // since this is an async callback, make sure to be on the right thread
                                       // when working with the UI.
                                       Dispatcher.BeginInvoke(() =>
                                                                  {
                                                                      txtAnswer.Text = result.ToString();
                                                                  });
                                   };

            // query to get all the friends
            var query =
                string.Format(
                    "SELECT uid,name,pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1={0})", "me()");
            fb.GetAsync("fql", new {q = query});
        }

        /// <summary>
        /// Search in Facebook friends
        /// </summary>
        public void SearchFriend()
        {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, args) =>
                                   {
                                       if (args.Error != null)
                                       {
                                           Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                           return;
                                       }

                                       var result = (IDictionary<string, object>) args.GetResultData();
                                       var data = (IList<object>) result["data"];
                                       var count = data.Count;

                                       // since this is an async callback, make sure to be on the right thread
                                       // when working with the UI.
                                       Dispatcher.BeginInvoke(() =>
                                                                  {
                                                                      txtAnswer.Text = result.ToString();
                                                                  });
                                   };

            // query to get all the friends
            var query =
                string.Format(
                    "SELECT uid,username, first_name FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1={0}) AND first_name ='Vikrant'",
                    "me()");
            fb.GetAsync("fql", new {q = query});
        }

        /// <summary>
        /// Graph Api interface to get result
        /// </summary>
        public void GraphApi()
        {
            var fb = new FacebookClient(_accessToken);
            fb.GetCompleted += (o, args) =>
                                   {
                                       if (args.Error != null)
                                       {
                                           Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                           return;
                                       }

                                       var result = (IDictionary<string, object>) args.GetResultData();

                                       Dispatcher.BeginInvoke(() =>
                                                                  {
                                                                      txtAnswer.Text = result.ToString();
                                                                  });
                                   };

            fb.GetAsync("me");
        }

        /// <summary>
        /// Fql Sample to get Result
        /// </summary>
        public void FqlSample()
        {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, args) =>
                                   {
                                       if (args.Error != null)
                                       {
                                           Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                                           return;
                                       }

                                       var result = (IDictionary<string, object>) args.GetResultData();
                                       var data = (IList<object>) result["data"];

                                       // since this is an async callback, make sure to be on the right thread
                                       // when working with the UI.
                                       Dispatcher.BeginInvoke(() =>
                                                                  {
                                                                      txtAnswer.Text = data.ToString();
                                                                  });
                                   };

            // query to get all the friends
            var query =string.Format("SELECT uid,pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1={0})","me()");
            fb.GetAsync("fql", new {q = query});
        }

        /// <summary>
        /// Checkin current location of user on facebook 
        /// </summary>
        public void CheckIn()
        {
            var fb = new FacebookClient(_accessToken);
            fb.PostCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                    return;
                }


                var result = (IDictionary<string, object>)args.GetResultData();
                _lastMessageId = (string)result["id"]; // get result id of post
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Checkin done successfully");
                });
            };

            var parameters = new Dictionary<string, object>();
            //parameters["message"] = txtInput.Text;                   
            parameters["place"] = "173205796066382"; // place of check in
            parameters["coordinates"] = "28.627116663763,77.375440942471"; // coordinates of checkin

            // post for checkin
            fb.PostAsync("me/feed", parameters);
        }
    }


}