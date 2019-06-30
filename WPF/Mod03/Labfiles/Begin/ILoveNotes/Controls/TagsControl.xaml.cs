using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ILoveNotes.Common;
using ILoveNotes.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ILoveNotes.Controls
{
    public sealed partial class TagsControl : UserControl
    {
        public TagsControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TagsProperty =
           DependencyProperty.Register("Tags", typeof(ObservableCollection<string>),
           typeof(TagsControl), null);

        public ObservableCollection<string> Tags
        {
            get { return (ObservableCollection<string>)GetValue(TagsProperty); }
            set
            {
                SetValue(TagsProperty, value);
            }
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            txtTagTitle.Text = string.Empty;
            popup.IsOpen = false;
        }

        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveTags();
        }

        void SaveTags()
        {
            var list = listSuggestions.SelectedItems.ToList();
            this.Tags.Clear();
            if (list.Count > 0)
            {
                foreach (ListViewItem tag in list)
                {
                    var tagVal = tag.Content.ToString();
                    this.Tags.Add(tagVal);
                }
            }

            if (!string.IsNullOrEmpty(txtTagTitle.Text))
                this.Tags.Add(txtTagTitle.Text);

            txtTagTitle.Text = string.Empty;
            popup.IsOpen = false;
        }

        public void ShowTagsBar()
        {
            popup.IsOpen = true;
            listSuggestions.ItemsSource = BuildTagList();
            txtTagTitle.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private List<ListViewItem> BuildTagList()
        {
            var allTags = NotesDataSource.GetGroups().SelectMany(nb => ((NoteBook)nb).Items).SelectMany(t => t.Tags).Distinct();

            List<ListViewItem> tags = new List<ListViewItem>();
            foreach (var tag in allTags)
            {
                var viewItem = new ListViewItem();
                viewItem.Content = tag;
                viewItem.Style = Application.Current.Resources["TagSuggestionItemTemplate"] as Style;
                viewItem.IsSelected = this.Tags.Contains(tag);
                tags.Add(viewItem);
            }
            return tags;
        }

        private void popup_Closed(object sender, object e)
        {
            SaveTags();
        }
    }
}
