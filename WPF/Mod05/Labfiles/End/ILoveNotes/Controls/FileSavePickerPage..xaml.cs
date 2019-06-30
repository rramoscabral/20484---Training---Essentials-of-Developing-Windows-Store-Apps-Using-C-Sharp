using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILoveNotes.Common;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ILoveNotes.Controls
{
    public sealed partial class FileSavePickerPage : UserControl
    {
        private NoteDataCommon note = null;
        private FileSavePickerUI fileSavePickerUI = null;

        public FileSavePickerPage()
        {
            this.InitializeComponent();
        }

        public void Activate(FileSavePickerActivatedEventArgs args)
        {
            this.fileSavePickerUI = args.FileSavePickerUI;
            this.fileSavePickerUI.TargetFileRequested += fileSavePickerUI_TargetFileRequested;
            Window.Current.Content = this;
            Window.Current.Activate();
        }

        /// <summary>
        /// File Save Operation, create new file under Images folder and linked the new file to the selected note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void fileSavePickerUI_TargetFileRequested(FileSavePickerUI sender, TargetFileRequestedEventArgs args)
        {
            if (note == null)
            {
                Helpers.ShowMessageAsync("Please select at least one Note to save the image.", "Save Image");
                return;
            }

            var deferral = args.Request.GetDeferral();
            args.Request.TargetFile = await DataManager.CreateFileAsync(this.fileSavePickerUI.FileName, FolderNames.Images);
            note.Images.Add(string.Format("{0}/{1}", FolderNames.Images, args.Request.TargetFile.Name));
            DataManager.Save();
            // Complete the deferral to let the Picker know the request is finished
            deferral.Complete();
        }

        /// <summary>
        /// Set all notebooks as NotebooksGridView itemsSource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            /*if (!NotesDataSource.DataLoaded)
                await DataManager.LoadAsync();*/

            NotebooksGridView.ItemsSource = NotesDataSource.GetGroups();
        }

        /// <summary>
        /// When the user choose a notebook we change the ItemsSource instead of Notebooks to Notes of the selected Notebook.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotebooksGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var nb = e.ClickedItem as NoteBook;
            var view = new CollectionViewSource();
            view.ItemsPath = new PropertyPath("Items");
            view.Source = nb.Sections;
            view.IsSourceGrouped = true;

            var bind = new Binding();
            bind.Source = view;
            itemGridView.SetBinding(GridView.ItemsSourceProperty, bind);

            ShowItems();
        }

        void ShowItems()
        {
            NotebooksGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            itemGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            backButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void ShowGroups()
        {
            NotebooksGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            itemGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            backButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            ShowGroups();
        }

        private void itemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemGridView.SelectedItem == null)
            {
                note = null;
                return;
            }

            note = itemGridView.SelectedItem as NoteDataCommon;
        }
    }
}
