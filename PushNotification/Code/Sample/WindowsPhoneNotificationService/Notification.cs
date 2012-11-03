using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Text;
using Microsoft.Phone.Notification;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;





namespace WindowsPhoneNotificationService
{
    /// <summary>
    /// Notification class helps in seting up and stopping push notification service.
    /// </summary>
    public class Notification
    {
        private ServerService server;
        //Default Channel Name
        private String _channelName = "ToastChannel";
        private String _deviceID { get; set; }

        /// <summary>
        /// Get or Set Channel Name.
        /// </summary>
        public String ChannelName {
            get
            {
                return _channelName;
            }
            set
            {
                _channelName = value;
            }
        }
        /// <summary>
        /// Creates a Notification Service and link it with web Service.
        /// </summary>
        public Notification(ServerService serverService)
        {
            server = serverService;
        }

        /// <summary>
        /// Adds Device to Notification list and setup notification service.
        /// </summary>
        public void SetupNotification()
        {
            try
            {
                HttpNotificationChannel pushChannel;
                  
                // Find Push Channel.
                pushChannel = HttpNotificationChannel.Find(ChannelName);

                //Create or get Unique DeviceID
                if (IsolatedStorageSettings.ApplicationSettings.Contains("deviceID"))
                {
                    _deviceID = (String)IsolatedStorageSettings.ApplicationSettings["deviceID"];
                }
                else
                {
                    _deviceID = Guid.NewGuid().ToString();
                    IsolatedStorageSettings.ApplicationSettings["deviceID"] = _deviceID;
                }
                
                // create a new HttpNotificationchannel if the required channel is not found.
                if (pushChannel == null)
                {
                    pushChannel = new HttpNotificationChannel(ChannelName);
                    pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                    pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                    pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);
                    pushChannel.Open();
                    pushChannel.BindToShellToast();

                }
                //required channel found
                else
                {
                    pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                    pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                    pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                    // Passing Uri to the server.
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("registered"))
                    {
                        //Registers device to server,if it's not already registered.
                        if (!((bool)IsolatedStorageSettings.ApplicationSettings["registered"]))
                        {
                            server.Register(_deviceID, pushChannel.ChannelUri.ToString());
                            IsolatedStorageSettings.ApplicationSettings["registered"] = true;
                            IsolatedStorageSettings.ApplicationSettings["uri"] = pushChannel.ChannelUri.ToString();
                        }
                    }
                    else
                    {
                        server.Register(_deviceID, pushChannel.ChannelUri.ToString());
                        IsolatedStorageSettings.ApplicationSettings["registered"] = true;
                        IsolatedStorageSettings.ApplicationSettings["uri"] = pushChannel.ChannelUri.ToString();
                    }

               }
               
            }
            catch (Exception e)
            {
                MessageBox.Show("Error:"+e.ToString());
            }
        }
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(()=>
            {               
                //Passing new Uri to the server in case uri is updated.
                server.Register(_deviceID, e.ChannelUri.ToString());
                IsolatedStorageSettings.ApplicationSettings["registered"] = true;
                IsolatedStorageSettings.ApplicationSettings["uri"] = e.ChannelUri.ToString();
            });
        }
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {           
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            MessageBox.Show("Error: "+e.Message)
             );
        }

        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            //Forward event to ServerService's NotificationReceived() method When Notification received event is triggered.
            server.NotificationReceived(sender,e);
        }

        /// <summary>
        /// Removes device from Notification List.
        /// </summary>
        public void StopNotifications()
        {
            try
            {
                server.Unregister((String)IsolatedStorageSettings.ApplicationSettings["deviceID"]);
                IsolatedStorageSettings.ApplicationSettings["registered"] = false;
            }
            catch (KeyNotFoundException e)
            {
                MessageBox.Show("Error:\nUser Not Subscribed.\n" +e.Message+"\n"+e.Data);
            }
            
        }
         

    }

    }

