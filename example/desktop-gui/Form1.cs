using System.Drawing.Imaging;
using Dynamsoft;
using Result = Dynamsoft.DocumentScanner.Result;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using static Dynamsoft.DocumentScanner;
using System;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        private DocumentScanner scanner;
        private VideoCapture capture;
        private bool isCapturing;
        private Thread? thread;
        private Mat _mat = new Mat();
        private Result[]? _results;
        private string _color = "_binary";

        public Form1()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form1_Closing);
            DocumentScanner.InitLicense("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ=="); // Get a license key from https://www.dynamsoft.com/customer/license/trialLicense?product=dbr
            scanner = DocumentScanner.Create();
            capture = new VideoCapture(0);
            isCapturing = false;
            scanner.SetParameters(DocumentScanner.Templates.binary);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Code
            scanner.Destroy();
        }

        private void ShowResults(Result[] results)
        {
            if (results == null)
                return;
        }
        private Bitmap DecodeMat(Mat mat)
        {
            _results = scanner.DetectBuffer(mat.Data, mat.Cols, mat.Rows, (int)mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888);
            if (_results != null)
            {
                foreach (Result result in _results)
                {
                    if (result.Points != null)
                    {
                        Point[] points = new Point[4];
                        for (int i = 0; i < 4; i++)
                        {
                            points[i] = new Point(result.Points[i * 2], result.Points[i * 2 + 1]);
                        }
                        Cv2.DrawContours(mat, new Point[][] { points }, 0, Scalar.Red, 2);
                    }
                }
            }

            Bitmap bitmap = BitmapConverter.ToBitmap(mat);
            return bitmap;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            StopScan();
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image files (*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _mat = Cv2.ImRead(dlg.FileName, ImreadModes.Color);
                        Mat copy = new Mat(_mat.Rows, _mat.Cols, MatType.CV_8UC3);
                        _mat.CopyTo(copy);
                        pictureBox1.Image = DecodeMat(copy);
                        PreviewNormalizedImage();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void PreviewNormalizedImage()
        {
            if (_results != null)
            {
                Result result = _results[0];
                {
                    NormalizedImage image = scanner.NormalizeBuffer(_mat.Data, _mat.Cols, _mat.Rows, (int)_mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888, result.Points);
                    if (image != null && image.Data != null)
                    {
                        Mat mat2;
                        if (image.Stride < image.Width)
                        {
                            // binary
                            byte[] data = image.Binary2Grayscale();
                            mat2 = new Mat(image.Height, image.Stride * 8, MatType.CV_8UC1, data);
                        }
                        else if (image.Stride >= image.Width * 3)
                        {
                            // color
                            mat2 = new Mat(image.Height, image.Width, MatType.CV_8UC3, image.Data);
                        }
                        else
                        {
                            // grayscale
                            mat2 = new Mat(image.Height, image.Stride, MatType.CV_8UC1, image.Data);
                        }
                        pictureBox2.Image = BitmapConverter.ToBitmap(mat2);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!capture.IsOpened())
            {
                MessageBox.Show("Failed to open video stream or file");
                return;
            }

            if (button2.Text == "Camera Scan")
            {
                StartScan();
            }
            else {
                StopScan();
            }
        }

        private void StartScan() {
            button2.Text = "Stop";
            isCapturing = true;
            // Task.Run(() => {
            //     while (isCapturing)
            //     {
            //         Mat frame = new Mat();
            //         capture.Read(frame);

            //         // Update UI
            //         this.BeginInvoke((Action)(() => {
            //             pictureBox1.Image = DecodeMat(frame);}));
            //     }
            // });

            thread = new Thread(new ThreadStart(FrameCallback));
            thread.Start();
        }

        private void StopScan() {
            button2.Text = "Camera Scan";
            isCapturing = false;
            if (thread != null) thread.Join();
        }

        private void FrameCallback() {
            while (isCapturing) {
                //Mat frame = new Mat();
                capture.Read(_mat);
                // pictureBox1.Image = BitmapConverter.ToBitmap(frame);
                //_mat = frame;
                Mat copy = new Mat(_mat.Rows, _mat.Cols, MatType.CV_8UC3);
                _mat.CopyTo(copy);
                pictureBox1.Image = DecodeMat(copy);
            }
        }

        private void Form1_Closing(object? sender, FormClosingEventArgs e)
        {
            StopScan();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (_results != null)
                {
                    foreach (Result result in _results)
                    {
                        NormalizedImage image = scanner.NormalizeBuffer(_mat.Data, _mat.Cols, _mat.Rows, (int)_mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888, result.Points);
                        if (image != null && image.Data != null)
                        {
                            Mat mat2;
                            if (image.Stride < image.Width)
                            {
                                // binary
                                byte[] data = image.Binary2Grayscale();
                                mat2 = new Mat(image.Height, image.Stride * 8, MatType.CV_8UC1, data);
                            }
                            else if (image.Stride >= image.Width * 3)
                            {
                                // color
                                mat2 = new Mat(image.Height, image.Width, MatType.CV_8UC3, image.Data);
                            }
                            else
                            {
                                // grayscale
                                mat2 = new Mat(image.Height, image.Stride, MatType.CV_8UC1, image.Data);
                            }
                            image.Save(Path.Join(folderBrowserDialog.SelectedPath, DateTime.Now.ToFileTimeUtc() + _color + ".png"));
                        }
                    }

                    MessageBox.Show("Saved normalized document images to " + folderBrowserDialog.SelectedPath);
                }
            }

            
        }

        private void radioBinary_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBinary.Checked)
            {
                scanner.SetParameters(DocumentScanner.Templates.binary);
                _color = "_binary";
                PreviewNormalizedImage();
            }
        }

        private void radioColor_CheckedChanged(object sender, EventArgs e)
        {
            if (radioColor.Checked)
            {
                scanner.SetParameters(DocumentScanner.Templates.color);
                _color = "_color";
                PreviewNormalizedImage();
            }
            
        }

        private void radioGrayscale_CheckedChanged(object sender, EventArgs e)
        {
            if (radioGrayscale.Checked)
            {
                scanner.SetParameters(DocumentScanner.Templates.grayscale);
                _color = "_grayscale";
                PreviewNormalizedImage();
            }
            
        }
    }
}
