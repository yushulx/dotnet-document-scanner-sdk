namespace Dynamsoft;

using System.Text;
using System.Runtime.InteropServices;

public class DocumentScanner
{
    private IntPtr handler;
    private static string? licenseKey;

#if (LINUX64)
    [DllImport("DynamsoftDocumentNormalizer")]
    static extern IntPtr DDN_CreateInstance();

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern void DDN_DestroyInstance(IntPtr handler);

    [DllImport("DynamsoftDocumentNormalizer")]
    static extern IntPtr DDN_GetVersion();
    
    [DllImport("DynamsoftCore")]
    static extern int DC_InitLicense(string license, [Out] byte[] errorMsg, int errorMsgSize);
#else 
    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern IntPtr DDN_CreateInstance();

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern void DDN_DestroyInstance(IntPtr handler);

    [DllImport("DynamsoftDocumentNormalizerx64")]
    static extern IntPtr DDN_GetVersion();
    
    [DllImport("DynamsoftCorex64")]
    static extern int DC_InitLicense(string license, [Out] byte[] errorMsg, int errorMsgSize);
#endif

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
        Console.WriteLine("InitLicense(): " + Encoding.ASCII.GetString(errorMsg));
    }
}
