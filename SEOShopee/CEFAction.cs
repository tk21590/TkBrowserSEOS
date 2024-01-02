using CefSharp.WinForms;
using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cookie = CefSharp.Cookie;
using System.IO;
using System.Drawing;
using System.Windows.Controls.Primitives;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SEOShopee
{
    public enum ScriptType
    {
        None,
        String,
        Boolean,
        Int
    }
    public class CEFAction
    {
        public ChromiumWebBrowser browser { get; set; }
        public CEFAction(ChromiumWebBrowser browser)
        {

            this.browser = browser;
        }
        public async Task Mouse_Scroll(int maxDown, string browserName)
        {
            var X = 150;
            var Y = 0;

            var endY = Y;

            for (int i = 0; i < 500; i++)
            {
                await Task.Delay(TkHelp.RandomNumber(100, 150) + (i * 2));
                var newY = endY;
                endY -= TkHelp.RandomNumber(1, 3);
                browser.GetBrowser().GetHost().SendMouseWheelEvent(X, newY, X, endY, CefEventFlags.None);
                if (endY <= maxDown)
                {
                    break;
                }
            }
        }
        public async Task<bool> ClickElement(string script_point, bool isright = false)
        {

            string point = await Script_Object<string>(script_point, 1, ScriptType.String);
            if (string.IsNullOrEmpty(point))
            {

                return false;
            }
            if (!string.IsNullOrEmpty(point))
            {
                string[] coordinates = point.Split('|'); // Tách chuỗi thành mảng các chuỗi con
                int X = int.Parse(coordinates[0].Split('.')[0]); // Chuyển chuỗi đầu tiên thành số nguyên và lấy phần nguyên
                int Y = int.Parse(coordinates[1].Split('.')[0]); // Chuyển chuỗi thứ hai thành số nguyên và lấy phần nguyên
                if (!isright)
                {
                    await Mouse_LeftClick(X, Y);

                }
                else
                {
                    await Mouse_RightClick(X, Y);

                }
                return true;
            }
            return false;
            //Lấy vị trí XY của element
            //Click vào vị trí đó của element
        }


        /// <summary>
        /// Gửi thông tin keyboard đến web theo dạng chữ
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public async Task SendStringToBrowser(string sData)
        {

            var charArray = sData.ToCharArray();

            foreach (char c in charArray)
            {
                CefSharp.KeyEvent keyEvent = new KeyEvent();

                keyEvent.WindowsKeyCode = (int)c;
                keyEvent.FocusOnEditableField = true;
                keyEvent.IsSystemKey = false;
                keyEvent.Type = KeyEventType.Char;
                browser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

                await Task.Delay(10);
            }
        }
        /// <summary>
        /// Gửi hàm Enter
        /// </summary>
        public async Task SendKeyEnter()
        {
            await Task.Delay(500);
            KeyEvent k = new KeyEvent();
            k.WindowsKeyCode = 0x0D;
            k.FocusOnEditableField = true;
            k.IsSystemKey = false;
            k.Type = KeyEventType.Char;
            browser.GetBrowser().GetHost().SendKeyEvent(k);
            await Task.Delay(500);

        }
        /// <summary>
        /// Gửi 1 key bất kì đến browser
        /// </summary>
        /// <param name="key">Ví dụ : Keys.Tab</param>
        /// <returns></returns>
        public async Task SendKeyToBrowser(System.Windows.Forms.Keys key)
        {


            CefSharp.KeyEvent keyEvent = new KeyEvent();

            keyEvent.WindowsKeyCode = (int)key;
            keyEvent.FocusOnEditableField = true;
            keyEvent.IsSystemKey = false;
            keyEvent.Type = KeyEventType.KeyDown;
            browser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

            await Task.Delay(5);

            keyEvent.WindowsKeyCode = (int)key;
            keyEvent.FocusOnEditableField = true;
            keyEvent.IsSystemKey = false;
            keyEvent.Type = KeyEventType.KeyUp;
            browser.GetBrowser().GetHost().SendKeyEvent(keyEvent);
            await Task.Delay(50);

        }
        public async Task<T> Script_Object<T>(string script, int delayFirst = 0, ScriptType scriptType = ScriptType.None)
        {
            string err_msg;
            await Task.Delay(delayFirst * 1000);

            try
            {
                var main = browser.GetMainFrame();
                if (main != null)
                {
                    JavascriptResponse task = await browser.GetMainFrame().EvaluateScriptAsync(script, null);
                    if (task.Success)
                    {
                        var respone = task.Result;
                        switch (scriptType)
                        {
                            case ScriptType.None:
                                return (T)Convert.ChangeType(respone, typeof(T));
                            case ScriptType.String:
                                return (T)Convert.ChangeType(respone.ToString(), typeof(T));
                            case ScriptType.Boolean:
                                if (bool.TryParse(respone.ToString(), out bool boolResult))
                                    return (T)Convert.ChangeType(boolResult, typeof(T));
                                break;
                            case ScriptType.Int:
                                if (int.TryParse(respone.ToString(), out int intResult))
                                    return (T)Convert.ChangeType(intResult, typeof(T));
                                break;
                        }
                    }
                }
            }
            catch (Exception errorr)
            {
                err_msg = "ERR_C:" + script + "\r\n" + errorr.Message;
                TkHelp.Comment("script_obj:" + err_msg);
            }
            return default(T);
        }
        public async Task<bool> CheckElement(string selector)
        {
            string query = "function checkElementExists(selector) { var elements = document.querySelectorAll(selector);  if (elements.length > 0) {  return true;  } else {  return false;  }}";

            await Script_Object(query);
            await Task.Delay(1000);
            return await Script_Object<bool>("checkElementExists('" + selector + "');", 0, ScriptType.Boolean);
        }
        /// <summary>
        /// Chạy hàm javascript trả về 1 object hoặc null
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public async Task<object> Script_Object(string script, int delayFirst = 0)
        {
            string err_msg;
            await Task.Delay(delayFirst * 1000);
            try
            {
                var main = browser.GetMainFrame();
                if (main != null)
                {
                    JavascriptResponse task = await browser.GetMainFrame().EvaluateScriptAsync(script, null);
                    if (task.Success)
                    {
                        var respone = task.Result;
                        return respone;
                    }

                }


            }
            catch (Exception errorr)
            {
                err_msg = "ERR_C:" + script + "" + errorr.Message;
                TkHelp.Comment("script_obj:" + err_msg);
            }
            return null;
        }

        /// <summary>
        /// Mô phỏng click chuột trái với vị trí chỉ định X và Y 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public async Task Mouse_LeftClick(int X, int Y)
        {

            browser.GetBrowser().GetHost().SendMouseMoveEvent(X, Y, false, CefEventFlags.None);
            await Task.Delay(50);

            browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
            await Task.Delay(55);

            browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Left, true, 1, CefEventFlags.None);

        }
        public async Task Mouse_RightClick(int X, int Y)
        {

            browser.GetBrowser().GetHost().SendMouseMoveEvent(X, Y, false, CefEventFlags.None);
            await Task.Delay(50);

            browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Right, false, 1, CefEventFlags.None);
            await Task.Delay(55);

            browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Right, true, 1, CefEventFlags.None);

        }
        public async Task MouseScrollDown(Control control)
        {
            int width = control.Width;
            int height = control.Height;


            // Tính toán vị trí trung tâm của trình duyệt
            Point center = new Point(width / 2, height / 2);

            // Lấy vị trí của con chuột trong WinForm
            Point mousePosition = Cursor.Position;




            browser.GetBrowser().GetHost().SendMouseClickEvent(center.X, center.Y, MouseButtonType.Middle, true, 1, CefEventFlags.None);

            for (int i = 1; i < 100; i++)
            {

                await Task.Delay(TkHelp.RandomNumber(3, 10) * 100);
                int X = center.X;
                int Y = center.Y + (i * 5);
                // browser.GetBrowser().GetHost().SendMouseClickEvent(X, Y, MouseButtonType.Right, true, 1, CefEventFlags.None);
                browser.GetBrowser().GetHost().SendMouseMoveEvent(X, Y, false, CefEventFlags.None);

                if (Y >= height)
                {
                    break;
                }


            }
        }
        public async Task<string> GetCookiesAsync(string url, bool isJson = false)
        {

            var cookieManager = browser.GetCookieManager();
            var visitor = new CookieCollector();

            cookieManager.VisitUrlCookies(url, true, visitor);

            var cookies = await visitor.Task; // AWAIT !!!!!!!!!
            string cookieHeader = string.Empty;
            if (isJson)
            {
                cookieHeader = CookieCollector.GetCookieJson(cookies);
            }
            else
            {
                cookieHeader = CookieCollector.GetCookieString(cookies);
            }
            return cookieHeader;
        }
        public bool AddCookiesString(string url, string cookies)
        {
            browser.GetCookieManager().DeleteCookies();
            var splitCookies = cookies.Split(';');

            foreach (var c in splitCookies)
            {
                var splitC = c.Split('=');

                var cookie = new CefSharp.Cookie();
                cookie.Name = splitC[0].Trim();
                cookie.Value = splitC[1].Trim();
                //cookie.Expires = StringToDateTime(cookieImportado.expires);
                //novoCookie.Domain = cookieImportado.domain;
                //novoCookie.Path = cookieImportado.path;
                //novoCookie.Secure = cookieImportado.secure;
                //novoCookie.HttpOnly = cookieImportado.httponly.GetValueOrDefault(false);
                browser.GetCookieManager().SetCookie(url, cookie);



            }



            return true;
        }

        public bool AddCookiesJson(string url, string cookies)
        {
            browser.GetCookieManager().DeleteCookies();
            dynamic cookiesObject = JsonConvert.DeserializeObject<List<Cookie>>(cookies);
            for (int i = 0; i < cookiesObject.Count; i++)
            {
                Cookie cookie = cookiesObject[i];
                browser.GetCookieManager().SetCookie(url, cookie);

            }

            return true;
        }
        public RequestContextSettings ContextSettings(int lenght)
        {
            RequestContextSettings lRCS = new RequestContextSettings();
            string a = TkHelp.RandomToken(lenght);
            //string a = "Myprofile1";
            a = Path.GetFullPath(a);
            lRCS.CachePath = a;

            return lRCS;
        }


        /// <summary>
        /// Xóa toàn bộ browser ra khỏi luồng điều khiển
        /// </summary>
        public static void CloseBrowser(ChromiumWebBrowser browser, Control thiscontrol)
        {
            try
            {
                Cef.UIThreadTaskFactory.StartNew(() =>
                {

                    browser.DestroyWindow();

                    Control parentControl = thiscontrol.Parent;
                    if (parentControl != null)
                    {
                        parentControl.BeginInvoke((Action)delegate ()
                        {
                            parentControl.Controls.Remove(thiscontrol);
                        });
                        thiscontrol.BeginInvoke((Action)delegate ()
                        {
                            thiscontrol.Dispose();
                        });
                        browser.Dispose();

                    }



                });
            }
            catch (Exception err)
            {
                TkHelp.Comment("CloseBrowser Exception : " + err.Message);
            }

        }


    }

}
