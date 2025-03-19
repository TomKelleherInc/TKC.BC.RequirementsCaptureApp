using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Senda.Requirements.Capture.UI.Converters
{
    public sealed class FileTypeToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string bitmapPath = string.Empty;
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            // "pack://application:,,,/Senda.Requirements.Capture.UI;component/Content/Pdf.bmp
            string fileExtension = value.ToString().Split('.').Last();
            switch (fileExtension.ToLower())
            {
                case "pdf":
                    bitmapPath = $"pack://application:,,,/{assemblyName};component/Content/Pdf.bmp";
                    break;

                case "docx":
                case "doc":
                    bitmapPath = $"pack://application:,,,/{assemblyName};component/Content/Word.bmp";
                    break;

                case "xls":
                case "xlsx":
                    bitmapPath = $"pack://application:,,,/{assemblyName};component/Content/Excel.bmp";
                    break;

                default:
                    bitmapPath = $"pack://application:,,,/{assemblyName};component/Content/unknownFile.bmp";
                    break;


            }
            try
            {
                //Uri myUri = new Uri(bitmapPath, UriKind.RelativeOrAbsolute);
                //BmpBitmapDecoder decoder2 = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                //BitmapSource bitmapSource2 = decoder2.Frames[0];
                //return bitmapSource2;

                return new BitmapImage(new Uri(bitmapPath));

            }
            catch(Exception ex)
            {
                var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                return null;
            }
        }


        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
