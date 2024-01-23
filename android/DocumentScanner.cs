using Com.Dynamsoft.Core;
using Com.Dynamsoft.Ddn;

namespace Dynamsoft
{
    public class DocumentScanner
    {
        private DocumentNormalizer normalizer;

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
            return DocumentNormalizer.Version;
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

        public class LicenseVerificationListener : Java.Lang.Object, ILicenseVerificationListener
        {
            public void LicenseVerificationCallback(bool isSuccess, CoreException ex)
            {
                if (!isSuccess)
                {
                    throw new Exception(ex.ToString());
                }
            }
        }

        public static void InitLicense(string license, object? context = null)
        {
            if (context == null) { return; }

            LicenseManager.InitLicense(license, (Android.Content.Context)context, new LicenseVerificationListener());
        }

        private DocumentScanner()
        {
            normalizer = new DocumentNormalizer();
        }

        public static DocumentScanner Create()
        {
            return new DocumentScanner();
        }

        public void SetParameters(string parameters)
        {
            try
            {
                normalizer.InitRuntimeSettingsFromString(parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Result[]? DetectFile(string filename)
        {
            DetectedQuadResult[]? results = normalizer.DetectQuad(filename);
            return GetResults(results);
        }

        public Result[]? DetectBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format)
        {
            ImageData imageData = new ImageData()
            {
                Bytes = buffer,
                Width = width,
                Height = height,
                Stride = stride,
                Format = (int)format,
            };
            DetectedQuadResult[]? results = normalizer.DetectQuad(imageData);
            return GetResults(results);
        }

        private Result[]? GetResults(DetectedQuadResult[]? results)
        {
            if (results == null) return null;

            var result = new Result[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                DetectedQuadResult tmp = results[i];
                Quadrilateral quad = tmp.Location;
                result[i] = new Result()
                {
                    Confidence = tmp.ConfidenceAsDocumentBoundary,
                    Points = new int[8]
                    {
                        quad.Points[0].X, quad.Points[0].Y,
                        quad.Points[1].X, quad.Points[1].Y,
                        quad.Points[2].X, quad.Points[2].Y,
                        quad.Points[3].X, quad.Points[3].Y,
                    }
                };
            }

            return result;
        }

        public NormalizedImage NormalizeFile(string filename, int[] points)
        {
            Quadrilateral quad = new Quadrilateral();
            quad.Points = new Android.Graphics.Point[4];
            quad.Points[0] = new Android.Graphics.Point(points[0], points[1]);
            quad.Points[1] = new Android.Graphics.Point(points[2], points[3]);
            quad.Points[2] = new Android.Graphics.Point(points[4], points[5]);
            quad.Points[3] = new Android.Graphics.Point(points[6], points[7]);
            NormalizedImageResult? result = normalizer.Normalize(filename, quad);
            return GetNormalizedImage(result);
        }

        public NormalizedImage NormalizeBuffer(byte[] buffer, int width, int height, int stride, ImagePixelFormat format, int[] points)
        {
            ImageData imageData = new ImageData()
            {
                Bytes = buffer,
                Width = width,
                Height = height,
                Stride = stride,
                Format = (int)format,
            };

            Quadrilateral quad = new Quadrilateral();
            quad.Points = new Android.Graphics.Point[4];
            quad.Points[0] = new Android.Graphics.Point(points[0], points[1]);
            quad.Points[1] = new Android.Graphics.Point(points[2], points[3]);
            quad.Points[2] = new Android.Graphics.Point(points[4], points[5]);
            quad.Points[3] = new Android.Graphics.Point(points[6], points[7]);
            NormalizedImageResult? result = normalizer.Normalize(imageData, quad);
            return GetNormalizedImage(result);
        }

        private NormalizedImage GetNormalizedImage(NormalizedImageResult? result)
        {
            NormalizedImage normalizedImage = new NormalizedImage();
            if (result != null)
            {
                ImageData imageData = result.Image;
                normalizedImage.Width = imageData.Width;
                normalizedImage.Height = imageData.Height;
                normalizedImage.Stride = imageData.Stride;
                normalizedImage.Format = (ImagePixelFormat)imageData.Format;
                normalizedImage.Data = imageData.Bytes.ToArray();
            }
            return normalizedImage;
        }
    }
}