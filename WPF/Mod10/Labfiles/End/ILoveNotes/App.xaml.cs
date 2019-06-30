using ILoveNotes.Common;
using ILoveNotes.Controls;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using ILoveNotes.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace ILoveNotes
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DataManager.Save();
        }
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                //Lab 10, Exercise 2, Task 1.7 : Modify the OnLaunched method to react to secondary Tile activation
                if (args.Arguments.StartsWith(Consts.SecondaryTileFormat))
                {
                    SecondaryTileNavigation(args.Arguments);
                }
                Window.Current.Activate();
                return;
            }

            var rootFrame = new Frame();
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            if (Window.Current.Content == null)
            {
                SplashScreen splashScreen = args.SplashScreen;
                Splash eSplash = new Splash(splashScreen, args);
                Window.Current.Content = eSplash;
            }
            Window.Current.Activate();
        }

       
        //Lab 10, Exercise 2, Task 1.8 : Implement navigation when activated from secondary tile.
        private void SecondaryTileNavigation(string args)
        {
            var unqiueId = args.Substring(args.IndexOf("=") + 1);
            var item = NotesDataSource.Find(unqiueId.ToString());
            var frame = Window.Current.Content as Frame;
            if (item == null)
                frame.Navigate(typeof(GroupedItemsPage));
            else if (item.GetType() == typeof(NoteBook))
            {
                frame.Navigate(typeof(GroupDetailPage), item.UniqueId);
            }
            else
                frame.Navigate(typeof(ItemDetailPage), item.UniqueId);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            DataManager.Save();
            deferral.Complete();
        }

        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            SplashScreen splashScreen = args.SplashScreen;
            Splash eSplash = new Splash(splashScreen, args);
            Window.Current.Activate();
        }

        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            SplashScreen splashScreen = args.SplashScreen;
            Splash eSplash = new Splash(splashScreen, args);
        }

        protected override void OnFileOpenPickerActivated(Windows.ApplicationModel.Activation.FileOpenPickerActivatedEventArgs args)
        {
            var fileOpenPickerPage = new FileOpenPickerPage();
            fileOpenPickerPage.Activate(args);
        }

        protected override void OnFileSavePickerActivated(Windows.ApplicationModel.Activation.FileSavePickerActivatedEventArgs args)
        {
            var fileSavePickerPage = new FileSavePickerPage();
            fileSavePickerPage.Activate(args);
        }
    }
}
