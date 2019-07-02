using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class FileOpenPicker : Page
    {
        public FileOpenPicker()
        {
            this.InitializeComponent();
        }


        async private void SelectFP_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;

            filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;

            filePicker.FileTypeFilter.Add(".txt");
            filePicker.FileTypeFilter.Add(".xml");

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                txtBlock1.Text = "OK";
            }
            else
            {
                txtBlock1.Text = "Operação cancelada";
            }
        }


    }

    
}
