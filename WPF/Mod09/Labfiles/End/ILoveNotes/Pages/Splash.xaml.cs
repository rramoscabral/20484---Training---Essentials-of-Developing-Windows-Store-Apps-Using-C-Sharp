using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ILoveNotes.Common;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using ILoveNotes.Controls.Settings;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ILoveNotes.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Splash : ILoveNotes.Common.LayoutAwarePage
    {
        private Rect splashImageCoordinates; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        private static ShareTargetActivatedEventArgs ShareTargetOperation = null;
        private static string DirectLink = string.Empty;
        private bool RestoreLastState = false;

        /// <summary>
        /// Splash Screen for Basic Application launch
        /// </summary>
        /// <param name="splashScreen">SplashScreen from IActivatedEventArgs</param>
        public Splash(SplashScreen splashScreen)
        {
            Init(splashScreen);
        }

        /// <summary>
        /// Splash Screen for ShareTarget Application launch
        /// </summary>
        /// <param name="splashScreen">SplashScreen from IActivatedEventArgs</param>
        /// <param name="shareArgs">ShareTargetActivatedEventArgs</param>
        public Splash(SplashScreen splashScreen, ShareTargetActivatedEventArgs shareArgs)
        {
            ShareTargetOperation = shareArgs;
            Init(splashScreen);
        }

        /// <summary>
        /// Splash Screen for Basic launch + Tile launch
        /// If args contains Note Unique Id as Argument then perform redirect to that specific note.
        /// </summary>
        /// <param name="splashScreen">SplashScreen from IActivatedEventArgs</param>
        /// <param name="args">LaunchActivatedEventArgs</param>
        public Splash(SplashScreen splashScreen, LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                RestoreLastState = true;
            }
            Init(splashScreen);
        }

        /// <summary>
        /// Place Splash Screen at the center of the page and register to the following events:
        /// DataTransferManager -> DataRequested
        /// SearchPane -> SuggestionsRequested, SearchPaneQuerySubmitted
        /// SettingsPane -> CommandsRequested
        /// NotesDataSource -> DataCompleted
        /// </summary>
        /// <param name="splashScreen">SplashScreen from IActivatedEventArgs</param>
        async private void Init(SplashScreen splashScreen)
        {
            if (!NotesDataSource.DataLoaded)
            {
                this.InitializeComponent();
                this.splashImageCoordinates = splashScreen.ImageLocation;
                this.splash = splashScreen;

                // Position the extended splash screen image in the same location as the splash screen image.
                this.loader.SetValue(Canvas.LeftProperty, this.splashImageCoordinates.X);
                this.loader.SetValue(Canvas.TopProperty, this.splashImageCoordinates.Y);
                this.loader.Height = this.splashImageCoordinates.Height;
                this.loader.Width = this.splashImageCoordinates.Width;

                DataTransferManager datatransferManager;
                datatransferManager = DataTransferManager.GetForCurrentView();
                datatransferManager.DataRequested += DataRequested;

                Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);
                SearchPane.GetForCurrentView().SuggestionsRequested += SearchPaneSuggestionsRequested;
                SearchPane.GetForCurrentView().QuerySubmitted += SearchPaneQuerySubmitted;

                SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

                NotesDataSource data = new NotesDataSource();
                data.Completed += Data_Completed;
                await data.Load();
            }
            else
            {
                Data_Completed(this, null);
            }
        }

        /// <summary>
        /// Define Settings Pages for the application once the OnCommandsRequested event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            Color _background = Color.FromArgb(255, 130, 45, 70);

            // Add an About command
            var about = new SettingsCommand("About", "About", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.Content = new AboutPage();
                settings.HeaderBackground = new SolidColorBrush(_background);
                settings.HeaderForeground = new SolidColorBrush(Colors.White);
                settings.Title = "About";
                settings.Show();
            });

            args.Request.ApplicationCommands.Add(about);
        }

        /// <summary>
        /// User has choose a search suggestion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void SearchPaneQuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            Frame frame;

            if (Window.Current.Content == null)
                frame = new Frame();
            else
                frame = Window.Current.Content as Frame;

            if (args.QueryText.Contains(Consts.SearchSplitter))
            {
                var notebookName = args.QueryText.Substring(0, args.QueryText.IndexOf(Consts.SearchSplitter));
                var noteName = args.QueryText.Substring(args.QueryText.IndexOf(Consts.SearchSplitter) + Consts.SearchSplitter.Length);
                var notebook = NotesDataSource.SearchNotebook(notebookName);
                if (notebook == null) return;
                var note = NotesDataSource.SearchNote(notebook.UniqueId, noteName);
                if (note == null) return;

                frame.Navigate(typeof(ItemDetailPage), note.UniqueId);
            }
            else
            {
                frame.Navigate(typeof(SearchResults), args.QueryText);
            }
        }


        /// <summary>
        /// Share Contract - User has choose to share data with I Love Notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            var currentPage = (Frame)Window.Current.Content;
            if (currentPage.SourcePageType == typeof(ItemDetailPage))
            {
                var deferral = e.Request.GetDeferral();

                var notePage = currentPage.Content as ItemDetailPage;
                var item = notePage.Note;
                e.Request.Data.Properties.Title = item.Title;
                if (!string.IsNullOrEmpty(notePage.Note.Description))
                    e.Request.Data.Properties.Description = item.Description.ToShortString();

                var sb = new StringBuilder();
                sb.AppendLine(string.IsNullOrEmpty(item.Description) ? string.Empty : item.Description);

                var storageItems = await DataManager.FindFilesAsync(item.Images, FolderNames.Images);
                if (storageItems.Count > 0)
                {
                    e.Request.Data.SetStorageItems(storageItems, true);
                }

                if (item.Type == NoteTypes.ToDo)
                {
                    sb.AppendLine("To Do List:");
                    foreach (var todo in item.ToDo)
                        sb.AppendLine(string.Format("{0} - Due Date: {1} - {2}", todo.Title, todo.DueDate, todo.Done == true ? "Done" : "Not Done"));
                }

                e.Request.Data.SetText(sb.ToString());
                deferral.Complete();
            }
            else
            {
                e.Request.Data.Properties.Title = "I Love Notes";
                e.Request.Data.Properties.Description = "I'm using 'I Love Notes' for Windows 8";
                e.Request.Data.SetWebLink(new Uri("http://www.microsoft.com"));
            }
        }

        /// <summary>
        /// Return Search results based on search value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">SearchPaneSuggestionsRequestedEventArgs contains QueryText of the search value</param>
        void SearchPaneSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            var items = NotesDataSource.Search(args.QueryText);
            foreach (NoteDataCommon item in items)
                args.Request.SearchSuggestionCollection.AppendQuerySuggestion(string.Format("{0}{1}{2}", item.NoteBook.Title, Consts.SearchSplitter, item.Title));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// After DataManager has finished loading the data, based on the application launch type we redirect the user to the proper page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        async void Data_Completed(object sender, Exception ex)
        {
            if (ex != null)
            {
                Helpers.ShowErrorMessageAsync("Loading Notebooks", "Unexpected error while loading notebooks.");
            }
            else
            {
                var rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                DataManager.Clean();

                if (NotesDataSource.GetGroups().Count == 0)
                {
                    rootFrame.Navigate(typeof(RestoreWorkingPage), "Getting Started");
                    Window.Current.Content = rootFrame;
                    return;
                }

                if (RestoreLastState)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        rootFrame.Navigate(typeof(GroupedItemsPage));
                        Window.Current.Content = rootFrame;
                    }
                }
                else if (ShareTargetOperation != null)
                {
                    ShareTarget share = new ShareTarget();
                    await share.ActivateAsync(ShareTargetOperation);
                }
                else if (!string.IsNullOrEmpty(DirectLink))
                {
                    var note = NotesDataSource.Find(DirectLink);

                    if (note == null)
                    {
                        rootFrame.Navigate(typeof(GroupedItemsPage));
                        Window.Current.Content = rootFrame;
                        return;
                    }

                    rootFrame.Navigate(typeof(GroupedItemsPage));
                    if (note.GetType() == typeof(NoteBook))
                    {
                        rootFrame.Navigate(typeof(GroupDetailPage), note.UniqueId);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(GroupDetailPage), note.NoteBookUniqueId);
                        rootFrame.Navigate(typeof(ItemDetailPage), note.UniqueId);
                    }

                    Window.Current.Content = rootFrame;
                }
                else
                {
                    rootFrame.Navigate(typeof(GroupedItemsPage));
                    Window.Current.Content = rootFrame;
                }
            }
        }

        void ExtendedSplash_OnResize(Object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            // Safely update the splash screen image coordinates
            if (this.splash != null)
            {
                this.splashImageCoordinates = this.splash.ImageLocation;

                // Re-position the extended splash screen image due to window resize event.
                this.loader.SetValue(Canvas.LeftProperty, this.splashImageCoordinates.X);
                this.loader.SetValue(Canvas.TopProperty, this.splashImageCoordinates.Y);
                this.loader.Height = this.splashImageCoordinates.Height;
                this.loader.Width = this.splashImageCoordinates.Width;
            }
        }
    }
}
