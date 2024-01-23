# .NET Document Scanner SDK

The .NET Document Scanner SDK is a C# wrapper for [Dynamsoft C++ Document Normalizer SDK](https://www.dynamsoft.com/document-normalizer/docs/introduction/?ver=latest). It is used to do document edge detection, image cropping, perspective correction and image enhancement.


## License Activation
Click [here](https://www.dynamsoft.com/customer/license/trialLicense?product=ddn) to get a valid license key.

## Supported Platforms
- Windows (x64)
- Linux (x64)
- Android

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
    # DEBUG
    dotnet run
    # RELEASE
    dotnet run --configuration Release
    ```

- [Command-line Document Scanner with OpenCVSharp Windows runtime](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/command-line-cv). To make it work on Linux, you need to install [OpenCVSharp4.runtime.ubuntu.18.04-x64](https://www.nuget.org/packages/OpenCvSharp4.runtime.ubuntu.18.04-x64) package.
    
    ```bash
    dotnet run
    ```


- [WinForms Desktop Document Scanner](https://github.com/yushulx/dotnet-document-scanner-sdk/tree/main/example/desktop-gui) (**Windows Only**)
  
    ```bash
    dotnet run
    ```
    
    ![.NET WinForms Document Scanner](https://www.dynamsoft.com/codepool/img/2022/09/dotnet-winform-document-scanner.png)
    
## Building NuGet Package from Source Code

```bash
dotnet build --configuration Release
```