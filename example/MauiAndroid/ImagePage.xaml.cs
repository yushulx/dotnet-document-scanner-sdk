using SkiaSharp;
using SkiaSharp.Views.Maui;

#if ANDROID 
using Dynamsoft;

#endif

namespace MauiAndroid;

public partial class ImagePage : ContentPage
{
    int[] points;


    SKBitmap bitmap;
    bool isDataReady = false;
#if ANDROID 
    private DocumentScanner documentScanner;
    
#endif

    public ImagePage(string imagepath)
    {
        InitializeComponent();


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
#if ANDROID
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

#if ANDROID 
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

        if (!isDataReady)
        {
            if (bitmap != null) canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
            return;
        }

        var imageCanvas = new SKCanvas(bitmap);

        float StrokeWidth = 4;

        SKPaint skPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = StrokeWidth,
        };

        if (isDataReady)
        {
            if (points != null)
            {
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


        float scale = Math.Min((float)info.Width / bitmap.Width,
                           (float)info.Height / bitmap.Height);
        float x = (info.Width - scale * bitmap.Width) / 2;
        float y = (info.Height - scale * bitmap.Height) / 2;
        SKRect destRect = new SKRect(x, y, x + scale * bitmap.Width,
                                           y + scale * bitmap.Height);

        canvas.DrawBitmap(bitmap, destRect);
    }

}