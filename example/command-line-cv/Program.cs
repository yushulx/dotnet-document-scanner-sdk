using System;
using System.Runtime.InteropServices;
using Dynamsoft;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check supported platforms
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Platform: Windows");
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Platform: Linux");
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("Platform: macOS");
            }

            DocumentScanner.InitLicense("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ=="); // Get a license key from https://www.dynamsoft.com/customer/license/trialLicense?product=dbr
            Console.WriteLine("Version: " + DocumentScanner.GetVersionInfo());
            DocumentScanner scanner = DocumentScanner.Create();
            scanner.SetParameters(DocumentScanner.Templates.binary);

            Mat mat = Cv2.ImRead("1.png", ImreadModes.Color);
            
            DocumentScanner.Result[]? resultArray = scanner.DetectBuffer(mat.Data, mat.Cols, mat.Rows, (int)mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888);
            if (resultArray != null)
            {
                foreach (DocumentScanner.Result result in resultArray)
                {
                    Console.WriteLine("Confidence: " + result.Confidence);
                    if (result.Points != null)
                    {
                        Point[] points = new Point[4];
                        for (int i = 0; i < 4; i++)
                        {
                            points[i] = new Point(result.Points[i * 2], result.Points[i * 2 + 1]);
                        }
                        Cv2.DrawContours(mat, new Point[][] { points }, 0, Scalar.Red, 2);
                        Cv2.ImShow("Source Image", mat);

                        DocumentScanner.NormalizedImage image = scanner.NormalizeFile("1.png", result.Points);
                        if (image != null && image.Data != null)
                        {
                            Mat mat2;
                            if (image.Stride < image.Width) {
                                // binary
                                byte[] data = image.Binary2Grayscale();
                                mat2 = new Mat(image.Height, image.Stride * 8, MatType.CV_8UC1, data);
                            }
                            else if (image.Stride >= image.Width * 3) {
                                // color
                                mat2 = new Mat(image.Height, image.Width, MatType.CV_8UC3, image.Data);
                            }
                            else {
                                // grayscale
                                mat2 = new Mat(image.Height, image.Stride, MatType.CV_8UC1, image.Data);
                            }
                            Cv2.ImShow("Normalized Document Image", mat2);
                            Cv2.WaitKey(0);
                            Cv2.DestroyAllWindows();
                            image.Save("1_normalized.png");
                        }
                    }

                }
            }
        }
    }
}
