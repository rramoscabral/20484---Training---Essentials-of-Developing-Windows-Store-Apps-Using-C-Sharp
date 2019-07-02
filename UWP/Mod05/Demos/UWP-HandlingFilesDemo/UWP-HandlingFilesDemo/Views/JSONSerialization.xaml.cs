using System;
using System.Collections.Generic;
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
    public sealed partial class JSONSerialization : Page
    {
        public JSONSerialization()
        {
            this.InitializeComponent();
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {

            var test = "Teste";

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync("JSONSerialization.json", CreationCollisionOption.ReplaceExisting);
            Stream stream = await storageFile.OpenStreamForWriteAsync();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(String));
            if (String.IsNullOrEmpty(this.txtBox1.Text))
            {
                ser.WriteObject(stream, test);
            }
            else
            {
                ser.WriteObject(stream, this.txtBox1.Text);
            }
            stream.Flush();
            stream.Dispose();
        }
    }
}
