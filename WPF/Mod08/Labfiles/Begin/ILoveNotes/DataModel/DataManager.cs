using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Common;
using ILoveNotes.Data;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ILoveNotes.DataModel
{
    public class DataManager
    {
        /// <summary>
        /// Create a copy of Storage Item in Images folder.
        /// </summary>
        /// <param name="files">Storage Item List</param>
        /// <param name="item">Owner of Images.</param>
        /// <returns></returns>
        async static public Task CopyImages(IReadOnlyList<StorageFile> files, NoteDataCommon item)
        {
            var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folder = await storage.CreateFolderAsync(FolderNames.Images.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);

            foreach (var file in files)
            {
                var copiedFile = await file.CopyAsync(folder, file.Name.ToCleanString(), NameCollisionOption.GenerateUniqueName);
                item.Images.Add(string.Format("{0}/{1}", FolderNames.Images.ToString(), copiedFile.Name));
            }
        }

        /// <summary>
        /// Search all files under each notebook and remove the unused files. 
        /// </summary>
        /// <returns></returns>
        static public void Clean()
        {
            Task.Factory.StartNew(() => RunCleanOperationAsync());
        }

        /// <summary>
        /// Remove all unused images from local storage
        /// </summary>
        /// <returns></returns>
        private async static Task RunCleanOperationAsync()
        {
            try
            {
                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await storage.CreateFolderAsync(FolderNames.Images.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
                if (folder == null) return;

                var notebooks = NotesDataSource.GetGroups().Cast<NoteBook>().ToList();

                var files = await folder.GetFilesAsync();
                var allImages = notebooks.SelectMany(n => n.Items).SelectMany(imgs => imgs.Images);
                foreach (var file in files)
                {
                    if (!allImages.Any(img => img.Contains(file.Name)))
                        await file.DeleteAsync();
                }
            }
            catch (Exception)
            {
                //Helpers.ShowErrorMessage("Clean", ex);
            }
        }

        /// <summary>
        /// Create new file under local folder
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="folderName">Folder Name (where the file will be created)</param>
        /// <param name="collision">Creation Collision Option</param>
        /// <returns>StorageFile</returns>
        async static public Task<StorageFile> CreateFileAsync(string fileName, FolderNames folderName, CreationCollisionOption collision = CreationCollisionOption.GenerateUniqueName)
        {
            try
            {
                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await storage.CreateFolderAsync(folderName.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
                return await folder.CreateFileAsync(fileName, collision);
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessageAsync("Create File", "Can't create new file under the specific folder because the file is used by the application.");
                return null;
            }
        }

        /// <summary>
        /// Create new file based on the IInputStream under Images folder.
        /// </summary>
        /// <param name="fileName">Image File Name</param>
        /// <param name="sourceStream">IInputStream of the image you want to save</param>
        /// <param name="options">Creation Collision Option</param>
        /// <returns>New file name</returns>
        async static public Task<string> SaveImageAsync(string fileName, IInputStream sourceStream, CreationCollisionOption options)
        {
            try
            {
                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                var imgFolderName = FolderNames.Images.ToString();
                var folder = await storage.CreateFolderAsync(imgFolderName, Windows.Storage.CreationCollisionOption.OpenIfExists);

                var file = await folder.CreateFileAsync(fileName, options);
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    await RandomAccessStream.CopyAsync(sourceStream, fileStream.AsOutputStream());
                }

                sourceStream.Dispose();
                return file.Name;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Delete file from specific folder
        /// </summary>
        /// <param name="Filename">File name to delete</param>
        /// <param name="folderType">Folder name where the file is located</param>
        /// <returns></returns>
        public async static Task DeleteFileAsync(string Filename, FolderNames folderType)
        {
            try
            {
                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await storage.CreateFolderAsync(folderType.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync(Filename);
                if (file == null) return;
                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessageAsync("Delete File", "Can't delete file because the file is used by the application.");
            }
        }

        /// <summary>
        /// Search for specific file name under specific folder
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="type">Folder Name</param>
        /// <returns>List with one IStorageFile</returns>
        public async static Task<List<IStorageFile>> FindFileAsync(string fileName, FolderNames type)
        {
            try
            {
                if (fileName.StartsWith("Assets") || fileName.StartsWith("ms-appx:///Assets")) return null;

                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await storage.CreateFolderAsync(type.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
                List<IStorageFile> storageFiles = new List<IStorageFile>();

                var file = await folder.GetFileAsync(fileName.Substring(fileName.IndexOf("/") + 1));
                storageFiles.Add(file);

                return storageFiles;
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessageAsync("Find File", "Cannot find file under the spesific folder.");
                return null;
            }
        }

        /// <summary>
        /// Search for specific file name under specific folder
        /// </summary>
        /// <param name="files">Collection of file names to search for</param>
        /// <param name="type">Folder Name</param>
        /// <returns>List of IStorageFile</returns>
        public async static Task<List<IStorageFile>> FindFilesAsync(ObservableCollection<string> files, FolderNames type)
        {
            try
            {
                var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await storage.CreateFolderAsync(type.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
                List<IStorageFile> storageFiles = new List<IStorageFile>();
                foreach (var fileName in files)
                {
                    if (fileName.StartsWith("Assets")) continue;

                    var file = await folder.GetFileAsync(fileName.Substring(fileName.IndexOf("/") + 1));
                    storageFiles.Add(file);
                }

                return storageFiles;
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessageAsync("Find Files", "Unexpected error getting files under the specific folder.");
                return null;
            }
        }

        /// <summary>
        /// Defines all known Note types for DataContractJsonSerializer, each notebook will be serialized and than converted to json format, finally a file will be created under Notebooks folder
        /// with the Notebook Unique id as file name.
        /// </summary>
        private async static void RunSaveOperation()
        {
            var storage = Windows.Storage.ApplicationData.Current.LocalFolder;

            List<Type> types = new List<Type>();
            types.Add(typeof(FoodDataItem));
            types.Add(typeof(NoteDataItem));
            types.Add(typeof(ToDoDataItem));
            types.Add(typeof(NoteBook));
            var notebooks = NotesDataSource.GetGroups();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NoteDataCommon), types);
            var folder = await storage.CreateFolderAsync(FolderNames.Notebooks.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
            foreach (var notebook in notebooks)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ser.WriteObject(ms, notebook);
                        byte[] json = ms.ToArray();
                        var value = Encoding.UTF8.GetString(json, 0, json.Length);

                        var file = await folder.CreateFileAsync(notebook.UniqueId,
                            Windows.Storage.CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteTextAsync(file, value);
                        await Task.Delay(500);
                    }
                }
                catch (Exception)
                {
                    //Helpers.ShowErrorMessage("Save Data", ex);
                }
            }
        }

        
        /// <summary>
        /// Defines all known Note types for DataContractJsonSerializer, serialize and than converted to json format, finally a file will be created under Notebooks folder
        /// with the Notebook Unique id as file name. 
        /// </summary>
        /// <param name="nb">Notebook to save</param>
        /// <returns></returns>
        private async static Task SaveNotebook(NoteBook nb)
        {
            var storage = Windows.Storage.ApplicationData.Current.LocalFolder;

            List<Type> types = new List<Type>();
            types.Add(typeof(FoodDataItem));
            types.Add(typeof(NoteDataItem));
            types.Add(typeof(ToDoDataItem));
            types.Add(typeof(NoteBook));

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NoteDataCommon), types);
            var folder = await storage.CreateFolderAsync(FolderNames.Notebooks.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ser.WriteObject(ms, nb);
                    byte[] json = ms.ToArray();
                    var value = Encoding.UTF8.GetString(json, 0, json.Length);

                    var file = await folder.CreateFileAsync(nb.UniqueId,
                        Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, value);
                    await Task.Delay(500);
                }
            }
            catch (Exception)
            {
                //Helpers.ShowErrorMessage("Save Data", ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Save specific Notebook data
        /// </summary>
        /// <param name="notebook">Notebook to save</param>
        static public void Save(NoteBook notebook)
        {
            Task saveTask = Task.Factory.StartNew(() => SaveNotebook(notebook));
            saveTask.Wait();
        }

        /// <summary>
        /// Save all notebooks data
        /// </summary>
        static public void Save()
        {
            Task saveTask = Task.Factory.StartNew(() => RunSaveOperation());
            saveTask.Wait();
        }

        /// <summary>
        /// Load local notebooks
        /// </summary>
        /// <returns></returns>
        static async public Task LoadAsync()
        {
            var storage = Windows.Storage.ApplicationData.Current.LocalFolder;
            var folder = await storage.CreateFolderAsync(FolderNames.Notebooks.ToString(), CreationCollisionOption.OpenIfExists);
            var notebooks = new ObservableCollection<NoteDataCommon>();
            List<Type> types = new List<Type>();
            types.Add(typeof(FoodDataItem));
            types.Add(typeof(NoteDataItem));
            types.Add(typeof(ToDoDataItem));
            types.Add(typeof(NoteBook));

            NotesDataSource.Unload();
            DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(NoteDataCommon), types);

            var files = await folder.GetFilesAsync();

            foreach (var file in files)
            {
                try
                {
                    using (var stream = await file.OpenReadAsync())
                    {
                        var noteB = (NoteDataCommon)des.ReadObject(stream.AsStreamForRead());
                        NotesDataSource.AddNoteBook(noteB);
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ShowErrorMessageAsync("Load Notebooks", "The application found data corruption.");
                }
            }
        }
    }
}
