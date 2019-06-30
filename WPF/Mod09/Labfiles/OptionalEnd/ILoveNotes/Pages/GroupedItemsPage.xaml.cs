﻿using ILoveNotes.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using ILoveNotes.Common;
using ILoveNotes.Pages;
using System.Threading;
using ILoveNotes.DataModel;
using Windows.UI.ApplicationSettings;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace ILoveNotes
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : ILoveNotes.Common.NotesBasePage
    {
        public GroupedItemsPage()
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
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = NotesDataSource.GetGroups();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
            this.groupGridView.ItemsSource = this.groupedItemsViewSource.View.CollectionGroups;

            if (sampleDataGroups.Count == 0)
                EmptyCollectionGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((NoteDataCommon)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var item = ((NoteDataCommon)e.ClickedItem);
            this.Frame.Navigate(typeof(ItemDetailPage), item.UniqueId);
        }

        private void AddNoteBookPopup(object sender, RoutedEventArgs e)
        {
            CreateNewNotebook(sender);
        }

        private void SearchBoxQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            Frame frame;

            if (Window.Current.Content == null)
                frame = new Frame();
            else
                frame = Window.Current.Content as Frame;

            if (args.QueryText.Contains(Consts.SearchSplitter))
            {
                var notebookName = args.QueryText.Substring(0, args.QueryText.IndexOf(Consts.SearchSplitter));
                var noteName = args.QueryText.Substring(args.QueryText.IndexOf(Consts.SearchSplitter) + Consts.SearchSplitter.Length);
                var notebook = NotesDataSource.SearchNotebook(notebookName);
                if (notebook == null) return;
                var note = NotesDataSource.SearchNote(notebook.UniqueId, noteName);
                if (note == null) return;

                frame.Navigate(typeof(ItemDetailPage), note.UniqueId);
            }
            else
            {
                frame.Navigate(typeof(SearchResults), args.QueryText);
            }
        }

        private void SearchBoxSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            var items = NotesDataSource.Search(args.QueryText);
            foreach (NoteDataCommon item in items)
                args.Request.SearchSuggestionCollection.AppendQuerySuggestion(string.Format("{0}{1}{2}", item.NoteBook.Title, Consts.SearchSplitter, item.Title));
        }
    }
}
