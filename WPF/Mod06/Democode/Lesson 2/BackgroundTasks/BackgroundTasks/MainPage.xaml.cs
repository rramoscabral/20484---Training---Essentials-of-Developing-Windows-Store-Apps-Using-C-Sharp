using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BackgroundTasks
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private bool IsTaskRegistered()
        {
            var taskRegistered = false;

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "DemoBackgroundTask")
                {
                    taskRegistered = true;
                    break;
                }
            }

            return taskRegistered;
        }

        private void RegisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            if (IsTaskRegistered())
            {
                result.Text = "Task is already registered.";
                return;
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = "DemoBackgroundTask";
            builder.TaskEntryPoint = "Tasks.DemoBackgroundTask";
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));

            BackgroundTaskRegistration task = builder.Register();
            task.Completed += new BackgroundTaskCompletedEventHandler(OnBackgroundTaskCompleted);

            result.Text = "Task registered successfully.";
        }


        private void OnBackgroundTaskCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            UpdateLastRunTime();
        }

        private void UpdateLastRunTime()
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("LastTaskRunTime"))
                    result.Text = string.Format("Task last ran on: {0}", ApplicationData.Current.LocalSettings.Values["LastTaskRunTime"]);
                else
                    result.Text = "Task has not run yet.";
            });
        }

        private void RefreshTaskStatus(object sender, RoutedEventArgs e)
        {
            UpdateLastRunTime();
        }

        private void UnregisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            result.Text = "";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "DemoBackgroundTask")
                {
                    task.Value.Unregister(true);
                    result.Text = "Task unregistered.";
                    return;
                }
            }

            result.Text = "Task is not registered.";
        }
    }
}
