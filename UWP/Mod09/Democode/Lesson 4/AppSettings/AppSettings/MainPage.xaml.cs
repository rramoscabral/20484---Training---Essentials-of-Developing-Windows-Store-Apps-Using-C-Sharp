using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppSettings
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }


        private void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            SettingsCommand generalCommand = new SettingsCommand("someCommand", "Do Something", new UICommandInvokedHandler(OnSomeCommand));
            eventArgs.Request.ApplicationCommands.Add(generalCommand);

            SettingsCommand helpCommand = new SettingsCommand("showSettingsCommand", "Show Settings", new UICommandInvokedHandler(OnShowSettingsCommand));
            eventArgs.Request.ApplicationCommands.Add(helpCommand);

            SettingsCommand flyoutCommand = new SettingsCommand("showFlyoutCommand", "Show Flyout", new UICommandInvokedHandler(OnShowFlyoutCommand));
            eventArgs.Request.ApplicationCommands.Add(flyoutCommand);
        }

        private void OnShowFlyoutCommand(IUICommand command)
        {
            result.Text = "Command clicked: " + command.Label;

            SettingsFlyout flyout = new SettingsFlyout();
            flyout.Title = "Sample flyout";
            flyout.HeaderBackground = new SolidColorBrush(Colors.Violet);
            flyout.Content = new FlyoutContent();
            flyout.Show();
        }
        
        private void OnSomeCommand(IUICommand command)
        {
            result.Text = "Command clicked: " + command.Label;
        }

        private void OnShowSettingsCommand(IUICommand command)
        {
            result.Text = "Command clicked: " + command.Label;
            settingsControl.Height = Window.Current.Bounds.Height;
            settingsPopup.IsOpen = true;
        }
        
        private void ShowSettingsPane(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();
        }
    }
}
