using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ILoveNotes.Common
{
    /// <summary>
    /// Implements generic Appbar actions shared with all pages.
    /// </summary>
    public class NotesBasePage : LayoutAwarePage
    {
        public NotesBasePage()
            : base()
        {
        }

        #region Appbar

        /// <summary>
        /// Create new Note, if there is no notebooks available the show error massage.
        /// Create new Note from GroupedItemsPage (no selected notebook) then create note under the first Notebook.
        /// Create new Note from GroupDetailPage then create note under the notebook presented in GroupDetailPage.
        /// Create new Note from ItemDetailPage then create note under the current Note Notebook.
        /// </summary>
        /// <param name="item">New Note To Create</param>
        public void CreateNew(NoteDataCommon item)
        {
            if (NotesDataSource.GetGroups().Count == 0)
            {
                Helpers.ShowMessageAsync("You first need to create a notebook.", "Create Notebook");
                return;
            }

            if (this.GetType() == typeof(GroupDetailPage))
            {
                item.NoteBook = this.DefaultViewModel["Group"] as NoteBook;
                NotesDataSource.Add(item, item.NoteBookUniqueId);
            }
            else if (this.GetType() == typeof(ItemDetailPage))
            {
                item.NoteBook = (this.DefaultViewModel["Item"] as NoteDataCommon).NoteBook;
                NotesDataSource.Add(item, item.NoteBookUniqueId);
            }
            else
            {
                item.NoteBook = NotesDataSource.GetGroups()[0] as NoteBook;
                NotesDataSource.Add(item, item.NoteBookUniqueId);
            }

            this.Frame.Navigate(typeof(ItemDetailPage), item.UniqueId);
        }

        //Lab 04 ,Exercise 1 , task 2.2 :	review the AppBarButtonAddItemsClick method. 
        /// <summary>
        /// Show PopupMenu for new Notes/Notebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void AppBarButtonAddItemsClick(object sender, RoutedEventArgs e)
        {
            PopupMenu menu = new PopupMenu();
            menu.Commands.Add(new UICommand("Food", new UICommandInvokedHandler((cmd) => CreateNew(new FoodDataItem()))));
            menu.Commands.Add(new UICommand("To Do", new UICommandInvokedHandler((cmd) => CreateNew(new ToDoDataItem()))));
            menu.Commands.Add(new UICommand("Note", new UICommandInvokedHandler((cmd) => CreateNew(new NoteDataItem()))));
            menu.Commands.Add(new UICommandSeparator());
            menu.Commands.Add(new UICommand("Notebook", CreateNewNotebook));
            var command = await menu.ShowForSelectionAsync(Helpers.GetElementRect((FrameworkElement)sender));
            if (command == null) return;
        }


        /// <summary>
        /// user choose to create new Notebook, search for txtNbName and apply focus.
        /// </summary>
        /// <param name="sender"></param>
        public void CreateNewNotebook(object sender)
        {
            var addNbPop = this.FindName("popupAddNoteBook") as Popup;
            var appBar = this.FindName("PageAppBar") as AppBar;
            var nbText = this.FindName("txtNbName") as TextBox;

            addNbPop.VerticalOffset = this.ActualHeight - 295;
            addNbPop.HorizontalOffset = this.ActualWidth - 305;
            appBar.IsOpen = addNbPop.IsOpen = true;
            nbText.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        /// <summary>
        /// Create new Notebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnAddNBClick(object sender, RoutedEventArgs e)
        {
            var addNbPop = this.FindName("popupAddNoteBook") as Popup;
            var nbText = this.FindName("txtNbName") as TextBox;

            if (string.IsNullOrEmpty(nbText.Text))
            {
                Helpers.ShowMessageAsync("Notebook Title can't be empty", "Create Notebook");
                return;
            }

            //Creates new Notebook with user text
            var nb = new NoteBook(nbText.Text);

            //Adding notebook to main collection
            NotesDataSource.AddNoteBook(nb);

            //Saving new notebook.
            // Lab #4 - Files
            addNbPop.IsOpen = false;
            this.Frame.Navigate(typeof(GroupDetailPage), nb.UniqueId);
        }

        public void txtNbNameKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter) return;
            btnAddNBClick(sender, e);
        }

        public void btnCancelNb(object sender, RoutedEventArgs e)
        {
            var addNbPop = this.FindName("popupAddNoteBook") as Popup;
            var nbText = this.FindName("txtNbName") as TextBox;

            nbText.Text = string.Empty;
            addNbPop.IsOpen = false;
        }
        #endregion
    }
}
