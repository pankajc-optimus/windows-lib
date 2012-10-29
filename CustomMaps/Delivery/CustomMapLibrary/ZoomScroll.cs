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
//using Microsoft.Phone.Controls.Maps.AutomationPeers;
//using Microsoft.Phone.Controls.Maps.Core;
//using Microsoft.Phone.Controls.Maps.Design;
//using Microsoft.Phone.Controls.Maps.Overlays;
//using Microsoft.Phone.Controls.Maps.Platform;
using System.Windows.Threading;




namespace CustomMapLibrary
{
    /// <summary>
    /// Give Access to ZoomScrollChanged event to handle zoom or scroll action performed on the map.
    /// </summary>
    public class ZoomScroll
    {
        public delegate void ZoomScrollChanged(object sender,ZoomScrollArgs args);
        /// <summary>
        /// Occurs when map is zoomed or scrolled.
        /// </summary>
        public event ZoomScrollChanged zoomScrollChanged;
        ZoomScrollArgs args = new ZoomScrollArgs();
        private DispatcherTimer timer;
        int secondsCount = 0;
        Map map;

        bool ChangedByEvents = false;

        private double _oldZoomLevel=-1, _newZoomLevel;
        /// <summary>
        /// Creates an object of ZoomScroll Class.
        /// </summary>
        /// <param name="map">Map Class Object</param>
        public ZoomScroll(Map map)
        {
            try
            {
                this.map = map;
                map.MapPan += new EventHandler<MapDragEventArgs>(map_MapPan);
                map.MapZoom += new EventHandler<MapZoomEventArgs>(map_MapZoom);
                map.MapResolved += new EventHandler(map_MapResolved);
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += new EventHandler(timer_Tick);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error:"+e.Message+"\n"+e.Data+"\n"+e.StackTrace);
            }
        }

        

        //Event firing
        void timer_Tick(object sender, EventArgs e)
        {
            secondsCount++;
            if (secondsCount >= 3)
            {
                secondsCount = 0;
                timer.Stop();
                if (args.ZoomOrScroll == ZoomScrollType.Scroll)
                {
                    if(zoomScrollChanged!=null)
                    zoomScrollChanged(this, args);
                }
                else
                {
                    if (zoomScrollChanged != null)
                    zoomScrollChanged(this, args);
                }
            
            }
        }


        
        void map_MapZoom(object sender, MapZoomEventArgs e)
        {
            timer.Stop();
            args.ZoomOrScroll = ZoomScrollType.Zoom;
            args.ZoomLevel = map.ZoomLevel;
            args.Center = map.ViewportPointToLocation(e.ViewportPoint);
            ChangedByEvents = true;
        }

        
        void map_MapPan(object sender, MapDragEventArgs e)
        {
            timer.Stop();
            args.ZoomOrScroll = ZoomScrollType.Scroll;
            args.Center = map.ViewportPointToLocation(e.ViewportPoint);
            ChangedByEvents = true;
        }

        void map_MapResolved(object sender, EventArgs e)
        {
            timer.Stop();
            if (!ChangedByEvents)
            {
                _newZoomLevel = map.ZoomLevel;
                if (_oldZoomLevel != _newZoomLevel)
                {
                    args.ZoomLevel = _newZoomLevel;
                    args.ZoomOrScroll = ZoomScrollType.Zoom;
                    args.Center = map.Center;
                }
                else
                {
                    args.ZoomOrScroll = ZoomScrollType.Scroll;
                    args.Center = map.Center;
                }
            }
            ChangedByEvents = false;
            _oldZoomLevel = _newZoomLevel;
            timer.Start();


        }

    }
}
