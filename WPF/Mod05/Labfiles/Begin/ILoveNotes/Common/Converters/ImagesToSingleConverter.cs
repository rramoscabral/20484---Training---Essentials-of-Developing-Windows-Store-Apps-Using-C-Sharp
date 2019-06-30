using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace ILoveNotes.Common
{
    /// <summary>
    /// This converter is using to choose a random image from a List of Notes or from specific note images.
    /// </summary>
    public class ImagesToSingleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;

            string selectedImage = null;
            if (value.GetType() == typeof(ObservableCollection<NoteDataCommon>))
            {
                var items = (ObservableCollection<NoteDataCommon>)value;
                var images = items.SelectMany(i => i.Images).Where(list => list != null && list.Length > 0).ToList();
                selectedImage = Helpers.GetRandomImage(images);
            }
            else
            {
                var images = (ObservableCollection<string>)value;
                selectedImage = Helpers.GetRandomImage(images.ToList());
            }
            
            if (parameter != null && parameter.ToString().Equals("Image") && selectedImage == null)
                selectedImage = "Assets/Tiles/notebook.png";

            if (selectedImage == null) return null;

            var img = new BitmapImage(new Uri(selectedImage.ToBaseUrl()));
            img.DecodePixelHeight = 250;
            img.DecodePixelWidth = 250;

            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
