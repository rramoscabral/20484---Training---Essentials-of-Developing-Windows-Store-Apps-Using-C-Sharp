using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToastNotifications
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CoreDispatcher _dispatcher;

        public MainPage()
        {
            this.InitializeComponent();

            _dispatcher = Window.Current.Dispatcher; // Store a reference to the window dispatcher.
        }

        private void TextToastNotification(object sender, RoutedEventArgs e)
        {
            // Build the toast notification XML
            StringBuilder toastXml = new StringBuilder();
            toastXml.Append("<toast>");
            toastXml.Append("<visual version='1'>");
            toastXml.Append("<binding template='ToastText02'>");
            toastXml.Append("<text id='1'>Ding!</text>");
            toastXml.Append("<text id='2'>Would you like butter on your toast?</text>");
            toastXml.Append("</binding>");
            toastXml.Append("</visual>");
            toastXml.Append("</toast>");

            // Load the XML string into a document
            XmlDocument toastDoc = new XmlDocument();
            toastDoc.LoadXml(toastXml.ToString());

            // Create the toast notification object from the XML document
            ToastNotification toast = new ToastNotification(toastDoc);

            // Display the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void ImageToastNotification(object sender, RoutedEventArgs e)
        {
            // Build the toast notification XML
            StringBuilder toastXml = new StringBuilder();
            toastXml.Append("<toast>");
            toastXml.Append("<visual version='1'>");
            toastXml.Append("<binding template='ToastImageAndText01'>");
            toastXml.Append("<text id='1'>More toast!</text>");
            toastXml.Append("<image id='1' src='ms-appx:///images/HelloSquare.png' alt='Hi there!'/>");
            toastXml.Append("</binding>");
            toastXml.Append("</visual>");
            toastXml.Append("</toast>");

            // Load the XML string into a document
            XmlDocument toastDoc = new XmlDocument();
            toastDoc.LoadXml(toastXml.ToString());

            // Create the toast notification object from the XML document
            ToastNotification toast = new ToastNotification(toastDoc);

            // Display the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void SilentToastNotification(object sender, RoutedEventArgs e)
        {
            // Build the toast notification XML
            StringBuilder toastXml = new StringBuilder();
            toastXml.Append("<toast>");
            toastXml.Append("<visual version='1'>");
            toastXml.Append("<binding template='ToastText02'>");
            toastXml.Append("<text id='1'>Shhh!</text>");
            toastXml.Append("<text id='2'>Silent toast.</text>");
            toastXml.Append("</binding>");
            toastXml.Append("</visual>");
            toastXml.Append("<audio silent='true'/>");  // No audio notification.
            toastXml.Append("</toast>");

            // Load the XML string into a document
            XmlDocument toastDoc = new XmlDocument();
            toastDoc.LoadXml(toastXml.ToString());

            // Create the toast notification object from the XML document
            ToastNotification toast = new ToastNotification(toastDoc);

            // Display the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void ReminderToastNotification(object sender, RoutedEventArgs e)
        {
            // Build the toast notification XML
            StringBuilder toastXml = new StringBuilder();
            toastXml.Append("<toast>");
            toastXml.Append("<visual version='1'>");
            toastXml.Append("<binding template='ToastText02'>");
            toastXml.Append("<text id='1'>Reminder!</text>");
            toastXml.Append("<text id='2'>Don't forget your toast!</text>");
            toastXml.Append("</binding>");
            toastXml.Append("</visual>");
            toastXml.Append("<audio src='ms-winsoundevent:Notification.Reminder'/>"); // Play the pre-defined 'Reminder' sound.
            toastXml.Append("</toast>");

            // Load the XML string into a document
            XmlDocument toastDoc = new XmlDocument();
            toastDoc.LoadXml(toastXml.ToString());

            // Create the toast notification object from the XML document
            ToastNotification toast = new ToastNotification(toastDoc);

            // Display the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private ToastNotification _toast = null;
        private void ShowToastNotification(object sender, RoutedEventArgs e)
        {
            // Build the toast notification XML
            StringBuilder toastXml = new StringBuilder();
            // The launch attribute value will be passed to the app when it is activated by the toast notification.
            // A long duration toast will last for up to 25 seconds, or until tapped/dismissed.
            toastXml.Append("<toast launch='LightlyToasted' duration='long'>");  
            toastXml.Append("<visual version='1'>");
            toastXml.Append("<binding template='ToastText02'>");
            toastXml.Append("<text id='1'>Tap or click the toast.</text>");
            toastXml.Append("<text id='2'>Swipe or click the close button to dismiss the toast.</text>");
            toastXml.Append("</binding>");
            toastXml.Append("</visual>");
            toastXml.Append("</toast>");

            // Load the XML string into a document
            XmlDocument toastDoc = new XmlDocument();
            toastDoc.LoadXml(toastXml.ToString());

            // Create the toast notification object from the XML document
            _toast = new ToastNotification(toastDoc);

            // Register for toast interaction events
            _toast.Activated += Toast_Activated;
            _toast.Dismissed += Toast_Dismissed;
            _toast.Failed += Toast_Failed;

            // Display the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(_toast);
        }

        private void HideToastNotification(object sender, RoutedEventArgs e)
        {
            if (_toast == null)
                return;

            ToastNotificationManager.CreateToastNotifier().Hide(_toast);
            _toast = null;
        }

        private async void Toast_Activated(ToastNotification sender, object args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                result.Text = "Toast tapped.";
            });
        }

        private async void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                result.Text = "Toast dismissed: " + args.Reason.ToString();
            });
        }

        private async void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                result.Text = "Failed to display toast.";
            });
        }
    }
}
