using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FaceRecognitionAPI
{
    //--------------------------------------------------
    // result events
    enum CallBackEvent
    {
        CALLBACK_EVENT_GOODFADE = 0,
        CALLBACK_EVENT_NOFACE = 1,
        CALLBACK_EVENT_MULTIFADE = 2,
        CALLBACK_EVENT_HEADPOS = 3,
        CALLBACK_EVENT_BIGFADE = 4,
        CALLBACK_EVENT_SMLFACE = 5,
        CALLBACK_EVENT_EMOTION = 6,
        CALLBACK_EVENT_MOTIVE = 7,
        CALLBACK_EVENT_BRIGHT = 8,
        CALLBACK_EVENT_NOTCENTER = 9,
        CALLBACK_EVENT_LROCC = 10,
        CALLBACK_EVENT_MOCC = 11,
        CALLBACK_EVENT_NOTINROI = 12,

        CALLBACK_EVENT_SUCC = 100,
        CALLBACK_EVENT_FAIL = 101,
        CALLBACK_EVENT_TIMEOUT = 102,
        CALLBACK_EVENT_SNAP = 103,
        CALLBACK_EVENT_CANCEL = 104,

        CALLBACK_EVENT_PREVIEW = 50
    }

    // image type for function ECF_GetImageData
    enum ImageType
    {
        IMAGE_TYPE_VIS = 0,
        IMAGE_TYPE_NIR = 1,
        IMAGE_TYPE_VIS_RC = 2,
        IMAGE_TYPE_NIR_RC = 3,
        IMAGE_TYPE_CROP_VIS = 4,
        IMAGE_TYPE_CROP_NIR = 5
    }

    class EcFaceCamSdkHelper
    {
        //--------------------------------------------------
        public delegate void CallbackDelegate(int eventId, IntPtr context);

        // open camera
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_Open", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_Open(string strParams);

        // close camera
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_Close", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_Close();

        // set video window
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_SetDisplayWindowEx", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_SetDisplayWindowEx(int nWndType, IntPtr hWnd, int left, int top, int right, int bottom);

        // set callback
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_SetCallBack", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_SetCallBack(CallbackDelegate cbd, IntPtr context);

        // start live detect
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_StartDetectAsyn", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_StartDetectAsyn();

        // stop live detect
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_Stop", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_Stop();

        // get result image
        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_GetImageData", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_GetImageData(int nType, byte[] dataBuf, int[] dataLen);

        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_CopyFrameWithAlpha", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_CopyFrameWithAlpha(int nImageType, byte[] pImgJpg, int[] pnJpgLen, int[] pFaceRect);

        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_RotateCamera", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_RotateCamera(int nAngle);

        [DllImport("EcFaceCamSDK.dll", EntryPoint = "ECF_PreviewPause", ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public extern static int ECF_PreviewPause(int nPause);
    }
}
