using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ILoveNotes.Common
{
    /// <summary>
    /// Using ToBaseUrl extension to convert images path to full.
    /// Assets/logo.png will be converterd to ms-appx:///Assets/logo.png
    /// </summary>
    public class FileToBaseUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var imgUri = string.IsNullOrEmpty(value.ToString()) ? null : value.ToString().ToBaseUrl();
            return imgUri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
