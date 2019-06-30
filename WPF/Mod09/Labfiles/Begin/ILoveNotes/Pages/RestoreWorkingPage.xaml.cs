using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using ILoveNotes.Common;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using Windows.ApplicationModel;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ILoveNotes.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class RestoreWorkingPage : ILoveNotes.Common.LayoutAwarePage
    {
        public RestoreWorkingPage()
        {
            this.InitializeComponent();
        }

        private void ShowProgress()
        {
            progressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
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
            if (navigationParameter != null)
            {
                InitializeFirstTimeData();
            }
            else
            {
                NotesDataSource.Unload();
            }
        }

        /// <summary>
        /// Check for GettingStarted.zip file located under Data folder for first use of the application.
        /// If file exists this means this is first use of the application, extract the files and then
        /// delete it. 
        /// </summary>
        private async void InitializeFirstTimeData()
        {
            try
            {
                var zipFile = await Package.Current.InstalledLocation.GetFileAsync("Data\\GettingStarted.zip");
                txtUploadStatus.Text = string.Format("Initialize 'I Love Notes' Getting Started...");
                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;

                var zipFiles = await OpenArchive(zipFile);
                foreach (var zip in zipFiles)
                {
                    var files = await OpenArchive(zip);
                    var destination = zip.Name.StartsWith(FolderNames.Images.ToString()) ? FolderNames.Images.ToString() : FolderNames.Notebooks.ToString();
                    var folder = await storage.CreateFolderAsync(destination, CreationCollisionOption.OpenIfExists);

                    foreach (var file in files)
                    {
                        var targetFile = await folder.CreateFileAsync(file.Name, CreationCollisionOption.OpenIfExists);
                        await file.CopyAndReplaceAsync(targetFile);
                    }
                }
                //await zipFile.DeleteAsync();
            }
            catch (Exception)
            {
                //Already performed....
            }

            await DataManager.LoadAsync();
            var rootFrame = new Frame();
            rootFrame = new Frame();
            rootFrame.Navigate(typeof(GroupedItemsPage));
            Window.Current.Content = rootFrame;
        }

        /// <summary>
        /// Using External library to open a Zip file and extract files
        /// </summary>
        /// <param name="mainFile">Compressed Zip File</param>
        /// <returns>List of extracted StorageFiles</returns>
        private async Task<IEnumerable<StorageFile>> OpenArchive(StorageFile mainFile)
        {
            return await CompressionManager.Zip.OpenArchive(mainFile);
        }

    }
}
