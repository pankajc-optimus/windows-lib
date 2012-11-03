/*---------------
 * Scanner Class
 *---------------
 * This library is used to get the barcode and its format.
 * It uses the 3rd party dll - ZXingwp7
 * 
 * It supports the following barcode formats: QR_Code, DataMatrix, PDF417, UPC_A, UPC_E, EAN_8, EAN_13, Code_39, Code_128, ITF
 * 
 * It contains the following:
 * Property:
 *      ImageSource - to get/set the source of image captured or selected from media library
 * Methods: 
 *      getBarcodeData() - Decodes the set image and returns it Barcoded Data
 *      getBarcodeFormat() - Decodes the set image and returns the Format of Barcode
 *      
 */
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
using zxingwp7;
using zxingwp7.common;
using zxingwp7.oned;
using zxingwp7.qrcode;
using zxingwp7.datamatrix;
using zxingwp7.pdf417;

namespace BarcodeScannerLib
{
    public class Scanner
    {
        private BitmapImage _ImageSource;
        MultiFormatOneDReader OneDreader = new MultiFormatOneDReader(null);
        QRCodeReader QRreader = new QRCodeReader();
        DataMatrixReader Matreader = new DataMatrixReader();
        PDF417Reader pdfReader = new PDF417Reader();

        /// <summary>
        /// Property for Image Source
        /// </summary>
        public BitmapImage ImageSource
        {
            get
            {
                return _ImageSource;
            }
            set
            {
                _ImageSource = value;
            }
        }

        /// <summary>
        /// To scan the Image and return the Barcode Data on it
        /// </summary>
        /// <returns>String</returns>
        public string getBarcodeData()
        {
            string barcode;
            
            WriteableBitmap wbmp = new WriteableBitmap(ImageSource);

            RGBLuminanceSource lum = new RGBLuminanceSource(wbmp, wbmp.PixelWidth, wbmp.PixelHeight);
            HybridBinarizer binarizer = new HybridBinarizer(lum);
            BinaryBitmap binBmp = new BinaryBitmap(binarizer);

            try
            {
                Result res = OneDreader.decode(binBmp);
                barcode = res.Text;
                return barcode;
            }
            catch { }
            try
            {
                Result res = QRreader.decode(binBmp);
                barcode = res.Text;
                return barcode;
            }
            catch { }

            try
            {
                Result res = Matreader.decode(binBmp);
                barcode = res.Text;
                return barcode;
            }
            catch { }
            try
            {
                Result res = pdfReader.decode(binBmp);
                barcode = res.Text;
                return barcode;
            }
            catch { }

            return barcode = "No Barcode Found"; //If no matching found
        }


        /// <summary>
        /// To scan the Image and return the Barcode format on it.
        /// </summary>
        /// <returns>String</returns>
        public string getBarcodeFormat()
        {
            string format;
            WriteableBitmap wbmp = new WriteableBitmap(ImageSource);

            RGBLuminanceSource lum = new RGBLuminanceSource(wbmp, wbmp.PixelWidth, wbmp.PixelHeight);
            HybridBinarizer binarizer = new HybridBinarizer(lum);
            BinaryBitmap binBmp = new BinaryBitmap(binarizer);

            try
            {
                Result res = OneDreader.decode(binBmp);
                format = res.BarcodeFormat.ToString();
                return format;
            }
            catch { }
            try
            {
                Result res = QRreader.decode(binBmp);
                format = res.BarcodeFormat.ToString();
                return format;
            }
            catch { }

            try
            {
                Result res = Matreader.decode(binBmp);
                format = res.BarcodeFormat.ToString();
                return format;
            }
            catch { }
            try
            {
                Result res = pdfReader.decode(binBmp);
                format = res.BarcodeFormat.ToString();
                return format;
            }
            catch { }

            return format = "No Format"; //If no matching found
        }
    }
}
