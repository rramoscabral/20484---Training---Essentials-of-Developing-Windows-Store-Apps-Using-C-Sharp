using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ILoveNotes.Common
{
    public class MediaManager
    {
        /// <summary>
        /// Showing Camera Capture UI
        /// After the user has approve the picture we create new image file under Images folder and add link to new image.
        /// </summary>
        /// <param name="forItem">Note item that will be linked to that image</param>
        /// <returns></returns>
        async public static Task ShowCamera(NoteDataCommon forItem)
        {
            try
            {
                CameraCaptureUI dialog = new CameraCaptureUI();
                Size aspectRatio = new Size(16, 9);
                dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;
                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);                
                if (file != null)
                {
                    var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                    var folder = await storage.CreateFolderAsync(FolderNames.Images.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);

                    var copiedFile = await file.CopyAsync(folder, file.Name, NameCollisionOption.GenerateUniqueName);
                    forItem.Images.Add(string.Format("{0}/{1}", FolderNames.Images.ToString(), copiedFile.Name));
                }
            }
            catch
            {
                Helpers.ShowErrorMessageAsync("Show Camera", "Unexpected error launching camera or save camera image");
            }
        }

        /// <summary>
        /// Open File Picker allowing the user to select images from his machine.
        /// After user select and close the FileOpenPicker we call DataManager.CopyImages with the choosen files and the Note the user is working on.
        /// </summary>
        /// <param name="forItem">Note item that will be linked to those images</param>
        /// <returns></returns>
        async public static Task OpenFilePicker(NoteDataCommon forItem)
        {
            if (Helpers.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".bmp");
                openPicker.FileTypeFilter.Add(".gif");
                openPicker.FileTypeFilter.Add(".tif");
                IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
                if (files.Count > 0)
                {
                    await DataManager.CopyImages(files, forItem);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
