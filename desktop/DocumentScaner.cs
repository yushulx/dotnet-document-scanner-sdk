namespace Dynamsoft;

using System.Text;
using System.Runtime.InteropServices;

public class DocumentScanner
{
    public class NormalizedImage
    {
        public int Width;
        public int Height;
        public int Stride;
        public ImagePixelFormat Format;
        public byte[] Data = new byte[0];

        public IntPtr _dataPtr = IntPtr.Zero;

        ~NormalizedImage()
        {
            if (_dataPtr != IntPtr.Zero) DDN_FreeNormalizedImageResult(ref _dataPtr);
        }

        public void Save(string filename)
        {
            if (_dataPtr != IntPtr.Zero)
            {
                NormalizedImageResult? image = (NormalizedImageResult?)Marshal.PtrToStructure(_dataPtr, typeof(NormalizedImageResult));
                if (image != null)
                {
                    int ret = DDN_SaveImageDataToFile(image.Value.ImageData, filename);

#if DEBUG
                    Console.WriteLine("SaveImageDataToFile: " + ret);
                    Console.WriteLine("Save image to " + filename);
#endif
                }

            }
        }

        public byte[] Binary2Grayscale()
        {
            byte[] output = new byte[Width * Height];
            int index = 0;

            int skip = Stride * 8 - Width;
            int shift = 0;
            int n = 1;

            foreach (byte b in Data)
            {
                int byteCount = 7;
                while (byteCount >= 0)
                {
                    int tmp = (b & (1 << byteCount)) >> byteCount;

                    if (shift < Stride * 8 * n - skip)
                    {
                        if (tmp == 1)
                            output[index] = 255;
                        else
                            output[index] = 0;
                        index += 1;
                    }

                    byteCount -= 1;
                    shift += 1;
                }

                if (shift == Stride * 8 * n)
                {
                    n += 1;
                }
            }
            return output;
        }
    }

    public class Result
    {
        public int Confidence { get; set; }
        public int[] Points { get; set; }

        public Result()
        {
            Points = new int[4];
        }
    }

    public class Templates
    {
        public static string binary = @"{
        ""GlobalParameter"":{
            ""Name"":""GP""
        },
        ""ImageParameterArray"":[
            {
                ""Name"":""IP-1"",
                ""NormalizerParameterName"":""NP-1""
            }
        ],
        ""NormalizerParameterArray"":[
            {
                ""Name"":""NP-1"",
                ""ColourMode"": ""ICM_BINARY"" 
            }
        ]
    }";

        public static string color = @"{
        ""GlobalParameter"":{
            ""Name"":""GP""
        },
        ""ImageParameterArray"":[
            {
                ""Name"":""IP-1"",
                ""NormalizerParameterName"":""NP-1""
            }
        ],
        ""NormalizerParameterArray"":[
            {
                ""Name"":""NP-1"",
                ""ColourMode"": ""ICM_COLOUR"" 
            }
        ]
    }";

        public static string grayscale = @"{
        ""GlobalParameter"":{
            ""Name"":""GP""
        },
        ""ImageParameterArray"":[
            {
                ""Name"":""IP-1"",
                ""NormalizerParameterName"":""NP-1""
            }
        ],
        ""NormalizerParameterArray"":[
            {
                ""Name"":""NP-1"",
                ""ColourMode"": ""ICM_GRAYSCALE"" 
            }
        ]
    }";
    }

    private IntPtr handler;
    private static string? licenseKey;

    // https://stackoverflow.com/questions/30153797/c-sharp-preprocessor-differentiate-between-operating-systems
#if _WINDOWS
    [DllImport("DynamsoftCorex64")]
    static extern int DC_InitLicense(string license, [Out] byte[] errorMsg, int errorMsgSize);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern IntPtr DDN_CreateInstance();

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern void DDN_DestroyInstance(IntPtr handler);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern IntPtr DDN_GetVersion();

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_InitRuntimeSettingsFromString(IntPtr handler, string settings, [Out] byte[] errorMsg, int errorMsgSize);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_DetectQuadFromFile(IntPtr handler, string sourceFilePath, string templateName, ref IntPtr pResultArray);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_FreeDetectedQuadResultArray(ref IntPtr pResultArray);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_NormalizeFile(IntPtr handler, string sourceFilePath, string templateName, IntPtr quad, ref IntPtr result);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_FreeNormalizedImageResult(ref IntPtr result);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_SaveImageDataToFile(IntPtr image, string filename);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_DetectQuadFromBuffer(IntPtr handler, IntPtr sourceImage, string templateName, ref IntPtr pResultArray);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern int DDN_NormalizeBuffer(IntPtr handler, IntPtr sourceImage, string templateName, IntPtr quad, ref IntPtr result);

#else 
    [DllImport("DynamsoftCore")]
    static extern int DC_InitLicense(string license, [Out] byte[] errorMsg, int errorMsgSize);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern IntPtr DDN_CreateInstance();

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern void DDN_DestroyInstance(IntPtr handler);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern IntPtr DDN_GetVersion();

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_InitRuntimeSettingsFromString(IntPtr handler, string settings, [Out] byte[] errorMsg, int errorMsgSize);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_DetectQuadFromFile(IntPtr handler, string sourceFilePath, string templateName, ref IntPtr pResultArray);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_FreeDetectedQuadResultArray(ref IntPtr pResultArray);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_NormalizeFile(IntPtr handler, string sourceFilePath, string templateName, IntPtr quad, ref IntPtr result);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_FreeNormalizedImageResult(ref IntPtr result);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_SaveImageDataToFile(IntPtr image, string filename);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_DetectQuadFromBuffer(IntPtr handler, IntPtr sourceImage, string templateName, ref IntPtr pResultArray);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern int DDN_NormalizeBuffer(IntPtr handler, IntPtr sourceImage, string templateName, IntPtr quad, ref IntPtr result);

#endif

    public enum ImagePixelFormat
    {
        IPF_BINARY,

        /**0:White, 1:Black */
        IPF_BINARYINVERTED,

        /**8bit gray */
        IPF_GRAYSCALED,

        /**NV21 */
        IPF_NV21,

        /**16bit with RGB channel order stored in memory from high to low address*/
        IPF_RGB_565,

        /**16bit with RGB channel order stored in memory from high to low address*/
        IPF_RGB_555,

        /**24bit with RGB channel order stored in memory from high to low address*/
        IPF_RGB_888,

        /**32bit with ARGB channel order stored in memory from high to low address*/
        IPF_ARGB_8888,

        /**48bit with RGB channel order stored in memory from high to low address*/
        IPF_RGB_161616,

        /**64bit with ARGB channel order stored in memory from high to low address*/
        IPF_ARGB_16161616,

        /**32bit with ABGR channel order stored in memory from high to low address*/
        IPF_ABGR_8888,

        /**64bit with ABGR channel order stored in memory from high to low address*/
        IPF_ABGR_16161616,

        /**24bit with BGR channel order stored in memory from high to low address*/
        IPF_BGR_888
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct NormalizedImageResult
    {
        public IntPtr ImageData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageData
    {
        public int bytesLength;

        public IntPtr bytes;

        public int width;

        public int height;

        public int stride;

        public ImagePixelFormat format;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DetectedQuadResultArray
    {
        public int resultsCount;
        public IntPtr results;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DetectedQuadResult
    {
        public IntPtr location;
        public int confidenceAsDocumentBoundary;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Quadrilateral
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public DM_Point[] points;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DM_Point
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] coordinate;
    }

    private DocumentScanner()
    {
        handler = DDN_CreateInstance();
    }

    ~DocumentScanner()
    {
        Destroy();
    }

    public static string? GetVersionInfo()
    {
        return Marshal.PtrToStringUTF8(DDN_GetVersion());
    }

    public static DocumentScanner Create()
    {
        if (licenseKey == null)
        {
            throw new Exception("Please call InitLicense first.");
        }
        return new DocumentScanner();
    }

    public void Destroy()
    {
        if (handler != IntPtr.Zero)
        {
            DDN_DestroyInstance(handler);
            handler = IntPtr.Zero;
        }
    }

    public static void InitLicense(string license, object? context = null)
    {
        byte[] errorMsg = new byte[512];
        licenseKey = license;
        int ret = DC_InitLicense(license, errorMsg, 512);
#if DEBUG
        Console.WriteLine("InitLicense(): " + Encoding.ASCII.GetString(errorMsg));
#endif
        if (ret != 0)
        {
            throw new Exception("InitLicense(): " + Encoding.ASCII.GetString(errorMsg));
        }
    }

    public int SetParameters(string parameters)
    {
        if (handler == IntPtr.Zero) return -1;

        byte[] errorMsg = new byte[512];
        int ret = DDN_InitRuntimeSettingsFromString(handler, parameters, errorMsg, 512);
#if DEBUG
        Console.WriteLine("SetParameters(): " + Encoding.ASCII.GetString(errorMsg));
#endif
        return ret;
    }

    public Result[]? DetectFile(string filename)
    {
        if (handler == IntPtr.Zero) return null;

        IntPtr pResultArray = IntPtr.Zero;

        int ret = DDN_DetectQuadFromFile(handler, filename, "", ref pResultArray);
#if DEBUG
        Console.WriteLine("DetectFile(): " + ret);
#endif
        return GetResults(pResultArray);
    }

    public NormalizedImage NormalizeFile(string filename, int[] points)
    {
        if (handler == IntPtr.Zero) return new NormalizedImage();

        IntPtr pResult = IntPtr.Zero;

        Quadrilateral quad = new Quadrilateral();
        quad.points = new DM_Point[4];
        quad.points[0].coordinate = new int[2] { points[0], points[1] };
        quad.points[1].coordinate = new int[2] { points[2], points[3] };
        quad.points[2].coordinate = new int[2] { points[4], points[5] };
        quad.points[3].coordinate = new int[2] { points[6], points[7] };

        IntPtr pQuad = Marshal.AllocHGlobal(Marshal.SizeOf(quad));
        Marshal.StructureToPtr(quad, pQuad, false);
        int ret = DDN_NormalizeFile(handler, filename, "", pQuad, ref pResult);
#if DEBUG
        Console.WriteLine("NormalizeFile(): " + ret);
#endif
        Marshal.FreeHGlobal(pQuad);

        NormalizedImage normalizedImage = new NormalizedImage();

        NormalizedImageResult? result = (NormalizedImageResult?)Marshal.PtrToStructure(pResult, typeof(NormalizedImageResult));
        if (result != null)
        {
            normalizedImage._dataPtr = pResult;
            ImageData? imageData = (ImageData?)Marshal.PtrToStructure(result.Value.ImageData, typeof(ImageData));
            if (imageData != null)
            {
                normalizedImage.Width = imageData.Value.width;
                normalizedImage.Height = imageData.Value.height;
                normalizedImage.Stride = imageData.Value.stride;
                normalizedImage.Format = imageData.Value.format;
                normalizedImage.Data = new byte[imageData.Value.bytesLength];
                Marshal.Copy(imageData.Value.bytes, normalizedImage.Data, 0, imageData.Value.bytesLength);
            }
        }
        return normalizedImage;
    }

    public NormalizedImage NormalizeBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format, int[] points)
    {
        if (handler == IntPtr.Zero) return new NormalizedImage();

        int length = buffer.Length;
        IntPtr pBufferBytes = Marshal.AllocHGlobal(length);
        Marshal.Copy(buffer, 0, pBufferBytes, length);

        IntPtr pResult = IntPtr.Zero;

        ImageData img = new ImageData();
        img.width = width;
        img.height = height;
        img.stride = stride;
        img.format = format;
        img.bytesLength = stride * height;
        img.bytes = pBufferBytes;

        IntPtr pimageData = Marshal.AllocHGlobal(Marshal.SizeOf(img));
        Marshal.StructureToPtr(img, pimageData, false);

        Quadrilateral quad = new Quadrilateral();
        quad.points = new DM_Point[4];
        quad.points[0].coordinate = new int[2] { points[0], points[1] };
        quad.points[1].coordinate = new int[2] { points[2], points[3] };
        quad.points[2].coordinate = new int[2] { points[4], points[5] };
        quad.points[3].coordinate = new int[2] { points[6], points[7] };

        IntPtr pQuad = Marshal.AllocHGlobal(Marshal.SizeOf(quad));
        Marshal.StructureToPtr(quad, pQuad, false);
        int ret = DDN_NormalizeBuffer(handler, pimageData, "", pQuad, ref pResult);
#if DEBUG
        Console.WriteLine("NormalizeBuffer(): " + ret);
#endif
        Marshal.FreeHGlobal(pQuad);
        Marshal.FreeHGlobal(pimageData);

        NormalizedImage normalizedImage = new NormalizedImage();

        NormalizedImageResult? result = (NormalizedImageResult?)Marshal.PtrToStructure(pResult, typeof(NormalizedImageResult));
        if (result != null)
        {
            normalizedImage._dataPtr = pResult;
            ImageData? imageData = (ImageData?)Marshal.PtrToStructure(result.Value.ImageData, typeof(ImageData));
            if (imageData != null)
            {
                normalizedImage.Width = imageData.Value.width;
                normalizedImage.Height = imageData.Value.height;
                normalizedImage.Stride = imageData.Value.stride;
                normalizedImage.Format = imageData.Value.format;
                normalizedImage.Data = new byte[imageData.Value.bytesLength];
                Marshal.Copy(imageData.Value.bytes, normalizedImage.Data, 0, imageData.Value.bytesLength);
            }
        }

        Marshal.FreeHGlobal(pBufferBytes);

        return normalizedImage;
    }

    public Result[]? DetectBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format)
    {
        if (handler == IntPtr.Zero) return null;

        int length = buffer.Length;
        IntPtr pBufferBytes = Marshal.AllocHGlobal(length);
        Marshal.Copy(buffer, 0, pBufferBytes, length);

        IntPtr pResultArray = IntPtr.Zero;

        ImageData imageData = new ImageData();
        imageData.width = width;
        imageData.height = height;
        imageData.stride = stride;
        imageData.format = format;
        imageData.bytesLength = stride * height;
        imageData.bytes = pBufferBytes;

        IntPtr pimageData = Marshal.AllocHGlobal(Marshal.SizeOf(imageData));
        Marshal.StructureToPtr(imageData, pimageData, false);
        int ret = DDN_DetectQuadFromBuffer(handler, pimageData, "", ref pResultArray);
        Marshal.FreeHGlobal(pimageData);
#if DEBUG
        Console.WriteLine("DetectBuffer(): " + ret);
#endif

        Marshal.FreeHGlobal(pBufferBytes);

        return GetResults(pResultArray);
    }

    private Result[]? GetResults(IntPtr pResultArray)
    {
        Result[]? resultArray = null;
        if (pResultArray != IntPtr.Zero)
        {
            DetectedQuadResultArray? results = (DetectedQuadResultArray?)Marshal.PtrToStructure(pResultArray, typeof(DetectedQuadResultArray));
            if (results != null)
            {
                int count = results.Value.resultsCount;
                if (count > 0)
                {
                    IntPtr[] documents = new IntPtr[count];
                    Marshal.Copy(results.Value.results, documents, 0, count);
                    resultArray = new Result[count];

                    for (int i = 0; i < count; i++)
                    {
                        DetectedQuadResult? result = (DetectedQuadResult?)Marshal.PtrToStructure(documents[i], typeof(DetectedQuadResult));
                        if (result != null)
                        {
                            Result r = new Result();
                            resultArray[i] = r;
                            r.Confidence = result.Value.confidenceAsDocumentBoundary;
                            Quadrilateral? Quadrilateral = (Quadrilateral?)Marshal.PtrToStructure(result.Value.location, typeof(Quadrilateral));
                            if (Quadrilateral != null)
                            {
                                DM_Point[] points = Quadrilateral.Value.points;
                                r.Points = new int[8] { points[0].coordinate[0], points[0].coordinate[1], points[1].coordinate[0], points[1].coordinate[1], points[2].coordinate[0], points[2].coordinate[1], points[3].coordinate[0], points[3].coordinate[1] };
                            }
                        }
                    }
                }
            }
            DDN_FreeDetectedQuadResultArray(ref pResultArray);
        }

        return resultArray;
    }
}
