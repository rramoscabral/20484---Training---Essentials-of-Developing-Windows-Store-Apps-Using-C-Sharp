using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ILoveNotes.Common
{
    /// <summary>
    /// GroupedItemPage and GroupDetailPage shows Note tiles, each note has a different UI based on his type.
    /// TileTemplateSelector will return DataTemplate based on the Note Type.
    /// </summary>
    public class TileTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            if (item == null) return base.SelectTemplateCore(item, container);

            var note = item as NoteDataCommon;

            if (note.Type == DataModel.NoteTypes.ToDo && note.ToDo.Count > 0)
            {
                return Application.Current.Resources["TileToDoTemplate"] as DataTemplate;
            }
            else
            {
                if (CanShowImage(note))
                {
                    return Application.Current.Resources["TileImageTemplate"] as DataTemplate;
                }
                else
                {
                    return Application.Current.Resources["TileTextTemplate"] as DataTemplate;
                }
            }
        }

        private bool CanShowImage(NoteDataCommon item)
        {
            if (item.Images != null && item.Images.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
