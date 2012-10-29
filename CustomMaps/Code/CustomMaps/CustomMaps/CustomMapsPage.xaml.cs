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
using CustomMapLibrary;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Overlays;
using System.Device.Location;
using System.Windows.Media.Imaging;

namespace CustomMaps
{
    public partial class MainPage : PhoneApplicationPage
    {
        CustomMapControls custom;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ZoomScroll obj = new ZoomScroll(map);
            obj.zoomScrollChanged += new ZoomScroll.ZoomScrollChanged(obj_zoomScrollChanged);
            custom = new CustomMapControls(map, new BitmapImage(new Uri("Images/home.png", UriKind.Relative)), new BitmapImage(new Uri("Images/pin.png", UriKind.Relative)));
            custom.popupClicked += new CustomMapControls.Popup_Button_Clicked(custom_popupClicked);
        }

        void custom_popupClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.Navigate(new Uri("/Views/NavigateTo.xaml", UriKind.Relative));
        }


        void obj_zoomScrollChanged(object sender, ZoomScrollArgs args)
        {
            if (args.ZoomOrScroll == ZoomScrollType.Zoom)
            {
                tbMapResult.Text = "lat:" + args.Center.Latitude + "\nlong:" + args.Center.Longitude + "\nzm:" + args.ZoomLevel;
            }
            else
            {
                tbMapResult.Text = "lat:" + args.Center.Latitude + "\nlong:" + args.Center.Longitude;
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            //setting defaultLocation's Zoom Level.
            custom.ZoomLevel = 5;
            custom.SetViewDefaultLocation();
        }
    }
}