using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ILoveNotes.Common
{
    /// <summary>
    /// ItemDetailPage shows all Note types, each note has a different UI based on his type.
    /// NoteTypeToTemplateConverter will return DataTemplate based on the Note Type.
    /// </summary>
    public class NoteTypeToTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var note = (NoteDataCommon)value;
            switch (note.Type)
            {
                case DataModel.NoteTypes.Food:
                    return parameter == null ? (DataTemplate)Application.Current.Resources["FoodPageTemplate"] : (DataTemplate)Application.Current.Resources["FoodPageSnappedTemplate"];
                case DataModel.NoteTypes.ToDo:
                    return (DataTemplate)Application.Current.Resources["ToDoPageTemplate"];
                default:
                    return parameter == null ? (DataTemplate)Application.Current.Resources["NotePageTemplate"] : (DataTemplate)Application.Current.Resources["NotePageSnappedTemplate"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
