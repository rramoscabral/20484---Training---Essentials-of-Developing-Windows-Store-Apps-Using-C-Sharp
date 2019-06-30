using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.UI.Xaml.Data;

namespace ILoveNotes.Common
{
    /// <summary>
    /// For Tiles in GroupDetailPage and GroupedItemsPage choose the default background image based on Note Type.
    /// </summary>
    public class NoteTypeToBackgroundImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var note = value as NoteDataCommon;
            switch (note.Type)
            {
                case DataModel.NoteTypes.Food:
                    return "Assets/Tiles/food.png".ToBaseUrl();
                case DataModel.NoteTypes.ToDo:
                    return "Assets/Tiles/todo.png".ToBaseUrl();
                case DataModel.NoteTypes.Notebook:
                    return "Assets/Tiles/notebook.png".ToBaseUrl();
                default:
                    return "Assets/Tiles/note.png".ToBaseUrl();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
