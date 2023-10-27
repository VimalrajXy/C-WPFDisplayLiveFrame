using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net.Http;

namespace FaceRecognitionAPI
{
    class XYREONAPI
    {
        //public class Liveness
        //{
        //    public string FaceLiveness { get; set; }
        //    public bool HasMask { get; set; }
        //    public int Eyes { get; set; }
        //    public float CompareConfidence { get; set; }
        //    public int FaceRectangleX { get; set; }
        //    public int FaceRectangleY { get; set; }
        //    public int FaceRectangleWidth { get; set; }
        //    public int FaceRectangleHeight { get; set; }
        //    public float HeadPositionX { get; set; }
        //    public float HeadPositionY { get; set; }
        //    public float HeadPositionZ { get; set; }
        //    public string error { get; set; }
        //}

        public class Heartbeat
        {
            public bool Error { get; set; }
        }

        public class Liveness
        {
            public bool Error { get; set; }
            public string ErrorDesc { get; set; }
            public int LivenessCode { get; set; }
            public string LivenessDesc { get; set; }
            public bool HasMask { get; set; }
            public int Eyes { get; set; }
            public bool FaceExist { get; set; }
            public string FaceName { get; set; }
            public int FaceRectangleX { get; set; }
            public int FaceRectangleY { get; set; }
            public int FaceRectangleWidth { get; set; }
            public int FaceRectangleHeight { get; set; }
            public float HeadPositionX { get; set; }
            public float HeadPositionY { get; set; }
            public float HeadPositionZ { get; set; }
        }

        public class FaceDetail
        {
            public bool Error { get; set; }
            public string ErrorDesc { get; set; }
            public int LivenessCode { get; set; }
            public string LivenessDesc { get; set; }
            public bool HasMask { get; set; }
            public int Eyes { get; set; }
            public int FaceRectangleX { get; set; }
            public int FaceRectangleY { get; set; }
            public int FaceRectangleWidth { get; set; }
            public int FaceRectangleHeight { get; set; }
            public float HeadPositionX { get; set; }
            public float HeadPositionY { get; set; }
            public float HeadPositionZ { get; set; }
        }

        public class RecognizeFace
        {
            public bool Error { get; set; }
            public string ErrorDesc { get; set; }
            public bool FaceExist { get; set; }
            public string FaceName { get; set; }
            public int FaceRectangleX { get; set; }
            public int FaceRectangleY { get; set; }
            public int FaceRectangleWidth { get; set; }
            public int FaceRectangleHeight { get; set; }
            public float HeadPositionX { get; set; }
            public float HeadPositionY { get; set; }
            public float HeadPositionZ { get; set; }
        }

        public class FaceMask
        {
            public bool HasMask { get; set; }
            public float Confidence { get; set; }
            public string Liveness { get; set; }
            public int RectPosXMin { get; set; }
            public int RectPosYMin { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public delegate void ClassExceptionHandler(object sender, Exception ex, string strSourceLocation);
        public event ClassExceptionHandler ObjectException;

        public string Api_URL { get; set; }
        public System.Net.HttpStatusCode Api_LastHttpStatusCode { get; set; }
        public string Api_LastError { get; set; }
        public string Api_LastResponse { get; set; }
        public string Api_LastFaceID { get; set; }  
        public string Api_LastObjectUri { get; set; }
        public Heartbeat Api_HeartBeatResponse { get; set; }
        public Liveness Api_LivenessResponse { get; set; }
        public FaceDetail Api_FaceDetail { get; set; }
        public RecognizeFace Api_RecognizeFace { get; set; }
        public List<FaceMask> Api_FaceMask { get; set; }

        public XYREONAPI(string strURL)
        {
            Api_URL = strURL;
        }

        public async Task<bool> GetApiHeartBeat(string WEBSERVICE_URL)
        {
            bool HasResponse = true;
            string Response = "";

            var settings = new JsonSerializerSettings { DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat, NullValueHandling = NullValueHandling.Include, MissingMemberHandling = MissingMemberHandling.Ignore };

            try
            {
                //"http://127.0.0.1:7071/api/heartbeat"
                string URL = WEBSERVICE_URL + "/facerecognitionapi/api/heartbeat";

                string JSONBODY = "";

                Task<string> tGetMethod = GetMethod(URL, JSONBODY);
                //await tGetMethod;

                Response = await tGetMethod;
                tGetMethod.Dispose();

                if (!string.IsNullOrEmpty(Response))
                {

                    Heartbeat res = JsonConvert.DeserializeObject<Heartbeat>(Response, settings);

                    if (res != null)
                    {

                        Api_HeartBeatResponse = new Heartbeat();

                        Api_HeartBeatResponse = res;

                        HasResponse = res.Error;
                        //if (string.IsNullOrEmpty(Api_LastSeachNearest4[0].Error))
                        //{
                        //    Api_LastObjectUri = Api_LastSeachNearest4[0].Error;
                        //    blnResult = true;
                        //}
                        //else
                        //{
                        //    Api_LastObjectUri = Api_LastSeachNearest4[0].Error;
                        //    blnResult = false;
                        //}



                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());

            }

            return HasResponse;
        }

        private async Task<string> GetMethod(string url, string msg, int overrideTimeOut = 0)
        {
            string result = "";
            int INTERNET_TIMEOUT = 2000;

            try
            {


                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);


                myHttpWebRequest.Method = "GET";  //GET
                myHttpWebRequest.ContentType = "application/json";
                myHttpWebRequest.ContentLength = buffer.Length;
                //myHttpWebRequest.ContinueTimeout = 200;
                //myHttpWebRequest.ReadWriteTimeout = 200;
                if (overrideTimeOut == 0)
                    myHttpWebRequest.Timeout = INTERNET_TIMEOUT; //The Timeout property has no effect on asynchronous requests
                else
                    myHttpWebRequest.Timeout = overrideTimeOut; //The Timeout property has no effect on asynchronous requests

                //using (var request = await myHttpWebRequest.GetRequestStreamAsync())  //using (var request = myHttpWebRequest.GetRequestStream())
                //{
                //    await request.WriteAsync(buffer, 0, buffer.Length);
                //    request.Close();
                //}

                //var htp = myHttpWebRequest.GetResponse();
                var htp = await myHttpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var reader = new StreamReader(htp.GetResponseStream(), Encoding.UTF8))
                {
                    result = await reader.ReadToEndAsync();
                    //myHttpWebResponse.Close();
                    htp.Close();
                }
            }
            catch (WebException we)
            {
                result = "";
                if (we.Response != null)
                {
                    result = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                }


            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<bool> HttpUploadFileAsyncCompareLive(string url, string file, byte[] bytImage, string file2, byte[] bytImage2, string paramName, string contentType, NameValueCollection nvc)
        {

            bool blnResult = false;

            try
            {

                Api_LastHttpStatusCode = HttpStatusCode.Unused;
                Api_LastResponse = "";

                //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                string boundary = "479013813620309453828770r";

                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                Stream rs = wr.GetRequestStream();

                //---------------------LIVE IMAGE-------------------------------------------------------//
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string strFileName2 = Path.GetFileName(file);
                string strFileName = Path.GetFileNameWithoutExtension(file);

                string headerTemplate2 = "Content-Disposition: form-data;name=\"{0}\"; filename=\"{1}\"\r\n\r\n";
                string header2 = string.Format(headerTemplate2, "liveframe", strFileName2);

                byte[] headerbytes2 = System.Text.Encoding.UTF8.GetBytes(header2);

                rs.Write(headerbytes2, 0, headerbytes2.Length);

                rs.Write(bytImage, 0, bytImage.Length);

                //---------------------PASSPORT IMAGE-------------------------------------------------------//
                string formdataTemplate2 = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate2, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string strFileName3 = Path.GetFileNameWithoutExtension(file2);

                string headerTemplate3 = "Content-Disposition: form-data;name=\"{0}\"; filename=\"{1}\"\r\n\r\n";
                string header3 = string.Format(headerTemplate3, "faceimage", strFileName3);

                byte[] headerbytes3 = System.Text.Encoding.UTF8.GetBytes(header3);

                rs.Write(headerbytes3, 0, headerbytes3.Length);

                rs.Write(bytImage2, 0, bytImage2.Length);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                WebResponse wresp = null;

                try
                {
                    wresp = await wr.GetResponseAsync().ConfigureAwait(false);

                    Api_LastHttpStatusCode = ((HttpWebResponse)wresp).StatusCode;

                    using (var reader = new StreamReader(wresp.GetResponseStream(), Encoding.UTF8))
                    {
                        Task<string> tRead = reader.ReadToEndAsync();
                        Api_LastResponse = await tRead;
                        wresp.Close();
                    }

                    blnResult = true;

                    //wresp = wr.GetResponse();

                    //Api_LastHttpStatusCode = ((HttpWebResponse)wresp).StatusCode;

                    //Stream stream2 = wresp.GetResponseStream();

                    //StreamReader reader2 = new StreamReader(stream2);

                    //Api_LastResponse = reader2.ReadToEnd();

                    //string strMsg = string.Format("File uploaded, server response is: {0}", Api_LastResponse);

                    //blnResult = true;

                }
                catch (WebException wex)
                {

                    if (wex.Response != null)
                    {

                        using (var errorResponse = (HttpWebResponse)wex.Response)
                        {

                            Api_LastHttpStatusCode = errorResponse.StatusCode;

                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                Task<string> tRead = reader.ReadToEndAsync();
                                Api_LastResponse = await tRead;

                            }
                        }

                        blnResult = true;

                    }

                }
                finally
                {

                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }

                }

            }
            catch (Exception ex)
            {
                if (ObjectException != null) ObjectException(this, ex, "HttpUploadFileAsync2()");
            }

            return blnResult;

        }

        public async Task<bool> FaceCompare(string strImageFileName, byte[] bytImage, string strImageFileName2, byte[] bytImage2)
        {

            bool blnResult = false;

            try
            {

                Api_LastError = "";

                Api_LastObjectUri = "";

                var settings = new JsonSerializerSettings { DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat, NullValueHandling = NullValueHandling.Include, MissingMemberHandling = MissingMemberHandling.Ignore };

                NameValueCollection nvc = new NameValueCollection();

                Task<bool> tUpload = HttpUploadFileAsyncCompareLive(Api_URL + @"/facerecognitionapi/api/comparelive", strImageFileName, bytImage, strImageFileName2, bytImage2, "liveframe", "image/jpeg", nvc);
                bool blnUpload = await tUpload;

                if (blnUpload)
                {

                    if (Api_LastHttpStatusCode == HttpStatusCode.OK)
                    {

                        Liveness res = JsonConvert.DeserializeObject<Liveness>(Api_LastResponse, settings);

                        if (res != null)
                        {

                            Api_LivenessResponse = new Liveness();

                            Api_LivenessResponse = res;

                            //if (string.IsNullOrEmpty(Api_LastSeachNearest4[0].Error))
                            //{
                            //    Api_LastObjectUri = Api_LastSeachNearest4[0].Error;
                            //    blnResult = true;
                            //}
                            //else
                            //{
                            //    Api_LastObjectUri = Api_LastSeachNearest4[0].Error;
                            //    blnResult = false;
                            //}



                        }

                    }
                    else
                    {

                        Liveness res = JsonConvert.DeserializeObject<Liveness>(Api_LastResponse, settings);

                        if (res.Error && res != null)
                        {
                            Api_LivenessResponse = new Liveness();

                            Api_LivenessResponse = res;

                            Api_LastError = res.ErrorDesc;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                if (ObjectException != null) ObjectException(this, ex, "FaceCompare()");
            }

            return blnResult;

        }

        public async Task<Tuple<HttpStatusCode,string>> HttpUploadFileAsync(string url, string file, byte[] bytImage, string paramName, string contentType, NameValueCollection nvc)
        {
            HttpStatusCode htpresult = HttpStatusCode.Unused;
            string responseBody = "";

            try
            {

                Api_LastResponse = "";

                //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                string boundary = "479013813620309453828770r";

                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                Stream rs = wr.GetRequestStream();

                //---------------------LIVE IMAGE-------------------------------------------------------//
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string strFileName2 = Path.GetFileName(file);
                string strFileName = Path.GetFileNameWithoutExtension(file);

                string headerTemplate2 = "Content-Disposition: form-data;name=\"{0}\"; filename=\"{1}\"\r\n\r\n";
                string header2 = string.Format(headerTemplate2, "liveframe", strFileName2);

                byte[] headerbytes2 = System.Text.Encoding.UTF8.GetBytes(header2);

                rs.Write(headerbytes2, 0, headerbytes2.Length);

                rs.Write(bytImage, 0, bytImage.Length);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                WebResponse wresp = null;

                try
                {
                    wresp = await wr.GetResponseAsync().ConfigureAwait(false);

                    htpresult = ((HttpWebResponse)wresp).StatusCode;

                    using (var reader = new StreamReader(wresp.GetResponseStream(), Encoding.UTF8))
                    {
                        Task<string> tRead = reader.ReadToEndAsync();
                        responseBody = await tRead;
                        wresp.Close();
                    }

                    //wresp = wr.GetResponse();

                    //Api_LastHttpStatusCode = ((HttpWebResponse)wresp).StatusCode;

                    //Stream stream2 = wresp.GetResponseStream();

                    //StreamReader reader2 = new StreamReader(stream2);

                    //Api_LastResponse = reader2.ReadToEnd();

                    //string strMsg = string.Format("File uploaded, server response is: {0}", Api_LastResponse);

                    //blnResult = true;

                }
                catch (WebException wex)
                {

                    if (wex.Response != null)
                    {

                        using (var errorResponse = (HttpWebResponse)wex.Response)
                        {

                            htpresult = errorResponse.StatusCode;

                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                Task<string> tRead = reader.ReadToEndAsync();
                                responseBody = await tRead;

                            }
                        }

                    }

                }
                finally
                {

                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }

                }

            }
            catch (Exception ex)
            {
                if (ObjectException != null) ObjectException(this, ex, "HttpUploadFileAsync3()");
            }

            Tuple<HttpStatusCode, string> tpResponse = new Tuple<HttpStatusCode, string>(htpresult,responseBody);
            return tpResponse;

        }

        public async Task<FaceDetail> LiveDetect(string strImageFileName, byte[] bytImage)
        {

            FaceDetail responseFaceDetail = new FaceDetail();

            try
            {

                Api_LastError = "";

                Api_LastObjectUri = "";

                Tuple<HttpStatusCode, string> tplResponse = null;

                HttpStatusCode htp = HttpStatusCode.Unused;

                var settings = new JsonSerializerSettings { DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat, NullValueHandling = NullValueHandling.Include, MissingMemberHandling = MissingMemberHandling.Ignore };

                NameValueCollection nvc = new NameValueCollection();

                Task<Tuple<HttpStatusCode,string>> tUpload = HttpUploadFileAsync(Api_URL + @"/facerecognitionapi/api/getfacedetail", strImageFileName, bytImage, "liveframe", "image/jpeg", nvc);
                tplResponse = await tUpload;

                if (tplResponse.Item1 == HttpStatusCode.OK)
                {

                    FaceDetail res = JsonConvert.DeserializeObject<FaceDetail>(tplResponse.Item2, settings);

                    if (res != null)
                    {



                        responseFaceDetail = res;

                        //if (string.IsNullOrEmpty(Api_FaceDetail.ErrorDesc))
                        //{
                        //    Api_LastError = Api_FaceDetail.ErrorDesc;
                        //    blnResult = true;
                        //}
                        //else
                        //{
                        //    Api_LastError = Api_FaceDetail.ErrorDesc;
                        //    blnResult = false;
                        //}



                    }

                }
                else
                {

                    FaceDetail res = JsonConvert.DeserializeObject<FaceDetail>(tplResponse.Item2, settings);

                    if (res != null && res.Error)
                    {
                        responseFaceDetail = res;

                        Api_LastError = res.ErrorDesc;

                    }

                }

            }
            catch (Exception ex)
            {
                if (ObjectException != null) ObjectException(this, ex, "LiveDetect()");
            }

            return responseFaceDetail;

        }

        public async Task<RecognizeFace> RecogniseFace(string strImageFileName, byte[] bytImage)
        {
            Tuple<HttpStatusCode, string> tplResponse = null;

            RecognizeFace recognize = new RecognizeFace();

            try
            {

                Api_LastError = "";

                Api_LastObjectUri = "";

                HttpStatusCode htp = HttpStatusCode.Unused;

                var settings = new JsonSerializerSettings { DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat, NullValueHandling = NullValueHandling.Include, MissingMemberHandling = MissingMemberHandling.Ignore };

                NameValueCollection nvc = new NameValueCollection();

                Task<Tuple<HttpStatusCode,string>> tUpload = HttpUploadFileAsync(Api_URL + @"/facerecognitionapi/api/recognizeface", strImageFileName, bytImage, "liveframe", "image/jpeg", nvc);
                tplResponse = await tUpload;

                if (tplResponse.Item1 == HttpStatusCode.OK)
                {

                    RecognizeFace res = JsonConvert.DeserializeObject<RecognizeFace>(tplResponse.Item2, settings);

                    if (res != null)
                    {

                        recognize = res;

                        //if (string.IsNullOrEmpty(Api_RecognizeFace.ErrorDesc))
                        //{
                        //    Api_LastError = Api_RecognizeFace.ErrorDesc;
                        //    Api_LastFaceID = Api_RecognizeFace.FaceName;
                        //    blnResult = true;
                        //}
                        //else
                        //{
                        //    Api_LastError = Api_RecognizeFace.ErrorDesc;
                        //    blnResult = false;
                        //}



                    }

                }
                else
                {

                    RecognizeFace res = JsonConvert.DeserializeObject<RecognizeFace>(tplResponse.Item2, settings);

                    if (res != null && res.Error)
                    {
                        recognize = res;

                        Api_LastError = res.ErrorDesc;
                    }

                }

            }
            catch (Exception ex)
            {
                if (ObjectException != null) ObjectException(this, ex, "RecogniseFace()");
            }

            return recognize;

        }

        public async Task<string> GetFaceDetail(byte[] bytImage)
        {
            string result = "";

            try
            {
                string METHOD = "POST";

                //"http://aims01.jeysoftware.com/StockOnline.api/OfficialReceipt/"
                string URL = "http://127.0.0.1:7071/api/test2";

                string CONTENT_TYPE = "image/jpeg";

                string webAPIstartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string webAPIendTime = "";

                Task<string> tPostMethod = PostMethod2(URL, bytImage, CONTENT_TYPE, METHOD);
                await tPostMethod;

                webAPIendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                result = tPostMethod.Result;


            }
            catch (Exception ex)
            {
                result = ex.ToString() + "\r\n\r\n";
            }

            return result;

        }

        async Task<string> PostMethod2(string url, byte[] bytImage, string contentType, string method, int overrideTimeOut = 0)
        {
            string result = "";
            int INTERNET_TIMEOUT = 20000;


            try
            {

                byte[] buffer = bytImage;
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);


                myHttpWebRequest.Headers.Add("Authorization", contentType);
                myHttpWebRequest.Method = method;  //GET
                myHttpWebRequest.ContentType = contentType;
                myHttpWebRequest.ContentLength = buffer.Length;
                if (overrideTimeOut == 0)
                    myHttpWebRequest.Timeout = INTERNET_TIMEOUT; //The Timeout property has no effect on asynchronous requests
                else
                    myHttpWebRequest.Timeout = overrideTimeOut; //The Timeout property has no effect on asynchronous requests

                using (var request = await myHttpWebRequest.GetRequestStreamAsync())  //using (var request = myHttpWebRequest.GetRequestStream())
                {
                    await request.WriteAsync(buffer, 0, buffer.Length);
                    request.Close();
                }

                //var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                var myHttpWebResponse = await myHttpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.UTF8))
                {
                    result = await reader.ReadToEndAsync();
                    myHttpWebResponse.Close();
                }

                //buffer = null;
                //bytImage = null;
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    result = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    result = we.Message;

                }

            }
            catch (Exception ex)
            {
                //throw ex;
                result = ex.Message;
            }
            return result;
        }
    }
}
