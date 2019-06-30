using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using NotificationsExtensions.TileContent;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace ILoveNotes.Common
{
    /// <summary>
    /// The main porpose of TileManager is creating Wide and Squre Tile templates for Notes.
    /// Squre -> http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#square_tiles
    /// Wide -> http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#wide_tiles
    /// </summary>
    public static class TileManager
    {
        /// <summary>
        /// Checks if there is a Pin exists for a specific id.
        /// </summary>
        /// <param name="itemId">Note Unique ID</param>
        /// <returns>True for Pin Exists for Note or False if not exists</returns>
        public static bool PinExists(string itemId)
        {
            return SecondaryTile.Exists(itemId);
        }

        /// <summary>
        /// Update Tile Information for Note, changing the default template of the tile to specific template based on the Note Type.
        /// </summary>
        /// <param name="UniqueId">Note Unique ID</param>
        public static void UpdateSecondaryTile(string UniqueId)
        {
            // Determine whether to pin or unpin.
            if (SecondaryTile.Exists(UniqueId))
            {
                var item = NotesDataSource.Find(UniqueId);
                if (item.Images.Count == 0)
                {
                    if (item.Type == NoteTypes.ToDo)
                        CreateTextToDoTile(item);
                    else
                        CreateTextOnlyTile(item);

                    return;
                }

                switch (item.Type)
                {
                    case DataModel.NoteTypes.ToDo:
                        CreateSeconderyForToDo(item as ToDoDataItem);
                        break;
                    case DataModel.NoteTypes.Notebook:
                        CreateSeconderyNoteBook(item as NoteBook);
                        break;
                    default:
                        CreateSeconderyWithImage(item);
                        break;
                }
            }
        }

        /// <summary>
        /// This method take a Note item and from his images choose one random image.
        /// If there is no images then return the default image preview based on Note type.
        /// </summary>
        /// <param name="item">Note item to get the preview image from</param>
        /// <param name="wide">true for wide image(only apply for default images)</param>
        /// <returns></returns>
        private static Uri GetPreviewImage(NoteDataCommon item, bool wide)
        {
            Uri logo;
            if (item.Type == DataModel.NoteTypes.Notebook)
            {
                var noteBook = item as NoteBook;

                var images = noteBook.Items.SelectMany(i => i.Images).Where(list => list.Length > 0).ToList();

                var result = Helpers.GetRandomImage(images);
                if (result == null)
                    logo = wide ? new Uri(Helpers.GetPreviewImageForNote(NoteTypes.Notebook, true)) : new Uri(Helpers.GetPreviewImageForNote(NoteTypes.Notebook, false));
                else
                    logo = new Uri(result.ToBaseUrl());
            }
            else
            {
                var result = Helpers.GetRandomImage(item.Images.ToList());
                if (result == null)
                    logo = new Uri(Helpers.GetPreviewImageForNote(item.Type, wide));
                else
                    logo = new Uri(result.ToBaseUrl());
            }

            return logo;
        }

        /// <summary>
        /// Creates or Remove Pin for Note
        /// If Pin exists then create new SecondaryTile with the Note Unique Id (to obtain the exists SecondaryTile) and call RequestDeleteForSelectionAsync to remove the SecondaryTile.
        /// If it's a new Pin then create new SecondaryTile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item">Note item to Pin</param>
        /// <returns>True for completion or False if an error occurred.</returns>
        async public static Task<bool> Pin(object sender, NoteDataCommon item)
        {
            // Determine whether to pin or unpin.
            if (SecondaryTile.Exists(item.UniqueId))
            {
                SecondaryTile secondaryTile = new SecondaryTile(item.UniqueId);
                Rect pinButtonRect = Helpers.GetElementRect((FrameworkElement)sender);
                return await secondaryTile.RequestDeleteForSelectionAsync(pinButtonRect, Windows.UI.Popups.Placement.Above);
            }
            else
            {
                Uri logo = GetPreviewImage(item, false);
                Uri logoWide = GetPreviewImage(item, true);

                string tileActivationArguments = string.Format("{0}{1}", Consts.SecondaryTileFormat, item.UniqueId);
                SecondaryTile secondaryTile = new SecondaryTile(item.UniqueId,
                                                                item.Title,
                                                                tileActivationArguments,
                                                                logo, TileSize.Square150x150);
                secondaryTile.VisualElements.Wide310x150Logo = logoWide;

                Rect pinButtonRect = Helpers.GetElementRect((FrameworkElement)sender);
                return await secondaryTile.RequestCreateForSelectionAsync(pinButtonRect, Windows.UI.Popups.Placement.Above);
            }
        }

        /// <summary>
        /// TileWideText01
        /// One header string in larger text on the first line, four strings of regular text on the next four lines. Text does not wrap.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideText01
        /// 
        /// TileSquareText01
        /// One header string in larger text on the first line; three strings of regular text on each of the next three lines. Text does not wrap.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileSquareText01
        /// </summary>
        /// <param name="item"></param>
        private static void CreateTextToDoTile(NoteDataCommon item)
        {
            ITileWide310x150Text01 tileContent = TileContentFactory.CreateTileWide310x150Text01();
            tileContent.RequireSquare150x150Content = true;
            tileContent.TextHeading.Text = item.Title;
            var count = item.ToDo.Count >= 4 ? 4 : item.ToDo.Count;
            for (var i = 0; i < count; i++)
            {
                var value = item.ToDo[i].Done == true ? string.Format("{0} {1}", "√", item.ToDo[i].Title) : item.ToDo[i].Title;
                switch (i)
                {
                    case 0:
                        tileContent.TextBody1.Text = value;
                        break;
                    case 1:
                        tileContent.TextBody2.Text = value;
                        break;
                    case 2:
                        tileContent.TextBody3.Text = value;
                        break;
                    case 3:
                        tileContent.TextBody4.Text = value;
                        break;
                }
            }

            ITileSquare150x150Text01 squareTileContent = TileContentFactory.CreateTileSquare150x150Text01();
            squareTileContent.TextHeading.Text = item.Title;
            count = item.ToDo.Count >= 3 ? 3 : item.ToDo.Count;
            for (var i = 0; i < count; i++)
            {
                var value = item.ToDo[i].Done == true ? string.Format("{0} {1}", "√", item.ToDo[i].Title) : item.ToDo[i].Title;
                switch (i)
                {
                    case 0:
                        squareTileContent.TextBody1.Text = value;
                        break;
                    case 1:
                        squareTileContent.TextBody2.Text = value;
                        break;
                    case 2:
                        squareTileContent.TextBody3.Text = value;
                        break;
                }
            }

            tileContent.Branding = TileBranding.Name;
            tileContent.Square150x150Content = squareTileContent;

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId).Update(tileContent.CreateNotification());
        }

        /// <summary>
        /// TileWideText09
        /// One header string in larger text over one string of regular text wrapped over a maximum of four lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideText09
        /// 
        /// TileSquareText02
        /// One header string in larger text on the first line, over one string of regular text wrapped over a maximum of three lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileSquareText02
        /// </summary>
        /// <param name="item"></param>
        private static void CreateTextOnlyTile(NoteDataCommon item)
        {
            ITileWide310x150Text09 tileContent = TileContentFactory.CreateTileWide310x150Text09();
            tileContent.RequireSquare150x150Content = true;
            tileContent.TextHeading.Text = item.Title;
            tileContent.TextBodyWrap.Text = item.Description.Equals(Consts.DefaultDescriptionText) ? string.Empty : item.Description;

            ITileSquare150x150Text02 squareTileContent = TileContentFactory.CreateTileSquare150x150Text02();
            squareTileContent.TextHeading.Text = item.Title;
            squareTileContent.TextBodyWrap.Text = item.Description.Equals(Consts.DefaultDescriptionText) ? string.Empty : item.Description;

            tileContent.Branding = TileBranding.Name;
            tileContent.Square150x150Content = squareTileContent;

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId).Update(tileContent.CreateNotification());
        }

        /// <summary>
        /// TileWideSmallImageAndText02
        /// On the left, one small image; on the right, one header string in larger text on the first line over four strings of regular text on the next four lines. Text does not wrap.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideSmallImageAndText02
        /// 
        /// TileSquareText01
        /// One header string in larger text on the first line; three strings of regular text on each of the next three lines. Text does not wrap.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileSquareText01
        /// </summary>
        /// <param name="item"></param>
        private static void CreateSeconderyForToDo(ToDoDataItem item)
        {
            ITileWide310x150SmallImageAndText02 tileContent = TileContentFactory.CreateTileWide310x150SmallImageAndText02();
            var imgUrl = GetPreviewImage(item, false).ToString();
            var imgWideUrl = GetPreviewImage(item, true).ToString();

            tileContent.Image.Src = imgWideUrl;
            tileContent.Image.Alt = item.Title;
            tileContent.RequireSquare150x150Content = true;
            tileContent.TextHeading.Text = item.Title;
            var count = item.ToDo.Count >= 4 ? 4 : item.ToDo.Count;
            for (var i = 0; i < count; i++)
            {
                var value = item.ToDo[i].Done == true ? string.Format("{0} {1}", "√", item.ToDo[i].Title) : item.ToDo[i].Title;
                switch (i)
                {
                    case 0:
                        tileContent.TextBody1.Text = value;
                        break;
                    case 1:
                        tileContent.TextBody2.Text = value;
                        break;
                    case 2:
                        tileContent.TextBody3.Text = value;
                        break;
                    case 3:
                        tileContent.TextBody4.Text = value;
                        break;
                }
            }

            ITileSquare150x150Text01 squareTileContent = TileContentFactory.CreateTileSquare150x150Text01();
            squareTileContent.TextHeading.Text = item.Title;
            count = item.ToDo.Count >= 3 ? 3 : item.ToDo.Count;
            for (var i = 0; i < count; i++)
            {
                var value = item.ToDo[i].Done == true ? string.Format("{0} {1}", "√", item.ToDo[i].Title) : item.ToDo[i].Title;
                switch (i)
                {
                    case 0:
                        squareTileContent.TextBody1.Text = value;
                        break;
                    case 1:
                        squareTileContent.TextBody2.Text = value;
                        break;
                    case 2:
                        squareTileContent.TextBody3.Text = value;
                        break;
                }
            }

            tileContent.Branding = TileBranding.Name;
            tileContent.Square150x150Content = squareTileContent;
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId).Update(tileContent.CreateNotification());
        }

        /// <summary>
        /// TileWideSmallImageAndText02
        /// On the left, one small image; on the right, one header string in larger text on the first line over four strings of regular text on the next four lines. Text does not wrap.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideSmallImageAndText02
        /// 
        /// TileSquarePeekImageAndText02
        /// Top: Square image, no text. Bottom: One header string in larger text on the first line, over one string of regular text wrapped over a maximum of three lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileSquarePeekImageAndText02
        /// </summary>
        /// <param name="item"></param>
        private static void CreateSeconderyNoteBook(NoteBook item)
        {
            var imgUrl = GetPreviewImage(item, false).ToString();
            var imgWideUrl = GetPreviewImage(item, true).ToString();

            ITileWide310x150SmallImageAndText02 tileContent = TileContentFactory.CreateTileWide310x150SmallImageAndText02();
            tileContent.TextHeading.Text = item.Title;
            var count = item.Items.Count < 4 ? item.Items.Count : 4;
            for (var i = 0; i < count; i++)
            {
                var noteTitle = item.Items[i].Title;
                switch (i)
                {
                    case 0:
                        tileContent.TextBody1.Text = noteTitle;
                        break;
                    case 1:
                        tileContent.TextBody2.Text = noteTitle;
                        break;
                    case 2:
                        tileContent.TextBody3.Text = noteTitle;
                        break;
                    case 3:
                        tileContent.TextBody3.Text = noteTitle;
                        break;
                }
            }

            tileContent.Image.Src = imgWideUrl;
            tileContent.Image.Alt = item.Title;

            tileContent.Branding = TileBranding.Name;

            ITileSquare150x150PeekImageAndText02 squareTileContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
            squareTileContent.Image.Src = imgUrl;
            squareTileContent.Image.Alt = item.Title;
            squareTileContent.TextHeading.Text = item.Title;
            squareTileContent.TextBodyWrap.Text = item.Description.Equals(Consts.DefaultDescriptionText) ? string.Empty : item.Description.ToShortString();
            tileContent.Square150x150Content = squareTileContent;

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId).Update(tileContent.CreateNotification());
        }

        /// <summary>
        /// TileWideSmallImageAndText04 
        /// On the left, one small image; on the right, one header string of larger text on the first line over one string of regular text wrapped over a maximum of four lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideSmallImageAndText04
        /// 
        /// TileSquarePeekImageAndText02
        /// Top: Square image, no text. Bottom: One header string in larger text on the first line, over one string of regular text wrapped over a maximum of three lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileSquarePeekImageAndText02
        /// </summary>
        /// <param name="item"></param>
        private static void CreateSeconderyWithImage(NoteDataCommon item)
        {
            var imgUrl = GetPreviewImage(item, false).ToString();
            var imgWideUrl = GetPreviewImage(item, true).ToString();

            ITileWide310x150SmallImageAndText04 tileContent = TileContentFactory.CreateTileWide310x150SmallImageAndText04();
            tileContent.TextHeading.Text = item.Title;
            tileContent.TextBodyWrap.Text = item.Description.Equals(Consts.DefaultDescriptionText) ? string.Empty : item.Description.ToShortString();
            tileContent.Image.Src = imgWideUrl;
            tileContent.Image.Alt = item.Title;

            ITileSquare150x150PeekImageAndText02 squareTileContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
            squareTileContent.Image.Src = imgUrl;
            squareTileContent.Image.Alt = item.Title;
            squareTileContent.TextHeading.Text = item.Title;
            squareTileContent.TextBodyWrap.Text = item.Description.Equals(Consts.DefaultDescriptionText) ? string.Empty : item.Description.ToShortString();
            tileContent.Square150x150Content = squareTileContent;

            tileContent.Branding = TileBranding.Name;
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId).Update(tileContent.CreateNotification());
        }

        /// <summary>
        /// TileWideImageAndText01
        /// One wide image over one string of regular text wrapped over a maximum of two lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideImageAndText01
        /// 
        /// TileSquarePeekImageAndText04
        /// Top: Square image, no text. Bottom: One string of regular text wrapped over a maximum of four lines.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileSquarePeekImageAndText04
        /// </summary>
        /// <param name="item"></param>
        private static void CreateNotePreviewTile(NoteDataCommon item)
        {
            var imgWideUrl = GetPreviewImage(item, true).ToString();
            ITileWide310x150ImageAndText01 tileContent = TileContentFactory.CreateTileWide310x150ImageAndText01();
            tileContent.TextCaptionWrap.Text = item.Title;
            tileContent.Image.Src = imgWideUrl;
            tileContent.Image.Alt = item.Title;

            var imgUrl = GetPreviewImage(item, false).ToString();
            ITileSquare150x150PeekImageAndText04 squareTileContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText04();
            squareTileContent.Image.Src = imgUrl;
            squareTileContent.Image.Alt = item.Title;
            squareTileContent.TextBodyWrap.Text = item.Title;
            tileContent.Square150x150Content = squareTileContent;

            tileContent.Branding = TileBranding.Logo;
            ScheduledTileNotification schduleTile = new ScheduledTileNotification(tileContent.GetXml(), DateTime.Now.AddSeconds(10));
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(schduleTile);
        }

        /// <summary>
        /// One large square image with four smaller square images to its right, no text.
        /// http://msdn.microsoft.com/en-us/library/windows/apps/hh761491#TileWideImageCollection
        /// </summary>
        /// <param name="item">The current Note item, this will allow us to get the notebooks for the image collection.</param>
        private static void TryCreateCollectionTile(NoteDataCommon item)
        {
            var totalImages = item.NoteBook.Items.SelectMany(i => i.Images).ToList();
            if (totalImages.Count >= 5)
            {
                ITileWide310x150ImageCollection tileContent = TileContentFactory.CreateTileWide310x150ImageCollection();
                for (var i = 0; i < 5; i++)
                {
                    var imgUrl = totalImages[i].ToBaseUrl();
                    switch (i)
                    {
                        case 0:
                            tileContent.ImageMain.Src = imgUrl;
                            tileContent.ImageMain.Alt = item.NoteBook.Title;
                            break;
                        case 1:
                            tileContent.ImageSmallColumn1Row1.Src = imgUrl;
                            tileContent.ImageSmallColumn1Row1.Alt = item.NoteBook.Title;
                            break;
                        case 2:
                            tileContent.ImageSmallColumn1Row2.Src = imgUrl;
                            tileContent.ImageSmallColumn1Row2.Alt = item.NoteBook.Title;
                            break;
                        case 3:
                            tileContent.ImageSmallColumn2Row1.Src = imgUrl;
                            tileContent.ImageSmallColumn2Row1.Alt = item.NoteBook.Title;
                            break;
                        case 4:
                            tileContent.ImageSmallColumn2Row2.Src = imgUrl;
                            tileContent.ImageSmallColumn2Row2.Alt = item.NoteBook.Title;
                            break;
                    }
                }

                ITileSquare150x150PeekImageAndText04 squareTileContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText04();
                squareTileContent.Image.Src = totalImages[0].ToBaseUrl();
                squareTileContent.Image.Alt = item.Title;
                squareTileContent.TextBodyWrap.Text = item.Title;
                tileContent.Square150x150Content = squareTileContent;

                ScheduledTileNotification schduleTile = new ScheduledTileNotification(tileContent.GetXml(), DateTime.Now.AddSeconds(10));
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(schduleTile);
            }
        }

        /// <summary>
        /// Update Application Tile with the changed Note Item
        /// </summary>
        /// <param name="item">Last preview Note</param>
        public static void SetTiles(NoteDataCommon item)
        {
            var tile = TileUpdateManager.CreateTileUpdaterForApplication();
            if (tile.Setting != NotificationSetting.Enabled)
                return;

            //TryCreateCollectionTile(item);


            CreateNotePreviewTile(item);
        }
    }
}
