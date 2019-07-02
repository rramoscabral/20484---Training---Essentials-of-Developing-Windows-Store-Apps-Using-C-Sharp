using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_DataBindingDemo.Data;
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

namespace UWP_DataBindingDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Propriedade ViewModel que vai ser utilizado no Binding:
        //    <TextBlock Text="{x:Bind ViewModel.DefaultPearson.Details}"
        public PearsonViewModel ViewModel { get; set; }


        // Propriedade ViewModelCollection que vai ser utilizado no Binding:
        //    <ListView ItemsSource="{x:Bind ViewModelCollection.Pearsons}
        public PearsonViewModelCollection ViewModelCollection { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = new PearsonViewModel();
            this.ViewModelCollection = new PearsonViewModelCollection();
        }
       

    }
}
