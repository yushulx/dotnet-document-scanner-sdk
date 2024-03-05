# .NET Document Scanner SDK

The .NET Document Scanner SDK is a C# wrapper for [Dynamsoft Document Normalizer SDK](https://www.dynamsoft.com/document-normalizer/docs/core/introduction/). It is used to do document edge detection, image cropping, perspective correction and image enhancement.


## License Activation
Click [here](https://www.dynamsoft.com/customer/license/trialLicense?product=ddn) to get a valid license key.

## Supported Platforms
- Windows (x64)
- Linux (x64)
- Android
- iOS

## API
- `public static void InitLicense(string license)`: Initialize the license key. It must be called before creating the document scanner object.
- `public static DocumentScanner Create()`: Create the document scanner object.
- `public Result[]? DetectFile(string filename)`: Detect documents from an image file.
- `public Result[]? DetectBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format)`: Detect documents from a buffer.
- `public NormalizedImage NormalizeFile(string filename, int[] points)`: Normalize the detected documents from an image file.
- `public NormalizedImage NormalizeBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format, int[] points)`: Normalize the detected documents from a buffer.
- `public static string? GetVersionInfo()`: Get SDK version number.
- `public void SetParameters(string parameters)`: Customize the parameters. Refer to [Parameter Organization](https://www.dynamsoft.com/document-normalizer/docs/core/parameters/parameter-organization-structure.html) for more details.

## Usage
- Set the license key:
    
    ```csharp
    DocumentScanner.InitLicense("LICENSE-KEY"); 
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
    Result[]? resultArray = scanner.DetectBuffer(bytes, width, height, stride, DocumentScanner.ImagePixelFormat.IPF_RGB_888);
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
                int length = mat.Cols * mat.Rows * mat.ElemSize();
                byte[] bytes = new byte[length];
                Marshal.Copy(mat.Data, bytes, 0, length);

                DocumentScanner.NormalizedImage image = scanner.NormalizeBuffer(bytes, mat.Cols, mat.Rows, (int)mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888, result.Points);
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
            DocumentScanner.InitLicense("LICENSE-KEY"); // Get a license key from https://www.dynamsoft.com/customer/license/trialLicense?product=ddn
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
    dotnet run
    ```

- [Command-line Document Scanner with OpenCVSharp Windows runtime](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/command-line-cv). To make it work on Linux, you need to install [OpenCVSharp4.runtime.ubuntu.18.04-x64](https://www.nuget.org/packages/OpenCvSharp4.runtime.ubuntu.18.04-x64) package.
    
    ```bash
    dotnet run
    ```


- [WinForms Desktop Document Scanner](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/desktop-gui) (**Windows Only**)
  
    ```bash
    dotnet run
    ```
    
    ![.NET WinForms Document Scanner](https://camo.githubusercontent.com/ee30a750052f5392f20aefcaffbb4308cfabafa9cd610642f6b0ff669195f2dc/68747470733a2f2f7777772e64796e616d736f66742e636f6d2f636f6465706f6f6c2f696d672f323032322f30392f646f746e65742d77696e666f726d2d646f63756d656e742d7363616e6e65722e706e67)

- [.NET MAUI](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/MauiApp)

    ![.NET MAUI Document Scanner](https://camo.githubusercontent.com/91a6cb1ea0a964510154e75100c828c2ccf6eece9acbd01ab92a07686e2caa12/68747470733a2f2f7777772e64796e616d736f66742e636f6d2f636f6465706f6f6c2f696d672f323032342f30332f646f746e65742d6d6175692d696f732d646f63756d656e742d646574656374696f6e2e706e67)
    
## Building NuGet Package from Source Code

```bash
# build dll for desktop
cd desktop
dotnet build --configuration Release

# build dll for android
cd android
dotnet build --configuration Release

# build dll for iOS
cd ios
dotnet build --configuration Release

# build nuget package
nuget pack .\DocumentScannerSDK.nuspec
```