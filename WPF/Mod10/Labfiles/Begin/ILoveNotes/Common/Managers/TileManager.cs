using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using ILoveNotes.DataModel;
using NotificationsExtensions.TileContent;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;   
using Windows.UI.Xaml;

namespace ILoveNotes.Common
{
    public static class TileManager
    {
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
        
        //////////////////////////////////////////
        //Lab 10 : Exercise 1 , Task 1.2 : Implement the SetMeainTile method
        //////////////////////////////////////////
       

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

        //Lab 10: Exercise 2, Task 1.1 : Create the Pin method which supports Secondary Tile creation.
        

        //Lab 10, Exercise 2, Task 1.5 : Implement the Update Secondary Tile method
        

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

        //Lab 10, Exercise 2, Task 2.3 : uncomment the CreateSecondaryTileWithImage method and review it.
        /*private static void CreateSecondaryTileWithImage(NoteDataCommon item)
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
            
            // Create the notification based on the XML content.
            var tileNotification = tileContent.CreateNotification();
            // Create a secondary tile updater and pass it the secondary tileId
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId);

            // Send the notification to the secondary tile.
            tileUpdater.Update(tileNotification);
        }*/


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

        //Lab 10, Exercise 2 : Task 1.4 : uncomment the CreateSecondaryNoteBook and review the code
        /*private static void CreateSecondaryNoteBook(NoteBook item)
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
                    
            // Create the notification based on the XML content.
            var tileNotification = tileContent.CreateNotification();
            // Create a secondary tile updater and pass it the secondary tileId
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.UniqueId);

            // Send the notification to the secondary tile.
            tileUpdater.Update(tileNotification);
        }*/
     }
}
