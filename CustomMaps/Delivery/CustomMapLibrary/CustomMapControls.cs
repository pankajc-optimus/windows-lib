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
        private GeoCoordinate _defaultLocation = new GeoCoordinate();
        private GeoCoordinate _customLocation = new GeoCoordinate();
        private GeoCoordinateWatcher _watcher;
        private Map _map;

        /// <summary>
        /// Represents a map layer which positions it's child(UIElement) using geographic coordinates.
        /// </summary>
        public MapLayer ImageLayer = new MapLayer();

        private Image _homeImage = new Image();
        private Image _pinImage = new Image();

        private PopupWindow _popupWindow = new PopupWindow();
        private double _click_X, _click_Y;

        public delegate void Popup_Button_Clicked(object sender, RoutedEventArgs args);

        /// <summary>
        /// Occurs when button on popup window is clicked.
        /// </summary>
        public event Popup_Button_Clicked popupClicked;

        /// <summary>
        /// Set Or Get Zoom Level For Default Location.
        /// </summary>
        public double ZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new CustomMapControls class object using default images.
        /// </summary>
        /// <param name="map">Map object on which actions are to be performed. </param>
        public CustomMapControls(Map map)
        {
            try
            {
                this._map = map;
                _map.Children.Add(ImageLayer);
                _homeImage.Source = new BitmapImage(new Uri("/CustomMapLibrary;component/Resources/home.png", UriKind.Relative));
                _pinImage.Source = new BitmapImage(new Uri("/CustomMapLibrary;component/Resources/pin.png", UriKind.Relative));

                _pinImage.Tap += new EventHandler<GestureEventArgs>(pinImage_Tap);
                _map.Tap += new EventHandler<GestureEventArgs>(map_Tap);
                _map.Hold += new EventHandler<GestureEventArgs>(map_Hold);
                _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                _watcher.MovementThreshold = 20;
                _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                _watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                _watcher.Start(true);

                //Forward Button Press Event from Popup Window 
                _popupWindow.btnDetails.Click += new RoutedEventHandler(btnDetails_Click);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message + "\n" + e.Data + "\n" + e.StackTrace);
            }
        }

        /// <summary>
        /// Creates a new CustomMapControls class object using supplied image source.
        /// </summary>
        /// <param name="map">Map object on which actions are to be performed. </param>
        /// <param name="defaultLocationImage">Default or Home Location Image</param>
        /// <param name="customLocationImage">Custom Location Image</param>
        public CustomMapControls(Map map, ImageSource defaultLocationImage, ImageSource customLocationImage)
        {
            try
            {
                this._map = map;
                _map.Children.Add(ImageLayer);
                _homeImage.Source = defaultLocationImage;
                _pinImage.Source = customLocationImage;

                _pinImage.Tap += new EventHandler<GestureEventArgs>(pinImage_Tap);
                _map.Tap += new EventHandler<GestureEventArgs>(map_Tap);
                _map.Hold += new EventHandler<GestureEventArgs>(map_Hold);
                _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                _watcher.MovementThreshold = 20;
                _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                _watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                _watcher.Start(true);

                //Forward Button Press Event from Popup Window 
                _popupWindow.btnDetails.Click += new RoutedEventHandler(btnDetails_Click);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message + "\n" + e.Data + "\n" + e.StackTrace);
            }
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
            //Discards any tap made on the customLocation's pin.
            //(Tap Event sequence -> (1)pinImage -> (2)map)
            if (!((_click_X == e.GetPosition(_map).X) && (_click_Y == e.GetPosition(_map).Y)))
            {
                ImageLayer.Children.Remove(_popupWindow);
            }

        }

        //adding popup to screen
        //(Tap Event sequence-> (1)pinImage -> (2)map)
        void pinImage_Tap(object sender, GestureEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("clicked");
                _click_X = e.GetPosition(_map).X;
                _click_Y = e.GetPosition(_map).Y;

                ImageLayer.Children.Remove(_popupWindow);
                ImageLayer.AddChild(_popupWindow, _customLocation, PositionOrigin.BottomRight);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace);
            }

        }

        //adding custom pin to screen
        void map_Hold(object sender, GestureEventArgs e)
        {
            try
            {
                _pinImage.MaxHeight = 50;
                _pinImage.MaxWidth = 50;
                _pinImage.Opacity = 0.8;
                ImageLayer.Children.Remove(_pinImage);
                _customLocation = _map.ViewportPointToLocation(e.GetPosition(_map));
                ImageLayer.AddChild(_pinImage, _customLocation, PositionOrigin.BottomCenter);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace);
            }
        }


        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:

                case GeoPositionStatus.NoData: MessageBox.Show("GPS Disabled or internet not available");
                    break;
                case GeoPositionStatus.Initializing:

                case GeoPositionStatus.Ready: break;
            }
        }

        //Updates default Position.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            try
            {
                _defaultLocation.Latitude = e.Position.Location.Latitude;
                _defaultLocation.Longitude = e.Position.Location.Longitude;
                _homeImage.MaxHeight = 50;
                _homeImage.MaxWidth = 50;
                _homeImage.Opacity = 0.8;
                ImageLayer.Children.Remove(_homeImage);
                ImageLayer.AddChild(_homeImage, _defaultLocation, PositionOrigin.Center);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace);
            }

        }

        /// <summary>
        /// Navigate View to current GPS Location with Default Zoom Level.
        /// Use Property "ZoomLevel" To set or get default Location's Zoom Level.
        /// </summary>
        public void SetViewDefaultLocation()
        {
            _map.SetView(_defaultLocation, ZoomLevel);
        }


        /// <summary>
        /// Navigate View to current GPS Location with Passed Zoom Level.
        /// </summary>
        /// <param name="zoomLevel">Zoom Level </param>
        public void SetViewDefaultLocation(double zoomLevel)
        {
            _map.SetView(_defaultLocation, zoomLevel);
        }


    }
}
