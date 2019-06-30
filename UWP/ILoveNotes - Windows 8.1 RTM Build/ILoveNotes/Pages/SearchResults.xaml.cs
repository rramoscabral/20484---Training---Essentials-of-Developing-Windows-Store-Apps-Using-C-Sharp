using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILoveNotes.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace ILoveNotes.Pages
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class SearchResults : ILoveNotes.Common.NotesBasePage
    {
        public SearchResults()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var query = (String)navigationParameter;
            var list = NotesDataSource.Search(query);
            this.pageTitle.Text = string.Format("Results for \"{0}\"", query);
            var nb = new NoteBook();

            foreach (NoteDataCommon note in list)
            {
                switch (note.Type)
                {
                    case DataModel.NoteTypes.Food:
                        nb.FoodSection.Add(note);
                        break;
                    case DataModel.NoteTypes.ToDo:
                        nb.ToDoSection.Add(note);
                        break;
                    default:
                        nb.NotesSection.Add(note);
                        break;
                }
            }

            this.DefaultViewModel["Groups"] = nb.Sections;
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (NoteDataCommon)e.ClickedItem;
            this.Frame.Navigate(typeof(ItemDetailPage), item.UniqueId);
        }
    }
}
