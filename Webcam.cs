using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceRecognitionAPI
{
    public class Webcam
    {
        //ADDED
        public enum CameraType
        {
            Realsense = 1,
            Eyecool = 2
        }

        public VideoCapture _capture = null;
        public Mat _frame;
        public Mat _frameFlip;
        public Mat _frameFlip5;

        string startupPath = "";

        public bool blnWebcamIsOpened = false;
        public int intCountFaces = 0;
        public bool blnHasGoodFace = false;
        public bool blnIsProcessing = false;

        public delegate void CallbackWebcamStatusChanged(string strMessage);
        public CallbackWebcamStatusChanged DelWebcamStatusChanged { get; set; }

        public delegate void CallbackOnceFaceDetected(BitmapSource bitmapSource, bool hasFace, bool hasMask, int FacePosX, int FacePosY, int FaceWidth, int FaceHeight);
        public CallbackOnceFaceDetected DelOnceFaceDetected { get; set; }

        public delegate void CallbackFaceDetectError();
        public CallbackFaceDetectError DelFaceDetectError { get; set; }

        public delegate void CallbackPromptUserComeCloser(bool blnPrompt);
        public CallbackPromptUserComeCloser DelPromptUserComeCloser { get; set; }

        public delegate void CallbackImageGrabbed(BitmapSource bitmapSource);
        public CallbackImageGrabbed DelWebcamImageGrabbed { get; set; }

        public delegate void CallbackImageGrabbed2(BitmapSource bitmapSource);
        public CallbackImageGrabbed2 DelWebcamImageGrabbed2 { get; set; }

        //ADDED
        public CameraType camType { get; set; } = CameraType.Eyecool;

        public int cameraRotation = 0;

        //private static int = 0;
        public async Task OpenWebcam(string WebcamName)
        {
            try
            {
                await Task.Delay(100);

                int intIndex = Convert.ToInt32(getFileSetting("CameraIndex"));

                //ADDED
                string CameraName = getFileSetting("CameraName");
                int RotateUp = Convert.ToInt32(getFileSetting("RotateUpDegree"));
                int RotateDown = Convert.ToInt32(getFileSetting("RotateDownDegree"));

                //ADDED
                if (CameraName.ToUpper() == "EYECOOL")
                {
                    camType = CameraType.Eyecool;
                }
                else
                {
                    camType = CameraType.Realsense;
                }

                //ADDED
                if (camType == CameraType.Eyecool)
                {
                    int res = OpenCamera();
                    //if (CallEventLog != null) CallEventLog?.Invoke($"EyeCool Open Camera : {res}");

                    if (res > 0)
                    {
                        //if (CallEventLog != null) CallEventLog?.Invoke($"EyeCool Connected");
                    }
                    else
                    {
                        //if (CallEventLog != null) CallEventLog?.Invoke($"EyeCool Open fail");
                    }
                }
                else
                {
                    DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

                    if (_SystemCamereas != null)
                    {
                        for (int i = 0; i < _SystemCamereas.Length; i++)
                        {

                            if (_SystemCamereas[i].Name.Equals(WebcamName))
                            {
                                intIndex = i;
                            }

                        }
                    }
                    else
                    {
                        DelWebcamStatusChanged("No Webcam Attached!");
                        return;
                    }

                    CvInvoke.UseOpenCL = false;

                    //MessageBox.Show(intIndex.ToString());
                    _capture = new VideoCapture(intIndex, VideoCapture.API.DShow);


                    _capture.SetCaptureProperty(CapProp.FrameWidth, 1280.0);
                    _capture.SetCaptureProperty(CapProp.FrameHeight, 720.0);
                    _capture.SetCaptureProperty(CapProp.Fps, 60);

                    //var expval = _capture.GetCaptureProperty(CapProp.Exposure);
                    _capture.ImageGrabbed += _capture_ImageGrabbed;

                    _frame = new Mat();
                    _frameFlip = new Mat();
                    _frameFlip5 = new Mat();
                    _capture.Start();

                    blnWebcamIsOpened = true;

                    //MessageBox.Show("Started!");
                    //DelWebcamStatusChanged("");

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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
        public void CloseWebcam()
        {
            //ADDED
            if (camType == CameraType.Eyecool)
            {
                CloseCamera();
            }
            else
            {
                ReleaseData();
            }
        }

        private void _capture_ImageGrabbed(object sender, EventArgs e)
        {

            try
            {
                if (_capture != null && _capture.Ptr != IntPtr.Zero)
                {

                    _capture.Retrieve(_frame, 0);

                    CvInvoke.Flip(_frame, _frameFlip, FlipType.Horizontal);
                    

                    BitmapSource bitmapSource = clsImageConvertor.ToBitmapSource(_frameFlip.ToBitmap());
                    bitmapSource.Freeze();
                    DelWebcamImageGrabbed2(bitmapSource);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ReleaseData()
        {
            try
            {
                if (_capture != null)
                {

                    _capture.Stop();

                    blnWebcamIsOpened = false;

                    _capture.ImageGrabbed -= _capture_ImageGrabbed;

                    _capture.Dispose();

                    //_frame = null;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //VIMALRAJ
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public async Task<bool> SaveWebcamImage(string strFileName, BitmapSource bitmapSource)
        {

            bool blnResult = false;

            try
            {
                await Task.Delay(1);
                if (string.IsNullOrEmpty(strFileName)) return blnResult;

                //if (_frame is null) return blnResult;

                //BitmapSource bmp = clsImageConvertor.ToBitmapSource(_frame);

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                BitmapFrame outputFrame = BitmapFrame.Create(bitmapSource);
                encoder.Frames.Add(outputFrame);
                encoder.QualityLevel = 100;

                using (FileStream file = File.OpenWrite(strFileName))
                {
                    encoder.Save(file);
                }

                blnResult = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return blnResult;

        }

        //ADDED
        #region EYECOOL
        EcFaceCamSdkHelper.CallbackDelegate m_callback_delegate = null;
        string strParams = "";
        public byte[] imgBuf;
        public byte[] imgBuf1;

        public void SaveImage(byte[] jpgImg, int len, string path)
        {
            FileStream fs = File.Create(path);
            fs.Write(jpgImg, 0, len);
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// According to eyecool documentation last 8 bytes of the byte array consist of face coordinates, width and height if use ImageType IMAGE_TYPE_VIS_RC for ECF_GetImageData function call.
        /// </summary>
        /// <param name="imBuf"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public (int, int, int, int) TranslateImageData(byte[] imBuf, int[] len)
        {
            int fHeight = 0;
            int fWidth = 0;
            int fY = 0;
            int fX = 0;

            byte[] faceHeight = new byte[2];
            byte[] faceX = new byte[2];
            byte[] faceY = new byte[2];
            byte[] faceWidth = new byte[2];

            try
            {
                faceX[0] = imgBuf[len[0] - 8];
                faceX[1] = imgBuf[len[0] - 7];

                faceY[0] = imgBuf[len[0] - 6];
                faceY[1] = imgBuf[len[0] - 5];

                faceWidth[0] = imgBuf[len[0] - 4];
                faceWidth[1] = imgBuf[len[0] - 3];

                faceHeight[0] = imgBuf[len[0] - 2];
                faceHeight[1] = imgBuf[len[0] - 1];

                fHeight = BitConverter.ToInt16(faceHeight, 0);
                fWidth = BitConverter.ToInt16(faceWidth, 0);
                fY = BitConverter.ToInt16(faceY, 0);
                fX = BitConverter.ToInt16(faceX, 0);

                Console.WriteLine($"fHeight : {fHeight}");
                Console.WriteLine($"fWidth : {fWidth}");
                Console.WriteLine($"fY : {fY}");
                Console.WriteLine($"fX : {fX}");

                //if (CallEventLog != null) CallEventLog?.Invoke($"fHeight : {fHeight}");
                //if (CallEventLog != null) CallEventLog?.Invoke($"fWidth : {fWidth}");
                //if (CallEventLog != null) CallEventLog?.Invoke($"fY : {fY}");
                //if (CallEventLog != null) CallEventLog?.Invoke($"fX : {fX}");

            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }

            return (fHeight, fWidth, fY, fX);
        }

        // this function will be called when the event is triggered.
        public void funCallBackEvent(int eventId, IntPtr context)
        {
            try
            {
                //if (CallEventLog != null) CallEventLog?.Invoke("event id : " + eventId.ToString());
                if (eventId >= (int)CallBackEvent.CALLBACK_EVENT_SUCC)      // LiveDetection is completed
                {
                    switch (eventId)
                    {
                        case (int)CallBackEvent.CALLBACK_EVENT_SUCC:

                            ////Get Result image after crop and save it
                            imgBuf1 = new byte[200000];
                            int[] len1 = new int[1];
                            EcFaceCamSdkHelper.ECF_GetImageData((int)ImageType.IMAGE_TYPE_CROP_VIS, imgBuf1, len1);

                            // get result image with face coordinates and width and height
                            imgBuf = new byte[200000];
                            int[] len = new int[1];
                            EcFaceCamSdkHelper.ECF_GetImageData((int)ImageType.IMAGE_TYPE_VIS_RC, imgBuf, len);

                            //if (SendData != null) SendData?.Invoke(imgBuf, len);

                            // save to file
                            string path = "result.jpg";
                            SaveImage(imgBuf1, len1[0], path);

                            // save to file
                            string path1 = "result2.jpg";
                            SaveImage(imgBuf, len[0] - 32, path1);

                            break;

                        case (int)CallBackEvent.CALLBACK_EVENT_FAIL:
                            // operate UI in thread
                            //Dispatcher.BeginInvoke(new m_ui_show_msg(ShowMsg), "failed");
                            //if (CallEventLog != null) CallEventLog?.Invoke("failed");
                            break;

                        case (int)CallBackEvent.CALLBACK_EVENT_TIMEOUT:
                            // operate UI in thread
                            //Dispatcher.BeginInvoke(new m_ui_show_msg(ShowMsg), "timeout");
                            //if (CallEventLog != null) CallEventLog?.Invoke("timeout");
                            break;

                        case (int)CallBackEvent.CALLBACK_EVENT_CANCEL:
                            // operate UI in thread
                            //Dispatcher.BeginInvoke(new m_ui_show_msg(ShowMsg), "cancel");
                            //if (CallEventLog != null) CallEventLog?.Invoke("cancel");
                            break;
                    }
                }
                else if (eventId == (int)CallBackEvent.CALLBACK_EVENT_PREVIEW)      // preview event
                {
                    // get preview data
                    byte[] jpgCol = new byte[200 * 1024];
                    int[] jpgColLen = new int[1];
                    int[] faceCoord = new int[4];
                    EcFaceCamSdkHelper.ECF_CopyFrameWithAlpha((int)ImageType.IMAGE_TYPE_VIS, jpgCol, jpgColLen, faceCoord);

                    try
                    {
                        // operate UI in thread
                        //Dispatcher.BeginInvoke(new m_ui_show_image_delegate(ShowColImage), jpgCol);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(jpgCol);
                        image.EndInit();

                        var imagetopass = image.Clone();
                        if (imagetopass != null)
                        {
                            imagetopass.Freeze();

                            //WebcamImageGrabbed?.Invoke(imagetopass);
                            DelWebcamImageGrabbed2(imagetopass);
                        }
                        else
                        {
                            // dont know what to do.
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    //WebcamImageGrabbed?.Invoke(image);

                }
                else
                {
                    // other event. ignored.
                }
            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public int OpenCamera()
        {
            int nRet = 0;

            try
            {
                // set callback
                if (m_callback_delegate == null)
                {
                    m_callback_delegate = new EcFaceCamSdkHelper.CallbackDelegate(funCallBackEvent);
                }
                EcFaceCamSdkHelper.ECF_SetCallBack(m_callback_delegate, IntPtr.Zero);

                // open
                //string strParams = textBoxParams.Text; //
                strParams = System.Windows.Forms.Application.StartupPath + "\\" + "xmlSamples.txt";

                if (File.Exists(strParams))
                {
                    string xmlParams = File.ReadAllText(strParams);
                    nRet = EcFaceCamSdkHelper.ECF_Open(xmlParams);
                }
                else
                {
                    //if (CallEventLog != null) CallEventLog?.Invoke("can't find xmlSamples.txt");
                    //this.Close();
                }


                //ShowMsg("ECF_Open=" + nRet.ToString());
                //if (CallEventLog != null) CallEventLog?.Invoke("ECF_Open=" + nRet.ToString());
            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }

            return nRet;
        }

        public int CloseCamera()
        {
            int nRet = 0;

            try
            {
                nRet = EcFaceCamSdkHelper.ECF_Close();
                //ShowMsg("ECF_Close=" + nRet.ToString());
                //if (CallEventLog != null) CallEventLog?.Invoke("ECF_Close=" + nRet.ToString());
            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }

            return nRet;
        }

        public int StartDetectionAsync()
        {
            int nRet = 0;

            try
            {
                nRet = EcFaceCamSdkHelper.ECF_StartDetectAsyn();
                //ShowMsg("ECF_StartDetectAsyn=" + nRet.ToString());
                //if (CallEventLog != null) CallEventLog?.Invoke("ECF_StartDetectAsyn=" + nRet.ToString());
            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }

            return nRet;
        }

        public int StopDetectionAsync()
        {
            int nRet = 0;

            try
            {
                nRet = EcFaceCamSdkHelper.ECF_Stop();
                //ShowMsg("ECF_Stop=" + nRet.ToString());
                //if (CallEventLog != null) CallEventLog?.Invoke("ECF_Stop=" + nRet.ToString());
            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }

            return nRet;
        }

        public void RotateCamera(int angle)
        {
            try
            {
                EcFaceCamSdkHelper.ECF_RotateCamera(angle);
            }
            catch (Exception ex)
            {
                //if (CallSysLog != null) CallSysLog?.Invoke(ex);
                MessageBox.Show(ex.Message);
            }
        }
        #endregion EYECOOL

    }

    public static class clsImageConvertor
    {
        public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                // from System.Media.BitmapImage to System.Drawing.Bitmap
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
                return bitmap;
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            BitmapSource bitmapSource = null;
            try
            {
                using (System.Drawing.Bitmap source = bitmap)
                {
                    lock (source)
                    {
                        IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                        bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            ptr,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                        DeleteObject(ptr); //release the HBitmap
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return bitmapSource;
        }

        public static byte[] ImageSourceToBytes(BitmapImage imageC)
        {

            MemoryStream memStream = new MemoryStream();

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(imageC));

            encoder.Save(memStream);

            return memStream.ToArray();

        }

        public static byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {

            byte[] result = null;

            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, bitmap.RawFormat);
                result = stream.ToArray();
            }

            return result;
        }


        public static System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

    }
}

