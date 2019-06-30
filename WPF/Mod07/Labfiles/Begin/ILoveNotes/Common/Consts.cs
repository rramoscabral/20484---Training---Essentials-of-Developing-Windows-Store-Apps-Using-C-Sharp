using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILoveNotes.Common
{
    public class Consts
    {
        public const int RecordingLimitation = 60;
        public const string DefaultTitleText = "Add a title";
        public const string DefaultDescriptionText = "Type your note here";
        public const string DefaultAddressText = "Type your address here";

        public const string MoveNote = "Are you sure you want to move this note from '{0}' to '{1}' notebook?";
        public const string SkyDriveSettingsKey = "SkyDriveBackup";
        public const string SkyDriveStorageFilesKey = "SkyDriveBackupFile";
        public const string SearchSplitter = "-->";
        public const string AudioFileName = "{0}.m4a";
        public const string MapPreviewFileFormat = "MapPreview-{0}.png";
        public const string FoodIcon = "Assets/Icons/food.png";
        public const string ToDoIcon = "Assets/Icons/todo.png";
        public const string NoteIcon = "Assets/Icons/note.png";
        public const string BackupFileName = "ILoveNotes-Backup.txt";
    }
}
