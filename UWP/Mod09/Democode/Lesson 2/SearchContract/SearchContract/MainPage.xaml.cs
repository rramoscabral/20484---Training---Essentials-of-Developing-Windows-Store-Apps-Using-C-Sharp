using SearchContract.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SearchContract
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SearchContract.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void SearchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            //// Take all the items in the Beatles collection which match the condition:
            //// "item titles starts with the text in 'queryText', ignoring case"
            //// and add each one to the SearchSuggesionCollection.
            //foreach (var artist in Artists.Beatles.Where(beatle => beatle.Title.StartsWith(args.QueryText, StringComparison.CurrentCultureIgnoreCase)))
            //    args.Request.SearchSuggestionCollection.AppendQuerySuggestion(artist.Title);
        }

        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            //// Navigate to the search results page, and pass the query text the navigation parameter.
            //Frame.Navigate(typeof(SearchResultsPage), args.QueryText);
        }
    }
}
