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
        private ZoomScrollArgs _args = new ZoomScrollArgs();
        private DispatcherTimer _timer;
        private int _secondsCount = 0;
        private Map _map;

        private bool _changedByEvents = false;

        private double _oldZoomLevel=-1, _newZoomLevel;
        /// <summary>
        /// Creates an object of ZoomScroll Class.
        /// </summary>
        /// <param name="map">Map Class Object</param>
        public ZoomScroll(Map map)
        {
            try
            {
                this._map = map;
                _map.MapPan += new EventHandler<MapDragEventArgs>(map_MapPan);
                _map.MapZoom += new EventHandler<MapZoomEventArgs>(map_MapZoom);
                _map.MapResolved += new EventHandler(map_MapResolved);
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += new EventHandler(timer_Tick);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error:"+e.Message+"\n"+e.Data+"\n"+e.StackTrace);
            }
        }

        

        //Event firing
        void timer_Tick(object sender, EventArgs e)
        {
            _secondsCount++;
            if (_secondsCount >= 3)
            {
                _secondsCount = 0;
                _timer.Stop();
                if (_args.ZoomOrScroll == ZoomScrollType.Scroll)
                {
                    if (zoomScrollChanged != null)
                    {
                        zoomScrollChanged(this, _args);
                    }
                }
                else
                {
                    if (zoomScrollChanged != null)
                    {
                        zoomScrollChanged(this, _args);
                    }
                }
            
            }
        }


        
        void map_MapZoom(object sender, MapZoomEventArgs e)
        {
            _timer.Stop();
            _args.ZoomOrScroll = ZoomScrollType.Zoom;
            _args.ZoomLevel = _map.ZoomLevel;
            _args.Center = _map.ViewportPointToLocation(e.ViewportPoint);
            _changedByEvents = true;
        }

        
        void map_MapPan(object sender, MapDragEventArgs e)
        {
            _timer.Stop();
            _args.ZoomOrScroll = ZoomScrollType.Scroll;
            _args.Center = _map.ViewportPointToLocation(e.ViewportPoint);
            _changedByEvents = true;
        }

        void map_MapResolved(object sender, EventArgs e)
        {
            _timer.Stop();
            if (!_changedByEvents)
            {
                _newZoomLevel = _map.ZoomLevel;
                if (_oldZoomLevel != _newZoomLevel)
                {
                    _args.ZoomLevel = _newZoomLevel;
                    _args.ZoomOrScroll = ZoomScrollType.Zoom;
                    _args.Center = _map.Center;
                }
                else
                {
                    _args.ZoomOrScroll = ZoomScrollType.Scroll;
                    _args.Center = _map.Center;
                }
            }
            _changedByEvents = false;
            _oldZoomLevel = _newZoomLevel;
            _timer.Start();


        }

    }
}
