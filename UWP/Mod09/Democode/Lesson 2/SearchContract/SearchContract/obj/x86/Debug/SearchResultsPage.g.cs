﻿#pragma checksum "C:\20484-GITHUB-MFILES\UWP\Mod09\Democode\Lesson 2\SearchContract\SearchContract\SearchResultsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "492D697F0903474F7F541BD835F37311"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SearchContract
{
    partial class SearchResultsPage : 
        global::SearchContract.Common.LayoutAwarePage, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // SearchResultsPage.xaml line 1
                {
                    this.pageRoot = (global::SearchContract.Common.LayoutAwarePage)(target);
                }
                break;
            case 2: // SearchResultsPage.xaml line 15
                {
                    this.resultsViewSource = (global::Windows.UI.Xaml.Data.CollectionViewSource)(target);
                }
                break;
            case 3: // SearchResultsPage.xaml line 32
                {
                    this.resultsPanel = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 4: // SearchResultsPage.xaml line 98
                {
                    this.noResultsTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5: // SearchResultsPage.xaml line 92
                {
                    this.backButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.backButton).Click += this.GoBack;
                }
                break;
            case 6: // SearchResultsPage.xaml line 93
                {
                    this.pageTitle = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 7: // SearchResultsPage.xaml line 94
                {
                    this.resultText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8: // SearchResultsPage.xaml line 95
                {
                    this.queryText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 9: // SearchResultsPage.xaml line 34
                {
                    this.typicalPanel = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 10: // SearchResultsPage.xaml line 58
                {
                    this.snappedPanel = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 11: // SearchResultsPage.xaml line 59
                {
                    this.resultsListView = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 12: // SearchResultsPage.xaml line 35
                {
                    this.resultsGridView = (global::Windows.UI.Xaml.Controls.GridView)(target);
                }
                break;
            case 13: // SearchResultsPage.xaml line 109
                {
                    this.ApplicationViewStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 14: // SearchResultsPage.xaml line 169
                {
                    this.ResultStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 15: // SearchResultsPage.xaml line 170
                {
                    this.ResultsFound = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 16: // SearchResultsPage.xaml line 172
                {
                    this.NoResultsFound = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 17: // SearchResultsPage.xaml line 110
                {
                    this.FullScreenLandscape = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 18: // SearchResultsPage.xaml line 111
                {
                    this.Filled = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 19: // SearchResultsPage.xaml line 114
                {
                    this.FullScreenPortrait = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 20: // SearchResultsPage.xaml line 136
                {
                    this.Snapped = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}
