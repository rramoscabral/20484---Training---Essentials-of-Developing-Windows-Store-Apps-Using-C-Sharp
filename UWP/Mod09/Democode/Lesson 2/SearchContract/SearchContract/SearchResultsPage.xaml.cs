using SearchContract.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace SearchContract
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : SearchContract.Common.LayoutAwarePage
    {

        public SearchResultsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            List<Artist> results = null;
            var queryText = navigationParameter as String;
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';

            //// Take all the items in the Beatles collection which match the condition:
            //// "item titles starts with the text in 'queryText', ignoring case."
            //results = Artists.Beatles.Where(beatle => beatle.Title.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            //this.DefaultViewModel["Results"] = results;

            if (results == null || results.Count == 0)
                VisualStateManager.GoToState(this, "NoResultsFound", true);
            else
                VisualStateManager.GoToState(this, "ResultsFound", true);
        }
    }
}
