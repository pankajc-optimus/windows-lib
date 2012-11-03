/*---------------------
 * Code behind file for Main application page.
 * -------------------
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
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using System.IO;


namespace PictureLibrary
{
    public partial class MainPage : PhoneApplicationPage
    {
        BitmapImage bitmapImage;
        CameraCaptureTask camera;
        int value = 1; 
        PhotoChooserTask photoChooserTask;
        BitmapImage temp=new BitmapImage();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            MediaLibrary mediaLibrary = new MediaLibrary();
        }

        /// <summary>
        /// Captures the Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btncapture_Click(object sender, RoutedEventArgs e)
        {
            camera = new CameraCaptureTask();
            camera.Show();
            camera.Completed += new EventHandler<PhotoResult>(camera_Completed);
        }

        void camera_Completed(object sender, PhotoResult e)
        {
            try
            {
                bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e.ChosenPhoto);
                temp.SetSource(e.ChosenPhoto);
                imgView.Source = bitmapImage;
               
            }
            catch
            {

            }
        }

        /// <summary>
        /// Picks the Picture From Media gallery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPickPictureFromGallery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                photoChooserTask = new PhotoChooserTask();
                photoChooserTask.Show();
                photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            }
            catch
            {

            }
        }
        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e.ChosenPhoto);
                temp.SetSource(e.ChosenPhoto);
                imgView.Source = bitmapImage;
            }
        }
       
        /// <summary>
        /// Rotates the Image 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            int angleToRotate = 90;
            Image image = new Image();
            image = imgView;
            BitmapImage bi = new BitmapImage();
            bi = (BitmapImage)image.Source;
            WriteableBitmap wbTarget = Helper.Rotate(angleToRotate, bi);
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            BitmapImage biTarget = new BitmapImage();
            biTarget.SetSource(targetStream);
            imgView.Source = biTarget;           
        }
         
        /// <summary>
        /// Compress the image based on the Quality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgCompress_Click(object sender, RoutedEventArgs e)
        {
           value++;
           string ImageName = "Image" + value + ".jpg";
           Helper.Compress(temp,value);
        }
        /// <summary>
        /// Save the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            value++;
            string ImageName = "Image" + value + ".jpg";
            Helper.SaveImage(bitmapImage, ImageName);
        }

        /// <summary>
        /// Create thumbnail of image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnThumbNail_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap = (BitmapImage)imgView.Source;
            WriteableBitmap wbTarget = new WriteableBitmap(bitmap);
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, 50, 50, 0, 100);
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(targetStream);
            imgToThumbNail.MaxWidth = bmp.PixelWidth;
            imgToThumbNail.MaxHeight = bmp.PixelHeight;
            imgToThumbNail.Source = bmp;
        }
    }
}
