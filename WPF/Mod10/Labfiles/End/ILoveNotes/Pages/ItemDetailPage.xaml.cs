using ILoveNotes.Data;
using ILoveNotes.Controls;
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
using Windows.Media.Capture;
using Windows.Storage;
using ILoveNotes.Common;
using ILoveNotes.DataModel;
using Windows.UI.Popups;
using ILoveNotes.Pages;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Automation;
using Windows.Management.Deployment;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.StartScreen;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace ILoveNotes
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : ILoveNotes.Common.NotesBasePage
    {
        public NoteDataCommon Note { get; set; }
        private PrinterManager _printer;
        private ToDoControl todoControl;
        private TextBox txtDescription;
        private TextBox txtAddress;

        public ItemDetailPage()
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
            if (pageState != null && pageState.ContainsKey("ID"))
            {
                navigationParameter = pageState["ID"];
            }

            Note = NotesDataSource.GetItem((string)navigationParameter) as NoteDataCommon;
            if (Note == null)
                this.Frame.Navigate(typeof(GroupedItemsPage));

            Note.PropertyChanged -= CurrentNotePropertyChanged;

            if (string.IsNullOrEmpty(Note.Title))
                Note.Title = Consts.DefaultTitleText;
            if (string.IsNullOrEmpty(Note.Description))
                Note.Description = Consts.DefaultDescriptionText;
            if (string.IsNullOrEmpty(Note.Address))
                Note.Address = Consts.DefaultAddressText;

            Note.PropertyChanged += CurrentNotePropertyChanged;

            this.DefaultViewModel["Item"] = Note;
            _printer = new PrinterManager(this, Note);

            // LAB #9 TILES
            pageTitle.Focus(FocusState.Keyboard);
            DrawDefaultAppBar();
        }

        /// <summary>
        /// Description and Address textboxs are part of the DataTemplate, in order to register to GotFocus and LostFocus events we need to find those controls first.
        /// GotFocus and LostFocus events will allow us to display Default Text on those fields in case the user didn't enter any value.
        /// </summary>
        private void RegisterTextBoxs()
        {
            txtDescription = Helpers.FindVisualChild(gridLayoutPanel, "txtDescription") as TextBox;
            if (txtDescription != null)
            {
                txtDescription.GotFocus += txtDescription_GotFocus;
                txtDescription.LostFocus += txtDescription_LostFocus;
            }

            txtAddress = Helpers.FindVisualChild(gridLayoutPanel, "txtAddress") as TextBox;
            if (txtAddress != null)
            {
                txtAddress.GotFocus += txtAddress_GotFocus;
                txtAddress.LostFocus += txtAddress_LostFocus;
            }
        }

        /// <summary>
        /// txtAddress textbox field lost focus, checking if the field in empty and if so define default text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddress.Text))
                txtAddress.Text = Consts.DefaultAddressText;
        }

        /// <summary>
        /// txtAddress textbox field got focus, checking if the field value is equal to default text then clear txtAddress value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddress.Text.Equals(Consts.DefaultAddressText))
                txtAddress.Text = string.Empty;
        }

        /// <summary>
        /// txtDescription textbox field got focus, checking if the field value is equal to default text then clear txtDescription value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescription.Text))
                txtDescription.Text = Consts.DefaultDescriptionText;
        }

        /// <summary>
        /// txtDescription textbox field lost focus, checking if the field in empty and if so define default text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtDescription.Text.Equals(Consts.DefaultDescriptionText))
                txtDescription.Text = string.Empty;
        }

        void CurrentNotePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateNote();
        }

        /// <summary>
        /// Update Note Modified Date, update note tile, application tile and save the notebook.
        /// </summary>
        void UpdateNote()
        {
            Note.NoteBook.DateModified = DateTime.Now;
            //Lab 10: Exercise 1, Task 1.5 : Invoke the Tile Update for the main tile when a note changes.
            TileManager.SetMainTile(Note);

            //Lab 10: Exercise 2, Task 2.5: Invoke the Tile Update for the secondary tile when a note changes.
            TileManager.UpdateSecondaryTile(Note.UniqueId);
            TileManager.UpdateSecondaryTile(Note.NoteBookUniqueId);
            DataManager.Save(Note.NoteBook);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            pageState["ID"] = Note.UniqueId;
            Note.PropertyChanged -= CurrentNotePropertyChanged;
            _printer.UnregisterForPrinting();
        }

        #region AppBar

        /// <summary>
        /// Only for ToDo Note Item we need to display additional AppBar button.
        /// </summary>
        void DrawDefaultAppBar()
        {
            if (Note.Type == NoteTypes.ToDo)
            {
                btnAddToDo.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// User clicked on "Move" appbar button, this method will build and display a popup menu with list of Notebooks.
        /// Allowing the user target notebook.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void btnMoveNotebookClick(object sender, RoutedEventArgs e)
        {
            PopupMenu nbMenu = new PopupMenu();
            var nbs = NotesDataSource.GetGroups();
            foreach (NoteBook nb in nbs)
            {
                if (!nb.UniqueId.Equals(Note.NoteBookUniqueId))
                    nbMenu.Commands.Add(new UICommand(nb.Title, null, nb.UniqueId));
            }

            var rect = Helpers.GetElementRect((FrameworkElement)sender);
            IUICommand command = await nbMenu.ShowForSelectionAsync(rect);
            if (command == null) return;


            var result = await Helpers.ShowDialog(string.Format(Consts.MoveNote, Note.NoteBook.Title, command.Label), "Move Note");
            var nbUniqueId = (string)command.Id;
            if (result)
            {
                NotesDataSource.Move(Note.UniqueId, nbUniqueId);
                DrawDefaultAppBar();
            }
        }


        private void pageTitleKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter) return;
            gridScrollViewer.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        void btnToggleReminderClick(object sender, RoutedEventArgs e)
        {
            todoControl.SetSelectedReminder(Note.Title);
        }

        void btnToggleDoneClick(object sender, RoutedEventArgs e)
        {
            todoControl.SetDoneSelected();
        }

        /// <summary>
        /// Dymanic Delete popup menu display, if there is ToDo Item selected then display "Delete ToDo option"
        /// If Images list has selected item then display "Delete Image" option.
        /// Finally display "Delete Note" option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            PopupMenu deleteMenu = new PopupMenu();
            if (Note.Type == NoteTypes.ToDo && todoControl.HasSelectedItems)
            {
                deleteMenu.Commands.Add(new UICommand("Delete ToDo", DeleteTag));
            }
            if (imgList.SelectedItems.Count > 0)
            {
                deleteMenu.Commands.Add(new UICommand("Delete Image", DeleteImageAsync));
            }
            if (deleteMenu.Commands.Count > 0)
            {
                deleteMenu.Commands.Add(new UICommandSeparator());
                deleteMenu.Commands.Add(new UICommand("Delete Note", DeleteAsync));
                var rect = Helpers.GetElementRect((FrameworkElement)sender);
                IUICommand Command = await deleteMenu.ShowForSelectionAsync(rect);
                if (Command == null) return;
            }
            else
            {
                DeleteAsync(sender);
            }
        }

        /// <summary>
        /// Display add attachment popupmenu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void btnAddAttachClick(object sender, RoutedEventArgs e)
        {
            PopupMenu ImageMenu = new PopupMenu();
            ImageMenu.Commands.Add(new UICommand("Pick a picture", null, 1));
            ImageMenu.Commands.Add(new UICommand("Take a photo", null, 2));

            var rect = Helpers.GetElementRect((FrameworkElement)sender);
            IUICommand Command = await ImageMenu.ShowForSelectionAsync(rect);
            if (Command == null) return;

            var CommandId = (int)Command.Id;
            switch (CommandId)
            {
                case 1:
                    await MediaManager.OpenFilePicker(Note);
                        UpdateNote();
                    break;
                case 2:
                    await MediaManager.ShowCamera(Note);
                    UpdateNote();
                    break;
            }
        }
        // LAB #9 TILES

        #endregion

        /// <summary>
        /// Show confirmation message for deleting Note, if the user choose Yes then delete note and return to GroupDetailPage based on deleted note Notebook.
        /// </summary>
        /// <param name="command"></param>
        async void DeleteAsync(object command)
        {
            var result = await Helpers.ShowDialog("Are you sure you want to delete this Note?", "Delete Note");
            if (result)
            {
                _printer.UnregisterForPrinting();
                await NotesDataSource.DeleteAsync(Note);
                    var frame = new Frame();
                frame.Navigate(typeof(GroupedItemsPage));
                frame.Navigate(typeof(GroupDetailPage), Note.NoteBook.UniqueId);
                Window.Current.Content = frame;
            }
        }

        /// <summary>
        /// Show confirmation message for deleting Image, if user choose Yes then remove Image path from Note Images collection.
        /// </summary>
        /// <param name="command"></param>
        async void DeleteImageAsync(object command)
        {
            if (imgList.SelectedItems.Count == 0) return;
            var result = await Helpers.ShowDialog(string.Format("Deleted image can't be recoverd, Are you sure you want to delete?"), "Delete Image");
            if (!result) return;
            var filePath = imgList.SelectedItem.ToString();
            Note.Images.Remove(filePath);
            if (Note.Images.Count == 0)
                Note.Images = new System.Collections.ObjectModel.ObservableCollection<string>();

            UpdateNote();
        }

        void DeleteTag(object sender)
        {
            todoControl.RemoveSelectedItems();
            UpdateNote();
        }

        private void imgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imgList.SelectedItems.Count == 0)
            {
                PageAppBar.IsOpen = false;
            }
            else
            {
                PageAppBar.IsOpen = true;
            }
        }

        private void btnAddNewToDoClick(object sender, RoutedEventArgs e)
        {
            todoControl.AddToDo();
        }

        private void pageTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pageTitle.Text.Equals(Consts.DefaultTitleText))
                pageTitle.Text = string.Empty;
        }

        private void pageTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pageTitle.Text))
                pageTitle.Text = Consts.DefaultTitleText;
        }

        #region Tags

        /// <summary>
        /// Show Add Tag Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenTagsPopupClick(object sender, RoutedEventArgs e)
        {
            var rect = Helpers.GetElementRect((FrameworkElement)sender);

            TagsPopup.VerticalOffset = rect.Bottom - 555;
            TagsPopup.HorizontalOffset = rect.Left;
            TagsPopup.IsOpen = true;
            listSuggestions.ItemsSource = BuildTagList();
            txtTagTitle.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            txtTagTitle.Text = string.Empty;
            TagsPopup.IsOpen = false;
        }

        private void btnTagAddClick(object sender, RoutedEventArgs e)
        {
            SaveTags();
            listSuggestions.ItemsSource = BuildTagList();
        }

        private void btnTagSaveClick(object sender, RoutedEventArgs e)
        {
            TagsPopup.IsOpen = false;
        }

        /// <summary>
        /// Foreach selected Tag from Add Tag window we add the selected tags to Note Tags collection.
        /// </summary>
        void SaveTags()
        {
            var list = listSuggestions.SelectedItems.ToList();
            Note.Tags.Clear();
            if (list.Count > 0)
            {
                foreach (ListViewItem tag in list)
                {
                    var tagVal = tag.Content.ToString();
                    Note.Tags.Add(tagVal);
                }
            }

            if (!string.IsNullOrEmpty(txtTagTitle.Text) && !Note.Tags.Contains(txtTagTitle.Text))
                Note.Tags.Add(txtTagTitle.Text);

            txtTagTitle.Text = string.Empty;

            UpdateNote();
        }

        /// <summary>
        /// Gets all used tags from all notebooks.
        /// </summary>
        /// <returns>List of all Tags</returns>
        private List<ListViewItem> BuildTagList()
        {
            var allTags = NotesDataSource.GetGroups().SelectMany(nb => ((NoteBook)nb).Items).SelectMany(t => t.Tags).Distinct();

            List<ListViewItem> tags = new List<ListViewItem>();
            foreach (var tag in allTags)
            {
                var viewItem = new ListViewItem();
                viewItem.Content = tag;
                viewItem.IsSelected = true;
                tags.Add(viewItem);
            }
            return tags;
        }

        private void TagsPopup_Closed(object sender, object e)
        {
            SaveTags();
        }

        #endregion

        /// <summary>
        /// When page is loaded we check for two things:
        /// if note type is Place then register btnOpenMapEditor click event to open Bing Map Page and if the current Place Item has no Latitude and Longitude defined then call "CenterMapAsync" method.
        /// if note type is ToDo then obtain the ToDo control element using Helpers.FindVisualChild
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (Note.Type == NoteTypes.ToDo)
            {
                todoControl = Helpers.FindVisualChild<ToDoControl>(gridLayoutPanel);
            }

            RegisterTextBoxs();
        }

        /// <summary>
        /// Pin or Unpin Note 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        //Lab 10, Exercise 2, Task 2.2 : 2.	Implement the click event handler for the Pin button 
        private async void btnPinClick(object sender, RoutedEventArgs e)
        {
            PageAppBar.IsSticky = true;
            var result = await TileManager.Pin(sender, Note);
            PageAppBar.IsSticky = false;
            TileManager.UpdateSecondaryTile(Note.UniqueId);
        }
    }
}
