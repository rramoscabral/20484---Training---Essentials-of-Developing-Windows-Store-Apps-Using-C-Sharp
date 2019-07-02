using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ILoveNotes.Common
{
    public class ToDateTimeStringFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime && targetType == typeof(DateTimeOffset))
            {
                DateTime dt = (DateTime)value;
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                DateTimeOffset offset = dt;
                return offset;
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToDateTime(value.ToString());
        }
    }
}
