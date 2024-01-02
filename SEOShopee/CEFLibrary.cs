using CefSharp.Handler;
using CefSharp.WinForms;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using xNet;
using System.CodeDom;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.IO;
using CefSharp.DevTools.Network;
using System.Threading;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;
using System.Security.Policy;
using System.Windows.Forms;
using CefSharp.Enums;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SEOShopee
{

    public class CustomDialogHandler : IDialogHandler
    {

        public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, IFileDialogCallback callback)
        {


            return true;

        }


    }
    public class CustomResourceRequestHandler : ResourceRequestHandler
    {
        public Flag flag { get; set; }
        private readonly System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

        public CustomResourceRequestHandler(Flag flag)
        {
            this.flag = flag;

        }
        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return new CefSharp.ResponseFilter.StreamResponseFilter(memoryStream);
        }
        protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {

            if (request.Url.Contains("https://shopee.vn/api/v4/search/search_items?"))
            {
                var bytes = memoryStream.ToArray();
                var get = System.Text.Encoding.UTF8.GetString(bytes);
                dynamic obj = JsonConvert.DeserializeObject(get);
                var items = obj["items"];

                if (items == null)
                {
                    TkHelp.ChangeTextDgv(flag.browser_name, "Hết trang", 2);
                    flag.end_page = true;

                }
            }


        }
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {

            return CefReturnValue.Continue;
        }
    }

    public class CustomRequestHandler : IRequestHandler
    {
        public Flag flag { get; set; }
        public CustomRequestHandler(Flag flag)
        {
            this.flag = flag;
        }

        //Phương thức trả về khi trình duyệt cần xác thực 
        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            //Trả lại khi gọi 1 authen
            return false;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new CustomResourceRequestHandler(flag);
            // return null;

        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {

            return false;



            //gọi trước khi browser navigate - nếu chấp nhận sẽ trả về frameloadstart và frameloadend
            //false sẽ chạy tiêp
            //Nếu bị cancel sẽ trả về framloaderror

        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //Được gọi khi url là http
            return true;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

            //Được gọi khi windowdocument đã tạo xong

        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return true;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {

        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;

        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }


        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;

        }
        bool IRequestHandler.GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {

            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            if (isProxy)
            {

                callback.Continue(flag.proxy.username, flag.proxy.password);
                TkHelp.ChangeTextDgv(flag.browser_name, $"Add proxy {flag.proxy.host}:{flag.proxy.port}!", 2);

                return true;
            }

            return false;

        }
    }
    public class CustomLifeSpanHandler : ILifeSpanHandler
    {
        public Flag flag { get; set; }
        public CustomLifeSpanHandler(Flag flag)
        {
            this.flag = flag;
        }

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {


        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            //if (targetUrl.Contains("google.com"))
            //{

            var poup_browser = (ChromiumWebBrowser)chromiumWebBrowser;
            ChromiumWebBrowser chromiumBrowser = null;
            poup_browser.Invoke(new Action(() =>
            {

                poup_browser.Size = new Size(400, 600);
                poup_browser.Location = new Point(300 * flag.browser_index, 0);

                var owner = poup_browser.FindForm();//Tìm form sở hữu
                chromiumBrowser = new ChromiumWebBrowser(targetUrl);
                //chromiumBrowser.LifeSpanHandler = this;
                CustomLifeSpanHandler life = new CustomLifeSpanHandler(flag);
                chromiumBrowser.LifeSpanHandler = life;
                flag.browser = chromiumBrowser;
                chromiumBrowser.SetAsPopup();
                //cefHelper.DisableWebRTC();
                chromiumBrowser.IsBrowserInitializedChanged += ChromiumBrowser_IsBrowserInitializedChanged;


                //chromiumBrowser.SetAsPopup();




                chromiumBrowser.Size = new Size(400, 600);
                chromiumBrowser.Location = new Point(300 * flag.browser_index, 0);

            }));



            newBrowser = chromiumBrowser;
            return false;




            //}
            //newBrowser = null;
            //return true;


        }

        private void ChromiumBrowser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            Cef.UIThreadTaskFactory.StartNew(async delegate
            {
                if (CEFStatic.isProxy)
                {
                    try
                    {


                        var rc = flag.browser.GetBrowser().GetHost().RequestContext;
                        var dict = new Dictionary<string, object>();
                        dict.Add("mode", "fixed_servers");
                        dict.Add("server", $"{flag.proxy.host}:{flag.proxy.port}");
                        string error = string.Empty;

                        var check = rc.SetPreference("proxy", dict, out error);

                    }
                    catch (Exception err)
                    {
                        TkHelp.Comment("Proxy err:" + err.Message);
                        return;
                    }
                }
                //await flag.browser.LoadUrlAsync(flag.u);
            });
        }
    }
    public class CustomMenuHandler : IContextMenuHandler
    {
        private const int ShowDevTools = 26501;
        private const int CloseDevTools = 26502;
        private const int Reload = 26503;
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //Xóa bảng
            // model.Clear();

            //Xóa item có sẵn
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

            //Add new custom menu items
            model.AddItem((CefMenuCommand)Reload, "Reload");
            model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
            model.AddItem((CefMenuCommand)CloseDevTools, "Close DevTools");
            model.AddItem(CefMenuCommand.CustomFirst, "Inspect");
        }
        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == ShowDevTools)
            {
                browser.ShowDevTools();
            }
            if ((int)commandId == CloseDevTools)
            {
                browser.CloseDevTools();
            }
            if ((int)commandId == Reload)
            {
                browser.Reload();
            }

            if (commandId != CefMenuCommand.CustomFirst)
            {
                return false;
            }
            else
            {
                browser.ShowDevTools(null, parameters.XCoord, parameters.YCoord);

                return true;
            }
            return false;
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }

    public class CustomExtensionHandler : IExtensionHandler
    {
        public Action<string> LoadExtensionPopup;

        public bool CanAccessBrowser(IExtension extension, IBrowser browser, bool includeIncognito, IBrowser targetBrowser)
        {
            return true;
        }

        public void Dispose()
        {
            LoadExtensionPopup = null;

        }

        public IBrowser GetActiveBrowser(IExtension extension, IBrowser browser, bool includeIncognito)
        {
            return browser;
        }

        public bool GetExtensionResource(IExtension extension, IBrowser browser, string file, IGetExtensionResourceCallback callback)
        {
            return true;
        }

        public bool OnBeforeBackgroundBrowser(IExtension extension, string url, IBrowserSettings settings)
        {
            // Xử lý trước khi trình duyệt trình duyệt điều hướng đến URL được yêu cầu
            return false;
        }

        public bool OnBeforeBrowser(IExtension extension, IBrowser browser, IBrowser activeBrowser, int index, string url, bool active, IWindowInfo windowInfo, IBrowserSettings settings)
        {
            //Được gọi khi một yêu cầu gọi extension được nhận.
            return false;
        }

        public void OnExtensionLoaded(IExtension extension)
        {
            TkHelp.Comment($"OnExtensionLoaded {extension}");

            var manifest = extension.Manifest;
            var identifier = extension.Identifier;
            var getJson = manifest.ToString();
            foreach (var key_manifest in manifest)
            {
                if (key_manifest.Key == "content_scripts")
                {
                    var obj = key_manifest.Value.GetList();
                    var list_script = obj.First().GetDictionary();
                    foreach (var script in list_script)
                    {
                        if (script.Key == "ks")
                        {
                            var js = script.Value;
                        }
                    }
                }

            }
        }

        public void OnExtensionLoadFailed(CefErrorCode errorCode)
        {
            TkHelp.Comment($"{errorCode}");
        }
        //Được gọi khi extension bị hủy.
        public void OnExtensionUnloaded(IExtension extension)
        {
            TkHelp.Comment($"OnExtensionUnloaded {extension}");

        }
    }

}
