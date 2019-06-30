using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisualStateManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //Window.Current.SizeChanged += this.WindowSizeChanged;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var list = new List<string>() { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8", "Item 9", "Item 10", "Item 11", "Item 12" };

            gridView.ItemsSource = list;
            listView.ItemsSource = list;


        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            string visualState = DetermineVisualState();
            Windows.UI.Xaml.VisualStateManager.GoToState(this, visualState, false);
        }

        private string DetermineVisualState()
        {
            string newState = "FullScreenLandscape";
            if (Window.Current.Bounds.Width < 510)
            {
                newState = "SideBySide";
            }
            else if (Window.Current.Bounds.Width < Window.Current.Bounds.Height)
            {
                newState = "FullScreenPortrait";
            }
            return newState;
        }


    }
}
