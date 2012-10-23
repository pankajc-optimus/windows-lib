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
using WindowsPhoneNotificationService;

namespace NotifyMe
{
    public class NotificationClass:ServerService
    {
        private NotificationWebService.NotificationServiceSoapClient webService = new NotificationWebService.NotificationServiceSoapClient();

        public NotificationClass()
        {
            webService.RemoveFromDeviceListCompleted += new EventHandler<NotificationWebService.RemoveFromDeviceListCompletedEventArgs>(webService_RemoveFromDeviceListCompleted);
            webService.SaveToDeviceListCompleted += new EventHandler<NotificationWebService.SaveToDeviceListCompletedEventArgs>(webService_SaveToDeviceListCompleted);
        }

        void webService_SaveToDeviceListCompleted(object sender, NotificationWebService.SaveToDeviceListCompletedEventArgs e)
        {
            MessageBox.Show("Subscribed: " + e.Result.ToString()); 
        }

        void webService_RemoveFromDeviceListCompleted(object sender, NotificationWebService.RemoveFromDeviceListCompletedEventArgs e)
        {
            MessageBox.Show("Unsubscribed: " + e.Result.ToString());
        }

        

        public void Register(string deviceID, string uri)
        {
            try
            {
                webService.SaveToDeviceListAsync(deviceID, uri);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error" + e.Message);
            }
        }

        public void Unregister(string deviceID)
        {
            try
            {
                webService.RemoveFromDeviceListAsync(deviceID);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error"+e.Message);
            }
        }
        

        public void NotificationReceived(object sender, Microsoft.Phone.Notification.NotificationEventArgs e)
        {
            String output="";
            foreach (String key in e.Collection.Keys)
            {
                output +=key+"="+e.Collection[key];
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(output);
                }
                );
        }


    }
}
