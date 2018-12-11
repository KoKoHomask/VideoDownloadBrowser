using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace 视频下载器
{
    #region 在同一窗口打开页面
    internal class OpenPageSelf : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures,
IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;
            chromiumWebBrowser.Load(targetUrl);
            return true; //Return true to cancel the popup creation copyright by codebye.com.
        }
    }
    #endregion
    #region reponseFilter
    public class FilterManager
    {
        private static readonly Dictionary<string, IResponseFilter> FilterList = new Dictionary<string, IResponseFilter>();

        public static IResponseFilter CreateFilter(string guid)
        {
            lock (FilterList)
            {
                IResponseFilter filter;

                filter = new TestFilter();
                FilterList.Add(guid, filter);
                return filter;
            }
        }
        public static void RemoveFileter(string guid)
        {
            lock (FilterList)
            {
                FilterList.Remove(guid);
            }
        }

        public static IResponseFilter GetFileter(string guid)
        {
            lock (FilterList)
            {
                return FilterList[guid];
            }
        }
    }
    public class TestFilter : IResponseFilter
    {
        private int contentLength = 0;
        public List<byte> dataAll = new List<byte>();
        public void SetContentLength(int contentLength)
        {
            this.contentLength = contentLength;
        }
        public void Dispose()
        {
            ;
        }

        public FilterStatus Filter(System.IO.Stream dataIn, out long dataInRead, System.IO.Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;
                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                dataAll.AddRange(bs);

                dataInRead = dataIn.Length;
                dataOutWritten = Math.Min(dataInRead, dataOut.Length);

                dataOut.Write(bs, 0, (int)dataOutWritten);
                dataOut.Seek(0, SeekOrigin.Begin);

                if (dataAll.Count < contentLength)
                {
                    return FilterStatus.NeedMoreData;
                }
                else
                {
                    return FilterStatus.Done;
                }
            }
            catch (Exception ex)
            {
                dataOutWritten = 0;
                dataInRead = 0;
                return FilterStatus.Error;
            }
        }

        public bool InitFilter()
        {
            return true;
        }
    }
    #endregion



    public class request : IRequestHandler
    {
        public List<string> downloadLst;
        public event Action<byte[]> NotifyMsg;
        /// <summary>
        /// 传入要判定下载的lst
        /// </summary>
        /// <param name="Lst"></param>
        public request(List<string> Lst)
        {
            downloadLst = Lst;
        }
        public bool CanGetCookies(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return true;
        }

        public bool CanSetCookie(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
        {
            return true;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var url = new Uri(request.Url);
            foreach(var single in downloadLst)
            {
                if (url.AbsoluteUri.Contains(single))
                {
                    var filter = FilterManager.CreateFilter(request.Identifier.ToString());
                    return filter;
                }
            }
            return null;
        }

        //public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        //{
        //    request.Headers.Add("Accept", "*/*");
        //    request.Headers.Add("Accept-Encoding", "gzip, deflate");
        //    request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        //    request.Headers.Add("Connection", "keep-alive");
        //    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36");

        //    var header= request.Headers;
        //    return false;
        //}

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            return CefReturnValue.Continue;
            //throw new System.NotImplementedException();
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return true;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
            ;
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, string url)
        {
            return false;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
            ;
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            ;
        }
        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            foreach(var single in downloadLst)
            {
                if (request.Url.Contains(single))
                {
                    var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestFilter;
                    NotifyMsg?.Invoke(filter.dataAll.ToArray());
                    FilterManager.RemoveFileter(request.Identifier.ToString());
                    break;
                }
            }
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            ;
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            if (request.Url.Contains(".ts"))
            {
                var content_length = int.Parse(response.ResponseHeaders["Content-Length"]);
                var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestFilter;
                filter.SetContentLength(content_length);
            }
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return true;
        }
    }
}
