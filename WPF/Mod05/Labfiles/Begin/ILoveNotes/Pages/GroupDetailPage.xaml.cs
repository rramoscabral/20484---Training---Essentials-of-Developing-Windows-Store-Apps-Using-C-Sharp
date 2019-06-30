using ILoveNotes.Data;

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
using System.Collections.ObjectModel;
using ILoveNotes.DataModel;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace ILoveNotes
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class GroupDetailPage : ILoveNotes.Common.NotesBasePage
    {
        private NoteBook notebook;
        private PopupMenu navigationMenu;
        public GroupDetailPage()
        {
            this.InitializeComponent();
        }

        void EditTitlepopup_Closed(object sender, object e)
        {
            if (string.IsNullOrEmpty(txtNotebookTitle.Text))
                notebook.Title = Consts.DefaultTitleText;
            else
            {
                notebook.Title = txtNotebookTitle.Text;
                notebook.DateModified = DateTime.Now;
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            pageState["ID"] = notebook.UniqueId;
            notebook.PropertyChanged -= notebook_PropertyChanged;
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
            if (pageState != null && pageState.ContainsKey("ID"))
            {
                navigationParameter = pageState["ID"];
            }

            notebook = NotesDataSource.GetNotebook((String)navigationParameter) as NoteBook;
            this.DefaultViewModel["Sections"] = notebook.Sections;
            this.DefaultViewModel["Group"] = notebook;
            notebook.PropertyChanged -= notebook_PropertyChanged;
            notebook.PropertyChanged += notebook_PropertyChanged;
        }

        void notebook_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateNoteBook();
        }

        /// <summary>
        /// Save and Update Notebook Tile (if exists)
        /// </summary>
        void UpdateNoteBook()
        {
            //Lab 05 : Exercise 1 , Task 2.3 : Invoke the Save method of the NotesDataSource
            
        }

        /// <summary>
        /// Navigating from Popup message to another Notebook.
        /// </summary>
        /// <param name="cmd"></param>
        void Navigate(object cmd)
        {
            var command = (UICommand)cmd;
            if (command.Id == null)
                this.Frame.Navigate(typeof(GroupedItemsPage));
            else
                this.Frame.Navigate(typeof(GroupDetailPage), command.Id.ToString());
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (NoteDataCommon)e.ClickedItem;
            this.Frame.Navigate(typeof(ItemDetailPage), item.UniqueId);
        }

        /// <summary>
        /// Build navigation popup menu when clicking on Notebook title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (navigationMenu != null) return;

            var rect = new Rect(170, 110, 0, 0);
            navigationMenu = new PopupMenu();
            var navigationNotebooks = NotesDataSource.GetGroups();
            var count = navigationNotebooks.Count > 4 ? 4 : navigationNotebooks.Count;
            for (var i = 0; i < count; i++)
            {
                navigationMenu.Commands.Add(new UICommand(navigationNotebooks[i].Title, Navigate, navigationNotebooks[i].UniqueId));
            }
            navigationMenu.Commands.Add(new UICommandSeparator());
            navigationMenu.Commands.Add(new UICommand("Home", Navigate, null));

            await navigationMenu.ShowForSelectionAsync(rect);
            navigationMenu = null;
        }

        /// <summary>
        /// Open Notebook title editor 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditTitlepopup.VerticalOffset = this.ActualHeight - 295;
            EditTitlepopup.HorizontalOffset = this.ActualWidth - EditTitleGrid.Width;
            EditTitlepopup.IsOpen = true;
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            EditTitlepopup.IsOpen = false;
        }

        private new void txtNbNameKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter) return;
            btnClose_Click(sender, e);
            e.Handled = true;
        }

        /// <summary>
        /// Delete Notebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = await Helpers.ShowDialog("Are you sure you want to delete this NoteBook?", "Delete NoteBook");
            if (result)
            {
                await NotesDataSource.DeleteAsync(notebook);
                var frame = new Frame();
                frame.Navigate(typeof(GroupedItemsPage));
                Window.Current.Content = frame;
            }
        }

        /// <summary>
        /// Sort Notebooks items menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnSoft_Click(object sender, RoutedEventArgs e)
        {
            PopupMenu menu = new PopupMenu();
            menu.Commands.Add(new UICommand("Created Date", Soft, 1));
            menu.Commands.Add(new UICommand("Tag", Soft, 2));
            menu.Commands.Add(new UICommand("Normal", Soft, 3));
            var command = await menu.ShowForSelectionAsync(Helpers.GetElementRect((FrameworkElement)sender));
            if (command == null) return;
        }

        /// <summary>
        /// Sort Notebook Items by type
        /// </summary>
        /// <param name="command"></param>
        void Soft(object command)
        {
            var id = ((UICommand)command).Id.ToString();
            var softSections = new ObservableCollection<Section>();
            switch (id)
            {
                case "1":
                    var today = new Section("Today");
                    var todayItems = notebook.Items.Where(i => i.CreatedDate == DateTime.Now.FormatedDate()).ToList();
                    today.Items = todayItems.ToObservable();

                    var yesterday = new Section("Yesterday");
                    var yesterdayItems = notebook.Items.Where(i => i.CreatedDate == DateTime.Now.AddDays(-1).FormatedDate()).ToList();
                    yesterday.Items = yesterdayItems.ToObservable();

                    var older = new Section("Older");
                    var olderItems = notebook.Items.Where(i => Convert.ToDateTime(i.CreatedDate) < DateTime.Now.AddDays(-1)).ToList();
                    older.Items = olderItems.ToObservable();

                    softSections.Add(today);
                    softSections.Add(yesterday);
                    softSections.Add(older);

                    this.DefaultViewModel["Sections"] = softSections;
                    break;
                case "2":
                    var tags = notebook.Items.SelectMany(item => item.Tags).Distinct();
                    if (tags.Count() == 0)
                    {
                        Helpers.ShowMessageAsync("No tags were found, choose another filter", "Soft Notebook"); ;
                        return;
                    }
                    foreach (var tag in tags)
                    {
                        var section = new Section(tag);
                        var tagItems = notebook.Items.Where(i => i.Tags.Contains(tag)).ToList();
                        section.Items = tagItems.ToObservable();
                        softSections.Add(section);
                    }
                    this.DefaultViewModel["Sections"] = softSections;
                    break;
                default:
                    this.DefaultViewModel["Sections"] = notebook.Sections;
                    break;
            }

            this.UpdateLayout();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
