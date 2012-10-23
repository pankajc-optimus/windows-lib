using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Notification;
using System.Text;
using WindowsPhoneNotificationService;
using System.IO.IsolatedStorage;




namespace NotifyMe
{
    public partial class MainPage : PhoneApplicationPage
    {
       
        public ServerService ser ;
        public Notification obj;
        // Constructor
        public MainPage()
        {
            
            InitializeComponent();
            ser = new NotificationClass();
            obj = new Notification(ser);

            //NotificationSwitch is set by user to start or stop push notifications.
            if (IsolatedStorageSettings.ApplicationSettings.Contains("notificationSwitch"))
            {
                //If notificationSwitch is ON(True) then only start notification service.
                if ((bool)IsolatedStorageSettings.ApplicationSettings["notificationSwitch"])
                {
                    //Start in-app Notification listening every time application starts.
                    obj.SetupNotification();
                }
            }
            else
            {
                //By Default Notifications are disabled.
                IsolatedStorageSettings.ApplicationSettings["notificationSwitch"] = false;
            }

            
        }

        private void btnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            //Start Push Notifications.
            IsolatedStorageSettings.ApplicationSettings["notificationSwitch"] = true;
            obj.SetupNotification();
        }   

        private void btnUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            //Stop Push Notifications.
            IsolatedStorageSettings.ApplicationSettings["notificationSwitch"] = false;
            obj.StopNotifications();
        }
    }

    
}