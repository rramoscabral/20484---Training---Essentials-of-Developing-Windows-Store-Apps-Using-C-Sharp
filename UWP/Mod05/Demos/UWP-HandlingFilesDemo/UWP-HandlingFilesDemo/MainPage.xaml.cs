using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_HandlingFilesDemo
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

        private void Button_ClickWriteFiles(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(Views.FileAccess));
        }


        private void Button_ClickListFiles(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(Views.ListDirectories));
        }

        private void Button_ClickXMLSerialization(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(Views.XMLSerialization));
        }

        private void Button_ClickJSONSerialization(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(Views.JSONSerialization));
        }

        private void Button_ClickJSONDeserialization(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(Views.JSONDeserialization));
        }

        private void Button_ClickFileOpenPicker(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(Views.FileOpenPicker));
        }

    }
}
