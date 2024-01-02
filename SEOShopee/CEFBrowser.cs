using CefSharp.WinForms;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.DevTools;
using System.Drawing;
using Newtonsoft.Json;
using CefSharp.DevTools.CSS;
using System.Xml.Linq;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using CefSharp.Handler;
using System.Security.Policy;
using System.Windows.Input;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.CodeDom;
using SEOShopee.Properties;
using System.Xml;

namespace SEOShopee
{
    public class CEFBrowser
    {
        public ChromiumWebBrowser browser;



        /// <summary>
        /// Control (groupbox hoặc panel đang điều khiển nó)
        /// </summary>
        public GroupBox thiscontrol { get; set; }
        public CEFAction CEFAction { get; set; }

        public Flag flag { get; set; }

        /// <summary>
        /// Cài đặt mặc định cần truyền vào 1 phương thức Flag chứa các cài đặt đc thiết lập và control là groupbox chứa browser
        /// </summary>
        /// <param name="flag">Cài đặt được thiết lập trước</param>
        /// <param name="thiscontrol">Control chứa browser</param>
        public CEFBrowser(Flag flag, GroupBox thiscontrol)
        {
            this.flag = flag;
            this.thiscontrol = thiscontrol;
            flag.browser_name = thiscontrol.Name;
            flag.browser_index = int.Parse(flag.browser_name.Replace("Browser_", string.Empty));

        }

        /// <summary>
        /// Tạo browser và gán nó vào control đưa vào
        /// </summary>
        /// <param name="webrtc">tắt webrtc khi mở browser</param>
        /// <param name="main_control">main control được browser gán vào</param>
        public void CreateBrowser(bool webrtc, Control main_control, out string log_err)
        {
            log_err = "";


            try
            {
                browser = new ChromiumWebBrowser(CEFStatic.url);
                CEFAction = new CEFAction(browser);

                //Cài đặt tổng thể
                flag.browser_name = thiscontrol.Name;
                flag.browser_index = int.Parse(thiscontrol.Name.Replace("Browser_", ""));




                //Cài đặt proxy
                if (CEFStatic.isProxy)
                {

                    var index = CEFStatic.proxy_index;
                    if (index >= CEFStatic.list_proxy.Count)
                    {
                        index = 0;
                    }
                    flag.proxy = CEFStatic.list_proxy[index];


                }




                //Lấy ra account từ server


                // Login shopee -điểm danh , nếu không điểm danh đc thì chạy tiếp
                CEFStatic.index_account++;
                if (CEFStatic.index_account >= SetUp._accountsActive.Count())
                {
                    TkHelp.Comment("HẾT TÀI KHOẢN!");
                    return;
                }
                flag.shopee.account = SetUp._accountsActive[CEFStatic.index_account];
                flag.shopee.is_get_coin = true;

                if (!flag.shopee.Login())
                {
                    TkHelp.ChangeTextDgv(flag.browser_name, flag.shopee.err_msg, 1);
                    CEFAction.CloseBrowser(this.browser, this.thiscontrol);
                    return;
                }
                TkHelp.ChangeTextDgv(flag.browser_name, "login thành công", 1);


                //Cài đặt path cho cef
                RequestContextSettings contextsetting = CEFAction.ContextSettings(5);

                //Cài đặt các phương thức vận hành khi tiến hành Request
                browser.RequestContext = new RequestContext(contextsetting);

                //Cài đặt proxy
                //if (CEFStatic.isProxy)
                //{
                //    browser.RequestContext.SetProxyAsync(flag.proxy.host, flag.proxy.port);
                //}

                //Cài đặt các phương thức vận hành khi browser làm việc
                browser.RequestHandler = new CustomRequestHandler(flag);

                //Cài đặt mở dialog
                browser.DialogHandler = new CustomDialogHandler();
                //browser.DragHandler = new CustomDragHandler();


                //Cài đặt xử lý vòng đời của các  popup, tab    
                var life = new CustomLifeSpanHandler(flag);
                browser.LifeSpanHandler = life;

                //Cài đặt right-click
                browser.MenuHandler = new CustomMenuHandler();

                //Tắt webtc
                if (webrtc)
                {
                    string err = "";
                    browser.RequestContext.SetPreference("webrtc.ip_handling_policy", "disable_non_proxied_udp", out err);
                    if (!string.IsNullOrEmpty(err))
                    {
                        log_err += "err set webrtc:" + err + "";

                    }

                }


                main_control.BeginInvoke((Action)delegate ()
                {
                    main_control.Controls.Add(browser);
                });



            }
            catch (Exception err)
            {
                log_err += "cactch err:" + err.Message + "";

            }


            if (CEFStatic.isLive && !string.IsNullOrEmpty(CEFStatic.Key))
            {
                browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
                browser.FrameLoadEnd += Browser_FrameLoadEnd_Main;
                browser.FrameLoadEnd += Browser_FrameLoadEnd_Sub;
                browser.FrameLoadEnd += Browser_FrameLoadEnd_Closed;

            }


            browser.FrameLoadStart += CheckDeleteWindow;

            timer.Start();//Check thời gian đóng windown
            timer.Tick += Timer_Tick;
        }

        private void Browser_FrameLoadEnd_Sub(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Url.Contains("shopee") && e.Url.Contains("-i.") && !flag.is_running)
            {
                if (TkHelp.RegexMatch(e.Url))
                {
                    flag.is_running = true;

                    Cef.UIThreadTaskFactory.StartNew(async delegate
                    {
                        TkHelp.ChangeTextDgv(flag.browser_name, $"Lướt trang sản phẩm!", 2);

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        await CEFAction.Mouse_Scroll(-250, flag.browser_name);
                        try
                        {
                            var url_fragment = TkHelp.GetHtmlFragment(e.Url);

                            Regex r = new Regex(@"i.(\d{6,12}).(\d{6,12})");
                            Match m = r.Match(url_fragment);
                            if (m.Success)
                            {
                                var seo = new SEO { item_id = m.Groups[2].Value, shop_id = m.Groups[1].Value };
                                if (TkHelp.RandomNumber(0, 101) <= CEFStatic.percent_seo)
                                {
                                    if (TkHelp.RandomNumber(0, 101) <= CEFStatic.percent_seo)
                                    {
                                        TkHelp.ChangeTextDgv(flag.browser_name, $"Like sản phẩm", 2);
                                        flag.shopee.Like(seo.shop_id, seo.item_id);

                                    }

                                    if (TkHelp.RandomNumber(0, 101) <= CEFStatic.percent_seo)
                                    {
                                        TkHelp.ChangeTextDgv(flag.browser_name, $"Follow shop", 2);
                                        flag.shopee.Follow(seo.shop_id);

                                    }
                                    if (TkHelp.RandomNumber(0, 101) <= CEFStatic.percent_seo)
                                    {
                                        var shop_count = await CEFAction.Script_Object<int>("document.querySelectorAll('.product-recommend-items__item-wrapper').length;", 1, ScriptType.Int);
                                        if (shop_count > 0)
                                        {

                                            var random = TkHelp.RandomNumber(0, shop_count);
                                            TkHelp.ChangeTextDgv(flag.browser_name, $"Vào sản phẩm liên quan {random}", 1);
                                            var shoplink = $"document.querySelectorAll('.product-recommend-items__item-wrapper a')[{random}].getAttribute('href');";
                                            var url_shop = "https://shopee.vn" + await CEFAction.Script_Object<string>(shoplink, 1, ScriptType.String);
                                            await browser.LoadUrlAsync(url_shop);

                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        stopwatch.Stop();
                        int time = 0;
                        if (CEFStatic.time_seo > stopwatch.Elapsed.TotalSeconds)
                        {
                            time = (int)(CEFStatic.time_seo - stopwatch.Elapsed.TotalSeconds);

                            await Task.Delay(TimeSpan.FromSeconds(time));
                        }
                        TkHelp.ChangeTextDgv(flag.browser_name, $"Chờ thêm {time + CEFStatic.time_seo} giây", 2);
                        await Task.Delay(TimeSpan.FromSeconds(time + CEFStatic.time_seo));

                        flag.is_running = false;
                        if (string.IsNullOrEmpty(flag.next_main_page))
                        {
                            TkHelp.ChangeTextDgv(flag.browser_name, $"Quay lại " + flag.current_main_page, 2);

                            await browser.LoadUrlAsync(flag.current_main_page);
                        }
                        else
                        {
                            TkHelp.ChangeTextDgv(flag.browser_name, $"Quay lại " + flag.next_main_page, 2);

                            await browser.LoadUrlAsync(flag.next_main_page);

                        }

                    });
                }
            }
        }

        private void Browser_FrameLoadEnd_Main(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Url.Contains("keyword=") && !flag.is_running_main)
            {
                Cef.UIThreadTaskFactory.StartNew(async delegate
            {
                flag.is_running_main = true;
                TkHelp.ChangeTextDgv(flag.browser_name, $"Load trang keyword", 1);

                string get_position_script = "function getPosition(el) {let rect = el.getBoundingClientRect();let centerX = rect.left + rect.width / 2 + window.pageXOffset;let centerY = rect.top + rect.height / 2 + window.pageYOffset;return centerX + '|' + centerY;}";
                await CEFAction.Script_Object(get_position_script);
                if (flag.end_page)
                {
                    flag.next_main_page = "";
                    flag.next_page = 0;
                    flag.is_running_main = false;
                    TkHelp.ChangeTextDgv(flag.browser_name, $"Chuyển keyword", 1);
                    if (flag.index_keyword >= CEFStatic.list_keyword.Count)
                    {
                        TkHelp.ChangeTextDgv(flag.browser_name, "Đã hết keyword", 1);
                        CEFAction.CloseBrowser(this.browser, thiscontrol);
                        return;
                    }
                    var keyword = Uri.EscapeDataString(CEFStatic.list_keyword[flag.index_keyword]);
                    TkHelp.ChangeTextDgv(flag.browser_name, keyword, 2);
                    flag.index_keyword++;
                    flag.current_main_page = "https://shopee.vn/search?keyword=" + keyword;
                    await browser.LoadUrlAsync(flag.current_main_page);
                    return;
                }

                await CEFAction.Mouse_Scroll(-120, flag.browser_name);


                var count_item = await CEFAction.Script_Object<int>("document.querySelectorAll('.shopee-search-item-result__item').length", 1, ScriptType.Int);

                var content = "var shopids = [" + string.Join(",", CEFStatic.list_SEO.Select(c => "'" + c.shop_id + "'")) + "];";
                string key = "";
                if (!CEFStatic.is_ads)//Bỏ qua sản phẩm có quảng cáo
                {
                    content += "function hasAd(ads_tag) {  if (ads_tag) {    var innerText = ads_tag.innerText;    if (innerText.includes(\"Ad\")) {      return true;    }  }  return false;} ;";
                    key = "!";
                    if (CEFStatic.seo_shop == 1)
                    {
                        content += "function getAllHrefs(max, shopids) {  var hrefs = [];    for (var i = 0; i < max; i++) {    var element = document.querySelectorAll('.shopee-search-item-result__item a')[i];        if (element) {      var href = element.getAttribute('href');      var containsShopId = false;      for (var j = 0; j < shopids.length; j++) {        var shopid = shopids[j];                if (href.includes(shopid)) {          containsShopId = true;          break;        }      }      if (!containsShopId && " + key + "hasAd(element)) {        hrefs.push(i);      }    }  }    return hrefs;}";

                    }
                    else
                    {
                        content += "function getAllHrefs(max, shopids) {  var hrefs = [];  for (var i = 0; i < max; i++) {    var element = document.querySelectorAll('.shopee-search-item-result__item a')[i];    if (element) {      var href = element.getAttribute('href');      for (var j = 0; j < shopids.length; j++) {        var shopid = shopids[j];        if (href.includes(shopid)) {            if(" + key + "hasAd(element)){                   hrefs.push(i);            }                 break;        }      }    }  }  return hrefs;}";

                    }
                }
                else
                {
                    if (CEFStatic.seo_shop == 1)
                    {
                        content += "function getAllHrefs(max, shopids) {  var hrefs = [];    for (var i = 0; i < max; i++) {    var element = document.querySelectorAll('.shopee-search-item-result__item a')[i];        if (element) {      var href = element.getAttribute('href');      var containsShopId = false;      for (var j = 0; j < shopids.length; j++) {        var shopid = shopids[j];                if (href.includes(shopid)) {          containsShopId = true;          break;        }      }      if (!containsShopId) {        hrefs.push(i);      }    }  }    return hrefs;}";
                    }
                    else
                    {
                        content += "function getAllHrefs(max, shopids) {  var hrefs = [];  for (var i = 0; i < max; i++) {    var element = document.querySelectorAll('.shopee-search-item-result__item a')[i];    if (element) {      var href = element.getAttribute('href');      for (var j = 0; j < shopids.length; j++) {        var shopid = shopids[j];        if (href.includes(shopid)) {       hrefs.push(i);                break;        }      }    }  }  return hrefs;}";

                    }


                }
                content += $"var allHrefs = getAllHrefs({count_item}, shopids);";
                await CEFAction.Script_Object(content);

                var shop_count = await CEFAction.Script_Object<int>("allHrefs.length", 1, ScriptType.Int);

                TkHelp.ChangeTextDgv(flag.browser_name, $"Tìm thấy {shop_count} sản phẩm hợp lệ", 1);

                for (int m = flag.current_item; m < shop_count; m++)
                {
                    flag.current_item++;
                    TkHelp.ChangeTextDgv(flag.browser_name, $"Vào sản phẩm {m}", 1);

                    var go_to_item = $"var count = allHrefs[{m}]; document.querySelectorAll('.shopee-search-item-result__item a')[count].click()";
                    await Task.Delay(2000);
                    await CEFAction.Script_Object(go_to_item);
                    break;
                }


                flag.is_running_main = false;
                if (flag.current_item >= shop_count)
                {
                    flag.current_item = 0;//reset item tại page
                    flag.next_page++;
                    if (flag.next_page >= CEFStatic.max_page)
                    {

                        flag.end_page = true;

                    }

                    flag.next_main_page = flag.current_main_page + "&page=" + flag.next_page;
                    TkHelp.ChangeTextDgv(flag.browser_name, $"Qua trang " + (flag.next_page + 1), 1);

                    await browser.LoadUrlAsync(flag.next_main_page);

                }


            });
            }
        }

        private void Browser_FrameLoadEnd_Closed(object sender, FrameLoadEndEventArgs e)
        {
            if (flag.is_completed || e.Url.Contains("unsupported.html"))
            {

                Cef.UIThreadTaskFactory.StartNew(async delegate
                {
                    if (e.Url.Contains("unsupported.html"))
                    {
                        TkHelp.ChangeTextDgv(flag.browser_name, "Lỗi trình duyệt!", 1);
                        flag.shopee.account.UserAgent = RandomUa.RandomUserAgentChrome;
                    }
                    TkHelp.ChangeTextDgv(flag.browser_name, flag.message, 1);
                    TkHelp.ChangeTextDgv(flag.browser_name, "Hoàn thành", 2);

                    CEFAction.CloseBrowser(this.browser, this.thiscontrol);
                });

            }
        }

        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            Cef.UIThreadTaskFactory.StartNew(async delegate
            {
                using (DevToolsClient DTC = browser.GetDevToolsClient())
                {

                    flag.shopee.account.UserAgent = RandomUa.RandomUserAgentChrome;
                    await DTC.Emulation.SetUserAgentOverrideAsync(flag.shopee.account.UserAgent);
                    // await DTC.Emulation.SetUserAgentOverrideAsync(RandomUa.RandomUserAgentChrome);
                    //Cài đặt timezone
                    ////await DTC.Emulation.SetTimezoneOverrideAsync("America/New_York");
                    //TkHelp.ChangeTextDgv(flag.browser_name, $"{flag.shopee.account.UserAgent}", 2);

                }

                if (CEFStatic.isProxy)
                {
                    try
                    {
                        var rc = browser.GetBrowser().GetHost().RequestContext;
                        var dict = new Dictionary<string, object>();
                        dict.Add("mode", "fixed_servers");
                        dict.Add("server", $"{flag.proxy.host}:{flag.proxy.port}");
                        string error = string.Empty;

                        var check = rc.SetPreference("proxy", dict, out error);
                        if (check)
                        {
                            TkHelp.ChangeTextDgv(flag.browser_name, "Add proxy", 1);

                        }
                    }
                    catch (Exception err)
                    {
                        TkHelp.Comment("Proxy err:" + err.Message);
                        return;
                    }
                }


                CEFAction.AddCookiesString("https://shopee.vn", flag.shopee.account.Cookies.ToString());
                if (flag.index_keyword >= CEFStatic.list_keyword.Count)
                {
                    TkHelp.ChangeTextDgv(flag.browser_name, "Đã hết keyword", 1);
                    CEFAction.CloseBrowser(this.browser, thiscontrol);
                    return;
                }
                var keyword = Uri.EscapeDataString(CEFStatic.list_keyword[flag.index_keyword]);
                flag.index_keyword++;
                flag.current_main_page = "https://shopee.vn/search?keyword=" + keyword;
                await browser.LoadUrlAsync(flag.current_main_page);





            });



        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            flag.checkDeleteWindow++;
            Task.Run(async () =>
            {
                if (flag.checkDeleteWindow > CEFStatic.deletedWindow)
                {
                    await Cef.UIThreadTaskFactory.StartNew(async delegate
                    {
                        flag.checkDeleteWindow = 0;
                        TkHelp.ChangeTextDgv(flag.browser_name, $"Trang load quá lâu!", 1);

                        CEFAction.CloseBrowser(this.browser, thiscontrol);
                        timer.Stop();
                        timer.Dispose();
                        return;
                    });

                }
            });

        }

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
        {
            Interval = 1000
        };

        private void CheckDeleteWindow(object sender, FrameLoadStartEventArgs e)
        {
            flag.checkDeleteWindow = 0;//Reset thời gian về lại 0 mỗi lần load pagwe
        }
    }

}
