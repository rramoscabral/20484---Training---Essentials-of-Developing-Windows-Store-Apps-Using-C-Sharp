using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FilePickers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void PickSingleFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            openPicker.FileTypeFilter.Add("*");  // Show all files.

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                // Save file in future access list, so the app will have permission to access it in the future.
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFile", file);

                result.Text = "Picked file: " + file.Name;
            }
            else
                result.Text = "You did not pick a file.";

        }

        private async void AccessPickedFile(object sender, RoutedEventArgs e)
        {
            if (!StorageApplicationPermissions.FutureAccessList.ContainsItem("PickedFile"))
            {
                result.Text = "Future access list does not contain the picked file.";
                return;
            }

            var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync("PickedFile");
            var properties = await file.GetBasicPropertiesAsync();
            result.Text = string.Format("Picked file: {0}, Size: {1}", file.Name, properties.Size);
        }

        private async void PickFolder(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)

                result.Text = "Picked folder: " + folder.Name;
            else
                result.Text = "You did not pick a folder.";
        }

        private async void SaveFile(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = "file";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, "You're saved!");
                result.Text = "Saved: " + file.Name;
            }
            else
                result.Text = "You did not save anything.";
        }
    }
}
