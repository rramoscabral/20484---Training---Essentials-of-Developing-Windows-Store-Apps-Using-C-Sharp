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

namespace UWPRestFullClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.uriServer.Text = "http://localhost:13758";
        }

        private void GetAllPearsons_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.GetAllPearsons),this.uriServer.Text);

        }

        private void AddPearson_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.AddPearson), this.uriServer.Text);
        }

        private void UpdatePearson_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.UpdatePearson), this.uriServer.Text);
        }

        private void RemovePearson_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.RemovePearson), this.uriServer.Text);
        }
    }
}

