using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace Tasks
{
    public sealed class DemoBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            ApplicationData.Current.LocalSettings.Values["LastTaskRunTime"] = DateTime.Now.ToString();
        }
    }
}
