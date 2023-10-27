using DirectShowLib;
using Emgu.CV;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Xyreon_Temperature_Measuring_System;

namespace Xyreon_Temperature_Measuring_System.Classes
{

    public class clsWebcam : IDisposable
    {
        private class clsWebcamExceptionHandler : ExceptionHandler
        {
            public override bool HandleException(Exception ex2)
            {
                bool result = false;

                try
                {
                    //GlobalKiosk.SystemLog(ex2, "");
                }
                catch (Exception ex)
                {

                    MessageBox.Show("");
                }

                return result;
            }
        }

        public delegate void WebcamStatusChange(bool status, string message);
        public event WebcamStatusChange WebcamStatusChanged;

        public delegate void ImageGrab(Bitmap image);
        public event ImageGrab WebcamImageGrabbed;

        VideoCapture videoCapture = null;
        public Mat liveMatFrame = new Mat();
        public Mat liveMatFrameFlip = new Mat();

        private bool disposedValue = false;
        private int cameraIndex = -1;
        public int resolutionWidth = 0;
        public int resolutionHeight = 0;
        public bool webcamStatus = false;

        Thread GetImageThread = null;

        public clsWebcam(int cameraIndex, int resolutionWidth, int resolutionHeight)
        {
            try
            {
                this.cameraIndex = cameraIndex;
                this.resolutionWidth = resolutionWidth;
                this.resolutionHeight = resolutionHeight;
            }
            catch (Exception ex)
            {

                MessageBox.Show("");
            }
        }

        public async void OpenVideoCapture()
        {
            try
            {
                CvInvoke.UseOpenCL = false;

                int curCameraIndex = GetCameraIndexByIndex(cameraIndex);

                if(curCameraIndex != -1)
                {
                    clsWebcamExceptionHandler handler = new clsWebcamExceptionHandler();
                    videoCapture = new VideoCapture(curCameraIndex);
                    videoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, resolutionWidth);
                    videoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, resolutionHeight);
                    //videoCapture.ImageGrabbed += VideoCapture_ImageGrabbed;
                    //videoCapture.Start(handler);

                    GetImageThread = new Thread(new ThreadStart(ImageGrabbed));
                    GetImageThread.Start();                    
                }
                else
                {
                    webcamStatus = false;
                    WebcamStatusChanged?.Invoke(webcamStatus, "NO WEBCAM FOUND");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("");
            }           
        }

        public void ImageGrabbed()
        {
            try
            {
                if (videoCapture.IsOpened)
                {
                    while (videoCapture.IsOpened)
                    {
                        if (videoCapture.Grab())
                        {
                            liveMatFrame = new Mat();
                            liveMatFrameFlip = new Mat();

                            if (videoCapture.Retrieve(liveMatFrame))
                            {
                                if (liveMatFrame.IsEmpty == false)
                                {
                                    CvInvoke.Flip(liveMatFrame, liveMatFrameFlip, Emgu.CV.CvEnum.FlipType.Horizontal);

                                    webcamStatus = true;
                                    WebcamStatusChanged?.Invoke(webcamStatus, "");

                                    WebcamImageGrabbed?.Invoke(liveMatFrameFlip.ToBitmap());
                                }
                                else
                                {
                                    webcamStatus = false;
                                    WebcamStatusChanged?.Invoke(webcamStatus, "Mat frame is empty");
                                    break;
                                }
                            }
                            else
                            {
                                webcamStatus = false;
                                WebcamStatusChanged?.Invoke(webcamStatus, "Failed to decode grabbed video frame");
                                break;

                            }
                        }
                        else
                        {
                            webcamStatus = false;
                            WebcamStatusChanged?.Invoke(webcamStatus, "Failed to grab a frame");
                            break;
                        }
                    }

                    webcamStatus = false;
                    WebcamStatusChanged?.Invoke(webcamStatus, "Video capture is closed");
                }
                else
                {
                    webcamStatus = false;
                    WebcamStatusChanged?.Invoke(webcamStatus, "Video capture is closed");
                }
            
            }
            catch (Exception ex)
            {

                MessageBox.Show("");
            }
            finally
            {
                liveMatFrame.Dispose();
                liveMatFrameFlip.Dispose();
            }
        }

        public Tuple<byte[], Mat> GetCurrentFrameAsByte()
        {
            Tuple<byte[], Mat> result = null;
            byte[] resultByte = null;
            Mat resultMatFrameClone = new Mat();
            Bitmap bitmap = null;

            try
            {
                if (webcamStatus)
                {
                    if (liveMatFrame.IsEmpty)
                    {
                        return null;
                    }

                    resultMatFrameClone = liveMatFrame.Clone();
                    bitmap = resultMatFrameClone.ToBitmap();
                   
                    System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
                    resultByte = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("");
            }
            finally
            {
                result = new Tuple<byte[], Mat>(resultByte, resultMatFrameClone);
                resultMatFrameClone.Dispose();
                bitmap.Dispose();
            }

            return result;
        }


        private int GetCameraIndexByIndex(int cameraIndex)
        {
            int result = -1;
            try
            {
                DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

                //GlobalKiosk.EventsLog("Total Camera index: " + (_SystemCamereas.Length - 1));

                if (_SystemCamereas != null)
                {
                    if( cameraIndex > (_SystemCamereas.Length - 1))
                    {
                        //GlobalKiosk.EventsLog("Camera Index out of bound. Total Camera index: " + (_SystemCamereas.Length - 1) + ", Selected Camera Index: " + cameraIndex);
                    }
                    else
                    {
                        result = cameraIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("");
            }

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                    GetImageThread.Suspend();

                    if (videoCapture != null)
                    {
                        videoCapture.Stop();
                        videoCapture.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~clsWebcam()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


    public static class clsImageConvertor
    {
        public static BitmapImage BitmapToBitmapImage(Bitmap src)
        {
            BitmapImage image = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                using (WrappingStream wrapper = new WrappingStream(ms))
                {
                    ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                    image.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    image.StreamSource = ms;
                    image.EndInit();


                    wrapper.Dispose();
                }
                ms.Dispose();
            }

            return image;
        }
        public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                using (WrappingStream wrapper = new WrappingStream(outStream))
                {
                    // from System.Media.BitmapImage to System.Drawing.Bitmap
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                    enc.Save(outStream);
                    bitmap = new System.Drawing.Bitmap(outStream);
                    return bitmap;

                }

            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(Bitmap image)
        {
            BitmapSource bitmapSource = null;
            try
            {
                using (image)
                {
                    lock (image)
                    {
                        IntPtr ptr = image.GetHbitmap(); //obtain the Hbitmap

                        bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            ptr,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                        DeleteObject(ptr); //release the HBitmap
                    }

                    image.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("");
            }

            bitmapSource.Freeze();
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

        public static byte[] BitmapSourceToByte(BitmapSource bitmapSource)
        {
            byte[] Result = null;
            try
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.QualityLevel = 100;
                // byte[] bit = new byte[0];
                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);
                    Result = stream.ToArray();
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("");
            }

            return Result;
        }

        public static byte[] BitmapSource2ByteArray(BitmapSource source)
        {
            byte[] Result = null;
            try
            {
                PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                BitmapFrame frame = System.Windows.Media.Imaging.BitmapFrame.Create(source);
                encoder.Frames.Add(frame);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (WrappingStream wrapper = new WrappingStream(stream))
                    {
                        encoder.Save(stream);
                        Result = stream.ToArray();

                        wrapper.Dispose();
                    }

                    stream.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("");
            }
            return Result;


        }

        static byte[] GetBytesFromBitmapSource(BitmapSource bmp)
        {
            byte[] Result = null;
            try
            {
                int width = bmp.PixelWidth;
                int height = bmp.PixelHeight;
                int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

                Result = new byte[height * stride];

                bmp.CopyPixels(Result, stride, 0);


            }
            catch (Exception ex)
            {
                MessageBox.Show("");
            }
            return Result;
        }
    }
}
