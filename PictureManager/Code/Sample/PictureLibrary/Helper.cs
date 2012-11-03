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
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace PictureLibrary
{
    public static class Helper
    {


        /// <summary>
        /// 
        /// To rotate the image
        /// </summary>
        /// <param name="angle">the angle on which the image is rotated</param>
        /// <param name="bi">Bitmap image to be rotated.</param>
        /// <returns></returns>
        public static WriteableBitmap Rotate(int angle, BitmapImage bi)
        {
            WriteableBitmap wbTarget = null;

            try
            {
                WriteableBitmap wbSource = new WriteableBitmap(bi);

                if (angle % 180 == 0)
                {
                    wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
                }
                else
                {
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                }
                for (int x = 0; x < wbSource.PixelWidth; x++)
                {
                    for (int y = 0; y < wbSource.PixelHeight; y++)
                    {
                        switch (angle % 360)
                        {
                            case 90:
                                wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                                break;
                            case 180:
                                wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                                break;
                            case 270:
                                wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                                break;
                        }

                    }

                }

            }
            catch
            {

            }
            return wbTarget;
        }




        /// <summary>
        /// Compresses the image and save it in media library.
        /// </summary>
        /// <param name="ImageToCompress">BitmapImage</param>
        /// <param name="compressValue">compression value</param>
        public static void Compress(BitmapImage temp,int value)
        {
            try
            {
                int compression = 100;
                BitmapImage bitmap = new BitmapImage();
                bitmap = temp;
                MemoryStream memoryStreamSource = new MemoryStream();
                WriteableBitmap wbSource = new WriteableBitmap(bitmap);
                wbSource.SaveJpeg(memoryStreamSource, bitmap.PixelWidth, bitmap.PixelHeight, 0, compression);
                byte[] imageBytes = memoryStreamSource.ToArray();
                memoryStreamSource.Seek(0, SeekOrigin.Begin);

                while (memoryStreamSource.Length > 524288) // .5MB
                {
                    compression -= 10;
                    WriteableBitmap wbTarget = new WriteableBitmap(bitmap);
                    memoryStreamSource.Seek(0, SeekOrigin.Begin);
                    wbTarget.SaveJpeg(memoryStreamSource, bitmap.PixelWidth, bitmap.PixelHeight, 0, compression);
                    memoryStreamSource.SetLength(memoryStreamSource.Position);
                    memoryStreamSource.Seek(0, SeekOrigin.Begin);
                    bitmap.SetSource(memoryStreamSource);
                    if (compression < 10)
                    {
                        break;
                    }
                }
                value++;
                string ImageName = "Image" + value + ".jpg";
                SaveImage(bitmap, ImageName);
            }
            catch
            {

            }
        }


        /// <summary>
        /// saves the Image into the media library
        /// </summary>
        /// <param name="ImageToSave">BitmapImage</param>
        /// <param name="ImageName">ImageName</param>
        public static void SaveImage(BitmapImage ImageToSave, string ImageName)
        {
            try
            {
                var writeableBitmap = new WriteableBitmap(ImageToSave);
                var memoryStream = new MemoryStream();
                writeableBitmap.SaveJpeg(memoryStream, (int)ImageToSave.PixelWidth, (int)ImageToSave.PixelHeight, 0, 100);
                memoryStream.Seek(0, SeekOrigin.Begin);
                MediaLibrary mediaLibrary = new MediaLibrary();
                mediaLibrary.SavePicture(ImageName, memoryStream);
                MessageBox.Show("Saved in your media library!", "Done", MessageBoxButton.OK);
            }
            catch
            {
                MessageBox.Show("There was an error.", "Cannot save", MessageBoxButton.OK);
            }
        }
    }
}
