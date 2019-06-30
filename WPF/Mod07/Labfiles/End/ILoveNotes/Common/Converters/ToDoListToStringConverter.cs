using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.UI.Xaml.Data;

namespace ILoveNotes.Common
{
    /// <summary>
    /// GroupDetailPage and GroupedItemsPage shows ToDo tiles as list of first 7 todo items, this converter will pull the first 7 items from the ToDo.
    /// If the ToDo item has more than 7 items we will display a message "and {number} more..." at the end of the list.
    /// </summary>
    public class ToDoListToStringConverter : IValueConverter
    {
        private int maxItems = 7;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var item = value as NoteDataCommon;

            var customList = new ObservableCollection<ToDo>();
            var count = item.ToDo.Count > maxItems ? maxItems : item.ToDo.Count;
            for (var i = 0; i < count; i++)
            {
                customList.Add(item.ToDo[i]);
            }

            if (item.ToDo.Count > maxItems)
            {
                customList.Add(new ToDo(string.Format("and {0} more...", item.ToDo.Count - maxItems)));
            }
            return customList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
