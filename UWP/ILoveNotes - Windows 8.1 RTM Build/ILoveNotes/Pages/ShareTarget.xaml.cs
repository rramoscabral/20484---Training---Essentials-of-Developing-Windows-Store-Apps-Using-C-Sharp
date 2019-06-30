using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ILoveNotes.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ILoveNotes.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareTarget : ILoveNotes.Common.LayoutAwarePage
    {
        ShareOperation shareOperation = null;
        public ShareTarget()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// On ShareTarget activation we display the sahre source data
        /// Also we allow the user to choose what type of Note he would like to create and inside which notebook to created the new note.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ActivateAsync(ShareTargetActivatedEventArgs args)
        {
            var groups = NotesDataSource.GetGroups();
            if (groups.Count == 0)
            {
                Helpers.ShowMessageAsync("Please create notebook first.", "No Notebooks");
                Window.Current.Activate();
                return;
            }
            comboNotebooks.ItemsSource = groups;
            comboNotebooks.SelectedItem = comboNotebooks.Items[0];

            var validNotes = System.Enum.GetValues(typeof(NoteTypes)).Cast<NoteTypes>();
            comboType.ItemsSource = validNotes.Where(n => n != NoteTypes.Section && n != NoteTypes.Notebook);
            comboType.SelectedItem = comboType.Items[0];

            this.shareOperation = (args as ShareTargetActivatedEventArgs).ShareOperation;
            txtTitle.Text = this.shareOperation.Data.Properties.Title;
            txtDescription.Text = this.shareOperation.Data.Properties.Description;

            if (this.shareOperation.Data.Contains(StandardDataFormats.WebLink))
            {
                Uri uri = await this.shareOperation.Data.GetWebLinkAsync();
                if (uri != null)
                {
                    txtDescription.Text = "Uri: " + uri.AbsoluteUri + Environment.NewLine;
                }
            }
            if (this.shareOperation.Data.Contains(StandardDataFormats.Text))
            {
                string text = await this.shareOperation.Data.GetTextAsync();
                if (text != null)
                {
                    txtDescription.Text += "Text: " + text + Environment.NewLine;
                }
            }
            if (this.shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> storageItems = null;
                storageItems = await this.shareOperation.Data.GetStorageItemsAsync();
                string fileList = String.Empty;
                for (int index = 0; index < storageItems.Count; index++)
                {
                    fileList += storageItems[index].Name;
                    if (index < storageItems.Count - 1)
                    {
                        fileList += ", ";
                    }
                }
                txtDescription.Text += "StorageItems: " + fileList + Environment.NewLine;
            }
            if (this.shareOperation.Data.Contains(StandardDataFormats.Html))
            {
                string htmlFormat = await this.shareOperation.Data.GetHtmlFormatAsync();
                string htmlFragment = HtmlFormatHelper.GetStaticFragment(htmlFormat);
                txtDescription.Text += "Text: " + Windows.Data.Html.HtmlUtilities.ConvertToText(htmlFragment) + Environment.NewLine;
            }
            if (this.shareOperation.Data.Contains(StandardDataFormats.Bitmap))
            {
                img.Visibility = Visibility.Visible;
                IRandomAccessStreamReference imageReceived = await this.shareOperation.Data.GetBitmapAsync();
                var stream = await imageReceived.OpenReadAsync();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
                img.Source = bitmapImage;
            }

            Window.Current.Content = this;
            Window.Current.Activate();
        }

        async private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            shareOperation.ReportStarted();

            var notebook = comboNotebooks.SelectedItem as NoteBook;
            var noteType = (NoteTypes)comboType.SelectedItem;
            switch (noteType)
            {
                case NoteTypes.ToDo:
                    var todo = new ToDoDataItem(txtTitle.Text.HttpEncode(), notebook);
                    todo.Description = txtDescription.Text.HttpEncode();
                    await CopyDataToLocalStorageAsync(todo);
                    NotesDataSource.Add(todo, notebook.UniqueId);
                    break;
                case NoteTypes.Food:
                    var food = new FoodDataItem(txtTitle.Text.HttpEncode(), notebook);
                    food.Description = txtDescription.Text.HttpEncode();
                    await CopyDataToLocalStorageAsync(food);
                    NotesDataSource.Add(food, notebook.UniqueId);
                    break;
                default:
                    var note = new NoteDataItem(txtTitle.Text.HttpEncode(), notebook);
                    note.Description = txtDescription.Text.HttpEncode();
                    await CopyDataToLocalStorageAsync(note);
                    NotesDataSource.Add(note, notebook.UniqueId);
                    break;
            }

            DataManager.Save();

            this.shareOperation.ReportCompleted();
        }

        /// <summary>
        /// If ShareSource contain Bitmap to share, we obtain the Bitmap from shareOperation.data and using DataManager.SaveImageAsync we creates we image under Images folder.
        /// </summary>
        /// <param name="item">Note Item to link with the shared image</param>
        /// <returns></returns>
        async Task CopyDataToLocalStorageAsync(NoteDataCommon item)
        {
            if (this.shareOperation.Data.Contains(StandardDataFormats.Bitmap))
            {
                img.Visibility = Visibility.Visible;
                IRandomAccessStreamReference imageReceived = await this.shareOperation.Data.GetBitmapAsync();
                var stream = await imageReceived.OpenReadAsync();
                var imgName = String.Format("{0}.png", Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
                var fileName = await DataManager.SaveImageAsync(imgName,
                                                            stream.AsStreamForRead().AsInputStream(),
                                                            CreationCollisionOption.GenerateUniqueName);
                item.Images.Add(string.Format("{0}/{1}", FolderNames.Images.ToString(), fileName));
            }
        }
    }
}
