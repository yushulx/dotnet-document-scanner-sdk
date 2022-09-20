using System.Drawing.Imaging;
using Dynamsoft;
using Result = Dynamsoft.DocumentScanner.Result;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;

namespace Test
{
    public partial class Form1 : Form
    {
        private DocumentScanner scanner;
        private VideoCapture capture;
        private bool isCapturing;
        private Thread? thread;

        public Form1()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form1_Closing);
            DocumentScanner.InitLicense("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ=="); // Get a license key from https://www.dynamsoft.com/customer/license/trialLicense?product=dbr
            scanner = DocumentScanner.Create();
            capture = new VideoCapture(0);
            isCapturing = false;
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
            Result[]? results = scanner.DetectBuffer(mat.Data, mat.Cols, mat.Rows, (int)mat.Step(), DocumentScanner.ImagePixelFormat.IPF_RGB_888);
            if (results != null)
            {
                foreach (Result result in results)
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
                    textBox1.Clear();
                    try
                    {
                        Mat mat = Cv2.ImRead(dlg.FileName, ImreadModes.Color);
                        pictureBox1.Image = DecodeMat(mat);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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
                Mat frame = new Mat();
                capture.Read(frame);
                // pictureBox1.Image = BitmapConverter.ToBitmap(frame);
                pictureBox1.Image = DecodeMat(frame);
            }
        }

        private void Form1_Closing(object? sender, FormClosingEventArgs e)
        {
            StopScan();
        }
    }
}
