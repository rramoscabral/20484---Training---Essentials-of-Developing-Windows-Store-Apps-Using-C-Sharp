using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace ILoveNotes.Common
{
    public class ToastManager
    {
        /// <summary>
        /// Checks if there is reminder exists for specific ToDo
        /// </summary>
        /// <param name="tdItem">ToDo Item</param>
        /// <returns>True if Reminder exists</returns>
        public static bool HasReminder(ToDo tdItem)
        {
            TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
            IReadOnlyList<ScheduledTileNotification> scheduled = updater.GetScheduledTileNotifications();
            if (scheduled.Any(i => i.Id.Equals(tdItem.UniqueId)))
            {
                tdItem.HasReminder = true;
                return true;
            }
            else
            {
                tdItem.HasReminder = false;
                return false;
            }
        }

        /// <summary>
        /// Remove Reminder for specific ToDo
        /// </summary>
        /// <param name="tdItem">ToDo item</param>
        public static void Remove(ToDo tdItem)
        {
            ToastNotifier updater = ToastNotificationManager.CreateToastNotifier();
            IReadOnlyList<ScheduledToastNotification> scheduled = updater.GetScheduledToastNotifications();
            for (var i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Id == tdItem.UniqueId)
                {
                    updater.RemoveFromSchedule(scheduled[i]);
                }
            }
        }

        /// <summary>
        /// Searching Notifications for spesific ToDo item unique Id.
        /// </summary>
        /// <param name="uniqueId">ToDo item unique id</param>
        /// <returns>If notification exists return the ScheduledToastNotification</returns>
        private static ScheduledToastNotification GetNotificationForToDo(string uniqueId)
        {
            ToastNotifier updater = ToastNotificationManager.CreateToastNotifier();
            IReadOnlyList<ScheduledToastNotification> scheduled = updater.GetScheduledToastNotifications();
            if (scheduled.Count == 0)
                return null;
            else
            {
                foreach (var item in scheduled)
                {
                    if (item.Id.Equals(uniqueId))
                        return item;
                }
                return null;
            }
        }

        /// <summary>
        /// Define Toast notification for ToDo item under ToDo Note
        /// </summary>
        /// <param name="Notetitle">ToDo Note Title</param>
        /// <param name="tdItem">ToDo Item</param>
        public static void SetScheduledToast(string Notetitle, ToDo tdItem)
        {
            if (tdItem.DueDate < DateTime.Now)
            {
                tdItem.HasReminder = false;
                return;
            }

            var notifier = ToastNotificationManager.CreateToastNotifier();
            // Make sure notifications are enabled
            if (notifier.Setting != NotificationSetting.Enabled)
            {
                Helpers.ShowMessageAsync("Notifications are currently disabled", "Set Reminder");
                return;
            }

            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(Notetitle));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(tdItem.Title));

            var d = new DateTimeOffset(tdItem.DueDate);

            ScheduledToastNotification toast = new ScheduledToastNotification(toastXml, d);// GetNotificationForToDo(tdItem.UniqueId);

            toast = new ScheduledToastNotification(toastXml, d);
            toast.Id = tdItem.UniqueId;
            tdItem.HasReminder = true;
            notifier.AddToSchedule(toast);
        }
    }
}

