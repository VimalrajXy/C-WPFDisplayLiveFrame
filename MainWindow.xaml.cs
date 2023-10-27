using FaceRecognitionAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Emgu.CV.CvEnum;
using Emgu.CV;
using Newtonsoft.Json;

namespace APITestProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static MainWindow ref_MainWindow = null;

        public delegate void CallbackImageGrabbed(BitmapSource bitmapSource);
        public CallbackImageGrabbed DelWebcamImageGrabbed { get; set; }

        string startupPath = "";
        string URL = "";

        public Webcam objWebcam { get; set; }
        XYREONAPI xyreonAPI = new XYREONAPI("");
        public DispatcherTimer tmrCompareRecognition = new DispatcherTimer();
        public DispatcherTimer tmrFaceRecognition = new DispatcherTimer();
        public DispatcherTimer tmrFaceDetail = new DispatcherTimer();
        public DispatcherTimer tmrCheckHeartbeat = new DispatcherTimer();
        public DispatcherTimer tmrFaceMask = new DispatcherTimer();


        //Setting.txt
        public static string Port = "";
        public static bool Draw = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void WebcamImageGrabbed(BitmapSource webcamImage)
        {
            try
            {

                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {

                    imgWebcam2.Source = webcamImage;

                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void StartCompareRecogition()
        {
            try
            {
                tmrCompareRecognition.Start();

                Dispatcher.Invoke(() =>
                {
                    btnCompareLive.Content = "Stop";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StartCompareRecogition()");
            }
        }

        public void StopCompareRecogition()
        {
            try
            {
                tmrCompareRecognition.Stop();

                

                objWebcam.blnIsProcessing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StopCompareRecogition()");
            }
        }


        public void StartFaceRecogition()
        {
            try
            {
                tmrFaceRecognition.Start();

                Dispatcher.Invoke(() =>
                {
                    btnFaceRecognition.Content = "Stop";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StartFaceRecogition()");
            }
        }

        public void StopFaceRecogition()
        {
            try
            {
                tmrFaceRecognition.Stop();



                objWebcam.blnIsProcessing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StopFaceRecogition()");
            }
        }

        public void StartFaceMask()
        {
            try
            {
                tmrFaceMask.Start();

                Dispatcher.Invoke(() =>
                {
                    btnFacemask.Content = "Stop";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StartFaceMask()");
            }
        }

        public void StopFaceMask()
        {
            try
            {
                tmrFaceMask.Stop();



                objWebcam.blnIsProcessing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StopFaceMask()");
            }
        }


        public void StartFaceDetail()
        {
            try
            {
                tmrFaceDetail.Start();

                Dispatcher.Invoke(() =>
                {
                    btnFaceDetail.Content = "Stop";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StartFaceDetail()");
            }
        }

        public void StopFaceDetail()
        {
            try
            {
                tmrFaceDetail.Stop();



                objWebcam.blnIsProcessing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StopFaceDetail()");
            }
        }


        private void btnCompareLive_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool blnStarted = tmrCompareRecognition.IsEnabled;

                if (blnStarted)
                {
                    StopCompareRecogition();

                    Dispatcher.Invoke(() =>
                    {
                        btnCompareLive.Content = "Live Compare";

                        tblXPOSE.Text = "";
                        tblYPOSE.Text = "";
                        tblZPOSE.Text = "";
                        tblConfidence.Text = "";
                        tblConfidence2.Content = "";
                        tblStatus.Text = "";

                        tblHASMASK.Text = "";
                        tblLIVE.Content = "";
                        tblWIDTH.Text = "";
                        tblHEIGHT.Text = "";
                        tblEyes.Text = "";
                        tblConfidence.Text = "";
                        tblRectXPosMin.Text = "";
                        tblRectYPosMin.Text = "";
                    });

                    return;
                }

                if (objWebcam._capture == null || !objWebcam.blnWebcamIsOpened)
                {
                    MessageBox.Show("Please start the Webcam.", "btnCompareRecognition_Click()");
                    return;
                }

                StartCompareRecogition();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "btnCompareRecognition_Click()");
            }
        }

        public async Task ConnectToHardware()
        {
            try
            {

                objWebcam = new Webcam();
                objWebcam.DelWebcamImageGrabbed2 += WebcamImageGrabbed;

                Task tOpenWebcam = objWebcam.OpenWebcam("");
                await tOpenWebcam;
                tOpenWebcam.Dispose();


                await Task.Delay(250);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        public void StartDispatcherTimers()
        {
            try
            {
                tmrCompareRecognition.Interval = new TimeSpan(0, 0, 0, 0, 700);
                tmrCompareRecognition.Tick += tmrCompareRecognition_Tick;
                tmrCompareRecognition.Stop();

                tmrFaceRecognition.Interval = new TimeSpan(0, 0, 0, 0, 150);
                tmrFaceRecognition.Tick += TmrFaceRecognition_Tick;
                tmrFaceRecognition.Stop();

                tmrFaceDetail.Interval = new TimeSpan(0, 0, 0, 0, 500);
                tmrFaceDetail.Tick += TmrFaceDetail_Tick;
                tmrFaceDetail.Stop();

                tmrCheckHeartbeat.Interval = new TimeSpan(0,0,0,0,100);
                tmrCheckHeartbeat.Tick += TmrCheckHeartbeat_Tick;
                tmrCheckHeartbeat.Stop();

                tmrFaceMask.Interval = new TimeSpan(0,0,0,0,100);
                tmrFaceMask.Tick += TmrFaceMask_Tick;
                tmrFaceMask.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void StartEyecoolDetectAsync()
        {
            try
            {
                //Dispatcher.Invoke(() =>
                //{
                //    lblStart.Content = "STOP";
                //});

                objWebcam.blnIsProcessing = true;

                int res2 = objWebcam.StartDetectionAsync();

                //GlobalKiosk.EventsLog($"EyeCool Start Detect : {res2}");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StartEyecoolDetectAsync()");
            }
        }

        public void StopEyecoolDetectAsync()
        {
            try
            {
                if (objWebcam != null)
                {
                    objWebcam.StopDetectionAsync();

                    //objWebcam.RotateCamera(GlobalKiosk.RotateDegree);
                    //UpdateLblRotation(GlobalKiosk.RotateDegree.ToString());
                }

                objWebcam.blnIsProcessing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "StopEyecoolDetectAsync()");
            }
        }

        private async void TmrCheckHeartbeat_Tick(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            Task<bool> tSearch = xyreonAPI.GetApiHeartBeat(URL);

            bool blnSearch = await tSearch;

            tblConfidence2.Content = blnSearch.ToString();

            stopwatch.Stop();

            tblStatus2.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";
        }

        private async void TmrFaceDetail_Tick(object sender, EventArgs e)
        {
            tmrFaceDetail.Stop();
            XYREONAPI.FaceDetail facedetail = new XYREONAPI.FaceDetail();

            try
            {

                //GETTING LIVE IMAGE
                if (objWebcam._frame.IsEmpty) return;
                if (objWebcam.blnIsProcessing) return;

                objWebcam.blnIsProcessing = true;

                //Mat 0 = objWebcam._frame.Clone();
                objWebcam._frameFlip5 = objWebcam._frame.Clone();
                //CvInvoke.Flip(objWebcam._frame.Clone(), objWebcam._frameFlip4, FlipType.Horizontal);

                string strFileName = "Temp.jpg";

                xyreonAPI.Api_URL = URL;

                Bitmap bitmap = objWebcam._frameFlip5.ToBitmap();
                ImageConverter converter = new ImageConverter();
                byte[] b = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                bitmap.Dispose();

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                Task<XYREONAPI.FaceDetail> tSearch = xyreonAPI.LiveDetect(strFileName, b);

                facedetail = await tSearch;

                stopwatch.Stop();

                tblStatus.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";

                if (!facedetail.Error)
                {

                    if (facedetail != null)
                    {

                        
                        int intLiveness = facedetail.LivenessCode;
                        string strLiveness = facedetail.LivenessDesc;
                        
                        int xPos = facedetail.FaceRectangleX;
                        int yPos = facedetail.FaceRectangleY;
                        int fwidth = facedetail.FaceRectangleWidth;
                        int fheight = facedetail.FaceRectangleHeight;
                        float headX = facedetail.HeadPositionX;
                        float headY = facedetail.HeadPositionY;
                        float headZ = facedetail.HeadPositionZ;

                        tblConfidence.Text = intLiveness.ToString();
                        tblLIVE.Content = strLiveness;
                        

                        tblXPOSE.Text = headX.ToString();
                        tblYPOSE.Text = headY.ToString();
                        tblZPOSE.Text = headZ.ToString();

                        tblWIDTH.Text = fwidth.ToString();
                        tblHEIGHT.Text = fheight.ToString();

                        tblRectXPosMin.Text = xPos.ToString();
                        tblRectYPosMin.Text = yPos.ToString();

                        System.Drawing.Rectangle r = new System.Drawing.Rectangle(facedetail.FaceRectangleX, facedetail.FaceRectangleY, facedetail.FaceRectangleWidth, facedetail.FaceRectangleHeight);

                        CvInvoke.Rectangle(objWebcam._frameFlip5, r, new Emgu.CV.Structure.Bgr(System.Drawing.Color.Red).MCvScalar, 3);

                    }



                }
                else
                {
                    if (facedetail != null)
                    {
                        
                        int intLiveness = facedetail.LivenessCode;
                        string strLiveness = facedetail.LivenessDesc;
                        
                        bool hMask = facedetail.HasMask;
                        int eyes = facedetail.Eyes;

                        
                        tblConfidence.Text = intLiveness.ToString();
                        tblLIVE.Content = strLiveness;
                        tblHASMASK.Text = hMask.ToString();
                        tblEyes.Text = eyes.ToString();

                        System.Drawing.Rectangle r = new System.Drawing.Rectangle(facedetail.FaceRectangleX, facedetail.FaceRectangleY, facedetail.FaceRectangleWidth, facedetail.FaceRectangleHeight);

                        CvInvoke.Rectangle(objWebcam._frameFlip5, r, new Emgu.CV.Structure.Bgr(System.Drawing.Color.Red).MCvScalar, 3);

                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "TmrFaceDetail_Tick()");
            }

            objWebcam._frameFlip5.ToBitmap().Save("tmp.jpg");
            objWebcam.blnIsProcessing = false;
            tmrFaceDetail.Start();
        }

        private async void TmrFaceRecognition_Tick(object sender, EventArgs e)
        {
            XYREONAPI.RecognizeFace recognizeface = new XYREONAPI.RecognizeFace();

            try
            {
                
                //GETTING LIVE IMAGE
                if (objWebcam._frame.IsEmpty) return;
                if (objWebcam.blnIsProcessing) return;

                objWebcam.blnIsProcessing = true;

                //Mat 0 = objWebcam._frame.Clone();
                objWebcam._frameFlip5 = objWebcam._frame.Clone();
                //CvInvoke.Flip(objWebcam._frame.Clone(), objWebcam._frameFlip4, FlipType.Horizontal);

                string strFileName = "Temp.jpg";

                xyreonAPI.Api_URL = URL;

                Bitmap bitmap = objWebcam._frameFlip5.ToBitmap();
                ImageConverter converter = new ImageConverter();
                byte[] b = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                bitmap.Dispose();

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                Task<XYREONAPI.RecognizeFace> tSearch = xyreonAPI.RecogniseFace(strFileName, b);

                recognizeface = await tSearch;

                stopwatch.Stop();

                tblStatus.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";

                if (!recognizeface.Error)
                {

                    if (recognizeface != null)
                    {

                        
                        bool faceexist = recognizeface.FaceExist;
                        string facename = recognizeface.FaceName;
                        int xPos = recognizeface.FaceRectangleX;
                        int yPos = recognizeface.FaceRectangleY;
                        int fwidth = recognizeface.FaceRectangleWidth;
                        int fheight = recognizeface.FaceRectangleHeight;
                        float headX = recognizeface.HeadPositionX;
                        float headY = recognizeface.HeadPositionY;
                        float headZ = recognizeface.HeadPositionZ;
                        
                        tblConfidence.Text = facename;
                        tblConfidence2.Content = facename;
                        tblLIVE.Content = faceexist;

                        tblXPOSE.Text = headX.ToString();
                        tblYPOSE.Text = headY.ToString();
                        tblZPOSE.Text = headZ.ToString();

                        tblWIDTH.Text = fwidth.ToString();
                        tblHEIGHT.Text = fheight.ToString();

                        tblRectXPosMin.Text = xPos.ToString();
                        tblRectYPosMin.Text = yPos.ToString();

                        

                    }



                }
                else
                {
                    if (recognizeface != null)
                    {
                        
                        
                        string facename = recognizeface.FaceName;
                        
                        tblConfidence.Text = facename;
                        tblConfidence2.Content = facename;
                        
                        
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "TmrFaceRecognition_Tick()");
            }

            objWebcam.blnIsProcessing = false;

        }

        private async void tmrCompareRecognition_Tick(object sender, EventArgs e)
        {

            try
            {

                string strFileName2 = "Temp2.jpg";
                Bitmap passportimage;
                byte[] passportimgbyte;

                // hardcoded passport image
                passportimage = new Bitmap(System.IO.Path.Combine(startupPath, "test.png"));
                ImageConverter conv = new ImageConverter();
                passportimgbyte = (byte[])conv.ConvertTo(passportimage, typeof(byte[]));


                //GETTING LIVE IMAGE
                if (objWebcam._frame.IsEmpty) return;
                if (objWebcam.blnIsProcessing) return;

                objWebcam.blnIsProcessing = true;

                //Mat 0 = objWebcam._frame.Clone();
                objWebcam._frameFlip5 = objWebcam._frame.Clone();
                //CvInvoke.Flip(objWebcam._frame.Clone(), objWebcam._frameFlip4, FlipType.Horizontal);

                string strFileName = "Temp.jpg";

                xyreonAPI.Api_URL = URL;

                Bitmap bitmap = objWebcam._frameFlip5.ToBitmap();
                ImageConverter converter = new ImageConverter();
                byte[] b = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                bitmap.Dispose();

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                Task<bool> tSearch = xyreonAPI.FaceCompare(strFileName, b, strFileName2, passportimgbyte);

                bool blnSearch = await tSearch;

                stopwatch.Stop();

                tblStatus.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";

                if (blnSearch)
                {

                    if (xyreonAPI.Api_LivenessResponse != null)
                    {

                        
                        
                        string strLiveness = xyreonAPI.Api_LivenessResponse.LivenessDesc;
                        
                        string facename = xyreonAPI.Api_LivenessResponse.FaceName;
                        int xPos = xyreonAPI.Api_LivenessResponse.FaceRectangleX;
                        int yPos = xyreonAPI.Api_LivenessResponse.FaceRectangleY;
                        int fwidth = xyreonAPI.Api_LivenessResponse.FaceRectangleWidth;
                        int fheight = xyreonAPI.Api_LivenessResponse.FaceRectangleHeight;
                        float headX = xyreonAPI.Api_LivenessResponse.HeadPositionX;
                        float headY = xyreonAPI.Api_LivenessResponse.HeadPositionY;
                        float headZ = xyreonAPI.Api_LivenessResponse.HeadPositionZ;
                        bool hMask = xyreonAPI.Api_LivenessResponse.HasMask;
                        int eyes = xyreonAPI.Api_LivenessResponse.Eyes;

                        tblConfidence.Text = facename;
                        tblConfidence2.Content = facename;
                        tblLIVE.Content = strLiveness;
                        tblHASMASK.Text = hMask.ToString();
                        tblEyes.Text = eyes.ToString();

                        tblXPOSE.Text = headX.ToString();
                        tblYPOSE.Text = headY.ToString();
                        tblZPOSE.Text = headZ.ToString();

                        tblWIDTH.Text = fwidth.ToString();
                        tblHEIGHT.Text = fheight.ToString();

                        tblRectXPosMin.Text = xPos.ToString();
                        tblRectYPosMin.Text = yPos.ToString();

                        

                    }



                }
                else
                {
                    if (xyreonAPI.Api_LivenessResponse != null)
                    {
                        
                        string strLiveness = xyreonAPI.Api_LivenessResponse.LivenessDesc;
                        
                        string facename = xyreonAPI.Api_LivenessResponse.FaceName;
                        bool hMask = xyreonAPI.Api_LivenessResponse.HasMask;
                        int eyes = xyreonAPI.Api_LivenessResponse.Eyes;

                        tblConfidence.Text = facename;
                        tblConfidence2.Content = facename;
                        tblLIVE.Content = strLiveness;
                        tblHASMASK.Text = hMask.ToString();
                        tblEyes.Text = eyes.ToString();

                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "tmrCompareRecognition_Tick()");
            }

            objWebcam.blnIsProcessing = false;

        }

        private async void TmrFaceMask_Tick(object sender, EventArgs e)
        {
            string facemask = "";

            try
            {

                //GETTING LIVE IMAGE
                if (objWebcam._frame.IsEmpty) return;
                if (objWebcam.blnIsProcessing) return;

                objWebcam.blnIsProcessing = true;

                //Mat 0 = objWebcam._frame.Clone();
                objWebcam._frameFlip5 = objWebcam._frame.Clone();
                //CvInvoke.Flip(objWebcam._frame.Clone(), objWebcam._frameFlip4, FlipType.Horizontal);

                xyreonAPI.Api_URL = URL;

                Bitmap bitmap = objWebcam._frameFlip5.ToBitmap();
                ImageConverter converter = new ImageConverter();
                byte[] b = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                bitmap.Dispose();

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                Task<string> tSearch = xyreonAPI.GetFaceDetail(b);

                facemask = await tSearch;

                tSearch.Dispose();
                stopwatch.Stop();

                tblStatus.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";
                xyreonAPI.Api_FaceMask = JsonConvert.DeserializeObject<List<XYREONAPI.FaceMask>>(facemask);
                if (xyreonAPI.Api_FaceMask.Count > 0)
                {

                    if (xyreonAPI.Api_FaceMask[0] != null)
                    {


                        bool hasmask = xyreonAPI.Api_FaceMask[0].HasMask;
                        int xPos = xyreonAPI.Api_FaceMask[0].RectPosXMin;
                        int yPos = xyreonAPI.Api_FaceMask[0].RectPosYMin;
                        int fwidth = xyreonAPI.Api_FaceMask[0].Width;
                        int fheight = xyreonAPI.Api_FaceMask[0].Height;
                        float confidence = xyreonAPI.Api_FaceMask[0].Confidence;

                        tblConfidence.Text = confidence.ToString();
                        tblLIVE.Content = hasmask.ToString();

                        tblWIDTH.Text = fwidth.ToString();
                        tblHEIGHT.Text = fheight.ToString();

                        tblRectXPosMin.Text = xPos.ToString();
                        tblRectYPosMin.Text = yPos.ToString();



                    }



                }
                else
                {
                    if (xyreonAPI.Api_FaceMask[0] != null)
                    {


                        bool hasmask = xyreonAPI.Api_FaceMask[0].HasMask;
                        float confidence = xyreonAPI.Api_FaceMask[0].Confidence;

                        tblConfidence.Text = confidence.ToString();
                        tblLIVE.Content = hasmask.ToString();


                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "TmrFaceRecognition_Tick()");
            }

            objWebcam.blnIsProcessing = false;

        }

        public string getFileSetting(string strKey)
        {
            try
            {
                startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
                string settingFile = startupPath + "setting.txt";
                if (!System.IO.File.Exists(settingFile))
                {
                    return "";
                }
                string[] strList = System.IO.File.ReadAllLines(settingFile);

                string strResult = "";
                bool blnFound = false;
                foreach (string s in strList)
                {

                    if (blnFound)
                    {
                        strResult = s;
                        break;
                    }

                    if (s.Trim().ToUpper().Equals(("[" + strKey + "]").ToUpper())) blnFound = true;
                }
                return strResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ref_MainWindow = this;

            startupPath = System.AppDomain.CurrentDomain.BaseDirectory;

            Task tConnectToHardware = ConnectToHardware();
            await tConnectToHardware;
            tConnectToHardware.Dispose();

            URL = getFileSetting("IP");

            StartDispatcherTimers();
        }

        private void btnFaceDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool blnStarted = tmrFaceDetail.IsEnabled;

                if (blnStarted)
                {
                    StopFaceDetail();

                    Dispatcher.Invoke(() =>
                    {
                        btnFaceDetail.Content = "Face Detail";

                        tblXPOSE.Text = "";
                        tblYPOSE.Text = "";
                        tblZPOSE.Text = "";
                        tblConfidence.Text = "";
                        tblConfidence2.Content = "";
                        tblStatus.Text = "";

                        tblHASMASK.Text = "";
                        tblLIVE.Content = "";
                        tblWIDTH.Text = "";
                        tblHEIGHT.Text = "";
                        tblEyes.Text = "";
                        tblConfidence.Text = "";
                        tblRectXPosMin.Text = "";
                        tblRectYPosMin.Text = "";
                    });

                    return;
                }

                if (objWebcam._capture == null || !objWebcam.blnWebcamIsOpened)
                {
                    MessageBox.Show("Please start the Webcam.", "btnFaceDetail_Click()");
                    return;
                }

                StartFaceDetail();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "btnFaceDetail_Click()");
            }
        }

        private void btnFaceRecognition_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool blnStarted = tmrFaceRecognition.IsEnabled;

                if (blnStarted)
                {
                    StopFaceRecogition();

                    Dispatcher.Invoke(() =>
                    {
                        btnFaceRecognition.Content = "Face Recognition";

                        tblXPOSE.Text = "";
                        tblYPOSE.Text = "";
                        tblZPOSE.Text = "";
                        tblConfidence.Text = "";
                        tblConfidence2.Content = "";
                        tblStatus.Text = "";

                        tblHASMASK.Text = "";
                        tblLIVE.Content = "";
                        tblWIDTH.Text = "";
                        tblHEIGHT.Text = "";
                        tblEyes.Text = "";
                        tblConfidence.Text = "";
                        tblRectXPosMin.Text = "";
                        tblRectYPosMin.Text = "";
                    });

                    return;
                }

                if (objWebcam._capture == null || !objWebcam.blnWebcamIsOpened)
                {
                    MessageBox.Show("Please start the Webcam.", "btnFaceRecognition_Click()");
                    return;
                }

                StartFaceRecogition();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "btnFaceRecognition_Click()");
            }
        }

        private async void btnGetHeartbeat_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            Task<bool> tSearch = xyreonAPI.GetApiHeartBeat(URL);

            bool blnSearch = await tSearch;

            tblConfidence2.Content = blnSearch.ToString();

            stopwatch.Stop();

            tblStatus.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";
        }

        private void btnFacemask_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool blnStarted = tmrFaceMask.IsEnabled;

                if (blnStarted)
                {
                    StopFaceMask();

                    Dispatcher.Invoke(() =>
                    {
                        btnFacemask.Content = "Face Mask";

                        tblXPOSE.Text = "";
                        tblYPOSE.Text = "";
                        tblZPOSE.Text = "";
                        tblConfidence.Text = "";
                        tblConfidence2.Content = "";
                        tblStatus.Text = "";

                        tblHASMASK.Text = "";
                        tblLIVE.Content = "";
                        tblWIDTH.Text = "";
                        tblHEIGHT.Text = "";
                        tblEyes.Text = "";
                        tblConfidence.Text = "";
                        tblRectXPosMin.Text = "";
                        tblRectYPosMin.Text = "";
                    });

                    return;
                }

                if (objWebcam._capture == null || !objWebcam.blnWebcamIsOpened)
                {
                    MessageBox.Show("Please start the Webcam.", "btnFacemask_Click()");
                    return;
                }

                StartFaceMask();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "btnFacemask_Click()");
            }
        }
    }
}
