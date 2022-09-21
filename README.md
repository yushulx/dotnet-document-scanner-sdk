# .NET Document Scanner SDK

The .NET Document Scanner SDK is a C# wrapper for [Dynamsoft C++ Document Normalizer SDK](https://www.dynamsoft.com/document-normalizer/docs/introduction/?ver=latest). It is used to do document edge detection, image cropping, perspective correction and image enhancement.


## License Activation
Click [here](https://www.dynamsoft.com/customer/license/trialLicense?product=ddn) to get a valid license key.

## Supported Platforms
- Windows (x64)
- Linux (x64)

## Download .NET 6 SDK
* [Windows](https://dotnet.microsoft.com/en-us/download#windowscmd)
* [Linux](https://dotnet.microsoft.com/en-us/download#linuxubuntu)

## Methods
- `public static void InitLicense(string license)`
- `public static DocumentScanner Create()`
- `public Result[]? DetectFile(string filename)`
- `public Result[]? DetectBuffer(IntPtr pBufferBytes, int width, int height, int stride, ImagePixelFormat format)`
- `public NormalizedImage NormalizeFile(string filename, int[] points)`
- `public NormalizedImage NormalizeBuffer(IntPtr pBufferBytes, int width, int height, int stride, ImagePixelFormat format, int[] points)`
- `public static string? GetVersionInfo()`
- `public void SetParameters(string parameters)`

## Usage
- Set the license key:
    
    ```csharp
    DocumentScanner.InitLicense("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ=="); 
    ```
- Initialize the document scanner object:
    
    ```csharp
    DocumentScanner scanner = DocumentScanner.Create();
    ```
- Detect documents from an image file:

    ```csharp
    Result[]? resultArray = scanner.DetectFile(filename);
    ```    
- Detect documents from a buffer:

    
    ```csharp
    Mat mat = Cv2.ImRead(filename, ImreadModes.Color);
    Result[]? resultArray = scanner.DetectBuffer(copy.Data, copy.Cols, copy.Rows, (int)copy.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888);
    ```     
    
- Normalize the detected documents from an image file:

    
    ```csharp
    if (resultArray != null)
    {
        foreach (Result result in resultArray)
        {
            if (result.Points != null)
            {
                NormalizedImage image = scanner.NormalizeFile(filename, result.Points);
                if (image != null)
                {
                    image.Save(DateTime.Now.ToFileTimeUtc() + ".png");
                }
            }

        }
    }
    ```
- Normalize the detected documents from a buffer:

    
    ```csharp
    if (resultArray != null)
    {
        foreach (DocumentScanner.Result result in resultArray)
        {
            if (result.Points != null)
            {
                DocumentScanner.NormalizedImage image = scanner.NormalizeBuffer(mat.Data, mat.Cols, mat.Rows, (int)mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888, result.Points);
                if (image != null && image.Data != null)
                {
                    image.Save(DateTime.Now.ToFileTimeUtc() + ".png");
                }
            }

        }
    }
    ```
- Get SDK version number:

    ```csharp
    string? version = DocumentScanner.GetVersionInfo();
    ```
- Customize the parameters:
    
    ```csharp
    // Refer to https://www.dynamsoft.com/document-normalizer/docs/parameters/parameter-organization-structure.html?ver=latest
    scanner.SetParameters(DocumentScanner.Templates.color);
    ```

## Quick Start

```csharp
using System;
using System.Runtime.InteropServices;
using Dynamsoft;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DocumentScanner.InitLicense("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ=="); // Get a license key from https://www.dynamsoft.com/customer/license/trialLicense?product=ddn
            DocumentScanner? scanner = null;
            try {
                scanner = DocumentScanner.Create();
                scanner.SetParameters(DocumentScanner.Templates.color);
                Console.WriteLine("Please enter an image file: ");
                string? filename = Console.ReadLine();
                if (filename != null) {
                    Result[]? resultArray = scanner.DetectFile(filename);
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
                                    image.Save(DateTime.Now.ToFileTimeUtc() + ".png");
                                }
                            }

                        }
                    }
                    else {
                        Console.WriteLine("No document detected.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
```


## Example
- [Command-line Document Scanner](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/command-line) (**Windows & Linux**)
    
    ```bash
    # DEBUG
    dotnet run
    # RELEASE
    dotnet run --configuration Release
    ```
    
    ![.NET 6 command-line document scanner](https://camo.githubusercontent.com/9d36e69330ed71a44e39680b3a6b65bd2b2c1698b9bc30e272b3675f283ccef7/68747470733a2f2f7777772e64796e616d736f66742e636f6d2f636f6465706f6f6c2f696d672f323032322f30392f646f746e65742d6c696e75782d646f63756d656e742d7363616e6e65722e706e67)

- [Command-line Document Scanner with OpenCVSharp Windows runtime](https://github.com/yushulx/dotnet-barcode-qr-code-sdk/tree/main/example/desktop-gui). To make it work on Linux, you need to install [OpenCVSharp4.runtime.ubuntu.18.04-x64](https://www.nuget.org/packages/OpenCvSharp4.runtime.ubuntu.18.04-x64) package.
    
    ```bash
    dotnet run
    ```
    
    ![.NET document scanner with OpenCVSharp](https://camo.githubusercontent.com/e50e27160a11015c675a78c790aeee579bf2b1693d77cfd8e3577e5892fc1546/68747470733a2f2f7777772e64796e616d736f66742e636f6d2f636f6465706f6f6c2f696d672f323032322f30392f646f746e65742d646f63756d656e742d7363616e6e65722e706e67) 

- [WinForms Desktop Document Scanner](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/desktop-gui) (**Windows Only**)
  
    ```bash
    dotnet run
    ```
    
    ![.NET WinForms Document Scanner](https://camo.githubusercontent.com/c43884777d0e2379c0aada242cbcd4edd7f66bfa518e72058774dbe1d2708363/68747470733a2f2f7777772e64796e616d736f66742e636f6d2f636f6465706f6f6c2f696d672f323032322f30392f646f746e65742d77696e666f726d2d646f63756d656e742d7363616e6e65722e706e67)
    
## Building NuGet Package from Source Code

```bash
dotnet build --configuration Release
```