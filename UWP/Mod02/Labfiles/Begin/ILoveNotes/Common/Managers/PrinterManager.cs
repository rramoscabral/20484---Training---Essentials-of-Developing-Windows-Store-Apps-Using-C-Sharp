using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILoveNotes.Data;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;

namespace ILoveNotes.Common
{
    internal enum DisplayContent : int
    {
        Text = 1,
        Images = 2,
        TextAndImages = 3
    }

    public class PrinterManager : IDisposable
    {
        protected ILoveNotes.Common.LayoutAwarePage rootPage = null;
        protected PrintDocument printDocument = null;
        protected IPrintDocumentSource printDocumentSource = null;
        protected List<UIElement> printPreviewPages = null;
        private const double ApplicationContentMarginLeft = 0.075;
        private const double ApplicationContentMarginTop = 0.03;
        private NoteDataCommon _noteForPrinting;

        public PrinterManager(ILoveNotes.Common.LayoutAwarePage _rootPage, NoteDataCommon note)
        {
            _noteForPrinting = note;
            rootPage = _rootPage;
            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            printDocument.Paginate += CreatePrintPreviewPages;
            printDocument.GetPreviewPage += GetPrintPreviewPage;
            printDocument.AddPages += AddPrintPages;

            printPreviewPages = new List<UIElement>();

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        private bool ShowTags = true;

        internal DisplayContent imageText = DisplayContent.TextAndImages;
        private bool ShowText
        {
            get { return ((int)imageText & (int)DisplayContent.Text) == (int)DisplayContent.Text; }
        }

        private bool ShowImage
        {
            get { return ((int)imageText & (int)DisplayContent.Images) == (int)DisplayContent.Images; }
        }

        public void UnregisterForPrinting()
        {
            printDocument = null;
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
        }

        protected event EventHandler pagesCreated;

        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = e.Request.CreatePrintTask("I Love Notes", sourceRequested => sourceRequested.SetSource(printDocumentSource));
            PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);

            printDetailedOptions.DisplayedOptions.Clear();
            printDetailedOptions.DisplayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies);
            printDetailedOptions.DisplayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation);
            printDetailedOptions.DisplayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.ColorMode);

            PrintCustomItemListOptionDetails tagsDisplay = printDetailedOptions.CreateItemListOption("Tags", "Tags");
            tagsDisplay.AddItem("ShowTags", "Show Tags");
            tagsDisplay.AddItem("HideTags", "Hide Tags");

            //printDetailedOptions.DisplayedOptions.Add("PageContent");
            printDetailedOptions.DisplayedOptions.Add("Tags");
            printDetailedOptions.OptionChanged += printDetailedOptions_OptionChanged;
        }

        async void printDetailedOptions_OptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            // Listen for PageContent changes
            string optionId = args.OptionId as string;
            if (string.IsNullOrEmpty(optionId))
                return;

            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, printDocument.InvalidatePreview);

        }

        private Canvas _printingRoot = null;
        public Canvas PrintingRoot
        {
            get
            {
                if (_printingRoot == null)
                    _printingRoot = new Canvas();

                return _printingRoot;
            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            // Clear the cache of preview pages 
            printPreviewPages.Clear();

            // Clear the printing root of preview pages
            PrintingRoot.Children.Clear();

            // This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
            RichTextBlockOverflow lastRTBOOnPage;

            // Get the PrintTaskOptions
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);

            // Get the page description to deterimine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);


            PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(e.PrintTaskOptions);
            //string pageContentSettings = (printDetailedOptions.Options["PageContent"].Value as string).ToLowerInvariant();
            string tagsSettings = (printDetailedOptions.Options["Tags"].Value as string).ToLowerInvariant();

            //imageText = (DisplayContent)((Convert.ToInt32(pageContentSettings.Contains("pictures")) << 1) | Convert.ToInt32(pageContentSettings.Contains("text")));
            ShowTags = tagsSettings.Contains("show");

            // We know there is at least one page to be printed. passing null as the first parameter to
            // AddOnePrintPreviewPage tells the function to add the first page.
            lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);

            // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
            // page has extra content 
            while (lastRTBOOnPage.HasOverflowContent)
            {
                lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
            }

            if (pagesCreated != null)
                pagesCreated.Invoke(printPreviewPages, null);

            // Report the number of preview pages created
            printDocument.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // give the PrintDocument the requested preview page
            printDocument.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < printPreviewPages.Count; i++)
            {
                printDocument.AddPage(printPreviewPages[i]);
            }

            // Indicate that all of the print pages have been provided
            printDocument.AddPagesComplete();
        }

        protected virtual RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
        {
            // Create a cavase which represents the page 
            Canvas page = new Canvas();
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;


            // Create a grid which contains the actual content to be printed
            Grid content = new Grid();

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width,
                                        printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);

            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height,
                                         printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set content size based on the given margins
            content.Width = printPageDescription.PageSize.Width - marginWidth;
            content.Height = printPageDescription.PageSize.Height - marginHeight;

            // Set content margins
            content.SetValue(Canvas.LeftProperty, marginWidth / 2);
            content.SetValue(Canvas.TopProperty, marginHeight / 2);

            // Add the RowDefinitions to the Grid which is a content to be printed
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(0.7, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(0.8, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(2.5, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(3.5, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(1.5, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(0.5, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(0.5, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(0.5, GridUnitType.Star);
            content.RowDefinitions.Add(rowDef);

            // Add the ColumnDefinitions to the Grid which is a content to be printed
            ColumnDefinition colDef = new ColumnDefinition();
            colDef.Width = new GridLength(100, GridUnitType.Star);
            content.ColumnDefinitions.Add(colDef);


            // Create the "Windows 8 SDK Sample" header which consists of an image and text in a stack panel
            // and add it to the content grid
            //Image windowsLogo = new Image();
            //windowsLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"));

            //TextBlock headerText = new TextBlock();
            //headerText.TextWrapping = TextWrapping.Wrap;
            //headerText.Text = "I Love Notes";
            //headerText.FontSize = 20;
            //headerText.Foreground = new SolidColorBrush(Colors.Black);

            //StackPanel sp = new StackPanel();
            //sp.Orientation = Orientation.Horizontal;
            //sp.Children.Add(windowsLogo);
            //sp.Children.Add(headerText);


            StackPanel outerPanel = new StackPanel();
            outerPanel.Orientation = Orientation.Vertical;
            outerPanel.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            //outerPanel.Children.Add(sp);
            outerPanel.SetValue(Grid.RowProperty, 1);


            TextBlock sampleTitle = new TextBlock();
            sampleTitle.TextWrapping = TextWrapping.Wrap;
            sampleTitle.Text = _noteForPrinting.NoteBook.Title;
            sampleTitle.FontSize = 22;
            sampleTitle.FontWeight = FontWeights.Bold;
            sampleTitle.Foreground = new SolidColorBrush(Colors.Black);
            outerPanel.Children.Add(sampleTitle);

            content.Children.Add(outerPanel);


            // Create Microsoft image used to end each page and add it to the content grid
            Image microsoftLogo = new Image();
            microsoftLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"));
            microsoftLogo.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            microsoftLogo.SetValue(Grid.RowProperty, 6);
            content.Children.Add(microsoftLogo);


            // Add the copywrite notice and add it to the content grid
            TextBlock copyrightNotice = new TextBlock();
            copyrightNotice.Text = "© 2012 Microsoft. All rights reserved.";
            copyrightNotice.FontSize = 16;
            copyrightNotice.TextWrapping = TextWrapping.Wrap;
            copyrightNotice.Foreground = new SolidColorBrush(Colors.Black);
            copyrightNotice.SetValue(Grid.RowProperty, 7);
            copyrightNotice.SetValue(Grid.ColumnSpanProperty, 2);
            content.Children.Add(copyrightNotice);


            // If lastRTBOAdded is null then we know we are creating the first page. 
            bool isFirstPage = lastRTBOAdded == null;

            FrameworkElement previousLTCOnPage = null;
            RichTextBlockOverflow rtbo = new RichTextBlockOverflow();
            // Create the linked containers and and add them to the content grid
            if (isFirstPage)
            {
                // The first linked container in a chain of linked containers is is always a RichTextBlock
                if (ShowText)
                {
                    RichTextBlock rtbl = new RichTextBlock();
                    rtbl.SetValue(Grid.RowProperty, 2);
                    rtbl = AddContentToRTBl(rtbl);
                    int a = rtbl.Blocks.Count();
                    rtbl.Foreground = new SolidColorBrush(Colors.Black);
                    content.Children.Add(rtbl);

                    // Save the RichTextBlock as the last linked container added to this page
                    previousLTCOnPage = rtbl;
                }

                if (ShowImage)
                {
                    if (_noteForPrinting.Images.Count > 0)
                    {
                        StackPanel imgList = new StackPanel();
                        if (ShowText)
                            imgList.Orientation = Orientation.Horizontal;
                        else
                            imgList.Orientation = Orientation.Vertical;

                        imgList.SetValue(Grid.RowProperty, 3);

                        for (var i = 0; i < _noteForPrinting.Images.Count; i++)
                        {
                            Image pic = new Image();
                            pic.Source = new BitmapImage(new Uri(_noteForPrinting.Images[i].ToBaseUrl()));
                            pic.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                            pic.Margin = new Thickness(2);

                            if (ShowText)
                            {
                                pic.Width = pic.Height = 250;
                                imgList.Children.Add(pic);
                                //content.RowDefinitions[3].Height = new GridLength(2.5, GridUnitType.Star);
                            }
                            else
                            {
                                pic.Width = pic.Height = 350;
                                imgList.Children.Add(pic);
                                //page.Children.Add(pic);
                            }
                        }

                        if (ShowText)
                            content.Children.Add(imgList);
                        else
                            page.Children.Add(imgList);
                    }
                }

                if (ShowTags)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var tag in _noteForPrinting.Tags)
                    {
                        sb.AppendFormat("[{0}] ", tag);
                    }

                    RichTextBlock rtbl = new RichTextBlock();
                    rtbl.SetValue(Grid.RowProperty, 5);

                    var run = new Run();
                    run.Text = sb.ToString();
                    Paragraph para = new Paragraph();
                    para.FontSize = 16;
                    para.Inlines.Add(run);

                    rtbl.Foreground = new SolidColorBrush(Colors.Black);
                    rtbl.Blocks.Add(para);

                    content.Children.Add(rtbl);
                }
            }
            else if (ShowText)
            {
                // This is not the first page so the first element on this page has to be a
                // RichTextBoxOverflow that links to the last RichTextBlockOverflow added to
                // the previous page.
                rtbo = new RichTextBlockOverflow();
                rtbo.SetValue(Grid.RowProperty, 2);
                content.Children.Add(rtbo);

                // Keep text flowing from the previous page to this page by setting the linked text container just
                // created (rtbo) as the OverflowContentTarget for the last linked text container from the previous page 
                lastRTBOAdded.OverflowContentTarget = rtbo;

                // Save the RichTextBlockOverflow as the last linked container added to this page
                previousLTCOnPage = rtbo;
            }


            if (ShowText)
            {
                // Create the next linked text container for on this page.
                rtbo = new RichTextBlockOverflow();
                rtbo.SetValue(Grid.RowProperty, 3);

                // If this linked container is not on the first page make it span 2 columns.
                if (!isFirstPage || !ShowImage)

                    // Add the RichTextBlockOverflow to the content to be printed.
                    content.Children.Add(rtbo);

                // Add the new RichTextBlockOverflow to the chain of linked text containers. To do this we much check
                // to see if the previous container is a RichTextBlock or RichTextBlockOverflow.
                if (previousLTCOnPage is RichTextBlock)
                    ((RichTextBlock)previousLTCOnPage).OverflowContentTarget = rtbo;
                else
                    ((RichTextBlockOverflow)previousLTCOnPage).OverflowContentTarget = rtbo;

                // Save the last linked text container added to the chain
                previousLTCOnPage = rtbo;

                // Create the next linked text container for on this page.
                rtbo = new RichTextBlockOverflow();
                rtbo.SetValue(Grid.RowProperty, 4);
                content.Children.Add(rtbo);

                // Add the new RichTextBlockOverflow to the chain of linked text containers. We don't have to check
                // the type of the previous linked container this time because we know it's a RichTextBlockOverflow element
                ((RichTextBlockOverflow)previousLTCOnPage).OverflowContentTarget = rtbo;
            }
            // We are done creating the content for this page. Add it to the Canvas which represents the page
            page.Children.Add(content);

            // Add the newley created page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            //PrintingRoot.Children.Add(page);
            //PrintingRoot.InvalidateMeasure();
            //PrintingRoot.UpdateLayout();

            // Add the newley created page to the list of pages
            printPreviewPages.Add(page);

            // Return the last linked container added to the page
            return rtbo;
        }

        /// <summary>
        /// This function adds content to the blocks collection of the RichTextBlock passed into the function.      
        /// </summary>
        /// <param name="rtbl">last rich text block</param>
        protected virtual RichTextBlock AddContentToRTBl(RichTextBlock rtbl)
        {
            // Create a Run and give it content
            Run run = new Run();
            run.Text = _noteForPrinting.Title;
            // Create a paragraph, set it's font size, add the run to the paragraph's inline collection
            Paragraph para = new Paragraph();
            para.FontSize = 32;
            para.Inlines.Add(run);
            // Add the paragraph to the blocks collection of the RichTextBlock
            rtbl.Blocks.Add(para);

            // Create a Run and give it content
            run = new Run();
            run.Text = _noteForPrinting.Description;
            // Create a paragraph, set it's font size, add the run to the paragraph's inline collection
            para = new Paragraph();
            para.FontSize = 16;
            para.Inlines.Add(run);
            // Add the paragraph to the blocks collection of the RichTextBlock
            rtbl.Blocks.Add(para);

            if (_noteForPrinting.Type == DataModel.NoteTypes.ToDo)
            {
                run = new Run();
                run.Text = "To Do List:";
                run.FontSize = 26;
                var MainPara = new Paragraph();
                MainPara.Inlines.Add(run);

                rtbl.Blocks.Add(MainPara);

                foreach (var todo in _noteForPrinting.ToDo)
                {
                    para = new Paragraph();
                    run = new Run();
                    run.Text = "Title: " + todo.Title;
                    run.FontSize = 18;

                    para.Inlines.Add(run);

                    run = new Run();
                    run.Text = "Due Date: " + todo.DueDate.ToString("dd/MM/yyyy hh:mm");
                    run.FontSize = 18;

                    para.Inlines.Add(run);

                    run = new Run();
                    run.Text = "Is Done: " + todo.Done.ToString();
                    run.FontSize = 18;

                    para.Inlines.Add(run);

                    rtbl.Blocks.Add(para);
                }

                // Add the paragraph to the blocks collection of the RichTextBlock
                
            }

            return rtbl;
        }

        public void Dispose()
        {
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;

            GC.SuppressFinalize(this);
        }
    }
}

