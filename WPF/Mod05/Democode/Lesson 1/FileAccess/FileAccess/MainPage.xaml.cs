using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FileAccess
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

        private async void CreateFile(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync("file.txt", CreationCollisionOption.ReplaceExisting);

            result.Text = "file.txt created in local storage.";
        }

        private async void DeleteFile(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                var file = await localFolder.GetFileAsync("file.txt");
                await file.DeleteAsync();

                result.Text = "file.txt deleted from local storage.";
            }
            catch
            {
                result.Text = "file.txt does not exist.";
            }
        }

        private async void WriteFile(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.KnownFolders.PicturesLibrary;
            var file = await localFolder.CreateFileAsync("file.txt", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(file, "Writing to files is easy!");

            result.Text = "file.txt written to PicturesLibrary folder.";
        }

        private async void ReadFile(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.KnownFolders.PicturesLibrary;
            var file = await localFolder.CreateFileAsync("file.txt", CreationCollisionOption.OpenIfExists);

            string fileContents = await FileIO.ReadTextAsync(file);
            result.Text = "file.txt: " + fileContents;
        }

        private async void WriteStream(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;
            var file = await localFolder.CreateFileAsync("file.txt", CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter writer = new DataWriter(stream))
                {
                    writer.WriteString("Writing to Streams, all day long!");
                    await writer.StoreAsync(); // Write the buffered data to the stream.
                }
            }

            result.Text = "file.txt written to temporary folder.";
        }

        private async void ReadStream(object sender, RoutedEventArgs e)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;
            var file = await localFolder.CreateFileAsync("file.txt", CreationCollisionOption.OpenIfExists);

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                stream.Seek(10); // Seek to position 10 within the stream
                using (DataReader reader = new DataReader(stream))
                {
                    uint length = await reader.LoadAsync(1024);
                    var fileContents = reader.ReadString(length);

                    result.Text = "file.txt: " + fileContents;
                }
            }
        }
    }
}
