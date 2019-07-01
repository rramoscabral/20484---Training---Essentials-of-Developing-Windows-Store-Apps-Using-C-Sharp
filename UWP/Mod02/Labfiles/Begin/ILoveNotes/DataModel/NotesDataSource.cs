using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ILoveNotes.DataModel;
using ILoveNotes.Common;
using Windows.UI.Xaml;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace ILoveNotes.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class NoteDataCommon : ILoveNotes.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public NoteDataCommon()
        {
            this.CreatedDate = DateTime.Now.FormatedDate();
            this._uniqueId = Helpers.GetUniqueId();
        }

        public NoteDataCommon(String title, String iconPath)
        {
            this._uniqueId = Helpers.GetUniqueId();
            this._title = title;
            this.IconPath = iconPath;
            this.CreatedDate = DateTime.Now.FormatedDate();
        }

        public NoteDataCommon Clone()
        {
            return (NoteDataCommon)this.MemberwiseClone();
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _createdDate = string.Empty;
        public string CreatedDate
        {
            get { return this._createdDate; }
            set { this.SetProperty(ref this._createdDate, value); }
        }

        private DateTime _dateModified = DateTime.Now;
        public DateTime DateModified
        {
            get { return this._dateModified; }
            set { this.SetProperty(ref this._dateModified, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private NoteTypes _type = NoteTypes.Note;
        public NoteTypes Type
        {
            get { return this._type; }
            set { this.SetProperty(ref this._type, value); }
        }

        private string _recordPath = string.Empty;
        public string RecordPath
        {
            get { return this._recordPath; }
            set { this.SetProperty(ref this._recordPath, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private string _address = string.Empty;
        public string Address
        {
            get { return this._address; }
            set { this.SetProperty(ref this._address, value); }
        }


        public string NoteBookUniqueId { get; set; }

        private NoteBook _noteBook;
        [IgnoreDataMember]
        public NoteBook NoteBook
        {
            get
            {
                if (_noteBook == null)
                    _noteBook = (NoteBook)NotesDataSource.GetGroups().SingleOrDefault(n => n.UniqueId.Equals(this.NoteBookUniqueId));

                return this._noteBook;
            }
            set
            {
                this.NoteBookUniqueId = value.UniqueId;
                this.SetProperty(ref this._noteBook, value);
            }
        }

        private ImageSource _icon = null;

        public String IconPath = string.Empty;
        [IgnoreDataMember]
        public ImageSource Icon
        {
            get
            {
                if (this._icon == null && this.IconPath != null)
                {
                    this._icon = new BitmapImage(new Uri(NoteDataCommon._baseUri, this.IconPath));
                }
                return this._icon;
            }

            set
            {
                this.IconPath = null;
                this.SetProperty(ref this._icon, value);
            }
        }

        private ObservableCollection<string> _tags = new ObservableCollection<string>();
        public ObservableCollection<string> Tags
        {
            get { return this._tags; }
            set { this.SetProperty(ref this._tags, value); }
        }

        private ObservableCollection<ToDo> _todo = new ObservableCollection<ToDo>();
        public ObservableCollection<ToDo> ToDo
        {
            get { return this._todo; }
            set { this.SetProperty(ref this._todo, value); }
        }

        private ObservableCollection<string> _images = new ObservableCollection<string>();
        public ObservableCollection<string> Images
        {
            get { return this._images; }
            set { this.SetProperty(ref this._images, value); }
        }
    }

    /// <summary>
    /// Item Model for Food
    /// </summary>
    public class FoodDataItem : NoteDataCommon
    {
        public FoodDataItem()
        {
            this.Type = NoteTypes.Food;
            this.IconPath = "Assets/Icons/food.png";
        }

        public FoodDataItem(String title, NoteBook notebook)
            : base(title, "Assets/Icons/food.png")
        {
            this.NoteBook = notebook;
            this.Type = NoteTypes.Food;
        }
    }

    /// <summary>
    /// Item Model for Note
    /// </summary>
    public class NoteDataItem : NoteDataCommon
    {
        public NoteDataItem()
        {
            this.Type = NoteTypes.Note;
            this.IconPath = "Assets/Icons/note.png";
        }

        public NoteDataItem(String title, NoteBook notebook)
            : base(title, "Assets/Icons/note.png")
        {
            this.NoteBook = notebook;
            this.Type = NoteTypes.Note;
            this.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut porta, dui nec molestie vulputate, nibh elit scelerisque purus, sit amet blandit turpis ante at dolor. Praesent tellus libero, varius a dignissim ut, molestie ac nisl. Nullam dolor erat, ullamcorper sed sodales eget, volutpat nec leo. Maecenas aliquet auctor mauris id tristique. Vestibulum a euismod eros. Phasellus enim est, dictum vitae placerat in, semper et odio. In adipiscing metus quis nibh placerat sit amet semper tortor ultricies. Morbi eget felis arcu. Pellentesque nec tellus at felis molestie dapibus nec ut nisi. Cras condimentum ipsum nec justo aliquam pellentesque. Nullam arcu urna, fermentum vitae feugiat ac, placerat a sem. Duis et leo justo, non tempus felis. Curabitur euismod imperdiet purus id fermentum.";
        }
    }

    /// <summary>
    /// Item Model for Todo
    /// </summary>
    public class ToDoDataItem : NoteDataCommon
    {
        public ToDoDataItem()
        {
            this.Type = NoteTypes.ToDo;
            this.IconPath = "Assets/Icons/todo.png";
        }

        public ToDoDataItem(String title, NoteBook notebook)
            : base(title, "Assets/Icons/todo.png")
        {
            this.Type = NoteTypes.ToDo;
            this.NoteBook = notebook;
        }
    }

    public class ToDo : ILoveNotes.Common.BindableBase
    {
        public ToDo()
        {
            this._uniqueId = Helpers.GetShortUniqueId();
        }
        public ToDo(string title)
        {
            this._title = title;
            this._uniqueId = Helpers.GetShortUniqueId();
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private DateTime _dueDate = DateTime.Now;
        public DateTime DueDate
        {
            get { return this._dueDate; }
            set { this.SetProperty(ref this._dueDate, value); }
        }

        private bool _hasReminder;
        public bool HasReminder
        {
            get
            {
                if (this._dueDate < DateTime.Now)
                    this._hasReminder = false;

                return this._hasReminder;
            }
            set
            {
                if (this._dueDate > DateTime.Now)
                    this.SetProperty(ref this._hasReminder, value);
            }
        }

        private bool _done;
        public bool Done
        {
            get { return this._done; }
            set { this.SetProperty(ref this._done, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class Section : NoteDataCommon
    {
        public Section()
        {
            this.Type = NoteTypes.Section;
        }
        public Section(String title)
            : base(title, null)
        {
            this.Type = NoteTypes.Section;
        }

        public ObservableCollection<NoteDataCommon> Items { get; set; }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    /// 
    public class NoteBook : NoteDataCommon
    {
        public NoteBook()
        {
            this.Type = NoteTypes.Notebook;
        }

        public NoteBook(String title)
            : base(title, null)
        {
            this.Type = NoteTypes.Notebook;
        }

        [IgnoreDataMember]
        public IEnumerable<NoteDataCommon> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this.Items.Take(4); }
        }

        [IgnoreDataMember]
        public ObservableCollection<NoteDataCommon> Items
        {
            get
            {
                var items = this.Sections.SelectMany(a => a.Items).ToList();
                return new ObservableCollection<NoteDataCommon>(items);
            }
        }

        public ObservableCollection<NoteDataCommon> FoodSection = new ObservableCollection<NoteDataCommon>();
        public ObservableCollection<NoteDataCommon> NotesSection = new ObservableCollection<NoteDataCommon>();
        public ObservableCollection<NoteDataCommon> ToDoSection = new ObservableCollection<NoteDataCommon>();

        private ObservableCollection<Section> _sections;
        [IgnoreDataMember]
        public ObservableCollection<Section> Sections
        {
            get
            {
                if (_sections == null)
                {
                    _sections = new ObservableCollection<Section>();
                    _sections.Add(new Section("Food") { Items = FoodSection });
                    _sections.Add(new Section("Notes") { Items = NotesSection });
                    _sections.Add(new Section("ToDo") { Items = ToDoSection });
                }
                return this._sections;
            }
        }
    }


    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class NotesDataSource
    {
        private static NotesDataSource _notesDataSource = new NotesDataSource();
        public static bool DataLoaded;
        public delegate void CompletedEventHandler(object sender, Exception ex);
        public event CompletedEventHandler Completed;

        private ObservableCollection<NoteDataCommon> _allGroups = new ObservableCollection<NoteDataCommon>();
        public ObservableCollection<NoteDataCommon> AllGroups
        {
            get { return this._allGroups; }
        }

        /// <summary>
        /// Order all notebooks by DateModified
        /// </summary>
        /// <returns>ObservableCollection of notebooks</returns>
        public static ObservableCollection<NoteDataCommon> GetGroups()
        {
            return _notesDataSource.AllGroups.OrderByDescending(d => d.DateModified).ToList().ToObservable();
        }

        /// <summary>
        /// Clean AllGroups collection
        /// </summary>
        public static void Unload() 
        {
            _notesDataSource._allGroups.Clear();
        }

        /// <summary>
        /// Search for specific notebook based on UniqueId
        /// </summary>
        /// <param name="uniqueId">Unique Id to search for</param>
        /// <returns>null if no notebook with the specific uqniue id was found or the notebook item with the specific unique id.</returns>
        public static NoteBook GetNotebook(string uniqueId)
        {
            var result = _notesDataSource.AllGroups.SingleOrDefault((group) => group.UniqueId.Equals(uniqueId));
            if (result == null) return null;
            return result as NoteBook;
        }

        /// <summary>
        /// Search for Note item based on UniqueId
        /// </summary>
        /// <param name="uniqueId">Uniue Id to search for</param>
        /// <returns>null if no note with the specific uqniue id was found or the note item with the specific unique id.</returns>
        public static NoteDataCommon GetItem(string uniqueId)
        {
            var result = _notesDataSource.AllGroups.SelectMany(group => ((NoteBook)group).Items).SingleOrDefault((item) => item.UniqueId.Equals(uniqueId));
            if (result == null) return null;
            return result;
        }

        /// <summary>
        /// Search notebook based on title
        /// </summary>
        /// <param name="title">Notebook title</param>
        /// <returns>null if no notebook with the specific title was found or the notebook item with the specific title.</returns>
        public static NoteBook SearchNotebook(string title)
        {
            var result = _notesDataSource.AllGroups.SingleOrDefault(n => n.Title.ToLower().Equals(title.ToLower()));
            if (result == null) return null;
            return result as NoteBook;
        }

        /// <summary>
        /// Search note by title under spesific Notebook (based on unique id)
        /// </summary>
        /// <param name="notebookUniqueId">Notebook Unique id</param>
        /// <param name="title">Note Title</param>
        /// <returns>null if there is not notebook with the spesific uniqueid or no notes with spesific title.</returns>
        public static NoteDataCommon SearchNote(string notebookUniqueId, string title)
        {
            var nb = GetNotebook(notebookUniqueId);
            if (nb == null) return null;
            var result = nb.Items.SingleOrDefault(item => item.Title.ToLower().Equals(title.ToLower()));
            if (result == null) return null;
            return result;
        }

        /// <summary>
        /// Search for all notes contains a spesific value in Title or Description.
        /// </summary>
        /// <param name="query">value to search</param>
        /// <returns>List of all notes contains search value</returns>
        public static List<NoteDataCommon> Search(string query)
        {
            return _notesDataSource.AllGroups.SelectMany(g => ((NoteBook)g).Items).Where(i => i.Title.ToLower().Contains(query.ToLower()) || i.Description.ToLower().Contains(query.ToLower())).ToList();
        }

        /// <summary>
        /// Search all notes and notebooks for specific Unique Id
        /// </summary>
        /// <param name="uniqueId">Unique Id for note or notebook</param>
        /// <returns>Note or Notebook item</returns>
        public static NoteDataCommon Find(string uniqueId)
        {
            var notebook = _notesDataSource.AllGroups.SingleOrDefault(g => g.UniqueId.Equals(uniqueId));
            if (notebook != null) return notebook;
            return (NoteDataCommon)_notesDataSource.AllGroups.SelectMany(item => ((NoteBook)item).Items).SingleOrDefault(i => i.UniqueId.Equals(uniqueId));
        }

        public NoteDataCommon CurrentNote { get; set; }

        /// <summary>
        /// Call DataManager.LoadAsync, after load operation completes raise Completed event
        /// </summary>
        /// <returns></returns>
        async public Task Load()
        {
            try
            {
                // Lab #4 - Files
                DataLoaded = false;
                Completed(this, null);
            }
            catch (Exception ex)
            {
                DataLoaded = false;
                Completed(this, ex);
            }
        }

        /// <summary>
        /// Delete Note or Notebook.
        /// </summary>
        /// <param name="itemToDelete">NoteDataCommon of Notebook or Note</param>
        /// <returns></returns>
        async public static Task DeleteAsync(NoteDataCommon itemToDelete)
        {
            if (itemToDelete.Type == NoteTypes.Notebook)
            {
                _notesDataSource.AllGroups.Remove(itemToDelete);
                // Lab #4 - Files
            }
            else
            {
                var nb = itemToDelete.NoteBook;
                switch (itemToDelete.Type)
                {
                    case NoteTypes.Food:
                        nb.FoodSection.Remove(itemToDelete);
                        break;
                    case NoteTypes.Note:
                        nb.NotesSection.Remove(itemToDelete);
                        break;
                    case NoteTypes.ToDo:
                        nb.ToDoSection.Remove(itemToDelete);
                        break;
                }

                // Lab #4 - Files
            }
        }

        /// <summary>
        /// Add new Notebook to AllGroups collection.
        /// </summary>
        /// <param name="nb">New Notebook</param>
        public static void AddNoteBook(NoteDataCommon nb)
        {
            _notesDataSource.AllGroups.Add(nb);
        }

        /// <summary>
        /// Add Note to spesific notebook and save changes.
        /// </summary>
        /// <param name="note">Note to add</param>
        /// <param name="notebook">Target Notebook</param>
        public static void AddToNoteBook(NoteDataCommon note, NoteBook notebook)
        {
            switch (note.Type)
            {
                case NoteTypes.Food:
                    notebook.FoodSection.Add(note);
                    break;
                case NoteTypes.Note:
                    notebook.NotesSection.Add(note);
                    break;
                case NoteTypes.ToDo:
                    notebook.ToDoSection.Add(note);
                    break;
            }
            note.NoteBook = notebook;
            // Lab #4 - Files
        }

        /// <summary>
        /// Add note to spesific notebook based on notebook Uniqueid
        /// </summary>
        /// <param name="note">Note to add</param>
        /// <param name="targetNotebookId">Target notebook Uniqueid</param>
        public static void Add(NoteDataCommon note, string targetNotebookId)
        {
            var Targetnotebook = (NoteBook)_notesDataSource.AllGroups.SingleOrDefault(nb => nb.UniqueId.Equals(targetNotebookId));
            if (Targetnotebook.Items.Any(i => i.UniqueId == note.UniqueId))
                return;
            AddToNoteBook(note, Targetnotebook);
        }

        /// <summary>
        /// Move note from one notebook to another, this operation will remove the note from current notebook and then add it to target notebook.
        /// </summary>
        /// <param name="noteId">Note Uniqueid</param>
        /// <param name="targetNotebookId">Target Notebook</param>
        public static void Move(string noteId, string targetNotebookId)
        {
            var note = GetItem(noteId);
            var Targetnotebook = GetNotebook(targetNotebookId);

            if (note == null || Targetnotebook == null) return;//throw new NullReferenceException("Cannot find NoteBook or Note based on Unique Id");

            var type = note.Type;
            switch (type)
            {
                case NoteTypes.Food:
                    note.NoteBook.FoodSection.Remove(note);
                    Targetnotebook.FoodSection.Add(note);
                    break;
                case NoteTypes.Note:
                    note.NoteBook.NotesSection.Remove(note);
                    Targetnotebook.NotesSection.Add(note);
                    break;
                case NoteTypes.ToDo:
                    note.NoteBook.ToDoSection.Remove(note);
                    Targetnotebook.ToDoSection.Add(note);
                    break;
            }
            note.NoteBook = Targetnotebook;
            // Lab #4 - Files
        }

        public void BuildDummyData()
        {
            this.AllGroups.Clear();

            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                      "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            ObservableCollection<ToDo> todo = new ObservableCollection<ToDo>();
            todo.Add(new ToDo("Test"));
            todo.Add(new ToDo("Test2") { Done = true });
            todo.Add(new ToDo("Test3"));
            todo.Add(new ToDo("Test"));
            todo.Add(new ToDo("Test2") { Done = true });
            todo.Add(new ToDo("Test3"));
            todo.Add(new ToDo("Test"));
            todo.Add(new ToDo("Test2") { Done = true });
            todo.Add(new ToDo("Test3"));
            todo.Add(new ToDo("Test"));
            todo.Add(new ToDo("Test2") { Done = true });
            todo.Add(new ToDo("Test3"));
            todo.Add(new ToDo("Test"));
            todo.Add(new ToDo("Test2") { Done = true });
            todo.Add(new ToDo("Test3"));

            var group1 = new NoteBook("Getting Started");

            var food1 = new FoodDataItem("Wheat Germ Whole-Wheat Buttermilk Pancakes", group1);
            food1.Description = "1. In a medium bowl, mix eggs with oil and buttermilk. Stir in baking soda, wheat germ, salt and flour; mix until blended. \n2. Heat a lightly oiled griddle or frying pan over medium-high heat. Pour or scoop the batter onto the griddle, using approximately 1/4 cup for each pancake. Brown on both sides, turning once. ";
            food1.Tags.Add("2 eggs, lightly beaten");
            food1.Tags.Add("1/4 cup canola oil");
            food1.Tags.Add("2 cups buttermilk");
            food1.Tags.Add("1/2 cup wheat germ");
            food1.Tags.Add("1/2 teaspoon salt");
            food1.Tags.Add("1 1/2 cups whole wheat pastry flour");
            food1.Images.Add("Assets/foodPreview1.jpg");
            food1.Images.Add("Assets/foodPreview2.jpg");
            group1.FoodSection.Add(food1);

            var todo1 = new ToDoDataItem("Shopping List", group1);
            todo1.ToDo = todo;
            group1.ToDoSection.Add(todo1);

            var note1 = new NoteDataItem("24 US States That Have Sweeping Self-Defense Laws Just Like Florida’s", group1);
            note1.Description = "\"Stand Your Ground,\" \"Shoot First,\" \"Make My Day\" – state laws asserting an expansive right to self-defense – have come into focus after the February 2012 killing of 17-year-old Trayvon Martin." +
                "\n\nIn 2005, Florida became the first state to explicitly expand a person’s right to use deadly force for self-defense. Deadly force is justified if a person is gravely threatened, in the home or \"any other place where he or she has a right to be.\" " +
                "\n\nMost states have long allowed the use of reasonable force, sometimes including deadly force, to protect oneself inside one’s home — the so-called Castle Doctrine. Outside the home, people generally still have a \"duty to retreat\" from an attacker, if possible, to avoid confrontation. In other words, if you can get away and you shoot anyway, you can be prosecuted. In Florida, there is no duty to retreat. You can \"stand your ground\" outside your home, too." +
                "\n\nIf self-defense is invoked in Florida, the person is immune from criminal or civil prosecution." +
                "\n\nIn the Martin case, the local police chief has said that they did not arrest the shooter, George Zimmerman, because their initial investigation supported his self-defense claim, and that they were therefore prohibited from making an arrest or prosecution. (The police report on the shooting refers to it as an \"unnecessary killing to prevent unlawful act.\")" +
                "\n\nThe police chief has since temporarily stepped down, after a vote of no-confidence from the city. The case is being investigated by the Department of Justice and a Florida state attorney. A grand jury will convene on April 10 to decide whether charges can be brought against Zimmerman." +
                "\n\nZimmerman’s lawyer said in an interview with ABC News that Zimmerman will be protected under Florida’s self-defense law." +
                "\n\nIn Florida, a homicide case can be thrown out by a judge before trial because the defendant successfully invokes self-defense. The burden is on the prosecution to disprove the claim in order to bring charges, rather than do so in the trial. The Florida state attorney leading the prosecution told ABC news that the self-defense law means it is \"more difficult than a normal criminal case\" to bring charges." +
                "\n\nFlorida is not alone in its expansive definition of self-defense. Twenty-four other states now allow people to stand their ground. Most of these laws were passed after Florida’s. (Some states never had a duty to retreat to begin with.)" +
                "\n\nHere’s a rundown of the states with laws mirroring the one in Florida, where there’s no duty to retreat in public places and where, in most cases, self-defense claims have some degree of immunity in court. (The specifics of what kind of immunity, and when the burden of proof lies on the prosecution, vary from state to state.)" +
                "\n\nMany of the laws were originally advocated as a way to address domestic abuse cases 2014 how could a battered wife retreat if she was attacked in her own home? Such legislation also has been recently pushed by the National Rifle Association and other gun-rights groups.";

            group1.NotesSection.Add(note1);


            this.AllGroups.Add(group1);

            var group2 = new NoteBook("Group Title: 2");

            var todo2 = new ObservableCollection<ToDo>();
            todo2.Add(new ToDo("Test") { Done = true });
            todo2.Add(new ToDo("Test2") { Done = true });
            todo2.Add(new ToDo("Test3") { Done = true });
            todo2.Add(new ToDo("Test") { Done = true });
            todo2.Add(new ToDo("Test2") { Done = true });
            todo2.Add(new ToDo("Test3") { DueDate = DateTime.Now.AddSeconds(50) });
            todo2.Add(new ToDo("Test3") { DueDate = DateTime.Now.AddSeconds(50) });
            todo2.Add(new ToDo("AAAAAAAAAAAA") { DueDate = DateTime.Now.AddSeconds(50) });
            var todo3 = new ToDoDataItem("To Do 3", group2);
            todo3.ToDo = todo2;
            group2.ToDoSection.Add(todo3);

            note1 = new NoteDataItem("24 US States That Have Sweeping Self-Defense Laws Just Like Florida’s ", group2);
            note1.Images.Add("Assets/seaPreview.png");

            var note2 = new NoteDataItem("Note 5", group2);
            var note3 = new NoteDataItem("Note 6", group2);
            var note4 = new NoteDataItem("Note 7", group2);
            var note5 = new NoteDataItem("Note 8", group2);

            group2.NotesSection.Add(note1);
            group2.NotesSection.Add(note2);
            group2.NotesSection.Add(note3);
            group2.NotesSection.Add(note4);
            group2.NotesSection.Add(note5);

            this.AllGroups.Add(group2);
        }

        /// <summary>
        /// In order to display design time data we need to create static data.
        /// </summary>
        public NotesDataSource()
        {
            BuildDummyData();
        }
    }
}
