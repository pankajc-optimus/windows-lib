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

namespace CustomMapLibrary
{
    /// <summary>
    /// Contains event data associated with the ZoomScrollChanged Event.
    /// </summary>
    public class ZoomScrollArgs:EventArgs
    {
        /// <summary>
        /// Set or get an action made on a given map.
        /// </summary>
        public ZoomScrollType ZoomOrScroll { get; set; }

        /// <summary>
        /// Set or get location coordinates of map's center.
        /// </summary>
        public GeoCoordinate Center { get; set; }
        private double _zoomLevel=-1;

        /// <summary>
        /// Set or get map's zoom level.
        /// </summary>
        public double ZoomLevel {
            get
            {
                return _zoomLevel;
            }
            set
            {
                
                _zoomLevel = value;
            }
        }
        /// <summary>
        /// Creates a ZoomScrollArgs object initialized with default values.
        /// </summary>
        public ZoomScrollArgs()
        {
          //Empty.
        }

        /// <summary>
        /// Creates a ZoomScrollArgs object with default ZoomLevel. 
        /// </summary>
        /// <param name="zoomScrollType">Type of Action occurred i.e. Zoom Action or Scroll Action.</param>
        /// <param name="center">Location Geocordinates</param>
        public ZoomScrollArgs(ZoomScrollType zoomScrollType,GeoCoordinate center)
        {
            ZoomOrScroll = zoomScrollType;
            Center = center;
        }

        /// <summary>
        /// Creates a ZoomScrollArgs object.
        /// </summary>
        /// <param name="zoomScrollType">Type of Action occurred i.e. Zoom Action or Scroll Action.</param>
        /// <param name="center">Location Geocordinates</param>
        /// <param name="zoomLevel">Maps's Zoom Level</param>
        public ZoomScrollArgs(ZoomScrollType zoomScrollType, GeoCoordinate center,double zoomLevel)
        {
            ZoomOrScroll = zoomScrollType;
            Center = center;
            ZoomLevel = zoomLevel;
        }

    }
}
