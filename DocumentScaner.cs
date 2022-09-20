namespace Dynamsoft;

using System.Text;
using System.Runtime.InteropServices;

public class DocumentScanner
{
    public class Result
    {
        public int Confidence { get; set; }
        public int[]? Points { get; set; }
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

#endif

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
        /**Reserved memory for the struct. The length of this array indicates the size of the memory reserved for this struct. */

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
        if (handler != IntPtr.Zero)
        {
            DDN_DestroyInstance(handler);
            handler = IntPtr.Zero;
        }
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

    public static void InitLicense(string license)
    {
        byte[] errorMsg = new byte[512];
        licenseKey = license;
        DC_InitLicense(license, errorMsg, 512);
        #if DEBUG
        Console.WriteLine("InitLicense(): " + Encoding.ASCII.GetString(errorMsg));
        #endif
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
