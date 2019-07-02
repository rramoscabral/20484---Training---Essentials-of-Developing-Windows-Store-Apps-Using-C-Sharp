using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_HandlingFilesDemo.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JSONDeserialization : Page
    {
        private ObservableCollection<string> items = new ObservableCollection<string>();

        public JSONDeserialization()
        {
            this.InitializeComponent();
        }


        async private void Button_Click(object sender, RoutedEventArgs e)
        {


            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.GetFileAsync("JSONSerialization.json");
            Stream stream = await storageFile.OpenStreamForReadAsync();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(String));
            
            String dataString = ser.ReadObject(stream) as String;
            items.Add(dataString);
 
            stream.Flush();
            stream.Dispose();
        }
    }
}
