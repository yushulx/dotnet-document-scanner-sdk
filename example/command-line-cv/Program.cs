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
            scanner.SetParameters(DocumentScanner.Templates.color);
            
            Mat mat = Cv2.ImRead("1.png", ImreadModes.Color);
            DocumentScanner.Result[]? resultArray = scanner.DetectBuffer(mat.Data, mat.Cols, mat.Rows, (int)mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888);
            if (resultArray != null)
            {
                foreach (DocumentScanner.Result result in resultArray)
                {
                    Console.WriteLine("Confidence: " + result.Confidence);
                    if (result.Points != null)
                    {
                        foreach (int point in result.Points)
                        {
                            Console.WriteLine("Point: " + point);
                        }

                        DocumentScanner.NormalizedImage image = scanner.NormalizeFile("1.png", result.Points);
                        if (image != null)
                        {
                            image.Save("1_normalized.png");
                        }
                    }

                }
            }
        }
    }
}
