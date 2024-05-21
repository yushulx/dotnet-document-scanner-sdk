using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Runtime.InteropServices;

#if ANDROID || IOS
using Dynamsoft;

#endif

namespace MauiAndroid;

public partial class ImagePage : ContentPage
{
    int[] points = null;
    SKBitmap bitmap = null;
    bool isDataReady = false;
#if ANDROID || IOS
    private DocumentScanner documentScanner;
    private string imagepath;
    bool isNormalized = false;
#endif

    public ImagePage(string imagepath)
    {
        InitializeComponent();

        this.imagepath = imagepath;
        try
        {
            using (var stream = new SKFileStream(imagepath))
            {
                bitmap = SKBitmap.Decode(stream);
                Refresh();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
#if ANDROID || IOS
        documentScanner = DocumentScanner.Create();
        DecodeFile(imagepath);
#endif

    }

    async void Refresh()
    {
        await Task.Run(() =>
        {
            return Task.CompletedTask;
        });
        canvasView.InvalidateSurface();
    }

#if ANDROID || IOS
    async void DecodeFile(string imagepath)
    {
        await Task.Run(() =>
        {
            try
            {
                DocumentScanner.Result[] results = documentScanner.DetectFile(imagepath);

                if (results != null && results.Length > 0)
                {

                    points = results[0].Points;
                }
            }
            catch { }
            
            isDataReady = true;
            return Task.CompletedTask;
        });
        canvasView.InvalidateSurface();
    }

#endif

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();

        float StrokeWidth = 4;

        SKPaint skPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = StrokeWidth,
        };

        float scale = Math.Min((float)info.Width / bitmap.Width,
                           (float)info.Height / bitmap.Height);
        float x = (info.Width - scale * bitmap.Width) / 2;
        float y = (info.Height - scale * bitmap.Height) / 2;
        SKRect destRect = new SKRect(x, y, x + scale * bitmap.Width,
                                           y + scale * bitmap.Height);
        if (isDataReady && !isNormalized)
        {
            if (points != null)
            {
                var imageCanvas = new SKCanvas(bitmap);

                SKPoint[] skPoints = new SKPoint[4];
                for (int i = 0; i < 4; i++)
                {
                    SKPoint p = new SKPoint();
                    p.X = points[i * 2];
                    p.Y = points[i * 2 + 1];
                    skPoints[i] = p;
                }
                imageCanvas.DrawLine(skPoints[0], skPoints[1], skPaint);
                imageCanvas.DrawLine(skPoints[1], skPoints[2], skPaint);
                imageCanvas.DrawLine(skPoints[2], skPoints[3], skPaint);
                imageCanvas.DrawLine(skPoints[3], skPoints[0], skPaint);

                // Draw the corners
                for (int i = 0; i < 4; i++)
                {
                    SKRect rect = new SKRect();
                    rect.Left = skPoints[i].X - StrokeWidth;
                    rect.Top = skPoints[i].Y - StrokeWidth;
                    rect.Right = skPoints[i].X + StrokeWidth;
                    rect.Bottom = skPoints[i].Y + StrokeWidth;
                    imageCanvas.DrawOval(rect, skPaint);
                }   
            }
        }

        if (bitmap != null) canvas.DrawBitmap(bitmap, destRect);
    }

    private void Normalize()
    {
#if ANDROID || IOS
        if (points != null)
        {
            DocumentScanner.NormalizedImage normalizedImage = documentScanner.NormalizeFile(imagepath, points);
            isNormalized = true;
            byte[] data = normalizedImage.Data;
            int width = normalizedImage.Width;
            int height = normalizedImage.Height;
            int stride = normalizedImage.Stride;
            DocumentScanner.ImagePixelFormat format = normalizedImage.Format;

            SKColorType colorType = SKColorType.Rgba8888; 
            switch (format)
            {
                case DocumentScanner.ImagePixelFormat.IPF_GRAYSCALED:
                    colorType = SKColorType.Gray8;
                    break;
                case DocumentScanner.ImagePixelFormat.IPF_RGB_888:
                    colorType = SKColorType.Rgb888x;
                    break;
                case DocumentScanner.ImagePixelFormat.IPF_BINARY:
                    data = normalizedImage.Binary2Grayscale();
                    stride = width;
                    colorType = SKColorType.Gray8;
                    break;

            }

            bitmap = new SKBitmap();
            SKImageInfo info = new SKImageInfo(width, height, colorType);

            byte[] rgbaData;
            if (format == DocumentScanner.ImagePixelFormat.IPF_RGB_888)
            {
                rgbaData = new byte[width * height * 4]; // RGB888 to RGBA8888 conversion
                for (int i = 0, j = 0; i < data.Length; i += 3, j += 4)
                {
                    rgbaData[j] = data[i + 2];
                    rgbaData[j + 1] = data[i + 1];
                    rgbaData[j + 2] = data[i];
                    rgbaData[j + 3] = 255;
                }

                stride = width * 4;
            }
            else
            {
                rgbaData = data;
            }

            GCHandle handle = GCHandle.Alloc(rgbaData, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                bitmap.InstallPixels(info, ptr, stride);
            }
            finally
            {
                handle.Free();
            }

            canvasView.InvalidateSurface();
        }
#endif
    }

    private void OnCropButtonClicked(object sender, EventArgs e)
    {
        Normalize();
    }

    private void OnShareButtonClicked(object sender, EventArgs e)
    {
        ShareCanvasImage();
    }

    private async void ShareCanvasImage()
    {
        if (bitmap == null)
        {
            await DisplayAlert("Error", "No image to share!", "OK");
            return;
        }

        // Encode the bitmap to PNG
        using (var image = SKImage.FromBitmap(bitmap))
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            // Save the encoded image to local storage temporarily
            var fileName = Path.Combine(FileSystem.CacheDirectory, "shared_image.png");

            using (var stream = File.OpenWrite(fileName))
            {
                data.SaveTo(stream);
            }

            // Share the image
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share Image",
                File = new ShareFile(fileName)
            });
        }
    }

    private void OnColorModeChanged(object sender, CheckedChangedEventArgs e)
    {
#if ANDROID || IOS
        if (e.Value)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null && points != null && documentScanner != null)
            {
                string mode = radioButton.Content.ToString();

                if (mode.Equals("Color"))
                {
                    documentScanner.SetParameters(DocumentScanner.Templates.color);
                }
                else if (mode.Equals("Grayscale"))
                {
                    documentScanner.SetParameters(DocumentScanner.Templates.grayscale);
                }
                else
                {
                    documentScanner.SetParameters(DocumentScanner.Templates.binary);
                }
                Normalize();
            }
        }
#endif
    }
}