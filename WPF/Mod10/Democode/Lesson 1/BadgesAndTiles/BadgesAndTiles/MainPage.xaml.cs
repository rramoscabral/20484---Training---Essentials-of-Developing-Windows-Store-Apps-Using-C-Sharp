using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BadgesAndTiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int _counter = 1;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ClearTileNotification(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            result.Text = "Tile cleared.";
        }

        private void SendTileNotification(object sender, RoutedEventArgs e)
        {
            // Build the tile notification XML
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.Append("<tile>");
            xmlBuilder.Append("<visual version='2'>");
            xmlBuilder.Append("<binding template='TileWide310x150Text04' fallback='TileWideText04'>");
            xmlBuilder.Append("<text id='1'>Wide Text :Lots of tile notification text goes here.</text>");
            xmlBuilder.Append("</binding>");
            xmlBuilder.Append("<binding  template='TileSquare150x150Text02' fallback='TileSquareText02'>");
            xmlBuilder.Append("<text id='1'>Tile Notification.</text>");
            xmlBuilder.Append("<text id='2'>medium square.</text>");
            xmlBuilder.Append("</binding>");
            xmlBuilder.Append("</visual>");
            xmlBuilder.Append("</tile>");

            // Load the XML string into a document
            XmlDocument tileDOM = new XmlDocument();
            tileDOM.LoadXml(xmlBuilder.ToString());

            // Create the tile notification from the XML document
            TileNotification tile = new TileNotification(tileDOM);

            // Update the tile notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

            result.Text = "Tile notification sent.";
        }

        private void NotificationWithImage(object sender, RoutedEventArgs e)
        {
            // Build the tile notification XML
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.Append("<tile>");
            xmlBuilder.Append("<visual version='2'>");
            xmlBuilder.Append("<binding template='TileWide310x150SmallImageAndText01' fallback='TileWideSmallImageAndText01'>");
            xmlBuilder.Append("<image id='1' src='ms-appx:///images/HelloSquare.png' alt='Hi there!'/>");
            xmlBuilder.Append("<text id='1'>Wide :Tile Notification with Image</text>");
            xmlBuilder.Append("</binding>");
            xmlBuilder.Append("<binding template='TileSquare150x150PeekImageAndText04' fallback='TileSquarePeekImageAndText04'>");
            xmlBuilder.Append("<image id='1' src='ms-appx:///images/HelloSquare.png' alt='Hi there!'/>");
            xmlBuilder.Append("<text id='1'>Medium: Tile Notification with Image</text>");
            xmlBuilder.Append("</binding>");
            xmlBuilder.Append("</visual>");
            xmlBuilder.Append("</tile>");


            // Load the XML string into a document
            XmlDocument tileDOM = new XmlDocument();
            tileDOM.LoadXml(xmlBuilder.ToString());

            // Create the tile notification from the XML document
            TileNotification tile = new TileNotification(tileDOM);

            // Update the tile notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

            result.Text = "Tile notification with image sent.";
        }

        private void EnableNotificationQueue(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            result.Text = "Notification queue enabled.";
        }

        private void DisableNotificationQueue(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(false);
            result.Text = "Notification queue disabled.";
        }

        private void SendBadgeNotification(object sender, RoutedEventArgs e)
        {
            // Create the XML document
            XmlDocument badgeDOM = new XmlDocument();
            // Load the badge notification XML into the document
            badgeDOM.LoadXml(string.Format("<badge value='{0}'/>", 57));

            // Create the badge notification from the XML document
            BadgeNotification badge = new BadgeNotification(badgeDOM);

            // Update the badge notification
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
            result.Text = "Number badge sent.";
        }

        private void ClearBadgeNotification(object sender, RoutedEventArgs e)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            result.Text = "Badge cleared.";
        }
    }
}
