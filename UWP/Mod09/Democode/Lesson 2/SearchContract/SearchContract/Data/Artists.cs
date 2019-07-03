using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchContract.Data
{
    public static class Artists
    {
        public static List<Artist> Beatles;
        static Artists()
        {
            Beatles = new List<Artist>();
            Beatles.Add(new Artist { Title = "John", Description = "Item", Image = "ms-appx:///Assets/SmallLogo.png" });
            Beatles.Add(new Artist { Title = "Paul", Description = "Item", Image = "ms-appx:///Assets/SmallLogo.png" });
            Beatles.Add(new Artist { Title = "Ringo", Description = "Item", Image = "ms-appx:///Assets/SmallLogo.png" });
            Beatles.Add(new Artist { Title = "George", Description = "Item", Image = "ms-appx:///Assets/SmallLogo.png" });
        }
    }
}
