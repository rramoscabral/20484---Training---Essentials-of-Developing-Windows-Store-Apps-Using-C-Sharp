using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ILoveNotes.Common;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
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
    public sealed partial class ToDoControl : UserControl
    {
        public ToDoControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty NoteiItleProperty =
           DependencyProperty.Register("NoteTitle", typeof(string),
           typeof(ToDoControl), null);

        public string NoteTitle
        {
            get { return (string)GetValue(NoteiItleProperty); }
            set
            {
                SetValue(NoteiItleProperty, value);
            }
        }

        public static readonly DependencyProperty NoteUniqueIdProperty =
           DependencyProperty.Register("NoteUniqueId", typeof(string),
           typeof(ToDoControl), null);

        public string NoteUniqueId
        {
            get { return (string)GetValue(NoteUniqueIdProperty); }
            set
            {
                SetValue(NoteUniqueIdProperty, value);
            }
        }

        public static readonly DependencyProperty ToDosProperty =
           DependencyProperty.Register("ToDo", typeof(ObservableCollection<ToDo>),
           typeof(ToDoControl), null);

        public ObservableCollection<ToDo> ToDo
        {
            get { return (ObservableCollection<ToDo>)GetValue(ToDosProperty); }
            set
            {
                SetValue(ToDosProperty, value);
            }
        }

        /// <summary>
        /// Property return if there is selected ToDo Items
        /// </summary>
        public bool HasSelectedItems
        {
            get
            {
                return ToDoList == null || ToDoList.SelectedItems.Count == 0 ? false : true;
            }
        }

        /// <summary>
        /// Remove Selected ToDo Items
        /// </summary>
        public void RemoveSelectedItems()
        {
            if (ToDoList == null || ToDoList.SelectedItems.Count == 0) return;

            var list = ToDoList.SelectedItems.ToList();
            foreach (ToDo todo in list)
                this.ToDo.Remove(todo);

            UpdateNote();
        }

        /// <summary>
        /// Set Reminder for Selected ToDo Items
        /// </summary>
        /// <param name="Notetitle"></param>
        public void SetSelectedReminder(string Notetitle)
        {
            //Lab 10: Exercise 3, Task 2.1: call the SetScheduledToast method on each todoItems
            

            UpdateNote();
        }

        /// <summary>
        /// Set selected ToDo items with Done flag.
        /// </summary>
        public void SetDoneSelected()
        {
            if (ToDoList == null || ToDoList.SelectedItems.Count == 0) return;

            var list = ToDoList.SelectedItems.ToList();
            foreach (ToDo todo in list)
            {
                todo.Done = !todo.Done;
            }

            UpdateNote();
        }

        /// <summary>
        /// User clicked on ToDo Item, open ToDo Item editor window at the center of the ToDo control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ToDoEditorPopup == null) return;
            var td = e.ClickedItem as ToDo;

            ToDoEditorPopup.VerticalOffset = (Window.Current.Content.DesiredSize.Height / 2) - (ToDoEditorGrid.Height);
            ToDoEditorPopup.HorizontalOffset = (Window.Current.Content.DesiredSize.Width / 2) - (ToDoEditorGrid.Width - 120);//Window.Current.CoreWindow.PointerPosition.X > Window.Current.Bounds.Width ? 0 : Window.Current.CoreWindow.PointerPosition.X - (ToDoEditorGrid.Width);

            ToDoEditorPopup.DataContext = td;
            ToDoEditorPopup.IsOpen = true;

            txtTBTitle.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        void btnTDDiscardClick(object sender, RoutedEventArgs e)
        {
            ToDoEditorPopup.IsOpen = false;
        }

        ToDo newToDo;
        /// <summary>
        /// Show add new ToDo Item window.
        /// </summary>
        public void AddToDo()
        {
            ToDoEditorPopup.VerticalOffset = (this.ActualHeight / 2);
            ToDoEditorPopup.HorizontalOffset = (this.ActualWidth / 2);
            newToDo = new Data.ToDo();
            ToDoEditorPopup.DataContext = newToDo;

            ToDoEditorPopup.IsOpen = true;
            txtTBTitle.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        /// <summary>
        /// Add new ToDo item window closed, if user enter ToDo title then save new ToDo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToDoEditorPopup_Closed(object sender, object e)
        {
            if (!string.IsNullOrEmpty(txtTBTitle.Text) && newToDo != null)
            {
                newToDo.DueDate =  newToDo.DueDate.AddMinutes(1);
                this.ToDo.Add(newToDo);
                
                //Lab 10, Exercise 3, Task 2.1 : call the SetScheduledToast method on any reminder item.
                
            }
            else
            {
                var todo = (ToDo)ToDoEditorPopup.DataContext;
                
                //Lab 10, Exercise 3, Task 2.1  : call the SetScheduledToast method on any reminder item.
                
            }

            newToDo = null;
            ToDoEditorPopup.DataContext = null;
            UpdateNote();
        }

        /// <summary>
        /// Update ToDo Note after each ToDo item update.
        /// </summary>
        void UpdateNote()
        {
            var item = NotesDataSource.GetItem(NoteUniqueId);
            
            //Lab 10: Exercise 2, Task 2.5: Invoke the Tile Update for the secondary tile when a note changes.
            
            DataManager.Save(item.NoteBook);
        }
    }
}
