using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ContosoPhoto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const Int32 c_offset = 8;
        private const string c_filename = "Bitmap.dat";
        private WriteableBitmap _bitmap;
        private string _filename;

        private bool _annotating = false;
        private Dictionary<uint, PointerPoint> _points = new Dictionary<uint, PointerPoint>();

        public MainPage()
        {
            this.InitializeComponent();

            // Register event handlers
            DataTransferManager.GetForCurrentView().DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(OnDataRequested);
            Application.Current.Suspending += new SuspendingEventHandler(OnSuspending);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Restore state if the app was suspended and terminated and
            // LocalSettings and LocalData contain PLM data
            var data = ApplicationData.Current.LocalSettings.Values;

            if (data.ContainsKey("Width") && data.ContainsKey("Height"))
            {
                // First restore the WriteableBitmap
                int width = (int)data["Width"];
                int height = (int)data["Height"];
                await RestoreBitmapAsync(width, height);

                // Then restore remaining state
                PhotoTransform.TranslateX = (double)data["TranslateX"];
                PhotoTransform.TranslateY = (double)data["TranslateY"];
                PhotoTransform.ScaleX = PhotoTransform.ScaleY = (double)data["Scale"];
                SaveButton.IsEnabled = (bool)data["Save"];
                OpeningMessage.Visibility = Visibility.Collapsed;
                _filename = (string)data["FileName"];
            }
        }

        private async void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var data = ApplicationData.Current.LocalSettings.Values;

            if (_bitmap != null)
            {
                // If an image has been loaded, persist the WriteableBitmap
                data["Width"] = _bitmap.PixelWidth;
                data["Height"] = _bitmap.PixelHeight;
                data["TranslateX"] = PhotoTransform.TranslateX;
                data["TranslateY"] = PhotoTransform.TranslateY;
                data["Scale"] = PhotoTransform.ScaleX;
                data["Save"] = SaveButton.IsEnabled;
                data["FileName"] = _filename;

                // Request a deferral
                var deferral = e.SuspendingOperation.GetDeferral();

                // Create a data file and wrap a DataWriter around it
                var file = await folder.CreateFileAsync(c_filename, CreationCollisionOption.ReplaceExisting);

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var output = stream.GetOutputStreamAt(0);
                    using (var writer = new DataWriter(output))
                    {
                        // Write the pixels to the file with the DataWriter
                        writer.WriteBuffer(_bitmap.PixelBuffer);
                        await writer.StoreAsync();
                        await output.FlushAsync();
                    }
                }

                // Indicate that state has been persisted
                deferral.Complete();
            }
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (_bitmap != null)
            {
                var deferral = e.Request.GetDeferral();
                var imageStream = new InMemoryRandomAccessStream();
                var streamReference = RandomAccessStreamReference.CreateFromStream(imageStream);
                await EncodeBitmapToStreamAsync(_bitmap, imageStream, BitmapEncoder.JpegEncoderId);
                e.Request.Data.Properties.Title = _filename;
                e.Request.Data.Properties.Description = "Image shared from Contoso Photo";
                e.Request.Data.SetBitmap(streamReference);
                deferral.Complete();
            }
        }

        private async Task OpenPhoto()
        {
            // Display a FileOpenPicker and allow the user to select a photo
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.CommitButtonText = "Open";
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                // If the user picked a file, store the file name and open the file
                _filename = file.Name;
                var stream = await file.OpenAsync(FileAccessMode.Read);

                // Wrap a WriteableBitmap around the selected photo and display it
                _bitmap = new WriteableBitmap(100, 100);
                _bitmap.SetSource(stream);
                Photo.Source = _bitmap;

                // Reset the CompositeTransform
                PhotoTransform.TranslateX = PhotoTransform.TranslateY = 0.0;
                PhotoTransform.ScaleX = PhotoTransform.ScaleY = 1.0;

                // Disable the Save button
                SaveButton.IsEnabled = false;

                // Hide the "Tap here" message in case it hasn't been removed already
                OpeningMessage.Visibility = Visibility.Collapsed;

                // Clear the Canvas
                int count = AnnotateCanvas.Children.Count;
                for (int i = 0; i < count; i++)
                    AnnotateCanvas.Children.RemoveAt(0);
            }
        }

        private async void OnOpenButtonClicked(object sender, RoutedEventArgs e)
        {
            await OpenPhoto();
        }

        private async void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            // Display a FileSavePicker and allow the user to save the photo
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPEG Image", new List<string>() { ".jpg" });
            picker.FileTypeChoices.Add("PNG Image", new List<string>() { ".png" });
            picker.FileTypeChoices.Add("BMP Image", new List<string>() { ".bmp" });
            picker.CommitButtonText = "Save";
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = _filename;
            StorageFile file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                Guid id;

                switch (file.FileType)
                {
                    case ".bmp":
                        id = BitmapEncoder.BmpEncoderId;
                        break;

                    case ".jpg":
                        id = BitmapEncoder.JpegEncoderId;
                        break;

                    case ".png":
                    default:
                        id = BitmapEncoder.PngEncoderId;
                        break;
                }

                using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await EncodeBitmapToStreamAsync(_bitmap, fileStream, id);
                    SaveButton.IsEnabled = false;
                }
            }
        }

        private void RemoveRedEye(WriteableBitmap bitmap, int x1, int x2, int y1, int y2)
        {
            // Read the pixels from the WriteableBitmap
            Stream stream = bitmap.PixelBuffer.AsStream();
            Byte[] pixels = new Byte[stream.Length];
            stream.Read(pixels, 0, pixels.Length);

            // Fix pixels representing red eyes in the specified area
            int width = bitmap.PixelWidth;

            for (int i = y1; i < y2; i++)
            {
                for (int j = x1; j < x2; j++)
                {
                    int index = ((i * width) + j) * 4;
                    Byte b = pixels[index];
                    Byte g = pixels[index + 1];
                    Byte r = pixels[index + 2];
                    if (r > (g + b))
                    {
                        pixels[index + 2] = (Byte)((g + b) / 2);
                    }
                }
            }

            // Write the modified pixels back to the WriteableBitmap and invalidate
            // the WriteableBitmap to update the onscreen image
            stream.Position = 0;
            stream.Write(pixels, 0, pixels.Length);
            bitmap.Invalidate();
        }

        private async Task EncodeBitmapToStreamAsync(WriteableBitmap bitmap, IRandomAccessStream output, Guid id)
        {
            // Read the pixels from the WriteableBitmap              
            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                Byte[] pixels = new Byte[stream.Length];
                stream.Read(pixels, 0, pixels.Length);

                // Swap red and blue pixel values
                int max = _bitmap.PixelWidth * _bitmap.PixelHeight;

                for (int i = 0; i < max; i++)
                {
                    int index = i << 2;
                    byte temp = pixels[index];
                    pixels[index] = pixels[index + 2];
                    pixels[index + 2] = temp;
                }

                // Encode the image
                var encoder = await BitmapEncoder.CreateAsync(id, output);
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, pixels);
                await encoder.FlushAsync();
            }
        }

        private async Task RestoreBitmapAsync(int width, int height)
        {
            _bitmap = new WriteableBitmap(width, height);
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(c_filename);

            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                using (var input = stream.GetInputStreamAt(0))
                {
                    using (var reader = new DataReader(input))
                    {
                        var size = await reader.LoadAsync((uint)stream.Size);
                        IBuffer pixels = reader.ReadBuffer(size);
                        pixels.CopyTo(_bitmap.PixelBuffer);
                        Photo.Source = _bitmap;
                        _bitmap.Invalidate();
                    }
                }
            }
        }


        private void Photo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // When the photo is tapped, remove red eyes in the tapped region
            if (_bitmap != null && !_annotating)
            {
                // Get pixel address of touch point
                double x = e.GetPosition(sender as FrameworkElement).X;
                double y = e.GetPosition(sender as FrameworkElement).Y;

                // Compute coordinates of selection region around the touch point
                int x1 = (int)x - c_offset;
                int x2 = (int)x + c_offset;
                int y1 = (int)y - c_offset;
                int y2 = (int)y + c_offset;

                // Constrain coordinates of selection region
                x1 = (int)Math.Max(0, x1);
                x2 = (int)Math.Min(Photo.ActualWidth, x2);
                y1 = (int)Math.Max(0, y1);
                y2 = (int)Math.Min(Photo.ActualHeight, y2);

                // Translate pixel coordinates to WriteableBitmap coordinates
                double wratio = _bitmap.PixelWidth / Photo.ActualWidth;
                double hratio = _bitmap.PixelHeight / Photo.ActualHeight;

                x1 = (int)(x1 * wratio);
                x2 = (int)(x2 * wratio);
                y1 = (int)(y1 * hratio);
                y2 = (int)(y2 * hratio);

                // Remove red-eye
                RemoveRedEye(_bitmap, x1, x2, y1, y2);

                // Enable the application bar's Save button
                SaveButton.IsEnabled = true;
            }
        }

        
        private void LayoutRoot_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Reset the CompositeTransform
            PhotoTransform.TranslateX = PhotoTransform.TranslateY = 0.0;
            PhotoTransform.ScaleX = PhotoTransform.ScaleY = 1.0;
        }

        private void LayoutRoot_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (_bitmap != null && e.KeyModifiers == Windows.System.VirtualKeyModifiers.Control)
            {
                if (e.GetCurrentPoint(sender as UIElement).Properties.MouseWheelDelta > 0)
                {
                    // Zoom in
                    if (PhotoTransform.ScaleX < 4.0)
                    {
                        PhotoTransform.ScaleX += 0.1;
                        PhotoTransform.ScaleY += 0.1;
                    }
                }
                else
                {
                    // Zoom out
                    if (PhotoTransform.ScaleX > 1.0)
                    {
                        PhotoTransform.ScaleX -= 0.1;
                        PhotoTransform.ScaleY -= 0.1;
                    }
                }
            }
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_annotating && e.Pointer.IsInContact)
            {
                var point = e.GetCurrentPoint(AnnotateCanvas);
                _points.Add(e.Pointer.PointerId, e.GetCurrentPoint(AnnotateCanvas));
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_annotating && e.Pointer.IsInContact)
            {
                var point = e.GetCurrentPoint(AnnotateCanvas);

                if (_points.ContainsKey(e.Pointer.PointerId))
                {
                    var prev = _points[e.Pointer.PointerId];
                    Line line = new Line();
                    line.X1 = prev.Position.X;
                    line.Y1 = prev.Position.Y;
                    line.X2 = point.Position.X;
                    line.Y2 = point.Position.Y;
                    line.Stroke = new SolidColorBrush(Colors.Yellow);
                    line.StrokeThickness = 8.0;
                    line.StrokeStartLineCap = line.StrokeEndLineCap = PenLineCap.Round;
                    AnnotateCanvas.Children.Add(line);
                }

                _points[e.Pointer.PointerId] = point;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_annotating && _points.ContainsKey(e.Pointer.PointerId))
                _points.Remove(e.Pointer.PointerId);
        }

        private void Canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (_annotating && _points.ContainsKey(e.Pointer.PointerId))
                _points.Remove(e.Pointer.PointerId);
        }

        private void OnRedEyeButtonClicked(object sender, RoutedEventArgs e)
        {
            _annotating = false;
            AnnotateCanvas.IsHitTestVisible = false;
            RedEyeButton.IsEnabled = false;
            AnnotateButton.IsEnabled = true;
        }

        private void OnAnnotateButtonClicked(object sender, RoutedEventArgs e)
        {
            _annotating = true;
            AnnotateCanvas.IsHitTestVisible = true;
            AnnotateCanvas.Width = Photo.ActualWidth;
            AnnotateCanvas.Height = Photo.ActualHeight;
            if (AnnotateCanvas.Clip == null)
                AnnotateCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, AnnotateCanvas.Width, AnnotateCanvas.Height) };
            RedEyeButton.IsEnabled = true;
            AnnotateButton.IsEnabled = false;
        }

    }
}
