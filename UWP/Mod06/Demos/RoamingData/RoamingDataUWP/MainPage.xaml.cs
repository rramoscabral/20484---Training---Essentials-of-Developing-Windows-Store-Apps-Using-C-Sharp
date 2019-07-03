using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace RoamingDataUWP
{
    public sealed partial class MainPage : Page
    {
        public List<string> Backgrounds { get; set; }
        ApplicationDataContainer roamingSettings;
        public MainPage()
        {
            this.InitializeComponent();
            Backgrounds = new List<string>{"ms-appx:///Assets/1.jpg", "ms-appx:///Assets/2.jpg", "ms-appx:///Assets/3.jpg", "ms-appx:///Assets/4.jpg"};
            roamingSettings = ApplicationData.Current.RoamingSettings;

            if (roamingSettings.Values.ContainsKey("backgroundImage"))
            {
                BackgroundImage.UriSource = new Uri(roamingSettings.Values["backgroundImage"].ToString());
            }
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var backgroundImage = e.AddedItems[0] as string;
            BackgroundImage.UriSource = new Uri(backgroundImage);
            roamingSettings.Values["backgroundImage"] = backgroundImage;
        }
    }
}