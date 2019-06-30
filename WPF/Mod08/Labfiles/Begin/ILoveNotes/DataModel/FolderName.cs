using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILoveNotes.DataModel
{
    [Flags()]
    public enum FolderNames
    {
        Notebooks, 
        Images,
        Audio
    }
}
