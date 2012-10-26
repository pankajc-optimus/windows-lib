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
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

/*
 * TODO:-
 * Ability to set image resource by dev.
    Correct Thread working...
 * handel status change gps for real device
 * */

namespace CustomMapLibrary
{
    /// <summary>
    ///Custom Maps adds following capabilities to a Map.
    ///1)Show Default location.
    ///2)Redirect to default location.
    ///3)Pin drop on long tap.
    ///4)Custom popup.
    /// </summary>
    public class CustomMapControls
    {
        private GeoCoordinate defaultLocation = new GeoCoordinate();
        private GeoCoordinate customLocation = new GeoCoordinate();
        private GeoCoordinateWatcher watcher;
        private Map map;
        public MapLayer imageLayer = new MapLayer();
        private Image homeImage = new Image();
        private Image pinImage = new Image();

        private PopupWindow popupWindow = new PopupWindow();
        private double _click_X, _click_Y;

        public delegate void Popup_Button_Clicked(object sender, RoutedEventArgs args);

        /// <summary>
        /// Occurs when popup button is clicked.
        /// </summary>
        public event Popup_Button_Clicked popupClicked;

        /// <summary>
        /// Set Or Get Default Location's Zoom Level via this property.
        /// </summary>
        public double ZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new CustomMapControls class object with default images.
        /// </summary>
        /// <param name="map">Map object on which actions are to be performed. </param>
        public CustomMapControls(Map map)
        {
            this.map = map;
            map.Children.Add(imageLayer);
            homeImage.Source = new BitmapImage(new Uri("/CustomMapLibrary;component/Resources/home.png", UriKind.Relative));
            pinImage.Source = new BitmapImage(new Uri("/CustomMapLibrary;component/Resources/pin.png", UriKind.Relative));

            pinImage.Tap += new EventHandler<GestureEventArgs>(pinImage_Tap);
            map.Tap += new EventHandler<GestureEventArgs>(map_Tap);
            map.Hold += new EventHandler<GestureEventArgs>(map_Hold);
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 20;
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.Start(true);

            //Forward Button Press Event from Popup Window 
            popupWindow.btnDetails.Click += new RoutedEventHandler(btnDetails_Click);    

        }

        /// <summary>
        /// Creates a new CustomMapControls class object with alternate images.
        /// </summary>
        /// <param name="map">Map object on which actions are to be performed. </param>
        /// <param name="defaultLocationImage">Default or Home Location Image</param>
        /// <param name="customLocationImage">Custom Location Image</param>
        public CustomMapControls(Map map,ImageSource defaultLocationImage,ImageSource customLocationImage)
        {
            this.map = map;
            map.Children.Add(imageLayer);
            homeImage.Source = defaultLocationImage;
            pinImage.Source = customLocationImage;

            pinImage.Tap += new EventHandler<GestureEventArgs>(pinImage_Tap);
            map.Tap += new EventHandler<GestureEventArgs>(map_Tap);
            map.Hold += new EventHandler<GestureEventArgs>(map_Hold);
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 20;
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.Start(true);

            //Forward Button Press Event from Popup Window 
            popupWindow.btnDetails.Click += new RoutedEventHandler(btnDetails_Click);

        }
        //forward event handling....
        void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (popupClicked != null)
            {
                popupClicked(sender, e);
            }
        }

        //removing popup
        void map_Tap(object sender, GestureEventArgs e)
        {
            if (!((_click_X == e.GetPosition(map).X) && (_click_Y == e.GetPosition(map).Y)))
            {
                imageLayer.Children.Remove(popupWindow);
            }
            
        }

        //adding popup to screen
        void pinImage_Tap(object sender, GestureEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("clicked");
            _click_X = e.GetPosition(map).X;
            _click_Y = e.GetPosition(map).Y;

            imageLayer.Children.Remove(popupWindow);
            imageLayer.AddChild(popupWindow, customLocation, PositionOrigin.BottomRight);
        }

        //adding custom pin to screen
        void map_Hold(object sender, GestureEventArgs e)
        {
            pinImage.MaxHeight = 50;
            pinImage.MaxWidth = 50;
            pinImage.Opacity = 0.8;
            imageLayer.Children.Remove(pinImage);
            customLocation = map.ViewportPointToLocation(e.GetPosition(map));
            imageLayer.AddChild(pinImage, customLocation, PositionOrigin.BottomCenter);
        }

               
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch(e.Status)
            {
                case GeoPositionStatus.Disabled:
                case GeoPositionStatus.NoData: MessageBox.Show("GPS Disabled or internet not available");
                                                break;
                case GeoPositionStatus.Initializing: MessageBox.Show("initializing GPS Please Wait.");
                                                    break;
                case GeoPositionStatus.Ready: break;
            }
        }

        //updates default Position.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            defaultLocation.Latitude = e.Position.Location.Latitude;
            defaultLocation.Longitude = e.Position.Location.Longitude;
            
            //img.Stretch = Stretch.None;
            homeImage.MaxHeight = 50;
            homeImage.MaxWidth = 50;
            homeImage.Opacity = 0.8;
            imageLayer.Children.Remove(homeImage);
            imageLayer.AddChild(homeImage, defaultLocation, PositionOrigin.Center);
            
        }

        /// <summary>
        /// Navigate View to current GPS Location with Default Zoom Level.
        /// Use Property "ZoomLevel" To set or get default Location's Zoom Level.
        /// </summary>
        public void SetViewDefaultLocation()
        {
            map.SetView(defaultLocation, ZoomLevel);
        }


        /// <summary>
        /// Navigate View to current GPS Location with Passed Zoom Level.
        /// </summary>
        /// <param name="zoomLevel">Zoom Level </param>
        public void SetViewDefaultLocation(double zoomLevel)
        {
            map.SetView(defaultLocation, zoomLevel);
        }


    }
}
