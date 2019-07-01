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
using TilesAndNotifications.Services;
using TilesAndNotifications.Models;
using Windows.UI.Notifications;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TilesAndNotifications
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int _count = 0;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void UpdateBadge (object sender, RoutedEventArgs e)
        {
            _count++;
            TileService.SetBadgeCountOnTile(_count);
        }

        private void UpdatePrimaryTile(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var xmlDoc = TileService.CreateTiles(new PrimaryTile());

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            TileNotification notification = new TileNotification(xmlDoc);
            updater.Update(notification);
        }
    }
}
