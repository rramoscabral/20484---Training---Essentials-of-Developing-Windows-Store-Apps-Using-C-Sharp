using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ILoveNotes.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ILoveNotes.Controls
{
    public sealed partial class FileOpenPickerPage : UserControl
    {
        private FileOpenPickerUI fileOpenPickerUI = null;

        public FileOpenPickerPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Load all notebooks and for each note load all images under one collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var nbs = NotesDataSource.GetGroups();
            var images = nbs.SelectMany(n => ((NoteBook)n).Items).SelectMany(i => i.Images).ToList();
            imagesGrid.ItemsSource = images;
        }

        public void Activate(FileOpenPickerActivatedEventArgs args)
        {
            this.fileOpenPickerUI = args.FileOpenPickerUI;
            this.fileOpenPickerUI.FileRemoved += fileOpenPickerUI_FileRemoved;
            Window.Current.Content = this;
            Window.Current.Activate();
        }

        void fileOpenPickerUI_FileRemoved(FileOpenPickerUI sender, FileRemovedEventArgs args)
        {
            this.fileOpenPickerUI.RemoveFile(args.Id);
        }

        /// <summary>
        /// When the user select a file we locate this file under Images folder using DataManager.FindFIlesAync and add those files to FileOpenPicker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void imagesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imagesGrid.SelectedItems.Count == 0) return;

            var selectedItems = imagesGrid.SelectedItems.Cast<string>().ToList().ToObservable();
            var files = await DataManager.FindFilesAsync(selectedItems, FolderNames.Images);

            foreach (var file in files)
            {
                if (this.fileOpenPickerUI.ContainsFile(file.Name)) continue;
                this.fileOpenPickerUI.AddFile(file.Name, file);
            }
        }
    }
}
