/*----------------------------
 * MainPage Code Behind File
 * ---------------------------
 * 
 * It contains the event handlers for camera and Photo gallery to set the Image source for Scanner Library
 * 
 * The Decode button function calles the getBarcodeData() and getBarcodeFormate() functions from Scanner library 
 * to fetch Batcode data and it format and display in a message
 * 
 */
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
using BarcodeScannerLib;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BarcodeScanner
{
    public partial class MainPage : PhoneApplicationPage
    {
        CameraCaptureTask camera = new CameraCaptureTask();
        PhotoChooserTask photo = new PhotoChooserTask();
        Scanner sc = new Scanner();

        public MainPage()
        {
            InitializeComponent();
        }

        void camera_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                try
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(e.ChosenPhoto);

                    ImageHolder.Source = bitmapImage;

                    sc.ImageSource = bitmapImage;
                }
                catch { }
            }
        }

        private void ApplicationBarScanButton_Click(object sender, EventArgs e)
        {
            camera.Show();
            camera.Completed += new EventHandler<PhotoResult>(camera_Completed);
        }

        private void ApplicationBarPicturteButton_Click(object sender, EventArgs e)
        {
            photo.Show();
            photo.Completed += new EventHandler<PhotoResult>(photo_Completed);
        }

        void photo_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                try
                {
                    BitmapImage bitMapImage = new BitmapImage();
                    bitMapImage.SetSource(e.ChosenPhoto);

                    ImageHolder.Source = bitMapImage;

                    sc.ImageSource = bitMapImage;
                }
                catch { }
            }
        }

        private void ApplicationBarDecodeButton_Click(object sender, EventArgs e)
        {
            string barcodeData = sc.getBarcodeData();
            string barcodeFormat = sc.getBarcodeFormat();

            MessageBox.Show("BarcodeData: " + barcodeData + "\r\n" + "BarcodeFormat: " + barcodeFormat);
        }

    }
}