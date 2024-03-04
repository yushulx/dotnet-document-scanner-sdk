using DynamsoftCore;
using Com.Dynamsoft.Ddn;
using static Dynamsoft.DocumentScanner;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Dynamsoft
{
    public class DocumentScanner
    {
        private DynamsoftDocumentNormalizer normalizer;

        public class NormalizedImage
        {
            public int Width;
            public int Height;
            public int Stride;
            public ImagePixelFormat Format;
            public byte[] Data = new byte[0];

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


        public static string GetVersionInfo()
        {
            return DynamsoftDocumentNormalizer.Version;
        }

        public class Result
        {
            public int Confidence { get; set; }
            public int[] Points { get; set; }

            public Result()
            {
                Points = new int[8];
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

        public class LicenseVerification : LicenseVerificationListener
        {
            public override void LicenseVerificationCallback(bool isSuccess, NSError error)
            {
                if (!isSuccess)
                {
                    System.Console.WriteLine(error.UserInfo);
                }
            }
        }

        public static void InitLicense(string license, object? context = null)
        {
            DynamsoftLicenseManager.InitLicense(license, new LicenseVerification());
        }

        private DocumentScanner()
        {
            normalizer = new DynamsoftDocumentNormalizer();
        }

        public static DocumentScanner Create()
        {
            return new DocumentScanner();
        }

        public void SetParameters(string parameters)
        {
            try
            {
                NSError error;
                normalizer.InitRuntimeSettingsFromString(parameters, out error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Result[]? DetectFile(string filename)
        {
            NSError error;
            iDetectedQuadResult[]? results = normalizer.DetectQuadFromFile(filename, out error);
            return GetResults(results);
        }

        public Result[]? DetectBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format)
        {
            NSData converted = NSData.FromArray(buffer);

            iImageData imageData = new iImageData()
            {
                Bytes = converted,
                Width = width,
                Height = height,
                Stride = stride,
                Format = (EnumImagePixelFormat)format,
            };
            NSError error;
            iDetectedQuadResult[]? results = normalizer.DetectQuadFromBuffer(imageData, out error);
            return GetResults(results);
        }

        private Result[]? GetResults(iDetectedQuadResult[]? results)
        {
            if (results == null) return null;

            var result = new Result[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                iDetectedQuadResult tmp = results[i];
                iQuadrilateral quad = tmp.Location;
                result[i] = new Result()
                {
                    Confidence = (int)tmp.ConfidenceAsDocumentBoundary,
                    Points = new int[8]
                    {
                        (int)((NSValue)quad.Points[0]).CGPointValue.X, (int)((NSValue)quad.Points[0]).CGPointValue.Y,
                        (int)((NSValue)quad.Points[1]).CGPointValue.X, (int)((NSValue)quad.Points[1]).CGPointValue.Y,
                        (int)((NSValue)quad.Points[2]).CGPointValue.X, (int)((NSValue)quad.Points[2]).CGPointValue.Y,
                        (int)((NSValue)quad.Points[3]).CGPointValue.X, (int)((NSValue)quad.Points[3]).CGPointValue.Y,
                    }
                };
            }

            return result;
        }

        public NormalizedImage NormalizeFile(string filename, int[] points)
        {
            iQuadrilateral quad = new iQuadrilateral();
            quad.Points = new NSObject[4];
            quad.Points[0] = NSValue.FromCGPoint(new CGPoint(points[0], points[1]));
            quad.Points[1] = NSValue.FromCGPoint(new CGPoint(points[2], points[3]));
            quad.Points[2] = NSValue.FromCGPoint(new CGPoint(points[4], points[5]));
            quad.Points[3] = NSValue.FromCGPoint(new CGPoint(points[6], points[7]));

            NSError error;
            iNormalizedImageResult? result = normalizer.NormalizeFile(filename, quad, out error);
            return GetNormalizedImage(result);
        }

        public NormalizedImage NormalizeBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format, int[] points)
        {
            iImageData imageData = new iImageData()
            {
                Bytes = NSData.FromArray(buffer),
                Width = width,
                Height = height,
                Stride = stride,
                Format = (EnumImagePixelFormat)format,
            };

            iQuadrilateral quad = new iQuadrilateral();
            quad.Points = new NSObject[4];
            quad.Points[0] = NSValue.FromCGPoint(new CGPoint(points[0], points[1]));
            quad.Points[1] = NSValue.FromCGPoint(new CGPoint(points[2], points[3]));
            quad.Points[2] = NSValue.FromCGPoint(new CGPoint(points[4], points[5]));
            quad.Points[3] = NSValue.FromCGPoint(new CGPoint(points[6], points[7]));
            NSError error;
            iNormalizedImageResult? result = normalizer.NormalizeBuffer(imageData, quad, out error);
            return GetNormalizedImage(result);
        }

        private NormalizedImage GetNormalizedImage(iNormalizedImageResult? result)
        {
            NormalizedImage normalizedImage = new NormalizedImage();
            if (result != null)
            {
                iImageData imageData = result.Image;
                normalizedImage.Width = (int)imageData.Width;
                normalizedImage.Height = (int)imageData.Height;
                normalizedImage.Stride = (int)imageData.Stride;
                normalizedImage.Format = (ImagePixelFormat)imageData.Format;
                normalizedImage.Data = imageData.Bytes.ToArray();
            }
            return normalizedImage;
        }
    }
}